using Crawler.Core;
using Crawler.Core.Manager;
using Palas.Common.Module;
using Palas.Common.WCFService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;

namespace SimpleCrawler
{
    public partial class CrawlHost : Form
    {
        public CrawlHost()
        {
            InitializeComponent();
            InitCrawlHost();
        }

        private ServiceHost _monitorHost;
#if !DEBUG
        private ServiceHost _dcpHost;
#endif
        private static string AnalyzeType = HooLab.Config.Configger.GetConfig("Palas.Crawler.AnalyzeType", "LOCAL");

        private void InitCrawlHost()
        {
            //启动DCP，要在CrawlerManager启动之前启动
#if !DEBUG
            _dcpHost = DCPManager.Instance.CreateDCPService();
            _dcpHost.Open();
#endif
            CrawlerManager.Start();
            _currentPipeline = CrawlerManager.CrawlerFactory.Pipelines;
            PipeGridView.AutoGenerateColumns = false;
            PipeGridView.DataSource = CrawlerManager.CrawlerFactory.Pipelines.Select(model => model.Info).ToArray();
            this.Text = "Palas Crawler : " + HooLab.Config.Configger.GetConfig("Palas.Crawler.Name", "No_Name");

            //启动Gecko监控窗口
            //if ((ConfigurationManager.AppSettings["WCFGeckoMode"] ?? "false") == "true")
            //{
            //    GeckoMonitor monitor = new GeckoMonitor();
            //    monitor.Show();
            //}

            if (AnalyzeType.ToUpper() != "STORM")
            {
                //启动分词监控
                if ((ConfigurationManager.AppSettings["WCFSpitterMode"] ?? "false") == "true")
                {
                    SplitWordMonitor splitWordMonitor = new SplitWordMonitor();
                    splitWordMonitor.Show();
                }
                //创建分析监听

                AnalyzeMatchMonitor analyzeMonitor = new AnalyzeMatchMonitor();
                analyzeMonitor.Show();
            }
            //启动AllJobMonitor监控窗口
            _monitorHost = WCFMonitorProxy.CreateServiceHost();
            _monitorHost.Open();
        }

        private const int _analyzeIdleSleepTime = 5000;

        private List<IPipeline> _currentPipeline = null;

        private List<IPipeMessage> _currentScheduleMessage = new List<IPipeMessage>();
        private object _scheduleSync = new object();
        private string _currentName = "";
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshSchedule();
            RefreshMessage();
            if (string.IsNullOrEmpty(_currentName) && PipeGridView.Rows.Count > 0) //fixed by chol 2013.11.12
            {
                try
                {
                    _currentName = PipeGridView.Rows[0].Cells["NameCol"].Value.ToString();
                }
                catch (Exception)
                {
                    throw;
                }

            }
            if (!string.IsNullOrEmpty(_currentName))
            {
                RefreshDetail(_currentName);
            }
        }

        private void RefreshMessage()
        {
            var pipeLines = CrawlerManager.CrawlerFactory.Pipelines;
            foreach (var item in pipeLines)
            {
                if (!_pipelineDic.ContainsKey(item.Info.Name))
                {
                    _pipelineDic[item.Info.Name] = new List<IPipeMessage>();
                }

                if (item.Info.Trigger_PipelineInfoChange == null)
                {
                    IPipeline item1 = item;
                    item.Info.Trigger_PipelineInfoChange = new Scheduler.del_MsgSender(msg =>
                    {
                        var messageList = _pipelineDic[item1.Info.Name];
                        IPipeMessage message = new IPipeMessage();
                        message.Message = msg;
                        message.GenerateTime = DateTime.Now;
                        lock (sync)
                        {
                            messageList.Add(message);
                        }

                        if (messageList.Count > 20)
                        {
                            lock (sync)
                            {
                                messageList.RemoveAt(0);
                            }
                        }
                    });
                }
            }
        }

        private void RefreshSchedule()
        {
            var info = CrawlerManager.CrawlerFactory.Info;
            SJobCntLbl.Text = info.JobCount.ToString();
            SCrawlCntLbl.Text = info.ItemCount.ToString();
            SErrorCntLbl.Text = info.ErrorCount.ToString();
            SStartTimeLbl.Text = info.StartTime.ToString();
            if (CrawlerManager.CrawlerFactory.Trigger_SchedulerInfoChange == null)
            {
                CrawlerManager.CrawlerFactory.Trigger_SchedulerInfoChange =
                    new Scheduler.del_MsgSender(
                        msg =>
                        {
                            IPipeMessage message = new IPipeMessage();
                            message.Message = msg;
                            message.GenerateTime = DateTime.Now;
                            lock (_scheduleSync)
                            {
                                _currentScheduleMessage.Add(message);
                                if (_currentScheduleMessage.Count > 20)
                                {
                                    _currentScheduleMessage.RemoveAt(0);
                                }
                            }
                        });
            }

            var selStart = SContentTxt.SelectionStart;
            var selLength = SContentTxt.SelectionLength;
            var point = SContentTxt.AutoScrollOffset;
            SContentTxt.Text = "";
            string[] result;
            lock (_scheduleSync)
            {
                result = (from item in _currentScheduleMessage
                          select string.Format("[{1}]{0}", item.Message,
                                               item.GenerateTime.ToString("HH:mm"))).Reverse().ToArray();

            }
            SContentTxt.Lines = result;
            try
            {
                SContentTxt.SelectionStart = selStart;
                SContentTxt.SelectionLength = selLength;
                SContentTxt.AutoScrollOffset = point;
            }
            catch { }


        }
        private ConcurrentDictionary<string, List<IPipeMessage>> _pipelineDic = new ConcurrentDictionary<string, List<IPipeMessage>>();
        private void PipeGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //刷新详情
                string name = PipeGridView.Rows[e.RowIndex].Cells["NameCol"].Value.ToString();
                _currentName = name;
                RefreshDetail(_currentName);
                PipeGridView.Rows[e.RowIndex].Selected = true;

            }
        }

        private string preName = "";
        private object sync = new object();


        private void RefreshDetail(string name)
        {
            if (!_pipelineDic.ContainsKey(name))
            {
                _pipelineDic[name] = new List<IPipeMessage>();
            }
            List<IPipeMessage> messageList = _pipelineDic[name];
            var currentJob = _currentPipeline.FirstOrDefault(model => model.Info.Name == name);
            var currentInfo = currentJob.Info;
            PNameLbl.Text = currentInfo.Name;
            PStartTimeLbl.Text = currentInfo.StartTime.ToString("HH:mm");
            PNextRuntimeLbl.Text = currentInfo.NextRunTime.ToString("HH:mm");
            PStatusLbl.Text = currentInfo.Status.ToString();
            PJobLbl.Text = currentInfo.CurrentJob_Name;
            PJobStartTimeLbl.Text = currentInfo.CurrentJob_StartTime.ToString("HH:mm");
            PItemCntLbl.Text = currentInfo.ItemCount.ToString();
            PJobCntLbl.Text = currentInfo.JobCount.ToString();
            PErrorCntLbl.Text = currentInfo.ErrorCount.ToString();
            PCPULbl.Text = currentInfo.AvgCPUCost.ToString();
            PNetworkLbl.Text = currentInfo.AvgNetworkCost.ToString();

            string[] result;
            lock (sync)
            {
                result = (from item in messageList
                          select string.Format("[{1}]{0}", item.Message,
                                               item.GenerateTime.ToString("HH:mm"))).Reverse().ToArray();

            }

            if (preName != name || !ContentTxt.Text.StartsWith(result.FirstOrDefault() ?? "EmptyArray"))
            {
                var selStart = ContentTxt.SelectionStart;
                var selLength = ContentTxt.SelectionLength;
                ContentTxt.Lines = result;
                try
                {

                    ContentTxt.SelectionStart = selStart;
                    ContentTxt.SelectionLength = selLength;

                }
                catch { }
                preName = name;
            }
            //foreach (var item in messageList)
            //{
            //    string message = string.Format("[{1}]{0}", item.Message,
            //                                   item.GenerateTime.ToString("HH:mm"));
            //    ContentList.Items.Add(message);

            //}
        }

        private void CrawlHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DCPManager.Instance.Dispose();
                SeleniumPool.Instance.Dispose();
                //_statusMonitor.DisconnectToMonitorServer();
            }
            catch
            { }
            Process.GetCurrentProcess().Kill();
        }
    }
    public class IPipeMessage
    {
        public string Message { get; set; }
        public DateTime GenerateTime { get; set; }

    }
}

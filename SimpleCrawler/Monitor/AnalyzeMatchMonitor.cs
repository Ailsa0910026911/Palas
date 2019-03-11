using Crawler.Core;
using Palas.Common.Data;
using Palas.Common.Utility;
using Palas.Common.WCFService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SimpleCrawler
{
    public partial class AnalyzeMatchMonitor : Form
    {
        public const string BaseAddress = "net.tcp://localhost/AnalyzeData";
        private ServiceHost _host;
        private const long _maxRecvMessageSize = 1024000000;
        private AnalyzeTask _analyzeTaskSvc;
        private TaskFactory _backGroundTaskFactory;

        public AnalyzeMatchMonitor()
        {
            InitializeComponent();
        }

        private void InitWCFHost()
        {
            _host = WCFUtility.CreateWCFHost<AnalyzeTask>
                                         (
                                          BaseAddress,
                                          new NetTcpBinding(SecurityMode.None)
                                          {
                                              CloseTimeout = TimeSpan.FromDays(3),
                                              MaxReceivedMessageSize = _maxRecvMessageSize,
                                              OpenTimeout = TimeSpan.FromDays(3),
                                              ReceiveTimeout = TimeSpan.FromDays(3),
                                              SendTimeout = TimeSpan.FromDays(3),
                                              ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                                          },
                                          true);
            _host.Opened += new EventHandler(host_Opened);
            _host.Open();
            _analyzeTaskSvc = (AnalyzeTask)_host.SingletonInstance;
            //启动分析线程
            Thread thread = new Thread(ProcessBackAnalyzeTask);
            thread.Name = "AnalyzeTask";
            thread.Start();
            _backGroundTaskFactory = new TaskFactory(TaskScheduler.Default);

            //_host.MonitorCall += new EventHandler<MonitorInfoEventArgs>(Process_MonitorCall);
        }

        void Process_MonitorCall(object sender, MonitorInfoEventArgs e)
        {
            var info = e.PointInfo;
            this.Invoke(new Action(() => UpdatePointInfo(info)));
        }
        ConcurrentDictionary<string, ProcessEndPoint> infoDictionary = new ConcurrentDictionary<string, ProcessEndPoint>();

        private void UpdatePointInfo(ProcessEndPoint pointInfo)
        {
            var key = pointInfo.ID;
            if (infoDictionary.ContainsKey(key))
            {
                var orgValue = infoDictionary[key];
                pointInfo.CopyTo(ref orgValue);

            }
            else
            {
                infoDictionary.TryAdd(key, pointInfo);
                var result = infoDictionary.Select(model => model.Value).ToArray();

                MonitorGridView.DataSource = result;
            }
        }
        void host_Opened(object sender, EventArgs e)
        {
            this.Text = "分析服务监控:侦听中....";

        }

        private void AnalyzeMatchMonitor_Load(object sender, EventArgs e)
        {
            InitWCFHost();
            //初始化控件 
            MonitorGridView.AutoGenerateColumns = false;
        }

        private const int _analyzeIdleSleepTime = 5000;

        static bool flagStopBackAnalyzeTask = false;

        /// <summary>
        /// 后端接受Item的线程（外部数据中心的复制任务）
        /// </summary>
        private void ProcessBackAnalyzeTask()
        {
            while (true && !flagStopBackAnalyzeTask && !CrawlerManager.CrawlerFactory.isStopping())
            {
                var task = _analyzeTaskSvc.GetAnalyzeTask();
                if (task == null)
                {
                    Thread.Sleep(_analyzeIdleSleepTime);
                }
                else
                {
                    _backGroundTaskFactory.StartNew(() => Analyzer.Core.Analyzer.AnalyzeIntermediaResult(task));
                }
            }
        }
    }
}

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
using System.Windows.Forms;
using Crawler.Core.RequestProcessor;
using Palas.Common;
using Palas.Common.Data;
namespace SimpleCrawler
{
    public partial class SplitWordMonitor : Form
    {
        public SplitWordMonitor()
        {
            InitializeComponent();
        }
        public const string BaseAddress = "net.pipe://localhost/SplitWord/Monitor";
        private ServiceHost _host;
        private void SpliteWordMonitor_Load(object sender, EventArgs e)
        {
            InitWCFHost();
            //初始化控件 
            MonitorGridView.AutoGenerateColumns = false;
        }

        private void InitWCFHost()
        {
            var svc = new ProcessMonitorSvc();
            _host = new ServiceHost(svc);
            //_host = new ServiceHost(typeof(ProcessMonitorSvc));
            var serviceMetadataBehavior = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (serviceMetadataBehavior == null)
            {
                ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                behavior.HttpGetEnabled = false;

                _host.Description.Behaviors.Add(behavior);
            }
            else
            {
                serviceMetadataBehavior.HttpGetEnabled = false;
            }

            _host.AddServiceEndpoint(
                                     typeof(IProcessMonitorSvc),
                                     new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), BaseAddress);


            var currentDebugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (currentDebugBehavior == null)
            {
                ServiceDebugBehavior behavior = new ServiceDebugBehavior {IncludeExceptionDetailInFaults = true};
                _host.Description.Behaviors.Add(behavior);
            }
            else
            {
                currentDebugBehavior.IncludeExceptionDetailInFaults = true;
            }

            _host.Opened += new EventHandler(host_Opened);

            _host.Open();
            svc.MonitorCall += new EventHandler<MonitorInfoEventArgs>(Process_MonitorCall);
        }

        void Process_MonitorCall(object sender, MonitorInfoEventArgs e)
        {
            var info = e.PointInfo;
            this.Invoke(new Action(()=> UpdatePointInfo(info)));
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
                var result = infoDictionary.Select(model=>model.Value).ToArray();
                
                MonitorGridView.DataSource = result;
            }
        }
        void host_Opened(object sender, EventArgs e)
        {
            this.Text = "分词监控:侦听中....";

        }
    }
}

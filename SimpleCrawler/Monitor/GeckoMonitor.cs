using Palas.Common.Data;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;

namespace SimpleCrawler
{
    public partial class GeckoMonitor : Form
    {
        public GeckoMonitor()
        {
            InitializeComponent();
        }

        public const string BaseAddress = "net.pipe://localhost/GeckoTask/Monitor";
        private ServiceHost _host;
        private void GeckoMonitor_Load(object sender, EventArgs e)
        {
            InitWCFHost();
            //初始化控件 
            MonitorGridView.AutoGenerateColumns = false;

        }

        private void InitWCFHost()
        {
            //_host = new ServiceHost(typeof(ProcessMonitorSvc));
            var svc = new ProcessMonitorSvc();
            _host = new ServiceHost(svc);
            var currentMetaBehavior = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (currentMetaBehavior == null)
            {
                ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                behavior.HttpGetEnabled = false;

                _host.Description.Behaviors.Add(behavior);
            }
            else
            {
                currentMetaBehavior.HttpGetEnabled = false;
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
            svc.MonitorCall += new EventHandler<MonitorInfoEventArgs>(GeckoSvc_MonitorCall);
        }

        void GeckoSvc_MonitorCall(object sender, MonitorInfoEventArgs e)
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
            this.Text = "Gecko监控:侦听中....";
        }
    }
}

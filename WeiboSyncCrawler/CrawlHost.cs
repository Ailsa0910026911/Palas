using Crawler.Core;
using Crawler.Core.ConductorSvc;
using HooLab.Config;
using Palas.Common.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WeiboSyncCrawler
{
    public partial class CrawlHost : Form
    {
        public static Scheduler CrawlerFactory = null;
        public static string CrawlerName = Configger.GetConfig("Palas.Crawler.Name", "Test");
        public static int DCPCount = Configger.GetConfigInt("Palas.Crawler.DCPCount", 10);

        public CrawlHost()
        {
            InitializeComponent();

            SchedulerSetting Setting = new SchedulerSetting();
            CrawlerFactory = new Scheduler(CrawlerName, Setting);
            StartJobs();
        }

        public void StartJobs()
        {
            //1. 通过ConductAPI获取一批账号的AuthorID
            string[] AccountIDs = ConductorClient.GetWeiboSyncTasks(CrawlerName, DCPCount).ToArray();

            //2. 创建这批账号的同步任务
            if (AccountIDs != null)
            {
                //提交到分析程序
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 1000 }; //最大线程数1000
                Parallel.ForEach(AccountIDs, options, Item =>
                {
                    CrawlerFactory.AddPipeline(new Crawler.Core.Crawler.WeiboSyncCrawler(Item, CrawlerManager.CrawlerFactory), DateTime.Now);
                });
            }

            //3. 将任务情况反馈到界面

            //4. 启动任务
            Start();
        }

        /// <summary>
        /// 启动爬虫工厂
        /// </summary>
        public static void Start()
        {
            if (CrawlerFactory != null) CrawlerFactory.Start(true);
        }

        /// <summary>
        /// 发送调度器级别消息
        /// </summary>
        /// <param name="Msg"></param>
        public static void SendMsg(string Msg)
        {
            CrawlerFactory.SendMsg(Msg);
        }
    }
}

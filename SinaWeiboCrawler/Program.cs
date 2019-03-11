using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Palas.Common.Module;
using Palas.Common.Utility;
using Palas.Common.WCFService;
using SinaWeiboCrawler.Workers;
using SinaWeiboCrawler.DatabaseManager;
using System.Reflection;
using SinaWeiboCrawler.Utility;
using Palas.Common.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Palas.Common;
using NetDimension.Weibo;
using Crawler.Core.Crawler;

namespace SinaWeiboCrawler
{
    class Program : SinaWeiboCrawler.DatabaseManager.MongoDBManager
    {
        //#region "Unmanaged"

        //[DllImport("user32.dll")]
        //static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //[DllImport("user32.dll")]
        //static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        //internal const uint SC_CLOSE = 0xF060;
        //internal const uint MF_GRAYED = 0x00000001;
        //internal const uint MF_BYCOMMAND = 0x00000000;

        //#endregion

        static void AddCBDs() 
        {
            WeiboAPIClient.AddNewCBDs(@"CBDs.txt");
            Console.WriteLine("finished");
        }

        /// <summary>
        /// 添加名人堂
        /// </summary>
        static void AddCelebrity() 
        {
            WeiboAPIClient.UpdateCelebrityList();
            Thread.Sleep(100000);
            Console.WriteLine("AddCelebrity finished");
        }

        /// <summary>
        /// 添加partner
        /// </summary>
        static void AddPartners() 
        {
            string[] list = File.ReadAllLines("partner.txt", Encoding.Default);
            for (int i = 0; i < list.Length; ++i)
            {
                WeiboAPIClient.AddNewAuthorWithID(list[i], Enums.AuthorSource.Partner);
                Console.WriteLine(i);
            }
            Thread.Sleep(100000);
            Console.WriteLine("AddPartners finished");
        }

        /// <summary>
        /// 添加意见领袖
        /// </summary>
        static void AddLeader() 
        {
            string[] list = File.ReadAllLines("leader.txt", Encoding.Default);
            for (int i = 0; i < list.Length; ++i)
            {
                WeiboAPIClient.AddNewAuthorWithUrl(list[i], Enums.AuthorSource.PublicLeader);
                Console.WriteLine(i);
            }
            Thread.Sleep(100000);
            Console.WriteLine("AddLeader finished");
        }

        private static void InitWork() 
        {
            //#region 取消关闭按钮，只能通过命令方式退出
            //IntPtr hMenu = Process.GetCurrentProcess().MainWindowHandle;
            //IntPtr hSystemMenu = GetSystemMenu(hMenu, false);

            //EnableMenuItem(hSystemMenu, SC_CLOSE, MF_GRAYED);
            //RemoveMenu(hSystemMenu, SC_CLOSE, MF_BYCOMMAND);
            //#endregion

            Thread t = new Thread(MainLoop);
            t.Start();

            WeiboUtilities.InitSinaCityTable();

            WeiboUtilities.InitPOISourceWhiteList();
        }

        private static void AddSubscribe()
        {
            var query = Query.And(Query.GT("FansCount", 12100), Query.EQ("InternalSubscribeID", MongoDB.Bson.BsonNull.Value));
            var collection = GetCollections<Author>();
            var result = collection.FindAs<Author>(query);
            int cnt = 0;
            foreach (var author in result) 
            {
                if (author.InternalSubscribeID == null)
                {
                    author.InternalSubscribeID = DefaultSettings.ToBeFollowed;
                    string[] parameters = new string[] { "InternalSubscribeID" };
                    UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
                    cnt++;
                }
                else Console.WriteLine(author.AuthorName);
                Console.WriteLine(cnt);
            }
            Console.WriteLine(cnt);
            Console.WriteLine("finished");
        }

        private static void RemoveAll() 
        {
            MongoItemAccess.Remove(Query.Or(Query.EQ("CrawlID", "WeiboSub"), Query.EQ("CrawlID", "WeiboCBD"),
                                   Query.EQ("CrawlID", "WeiboLocHistory"), Query.EQ("CrawlID", "WeiboUserCensus")));
            ESItemAccess.DeleteByCrawlID("WeiboSub");
            ESItemAccess.DeleteByCrawlID("WeiboCBD");
            ESItemAccess.DeleteByCrawlID("WeiboLocHistory");
            ESItemAccess.DeleteByCrawlID("WeiboUserCensus");
        }

        private static void TestAccount() 
        {
            string[] acc = File.ReadAllLines("1111.txt");
            foreach (var st in acc)
            {
                string[] tmp = st.Split(',');
                string username = tmp[0], password = tmp[1];

                OAuth o = new OAuth(DefaultSettings.AppKey, DefaultSettings.AppSecret, String.Empty);
                o.ClientLogin(username, password);
                Client client = new Client(o);
                try
                {
                    var s = client.API.Entity.Friendships.Create("1266321801");
                    Console.WriteLine(username + " " + s.Name);
                    File.AppendAllText("accountResult.txt", username + " " + password + '\n');
                }
                catch (WeiboException ex) 
                {
                    Console.WriteLine(username + " " + ex.ErrorCode + " " + ex.ErrorMessage);
                }
                Thread.Sleep(10000);
            }
        }

        private static void NormalizeAccount() 
        {
            string[] acc = File.ReadAllLines("moreAccount.txt");
            foreach (var st in acc) 
            {
                string[] tmp = st.Split(' ');
                string username = tmp[0], password = tmp[1];

                OAuth o = new OAuth(DefaultSettings.AppKey, DefaultSettings.AppSecret, String.Empty);
                o.ClientLogin(username, password);
                Client client = new Client(o);

                try 
                {
                    var s = client.API.Entity.Account.GetUID();
                    var collection = WeiboAPI.GetFriendsIDs(s, Enums.SampleMethod.All);
                    foreach (var id in collection) 
                    {
                        client.API.Entity.Friendships.Destroy(id);
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine(username + " successed");
                    File.AppendAllText("accountResult.txt", username + " " + password + '\n');
                }
                catch (WeiboException ex)
                {
                    Console.WriteLine(username + " " + ex.ErrorCode + " " + ex.ErrorMessage);
                }
                Thread.Sleep(9000);
            }
        }

        private static Scheduler scheduler = null;
        public static HourCounter JobCounter = null;
        public static string JobCounterDesc = null;
        
        static void Main(string[] args)
        {
            InitWork();
            //NormalizeAccount();
            //TestAccount();
            //RemoveAll();
            //AddSubscribe();
            //Console.WriteLine(DatabaseRecovery.Count());
            //AddCBDs();
            //DatabaseRecovery.Work();
            //AuthorLocHistWorker.BackToOrigin();
            //StatusSubscribeWorker.BackToOrigin();
            //CBDWorker.BackToOrigin();
            //AuthorCensusWorker.BackToOrigin();
            //Console.WriteLine("all is well");
            //return;

            scheduler = new Scheduler("SinaWeibo", new SchedulerSetting());
            for (int j = 0; j < int.Parse(ConfigurationManager.AppSettings["WorkerThreadCount"]); ++j)
            {
                IPipeline worker = null;
                switch (ConfigurationManager.AppSettings["WorkerType"]) 
                {
                    case "WeiboAuthorCrawler":
                        {
                            //var tmp = new AuthorCensusWorker("WeiboUserCensus_" + j, scheduler);

                            WeiboAuthorCrawler work = new WeiboAuthorCrawler("WeiboUserCensus_" + j, scheduler);
                            work.DoOneJob(work);

                            //worker = tmp;
                            //JobCounter = tmp.CntData;
                            //JobCounterDesc = "用户普查";
                            break;
                        }
                        
                    case "WeiboSub":
                        {
                            var tmp = new Crawler.Core.Crawler.WeiboSubscribeCrawler("WeiboSub_" + j, scheduler);
                            worker = tmp;

                            tmp.DoOneJob(tmp);
                            //JobCounter = tmp.CntData;
                            JobCounterDesc = "微博订阅";
                            break;
                        }
                    case "WeiboCBD":
                        {
                            var tmp = new CBDWorker("WeiboCBD_" + j, scheduler);
                            worker = tmp;

                            WeiboCBDCrawler work = new WeiboCBDCrawler("WeiboCBD_" + j, scheduler);
                            work.DoOneJob(work);

                            JobCounter = tmp.CntData;
                            JobCounterDesc = "CBD扫描";
                            break;
                        }
                        
                    case "WeiboLocHistory":
                        {
                            var tmp = new AuthorLocHistWorker("WeiboLocHistory_" + j, scheduler);
                            worker = tmp;

                            WeiboAuthorLocHistoryCrawler work = new WeiboAuthorLocHistoryCrawler("WeiboLocHistory_" + j, scheduler);
                            work.DoOneJob(work);

                            JobCounter = tmp.CntData;
                            JobCounterDesc = "地理信息";
                            break;
                        }

                    case "Forward":
                        {

                            WeiboForwardTrackingCrawler work = new WeiboForwardTrackingCrawler("Forward", scheduler);
                            work.DoOneJob(work);

                            var tmp = new LoginAccountWorker("Forward", scheduler);
                            worker = tmp;
                            JobCounter = tmp.CntData;
                            JobCounterDesc = "关注订阅";
                            break;
                        }
                    case "Reply":
                        {

                            WeiboReplyTrackingCrawler work = new WeiboReplyTrackingCrawler("Reply" + j, scheduler);
                            work.DoOneJob(work);

                            var tmp = new RelationshipWorker("Reply" + j, scheduler);
                            worker = tmp;
                            JobCounter = tmp.CntData;
                            JobCounterDesc = "粉丝&关注";
                            break;
                        }
                    default: break;
                }
                if (worker == null) 
                {
                    Console.WriteLine("工人类型配置项错误，请输入exit退出程序");
                    return;
                }
                scheduler.AddPipeline(worker, DateTime.Now);
                if (ConfigurationManager.AppSettings["WorkerType"] == "LoginAccountWorker") break;
            }
            _statusMonitor.ConnectToMonitorServer();
            scheduler.Start();
        }
        private static WCFStatusReport<ServiceMonitorClient> _statusMonitor = new WCFStatusReport<ServiceMonitorClient>(HostType.Weibo); 
        
        private static void MainLoop()
        {
            while (true)
            {
                string x = Console.ReadLine();
                if (!String.IsNullOrEmpty(x))
                {
                    switch (x.ToUpperInvariant())
                    {
                        case "STOP":
                            Console.WriteLine("正在终止任务...");
                            if (scheduler != null)
                                scheduler.Stop(true, false);
                            scheduler = null;
                            Console.WriteLine("任务已经终止，请确保Mongo数据插入完成后再退出");
                            break;
                        case "EXIT":
                        case "QUIT":
                            if (scheduler != null)
                                Console.WriteLine("请先使用stop命令终止任务");
                            else System.Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("仅支持exit,quit和stop");
                            break;
                    }
                }
            }
        }
    }
}

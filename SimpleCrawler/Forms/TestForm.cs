using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Aspose.Cells;
using Crawler.Core.Parser;
using Crawler.Core.RequestProcessor;
using Crawler.Core.Utility;
using ElasticSearch.Client;
using ElasticSearch.Client.DSL;
using HooLab.Log;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Palas.Common;
using Palas.Common.Data;
using Palas.Common.DataAccess;
using Palas.Common.Lib.Business;
using Palas.Common.Lib.Entity;
using Palas.Common.Utility;
using Palas.Common.WCFService;
using Query = MongoDB.Driver.Builders.Query;
using SortOrder = ElasticSearch.Client.SortOrder;

namespace Crawler.Host
{
    public partial class TestForm : Form
    {
        private ServiceHost _host;
        private IStatusReportServer _server;

        public TestForm()
        {
           
            InitializeComponent();
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            
            //DeleteCrawlItem();
            //ChangeItemAnalyzeData();
            //TestLoadHourCount();
            //TestHourCount();
            //ImportDataToClient();
            //TestResourceMonitor();
            //TestWCF();
            //ResetAllParentMedia();
            //ImportArticle();
            //UpdateParentMediaID();
            //GetSum();
            //TestDelegate();
            //AnalyzeAggr();
            //GroupByTag();
            //AnalyzeDetail();
            //SetAllDuplicateCrawl();
            //MediaImport();
            //AutoCreateCrawl();
            //UpdateXinHua();
            //TestESOperator();
            //TestESQuery();
            //AddTopic4Project();
            //ImportPGIGoogleCrawl();
            //GenerateWeiboReport();
            //SearchFudanWeiboPeople();
            //GetSinaPeople();
            //MigrationES();
            //TestAnalyzeHost();
            AdjustSiteInterval();

            MessageBox.Show("运行成功");
        }

        private void AdjustSiteInterval()
        {
            using (PalasDB db = new PalasDB())
            {
                var updatedCrawls = db.Crawl.Where(uCrawl => uCrawl.IsDisabled ==false&&uCrawl.Priority!=1);
                foreach (var crawl in updatedCrawls)
                {
                    SearchBuilder<Item> builder = new SearchBuilder<Item>();
                    builder.Query.Must.AddQuery(
                                                item => item.CrawlID, ActionType.Equal,
                                                crawl.CrawlID);
                    builder.AddSort(item => item.PubDate, SortOrder.Desc);
                    builder.AddField(item => item.PubDate, item => item.ItemID);
                    builder.Take(50);
                    var items =  ESItemAccess.Search(builder);
                    if (items.Count() != 50)
                    {
                        continue;
                        
                    }
                    var dayGroups = items.GroupBy(item => (item.PubDate ?? DateTime.MinValue).DayOfYear);
                    var todayDay = DateTime.Now.DayOfYear;
                    var baseIntervalMin = crawl.IntervalMins > 1440 ? 1440 : crawl.IntervalMins;
                    var adjustInterval = baseIntervalMin;
                    bool needAdjust = dayGroups.Any(grouping => grouping.Count() > 20&& todayDay - grouping.Key < 7);
                    using (PalasDB updatedDB = new PalasDB())
                    {
                        var updatedCrawl = updatedDB.Crawl.First(uCrawl => uCrawl.CrawlID == crawl.CrawlID);
                        if (needAdjust)
                        {
                            updatedCrawl.IntervalMins = adjustInterval / 2;
                        }
                        
                        
                        updatedCrawl.Priority = 1;
                        updatedDB.SaveChanges();
                    }
                    

                }
            }

        }

        private const string _analyzeDataUrl = "net.tcp://localhost:10229";
        private const long _maxRecvMessageSize = 1024000000;
        private ServiceHost _analyzeHost;
        private AnalyzeTask _analyzeTaskSvc;
        private TaskFactory _backGroundTaskFactory;
        private void TestAnalyzeHost()
        {
            //创建分析监听
            _analyzeHost = WCFUtility.CreateWCFHost<AnalyzeTask>
                                                     (
                                                      _analyzeDataUrl,
                                                      new NetTcpBinding(SecurityMode.None)
                                                      {
                                                          MaxReceivedMessageSize = _maxRecvMessageSize,
                                                          CloseTimeout = TimeSpan.FromDays(3),
                                                          OpenTimeout = TimeSpan.FromDays(3),
                                                          ReceiveTimeout = TimeSpan.FromDays(3),
                                                          SendTimeout = TimeSpan.FromDays(3),
                                                          ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                                                      },
                                                      true);
            _analyzeHost.Open();
            _analyzeTaskSvc = (AnalyzeTask)_analyzeHost.SingletonInstance;
            //启动分析线程
            Thread thread = new Thread(ProcessAnalyzeTask);
            thread.Start();
            _backGroundTaskFactory = new TaskFactory(TaskScheduler.Default);

            
        }
        private static IAnalyzeTask _taskSendChannel = null;
        private const int _analyzeIdleSleepTime = 5000;
        private void ProcessAnalyzeTask()
        {
            while (true)
            {
                var task = _analyzeTaskSvc.GetAnalyzeTask();
                if (task == null)
                {
                    Thread.Sleep(_analyzeIdleSleepTime);
                }
                else
                {
                    _backGroundTaskFactory.StartNew(() => this.Invoke(new Action(
                                                                          () =>
                                                                              {
                                                                                  StatusLbl.Text =
                                                                                      task.Item.CleanTitle;
                                                                              })));
                }
            }
        }
        private class QueryDateSpan
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private void MigrationES()
        {

            Retry:
            var startDate = DeterminedSyncStartDate();
            var endDate = DeterminedSyncEndDate();
            QueryDateSpan[] timeSpans = AllDateSpan(startDate, endDate, 60);
            var parallelGroupDate = from item in timeSpans
                                    group item by item.StartDate.Date
                                    into dateGroup
                                    select dateGroup;
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 6;
            foreach (var dayGroup in parallelGroupDate)
            {
                var currentDate = dayGroup.Key.ToString("yyyy-MM-dd");
                try
                {
                    //foreach (QueryDateSpan queryDateSpan in dayGroup)
                    //{
                    //    MigrateSpanData(queryDateSpan);
                    //}
                    Parallel.ForEach(dayGroup, options,MigrateSpanData);
                    StatusLbl.Text = "正在导入:" + currentDate + "的数据";
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    Logger.Error("迁移异常,时间点为"+currentDate,ex);
                    DeleteData(dayGroup.Key);
                    goto Retry;
                    //throw;
                }
                

            }
            
                             


            
                
            
            
        }

        private static void DeleteData(DateTime date)
        {
            IConnectionSettings connection = new ConnectionSettings("222.73.242.112", ESConfig.Port)
                .SetDefaultIndex(ESConfig.DefaultIndex);
            ElasticClient client = new ElasticClient(connection);
            SearchBuilder<Item> builder = new SearchBuilder<Item>();
            builder.Query.Must.AddQuery(
                                        item => item.FetchTime,
                                        ActionType.GreatEqual,
                                        date.Date);
            builder.Query.Must.AddQuery(
                                        item => item.FetchTime,
                                        ActionType.Less,
                                        date.Date.AddDays(1));
            client.Delete(builder);
        }

        private DateTime DeterminedSyncEndDate()
        {
            SearchBuilder<Item> builder = new SearchBuilder<Item>();
            builder.Query.Must.AddQuery(item => item.FetchTime, ActionType.GreatEqual, new DateTime(2011, 1, 1));
            builder.AddSort(item => item.FetchTime, SortOrder.Desc);
            
            var result = ESItemAccess.SearchTop(builder);
            return result.FetchTime;

        }

        private DateTime DeterminedSyncStartDate()
        {
            IConnectionSettings connection = new ConnectionSettings("222.73.242.112", ESConfig.Port)
                .SetDefaultIndex(ESConfig.DefaultIndex);
            ElasticClient client = new ElasticClient(connection);
            SearchBuilder<Item> builder = new SearchBuilder<Item>();
            builder.Query.Must.AddQuery(item => item.FetchTime, ActionType.GreatEqual, new DateTime(2011, 1, 1));
            builder.AddSort(item => item.FetchTime, SortOrder.Desc);
            builder.Take(1);
            var result = client.Search(builder);
            var lastFetchTime = result.Documents.First().FetchTime;
            return lastFetchTime.AddSeconds(1);
        }

        private static void MigrateSpanData(QueryDateSpan queryDateSpan)
        {
            IConnectionSettings connection = new ConnectionSettings("222.73.242.112", ESConfig.Port)
                .SetDefaultIndex(ESConfig.DefaultIndex);
            ElasticClient client = new ElasticClient(connection);
            SearchBuilder<Item> builder = new SearchBuilder<Item>();
            builder.Query.Must.AddQuery(
                                        item => item.FetchTime,
                                        ActionType.GreatEqual,
                                        queryDateSpan.StartDate);
            builder.Query.Must.AddQuery(
                                        item => item.FetchTime,
                                        ActionType.Less,
                                        queryDateSpan.EndDate);
            int count = 0;
            try
            {
                
                count = ESItemAccess.Count(builder);
                if (count == 0)
                {
                    return;
                }
            }
            catch (Exception)
            {

                count = ESItemAccess.Count(builder);
            }
            
            builder.Take(count);
            Item[] result = new Item[]{};
            try
            {
                result = ESItemAccess.Search(builder);
            }
            catch (Exception)
            {

                result = ESItemAccess.Search(builder);
            }
            

            foreach (Item item in result)
            {
                client.Index(item);
            }
        }

        private QueryDateSpan[] AllDateSpan(DateTime startDate, DateTime endDate, int spanMinutes)
        {
            var sDate = startDate;
            List<QueryDateSpan> resultList = new List<QueryDateSpan>();
            while (sDate < endDate)
            {
                var endSpanDate = sDate.AddMinutes(spanMinutes);
                var date = new QueryDateSpan {StartDate = sDate, EndDate = endSpanDate};
                sDate = endSpanDate;
                resultList.Add(date);
            }
            return resultList.ToArray();
        }

        private void GetSinaPeople()
        {
            Workbook book = new Workbook();
            book.Open(@"d:\temp\input.xlsx");
            var dataSheet = book.Worksheets[0];
            int rowPos = 1;
            Crawler.Core.Utility.WeiboUtility.MaxRequestCountPerLogin = 15;
            while (!string.IsNullOrEmpty(dataSheet.Cells[rowPos, 0].StringValue))
            {
                var uid = dataSheet.Cells[rowPos, 0].StringValue;
                try
                {
                    var people = Crawler.Core.Utility.WeiboUtility.GetSinglePeopleFromUid(uid);
                    if (!string.IsNullOrEmpty(people.Education) || !string.IsNullOrEmpty(people.Company))
                    {
                        dataSheet.Cells[rowPos, 10].PutValue(people.Education);
                        dataSheet.Cells[rowPos, 11].PutValue(people.Company);
                        book.Save(@"d:\temp\output.xls");
                    }
                    
                }
                catch 
                {
                    
                    
                }
                
                StatusLbl.Text = "已运行" + rowPos + "条";
                rowPos++;
                Application.DoEvents();
            }

        }

        private void SearchFudanWeiboPeople()
        {
            string[] addtionQuerys = new string[]
                                         {
                                             
                                             //"region=custom:34:1000", //安徽            720
                                             //"region=custom:11:1",//北京东城区          465 
                                             //"region=custom:11:2",//北京西城区           253
                                             //"region=custom:11:3",//北京崇文区           33
                                             //"region=custom:11:4",//北京-宣武区          58
                                             //"region=custom:11:5",//北京-朝阳区          700
                                             //"region=custom:11:6",//北京-丰台区          53
                                             //"region=custom:11:7",//北京-石景山区        18 
                                             //"region=custom:11:8",//北京-海淀区          600
                                             //"region=custom:11:9",//北京-门头沟区         4
                                             //"region=custom:11:11",//北京-房山区         5
                                             //"region=custom:11:12",//北京-通州区         14
                                             //"region=custom:11:13",//北京-顺义区         17
                                             //"region=custom:11:14",//北京-昌平区         36
                                             //"region=custom:11:15",//北京-大兴区         32
                                             //"region=custom:11:16",//北京-怀柔区         3
                                             //"region=custom:11:17",//北京-平谷区         4
                                             //"region=custom:11:28",//北京-密云县         4
                                             //"region=custom:11:29",//北京-延庆县         3
                                             //"region=custom:50:1000&gender=man",//重庆男 970
                                             //"region=custom:50:1000&gender=women&auth=ord",//重庆普通女  1000 
                                             //"region=custom:50:1000&gender=women&auth=vip",//重庆认证女  2
                                             //"region=custom:35:1000&gender=women",//福建女 414 
                                             //"region=custom:35:1000&gender=man",//福建男 620 
                                             //"region=custom:62:1000",//甘肃 165
                                             //"region=custom:44:1&gender=man",//广东-广州男 600 
                                             //"region=custom:44:1&gender=women",//广东-广州女 396 不全
                                             //"region=custom:44:2",//广东-韶关 22
                                             //"region=custom:44:3&gender=man",//广东-深圳男 720
                                             //"region=custom:44:3&gender=women",//广东-深圳女 354
                                             //"region=custom:44:4",//广东-珠海 72
                                             //"region=custom:44:5",//广东-汕头 103
                                             //"region=custom:44:6",//广东-佛山 173
                                             //"region=custom:44:7",//广东-江门 71
                                             //"region=custom:44:8",//广东-湛江 52
                                             //"region=custom:44:9",//广东-茂名 39
                                             //"region=custom:44:12",//广东-肇庆 42
                                             //"region=custom:44:13",//广东-惠州 76
                                             //"region=custom:44:14",//广东-梅州 29 
                                             //"region=custom:44:15",//广东-汕尾 38 
                                             //"region=custom:44:16",//广东-河源 19 
                                             //"region=custom:44:17",//广东-阳江 24 
                                             //"region=custom:44:18",//广东-清远 27 
                                             //"region=custom:44:19",//广东-东莞 166 
                                             //"region=custom:44:20",//广东-中山 88 
                                             //"region=custom:44:51",//广东-潮州 37 
                                             //"region=custom:44:52",//广东-揭阳 38 
                                             //"region=custom:44:53",//广东-云浮 26 
                                             //"region=custom:45:1000",//广西 487 
                                             //"region=custom:52:1000",//贵州 344
                                             //"region=custom:46:1000",//海南 230
                                             //"region=custom:13:1000",//河北 453
                                             //"region=custom:23:1000",//黑龙江 358
                                             //"region=custom:41:1000",//河南 940 
                                             //"region=custom:42:1000",//湖北 600 
                                             //"region=custom:43:1000",//湖南 620 
                                             //"region=custom:15:1000",//内蒙古 172 
                                             
                                             
                                             //"region=custom:32:1",//江苏-南京 540
                                             //"region=custom:32:2",//江苏-无锡 304
                                             //"region=custom:32:3",//江苏-徐州 106
                                             //"region=custom:32:4",//江苏-常州 175
                                             //"region=custom:32:5",//江苏-苏州 640
                                             //"region=custom:32:6",//江苏-南通 173
                                             //"region=custom:32:7",//江苏-连云港 64
                                             //"region=custom:32:8",//江苏-淮安 34
                                             //"region=custom:32:9",//江苏-盐城 81
                                             //"region=custom:32:10",//江苏-扬州 95
                                             //"region=custom:32:11",//江苏-镇江 53
                                             //"region=custom:32:12",//江苏-泰州 76
                                             //"region=custom:32:13",//江苏-宿迁 29
                                             //"region=custom:36:1000",//江西 520
                                             //"region=custom:22:1000",//吉林 222
                                             //"region=custom:21:1000",//辽宁 560
                                             //"region=custom:64:1000",//宁夏 87
                                             //"region=custom:63:1000",//青海 60
                                             //"region=custom:14:1000",//山西 330
                                             //"region=custom:37:1000&gender=man",//山东男 780
                                             //"region=custom:37:1000&gender=women",//山东女 540
                                             
                                             //"region=custom:31:1&gender=women&auth=vip",//上海-黄浦区认证女 46
                                             //"region=custom:31:1&gender=women&auth=ord&age=18y",//上海-黄浦区普通女18y 108
                                             //"region=custom:31:1&gender=women&auth=ord&age=22y",//上海-黄浦区普通女22y 258
                                             //"region=custom:31:1&gender=women&auth=ord&age=29y",//上海-黄浦区普通女29y 620
                                             //"region=custom:31:1&gender=women&auth=ord&age=39y",//上海-黄浦区普通女39y 172
                                             //"region=custom:31:1&gender=women&auth=ord&age=40y",//上海-黄浦区普通女40y 1000
                                             //"region=custom:31:1&gender=man&auth=vip",//上海-黄浦区认证男 83
                                             //"region=custom:31:1&gender=man&auth=ord&age=18y",//上海-黄浦区普通男18y 79
                                             //"region=custom:31:1&gender=man&auth=ord&age=22y",//上海-黄浦区普通男22y 163
                                             //"region=custom:31:1&gender=man&auth=ord&age=29y",//上海-黄浦区普通男29y 540
                                             //"region=custom:31:1&gender=man&auth=ord&age=39y",//上海-黄浦区普通男39y 289
                                             //"region=custom:31:1&gender=man&auth=ord&age=40y",//上海-黄浦区普通男40y 1000
                                             //"region=custom:31:3&gender=man",//上海-卢湾区男 600
                                             //"region=custom:31:3&gender=women",//上海-卢湾区女 600
                                             //"region=custom:31:4&gender=women&auth=vip",//上海-徐汇区认证女 69
                                             //"region=custom:31:4&gender=women&auth=ord&age=18y",//上海-徐汇区普通女18y 89
                                             //"region=custom:31:4&gender=women&auth=ord&age=22y",//上海-徐汇区普通女22y 260
                                             //"region=custom:31:4&gender=women&auth=ord&age=29y",//上海-徐汇区普通女29y 1000
                                             //"region=custom:31:4&gender=women&auth=ord&age=39y",//上海-徐汇区普通女39y 294
                                             //"region=custom:31:4&gender=women&auth=ord&age=40y",//上海-徐汇区普通女40y 1000
                                             //"region=custom:31:4&gender=man&auth=ord&age=18y",//上海-徐汇区普通男18y 56
                                             //"region=custom:31:4&gender=man&auth=ord&age=22y",//上海-徐汇区普通男22y 181
                                             //"region=custom:31:4&gender=man&auth=ord&age=29y",//上海-徐汇区普通男29y 780
                                             //"region=custom:31:4&gender=man&auth=ord&age=39y",//上海-徐汇区普通男39y 375
                                             //"region=custom:31:4&gender=man&auth=ord&age=40y",//上海-徐汇区普通男40y 1000
                                             //"region=custom:31:4&gender=man&auth=vip",//上海-徐汇区认证男 132
                                             //"region=custom:31:5&gender=man&auth=vip",//上海-长宁区认证男 74
                                             //"region=custom:31:5&gender=women&auth=ord&age=18y",//上海-长宁区普通女18y 133
                                             //"region=custom:31:5&gender=women&auth=ord&age=22y",//上海-长宁区普通女22y 271
                                             //"region=custom:31:5&gender=women&auth=ord&age=29y",//上海-长宁区普通女29y 520
                                             //"region=custom:31:5&gender=women&auth=ord&age=39y",//上海-长宁区普通女39y 148
                                             //"region=custom:31:5&gender=women&auth=ord&age=40y",//上海-长宁区普通女40y 680
                                             //"region=custom:31:5&gender=man&auth=ord&age=18y",//上海-长宁区普通男18y 72
                                             //"region=custom:31:5&gender=man&auth=ord&age=22y",//上海-长宁区普通男22y 164
                                             //"region=custom:31:5&gender=man&auth=ord&age=29y",//上海-长宁区普通男29y 435
                                             //"region=custom:31:5&gender=man&auth=ord&age=39y",//上海-长宁区普通男39y 241
                                             //"region=custom:31:5&gender=man&auth=ord&age=40y",//上海-长宁区普通男40y 920
                                             //"region=custom:31:5&gender=women&auth=vip",//上海-长宁区认证女 39
                                             //"region=custom:31:6&gender=women&auth=vip",//上海-静安区认证女 39
                                             
                                             
                                             //"region=custom:31:6&gender=women&auth=ord&age=18y",//上海-静安区普通女18y 59 
                                             //"region=custom:31:6&gender=women&auth=ord&age=22y",//上海-静安区普通女22y 111 
                                             //"region=custom:31:6&gender=women&auth=ord&age=29y",//上海-静安区普通女29y 306 
                                             //"region=custom:31:6&gender=women&auth=ord&age=39y",//上海-静安区普通女39y 126 
                                             //"region=custom:31:6&gender=women&auth=ord&age=40y",//上海-静安区普通女40y 520 
                                             //"region=custom:31:6&gender=man&auth=ord&age=18y",//上海-静安区普通男18y 33 
                                             //"region=custom:31:6&gender=man&auth=ord&age=22y",//上海-静安区普通男22y 63 
                                             //"region=custom:31:6&gender=man&auth=ord&age=29y",//上海-静安区普通男29y 239 
                                             //"region=custom:31:6&gender=man&auth=ord&age=39y",//上海-静安区普通男39y 141 
                                             //"region=custom:31:6&gender=man&auth=ord&age=40y",//上海-静安区普通男40y 560 
                                             
                                             //"region=custom:31:6&gender=man&auth=vip",//上海-静安区认证男 68
                                             //"region=custom:31:7&gender=man&auth=vip",//上海-普陀区认证男 44
                                             //"region=custom:31:7&gender=women&auth=ord&age=18y",//上海-普陀区普通女18y 35
                                             //"region=custom:31:7&gender=women&auth=ord&age=22y",//上海-普陀区普通女22y 125
                                             //"region=custom:31:7&gender=women&auth=ord&age=29y",//上海-普陀区普通女29y 443
                                             //"region=custom:31:7&gender=women&auth=ord&age=39y",//上海-普陀区普通女39y 110
                                             //"region=custom:31:7&gender=women&auth=ord&age=40y",//上海-普陀区普通女40y 440
                                             //"region=custom:31:7&gender=man&auth=ord&age=18y",//上海-普陀区普通男18y 35
                                             //"region=custom:31:7&gender=man&auth=ord&age=22y",//上海-普陀区普通男22y 72
                                             //"region=custom:31:7&gender=man&auth=ord&age=29y",//上海-普陀区普通男29y 346
                                             //"region=custom:31:7&gender=man&auth=ord&age=39y",//上海-普陀区普通男39y 197
                                             //"region=custom:31:7&gender=man&auth=ord&age=40y",//上海-普陀区普通男40y 620
                                             //"region=custom:31:7&gender=women&auth=vip",//上海-普陀区认证女 20
                                             //"region=custom:31:8&gender=women",//上海-闸北区普通女 731
                                             //"region=custom:31:8&gender=man",//上海-闸北区男 900
                                             //"region=custom:31:9&gender=man&auth=vip",//上海-虹口区认证男 57
                                             //"region=custom:31:9&gender=women&auth=ord&age=18y",//上海-虹口区普通女18y 53
                                             //"region=custom:31:9&gender=women&auth=ord&age=22y",//上海-虹口区普通女22y 203
                                             //"region=custom:31:9&gender=women&auth=ord&age=29y",//上海-虹口区普通女29y 520
                                             //"region=custom:31:9&gender=women&auth=ord&age=39y",//上海-虹口区普通女39y 141
                                             //"region=custom:31:9&gender=women&auth=ord&age=40y",//上海-虹口区普通女40y 560
                                             //"region=custom:31:9&gender=man&auth=ord&age=18y",//上海-虹口区普通男18y 48
                                             //"region=custom:31:9&gender=man&auth=ord&age=22y",//上海-虹口区普通男22y 107
                                             //"region=custom:31:9&gender=man&auth=ord&age=29y",//上海-虹口区普通男29y 425
                                             //"region=custom:31:9&gender=man&auth=ord&age=39y",//上海-虹口区普通男39y 233
                                             //"region=custom:31:9&gender=man&auth=ord&age=40y",//上海-虹口区普通男40y 760
                                             //"region=custom:31:9&gender=women&auth=vip",//上海-虹口区认证女 31
                                             
                                             //"region=custom:31:10&gender=women&auth=ord&age=18y",//上海-杨浦区普通女18y 446
                                             //"region=custom:31:10&gender=women&auth=ord&age=22y",//上海-杨浦区普通女22y 1000
                                             //"region=custom:31:10&gender=women&auth=ord&age=29y",//上海-杨浦区普通女29y 1000
                                             //"region=custom:31:10&gender=women&auth=ord&age=39y",//上海-杨浦区普通女39y 298
                                             //"region=custom:31:10&gender=women&auth=ord&age=40y",//上海-杨浦区普通女40y 1000
                                             //"region=custom:31:10&gender=man&auth=ord&age=18y",//上海-杨浦区普通男18y 330
                                             //"region=custom:31:10&gender=man&auth=ord&age=22y",//上海-杨浦区普通男22y 940
                                             //"region=custom:31:10&gender=man&auth=ord&age=29y",//上海-杨浦区普通男29y 1000
                                             //"region=custom:31:10&gender=man&auth=ord&age=39y",//上海-杨浦区普通男39y 520
                                             //"region=custom:31:10&gender=man&auth=ord&age=40y",//上海-杨浦区普通男40y 1000
                                             //"region=custom:31:10&gender=women&auth=vip",//上海-杨浦区认证女 79
                                             //"region=custom:31:10&gender=man&auth=vip",//上海-杨浦区认证男 166
                                             //"region=custom:31:12&gender=women&auth=ord&age=18y",//上海-闵行区普通女18y 114
                                             //"region=custom:31:12&gender=women&auth=ord&age=22y",//上海-闵行区普通女22y 178
                                             //"region=custom:31:12&gender=women&auth=ord&age=29y",//上海-闵行区普通女29y 418
                                             //"region=custom:31:12&gender=women&auth=ord&age=39y",//上海-闵行区普通女39y 152
                                             //"region=custom:31:12&gender=women&auth=ord&age=40y",//上海-闵行区普通女40y 700
                                             //"region=custom:31:12&gender=man&auth=ord&age=18y",//上海-闵行区普通男18y 71
                                             //"region=custom:31:12&gender=man&auth=ord&age=22y",//上海-闵行区普通男22y 114
                                             //"region=custom:31:12&gender=man&auth=ord&age=29y",//上海-闵行区普通男29y 359
                                             //"region=custom:31:12&gender=man&auth=ord&age=39y",//上海-闵行区普通男39y 222
                                             //"region=custom:31:12&gender=man&auth=ord&age=40y",//上海-闵行区普通男40y 980
                                             //"region=custom:31:12&gender=man&auth=vip",//上海-闵行区认证男 34
                                             //"region=custom:31:12&gender=women&auth=vip",//上海-闵行区认证女 15
                                             //"region=custom:31:13&gender=women",//上海-宝山区女 880
                                             //"region=custom:31:13&gender=man&auth=ord",//上海-宝山区普通男 1000
                                             //"region=custom:31:13&gender=man&auth=vip",//上海-宝山区认证男 22
                                             //"region=custom:31:14&gender=man",//上海-嘉定区男 600
                                             //"region=custom:31:14&gender=women",//上海-嘉定区女 391
                                             //"region=custom:31:15&gender=women&auth=ord&age=18y",//上海-浦东新区区普通女18y 155
                                             //"region=custom:31:15&gender=women&auth=ord&age=22y",//上海-浦东新区区普通女22y 426
                                             //"region=custom:31:15&gender=women&auth=ord&age=29y",//上海-浦东新区区普通女29y 1000
                                             //"region=custom:31:15&gender=women&auth=ord&age=39y",//上海-浦东新区区普通女39y 469
                                             //"region=custom:31:15&gender=women&auth=ord&age=40y",//上海-浦东新区区普通女40y 1000
                                             //"region=custom:31:15&gender=man&auth=ord&age=18y",//上海-浦东新区区普通男18y 126
                                             //"region=custom:31:15&gender=man&auth=ord&age=22y",//上海-浦东新区区普通男22y 285
                                             //"region=custom:31:15&gender=man&auth=ord&age=29y",//上海-浦东新区区普通男29y 1000
                                             //"region=custom:31:15&gender=man&auth=ord&age=39y",//上海-浦东新区区普通男39y 920
                                             //"region=custom:31:15&gender=man&auth=ord&age=40y",//上海-浦东新区区普通男40y 1000
                                             //"region=custom:31:15&gender=women&auth=vip",//上海-浦东新区认证女 91
                                             //"region=custom:31:15&gender=man&auth=vip",//上海-浦东新区认证男 183
                                             //"region=custom:31:16&gender=man",//上海-金山区男 263
                                             //"region=custom:31:16&gender=women",//上海-金山区女 231
                                             //"region=custom:31:17&gender=women&auth=ord&age=18y",//上海-松江区普通女18y 48
                                             //"region=custom:31:17&gender=women&auth=ord&age=22y",//上海-松江区普通女22y 404
                                             //"region=custom:31:17&gender=women&auth=ord&age=29y",//上海-松江区普通女29y 307
                                             //"region=custom:31:17&gender=women&auth=ord&age=39y",//上海-松江区普通女39y 30
                                             //"region=custom:31:17&gender=women&auth=ord&age=40y",//上海-松江区普通女40y 416
                                             //"region=custom:31:17&gender=man&auth=ord&age=18y",//上海-松江区普通男18y 36
                                             //"region=custom:31:17&gender=man&auth=ord&age=22y",//上海-松江区普通男22y 183
                                             //"region=custom:31:17&gender=man&auth=ord&age=29y",//上海-松江区普通男29y 301
                                             //"region=custom:31:17&gender=man&auth=ord&age=39y",//上海-松江区普通男39y 50
                                             //"region=custom:31:17&gender=man&auth=ord&age=40y",//上海-松江区普通男40y 660
                                             //"region=custom:31:17&gender=women&auth=vip",//上海-松江区认证女 14
                                             
                                             //"region=custom:31:17&gender=man&auth=vip",//上海-松江区认证男 13
                                             //"region=custom:31:18",//上海-青浦区 720
                                             //"region=custom:31:19",//上海-南汇区 438
                                             //"region=custom:31:20",//上海-奉贤区 438
                                             //"region=custom:31:30",//上海-崇明县 265
                                             //"region=custom:51:1000",//四川 980
                                             //"region=custom:12:1000",//天津 272
                                             //"region=custom:54:1000",//西藏 83
                                             //"region=custom:65:1000",//新疆 217
                                             //"region=custom:53:1000",//云南 364 
                                             //"region=custom:33:1&gender=man",//浙江-杭州男 620
                                             //"region=custom:33:1&gender=women",//浙江-杭州女 405
                                             //"region=custom:33:2",//浙江-宁波 423
                                             //"region=custom:33:3",//浙江-温州 405
                                             //"region=custom:33:4",//浙江-嘉兴 159
                                             //"region=custom:33:5", //浙江-湖州 57
                                             //"region=custom:33:6", //浙江-绍兴 138
                                             //"region=custom:33:7", //浙江-金华 123
                                             //"region=custom:33:8", //浙江-衢州 36
                                             //"region=custom:33:9", //浙江-舟山 59
                                             //"region=custom:33:10", //浙江-台州 151
                                             //"region=custom:33:11", //浙江-丽水 33
                                             //"region=custom:61:1000", //陕西 398
                                             //"region=custom:71:1000", //台湾 397
                                             //"region=custom:81:1000", //香港 640
                                             //"region=custom:82:1000", //澳门 86
                                             
                                             //"region=custom:400:1&gender=man&auth=ord", //海外-美国普通男 980
                                             //"region=custom:400:1&gender=man&auth=vip", //海外-美国认证男 12
                                             //"region=custom:400:1&gender=women&auth=ord", //海外-美国普通女  1000
                                             //"region=custom:400:1&gender=women&auth=vip", //海外-美国认证女 18
                                             //"region=custom:400:2", //海外-英国 401
                                             //"region=custom:400:3", //海外-法国 223 
                                             //"region=custom:400:4", //海外-俄罗斯 13
                                             //"region=custom:400:5", //海外-加拿大 236
                                             //"region=custom:400:6", //海外-巴西 13
                                             //"region=custom:400:7", //海外-澳大利亚 270
                                             //"region=custom:400:8", //海外-印尼 18
                                             //"region=custom:400:9", //海外-泰国 35
                                             //"region=custom:400:10", //海外-马来西亚 67
                                             //"region=custom:400:11", //海外-新加坡 250
                                             //"region=custom:400:12", //海外-菲律宾 16
                                             //"region=custom:400:13", //海外-越南 17
                                             //"region=custom:400:14", //海外-印度 7
                                             //"region=custom:400:15", //海外-日本 368
                                             //"region=custom:400:16", //海外-其他 176
                                             //"region=custom:100:1000&gender=women&auth=ord&age=18y",//其他普通女18y 218
                                             //"region=custom:100:1000&gender=women&auth=ord&age=22y",//其他普通女22y 228
                                             //"region=custom:100:1000&gender=women&auth=ord&age=29y",//其他普通女29y 292
                                             //"region=custom:100:1000&gender=women&auth=ord&age=39y",//其他普通女39y 47
                                             //"region=custom:100:1000&gender=women&auth=ord&age=40y",//其他普通女40y 1000
                                             //"region=custom:100:1000&gender=man&auth=ord&age=18y",//其他普通男18y 190
                                             //"region=custom:100:1000&gender=man&auth=ord&age=22y",//其他普通男22y 180
                                             //"region=custom:100:1000&gender=man&auth=ord&age=29y",//其他普通男29y 271
                                             //"region=custom:100:1000&gender=man&auth=ord&age=39y",//其他普通男39y 87
                                             //"region=custom:100:1000&gender=man&auth=ord&age=40y",//其他普通男40y 1000
                                             
                                             //"region=custom:100:1000&gender=man&auth=vip", //其他认证男 23
                                             
                                             //"region=custom:100:1000&gender=women&auth=vip", //其他认证女 10
                                             

                                         };
            string exportPath = "D:\\temp\\fudan.xlsx";
            Workbook book = new Workbook();
            var dataSheet = book.Worksheets.Add("复旦");
            dataSheet.Cells[0, 0].PutValue("Uid");
            dataSheet.Cells[0, 1].PutValue("Url");
            dataSheet.Cells[0, 2].PutValue("名字");
            dataSheet.Cells[0, 3].PutValue("标签");
            dataSheet.Cells[0, 4].PutValue("区域");
            dataSheet.Cells[0, 5].PutValue("性别");
            dataSheet.Cells[0, 6].PutValue("新浪认证");
            dataSheet.Cells[0, 7].PutValue("关注数");
            dataSheet.Cells[0, 8].PutValue("粉丝数");
            dataSheet.Cells[0, 9].PutValue("微博数");
            dataSheet.Cells[0, 10].PutValue("用户摘要");
            dataSheet.Cells[0, 11].PutValue("简介");
            dataSheet.Cells[0, 12].PutValue("教育信息");
            dataSheet.Cells[0, 13].PutValue("工作信息");
            int rowPos = 1;
            foreach (string addtionQuery in addtionQuerys)
            {
                var peoples = Crawler.Core.Utility.WeiboUtility.SearchPeople("复旦", 0, 50, addtionQuery: addtionQuery);
                foreach (var item in peoples)
                {
                    ExportSinaPeopleToExcel(dataSheet, rowPos, item);
                    rowPos++;
                }
                StatusLbl.Text = "当前正在抓取:" + addtionQuery;
                Application.DoEvents();
                book.Save(exportPath);
            }
            

        }

        private static void ExportSinaPeopleToExcel(Worksheet dataSheet, int rowPos, SinaPeople item)
        {
            dataSheet.Cells[rowPos, 0].PutValue(item.Uid);
            dataSheet.Cells[rowPos, 1].PutValue(item.Url);
            dataSheet.Cells[rowPos, 2].PutValue(item.Name);
            dataSheet.Cells[rowPos, 3].PutValue(item.Tag);
            dataSheet.Cells[rowPos, 4].PutValue(item.Region);
            switch (item.Gender)
            {
                case SinaPeopleGender.Female:
                    {
                        dataSheet.Cells[rowPos, 5].PutValue("女");
                        break;
                    }
                case SinaPeopleGender.Male:
                    {
                        dataSheet.Cells[rowPos, 5].PutValue("男");
                        break;
                    }
                default:
                    {
                        dataSheet.Cells[rowPos, 5].PutValue("未填写");
                        break;
                    }
            }
            switch (item.Certificate)
            {
                case SinaPeopleCertificate.Organization:
                    {
                        dataSheet.Cells[rowPos, 6].PutValue("机构认证");
                        break;
                    }
                case SinaPeopleCertificate.Person:
                    {
                        dataSheet.Cells[rowPos, 6].PutValue("个人认证");
                        break;
                    }
                default:
                    {
                        dataSheet.Cells[rowPos, 6].PutValue("无认证");
                        break;
                    }
            }


            dataSheet.Cells[rowPos, 7].PutValue(item.Follow);
            dataSheet.Cells[rowPos, 8].PutValue(item.Follower);
            dataSheet.Cells[rowPos, 9].PutValue(item.Tweet);
            dataSheet.Cells[rowPos, 10].PutValue(item.Summary);
            dataSheet.Cells[rowPos, 11].PutValue(item.Desc);
            dataSheet.Cells[rowPos, 12].PutValue(item.Education);
            dataSheet.Cells[rowPos, 13].PutValue(item.Job);
        }

        private void GenerateWeiboSearchPeopleReport()
        {
            
        }
        private void GenerateWeiboReport()
        {
            string[] users = new string[]
                {
                    "济南公安",
"上海发布",
"联合国",
"上海地铁shmetro",
"北京发布",
"美国驻港总领事馆",
"成都发布",
"美国驻华大使馆",
"警民直通车-上海",
"上海铁警发布",
"微博贵州",
"新疆发布",
"首都健康",
"武汉食品药品监管",
"江宁公安在线",
"重庆共青团-青年近卫军",
"成都共青",
"青年说",
"河北共青",
"香港政府新闻网",
"高雄市政府观光局",
"新北市政府观光旅游局",
"头条新闻",
"财经网",
"新闻晨报",
"南方都市报",
"微天下",
"人民日报",
"扬子晚报",
"广州日报",
"南方周末",
"新京报",
"经济观察",
"南方日报",
"每日经济",
"21世纪经济报道",
"楚天都市报",
"新周刊",
"中国新闻",
"南风窗",
"凤凰卫视",
"辽宁卫视",
"东方卫视",
"山东卫视",
"法新社",
"美联社",
"日本共同社",
"俄新社",
"人民网",
"FT中文网",
"新民周刊",
"第一财经",
"美国全国公共广播电台",
"邓飞",
"韩志国",
"何兵",
"贺卫方",
"加藤喜一",
"嘉措活佛",
"老榕",
"老徐时评",
"李承鹏",
"李开复",
"刘向南",
"罗永浩可爱多",
"宁财神",
"潘石屹",
"任志强",
"芮成钢",
"西门不暗",
"薛蛮子",
"姚晨",
"于建嵘",
"袁裕来律师",
"赵楚",
"左小祖咒",
"作业本",
"刘萍196412",
"江西新余魏忠平",
"江西新余李学梅",
"选举日报",
"Lucy若水",
"陈有西",
"律坛怪侠杨金柱",
"五岳散人",
"韩寒",
"路透中文网Reuters",
"占海特",
"庐山花开",
"HIV志愿者",
"夏商",
"胡常根57",
"茅于轼",
"徐昕",
"韩青子",
"文史参考",
"喻尘",
"南都读书俱乐部",
"狼化石杨光",
"林楚方",
"元芳视角",
"何镇飚",
"陈志武",
"汉理资本_钱学锋",
"曹国伟",
"刘强东",
"正和岛刘东华",
"胡德平",
"高梅MeiGao",
"李亦非",
"曹辉宁",
"财新新世纪周刊",
"北京崔卫平",
"光头王凯",
"北京柳红",
"刘晓光",
"李庄",
"王利芬",
"新望",
"俞靓NewYork",
"黎彦修Matthew_Li",
"王石",
"展江",
"闾丘露薇",
"吴稼祥",
"吴法天",
"点子正",
"王克勤",
"龚晓跃",
"张力奋",
"郎咸平",
"吴敬琏",
"谢国忠",
"许小年",
"叶檀",
"方舟子",
"笑蜀",
"押沙龙",
"加藤嘉一",
"孔庆东",
"梁文道",
"李银河",
"杨恒均",
"土家野夫"
                };
            Workbook book = new Workbook();
            int progress = 0;
            foreach (var user in users)
            {
                progress++;
                //总共翻20页,取2000条数据
                var sheet = book.Worksheets.Add(user);
                var userInfo = WeiboAPIUtility.Client.GetUser(user);
                if (userInfo == null)
                {
                    continue;
                }
                var userid = userInfo.id;

                List<WeiboReport> weiboList = new List<WeiboReport>();
                sheet.Cells[0,0].PutValue("Url");
                sheet.Cells[0, 1].PutValue("微博");
                sheet.Cells[0, 2].PutValue("发布日期");
                sheet.Cells[0, 3].PutValue("转发数");
                sheet.Cells[0, 4].PutValue("评论数");
                for(int page=1;page<= 20;page++)
                {
                    var result = WeiboAPIUtility.Client.GetUserWeibo(user,pageNum: page);
                    var weiboArray = result.statuses;
                    int currentNum = 0;
                    foreach (var weibo in weiboArray)
                    {
                        WeiboReport report = new WeiboReport()
                            {
                                Url = WeiboAPIUtility.Client.ID2sinaWburl(long.Parse(weibo.id), userid.ToString()),
                                PubDate = WeiboAPIUtility.Client.TransDate(weibo.created_at),
                                Weibo = weibo.text,
                                Forward = int.Parse(weibo.reposts_count),
                                Comment = int.Parse(weibo.comments_count),
                            };
                        weiboList.Add(report);
                        currentNum++;
                    }
                    if (currentNum<99)
                    {
                        break;
                    }
                }
                var topResult = weiboList.Distinct().OrderByDescending(model => model.Forward).Take(10);
                int rowPos = 1;
                foreach (var item in topResult)
                {
                    
                    

                    sheet.Cells[rowPos, 0].PutValue(item.Url);
                    sheet.Cells[rowPos, 1].PutValue(item.Weibo);
                    sheet.Cells[rowPos, 2].PutValue(item.PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sheet.Cells[rowPos, 3].PutValue(item.Forward.ToString());
                    sheet.Cells[rowPos, 4].PutValue(item.Comment.ToString());
                    rowPos++;

                }


            }
            book.Save("d:\\temp\\weibo.xlsx");
            
           

        }

        private void ImportPGIGoogleCrawl()
        {
            var benchCrawl = CrawlBusiness.GetByCrawlID("ada84ba917394394903ee5445b07682c");
            benchCrawl.SiteID = "179e936a3589414c882b816ca95301a3";
            benchCrawl.InitRequiredCount = 3000;
            benchCrawl.NextCrawlTime = new DateTime(2012,11,1);
            benchCrawl.LastItemID = null;
            Workbook workbook = new Workbook();
            workbook.Open("E:\\pgi.xlsx");
            var keywordSheet = workbook.Worksheets[0];
            var mediaSheet = workbook.Worksheets[1];
            List<string> keywordsList = new List<string>(); //关键词列表
            int keywordSheetRowPos = 0;
            //初始化关键字列表
            while(!string.IsNullOrEmpty(keywordSheet.Cells[keywordSheetRowPos,0].StringValue))
            {
                var cellValue = keywordSheet.Cells[keywordSheetRowPos, 0].StringValue;
                keywordsList.Add(cellValue);

                keywordSheetRowPos++;
            }
            int mediaSheetRowPos = 1;
            while(!string.IsNullOrEmpty(mediaSheet.Cells[mediaSheetRowPos,0].StringValue))
            {
                var mediaName = mediaSheet.Cells[mediaSheetRowPos, 0].StringValue.Trim();
                var mediaUrl = mediaSheet.Cells[mediaSheetRowPos, 1].StringValue.Trim();
                #region 生成关键词组并入库
                foreach (string keyword in keywordsList)
                {
                    var clonedCrawl = benchCrawl.SwallowClone();
                    clonedCrawl.CrawlID = CrawlEntity.NewGuid;
                    var queryKeyword = keyword + " site:" + mediaUrl;
                    clonedCrawl.KeywordQuery = queryKeyword;
                    clonedCrawl.Name = mediaName + keyword.Trim();
                    CrawlBusiness.Insert(clonedCrawl);
                }
                #endregion
                mediaSheetRowPos++;
            }



        }

        private void DeleteCrawlItem()
        {
            var deletedItem = ESItemAccess.SearchByOneField(model => model.CrawlID, ActionType.Equal, "8a7ec1cc5a4c492abdb5efbed29f11b4",
                                          0, 10000);
            foreach (var item in deletedItem)
            {
                GeneralItemAccess.Delete(item);
            }

        }

        private void ChangeItemAnalyzeData()
        {
            var destIssueID = "GM";
            var queryCrawlIDs = new string[]
                {
                    "d7f08c6b12454b42bb3f76583d897267",

                };
            foreach (var queryCrawlID in queryCrawlIDs)
            {
                SearchBuilder<Item> builder = new SearchBuilder<Item>();
                builder.Query.Must.AddQuery(model => model.CrawlID, ActionType.Equal, queryCrawlID);
                //builder.Query.Must.AddQuery(model => model.CrawlID, ActionType.Equal, "98d168ede9e944d683b5441bc11f8a7c");
                builder.Query.Must.AddQuery(model => model.FetchTime, ActionType.Great, new DateTime(2012, 10, 6));
                builder.Take(10000);
                builder.AddField(model => model.ItemID);

                var items = ESItemAccess.Search(builder);
                foreach (var item in items)
                {
                   
                    var rawItem = ESItemAccess.SearchByItemID(item.ItemID);
                    var analyzeData = rawItem.AnalyzeData;
                    if (analyzeData == null)
                    {
                        continue;
                    }
                    var newAnalyzeData = new List<ItemAnalyzeData>();
                    newAnalyzeData.AddRange(analyzeData);
                    newAnalyzeData.RemoveAll(model => model.IssueID == destIssueID);
                    var commonAnalyze = newAnalyzeData.FirstOrDefault(model => model.IssueID == "Common" || model.IssueID == "Test");
                    if (commonAnalyze == null)
                    {
                        continue;

                    }
                    var newAnalyze = new ItemAnalyzeData(commonAnalyze);

                    newAnalyze.IssueID = destIssueID;
                    var newOrgsData = new List<string>();
                    newOrgsData.Add(destIssueID);
                    if (commonAnalyze.OrganizationIDs != null)
                    {
                        newOrgsData.AddRange(commonAnalyze.OrganizationIDs);
                    }
                    newAnalyze.Product = new string[] { "中国-通用汽车-昂科拉ENCORE" };
                    rawItem.MediaID = "99d69bbe540f4fbcb3581f8e61a95f7d";
                    rawItem.MediaName = "汽车之家论坛";
                    rawItem.MediaChannel = "昂科拉";
                    
                    newAnalyze.OrganizationIDs = newOrgsData.ToArray();
                    newAnalyzeData.Add(newAnalyze);
                    rawItem.AnalyzeData = newAnalyzeData.ToArray();
                    
                    ESItemAccess.Update(rawItem);
                }
            }
            

        }

        private void TestLoadHourCount()
        {
            HourCounter hourCounter = new HourCounter();
            hourCounter = hourCounter.LoadFile("test.bry");

        }

        private HourCounter _test = null;
        private void TestHourCount()
        {
            _test = new HourCounter();
            _test.Tick(10);
            _test.IntervalSerialize(new TimeSpan(0, 1, 0), "test.bry");


        }

        private void TestResourceMonitor()
        {
            var result = ResourceMonitor.GetTotalCPUUsage();
            StatusLbl.Text = result.ToString("N2")+"%";
        }

        private void TestWCF()
        {
            
            InstanceContext instanceContext = new InstanceContext(new ServiceMonitorClient());
            DuplexChannelFactory<IStatusReportServer> channelFactory =
                new DuplexChannelFactory<IStatusReportServer>(
                    instanceContext,
                    new NetTcpBinding(SecurityMode.None), ConfigurationManager.AppSettings["WCFMonitorServer"]);
            _server = channelFactory.CreateChannel();
            _server.Connect(UrlTxt.Text,HostType.Crawler);

        }

        private static void ResetAllParentMedia()
        {
            PalasDB exeDB = new PalasDB();
            using (PalasDB db = new PalasDB())
            {
                var parentMedia = db.Media.Where(model => string.IsNullOrEmpty(model.Channel));
                string updateDateSql =
                    "update media set parentmediaid = '{0}' where medianame = '{1}' and channel is not null and channel !=''";
                foreach (var media in parentMedia)
                {
                    exeDB.ExecuteStoreCommand(string.Format(updateDateSql, media.MediaID, media.MediaName));
                }
            }
            exeDB.Dispose();
        }

        

        private void UpdateParentMediaID()
        {
            string[] needUpdateMedia = {
//                                           "中国财经报网",
//"中国工商报",
//"中国经济导报",
//"中国金融新闻网",
//"中国信息产业网",
//"财讯",
//"中央人民政府网",
//"期货日报网",
//"价值中国网",
//"春晖投行在线",
//"迈博汇金",
//"半岛都市报",
//"btv在线",
//"北京商网",
//"财华网",
//"法制晚报",
//"中国证监会",
//"理想在线",
//"期货日报网",
//"上海商报",
//"数米基金网",
//"同花顺",
//"信息早报",
//"中国产经新闻网",
//"私募排排网",
//"中国贸易新闻网",
"中国人民银行",
"中国上市公司协会",
"创业者家园",
"中国资本市场标准网",
"中国证券投资者保护网",
"投资中国网",
"巨潮资讯网"
                                       };
            using (PalasDB db = new PalasDB())
            {
                foreach (var item in needUpdateMedia)
                {
                    var parentMedia=db.Media.FirstOrDefault(
                        model => string.IsNullOrEmpty(model.ParentMediaID) && model.MediaName == item &&
                                 string.IsNullOrEmpty(model.Channel));
                    var sqlFmt =
                        @"UPDATE media
SET media.ParentMediaID = '{0}'
WHERE media.MediaName = '{1}' AND media.Channel != '' ";
                    var sqlExec = string.Format(sqlFmt, parentMedia.MediaID, parentMedia.MediaName);
                    db.ExecuteStoreCommand(sqlExec);
                }
            }

        }

        private void GetSum()
        {
            int sum = 0;
            for (int i = 10; i < 30; i++)
            {
                var query = Query.And(Query.Size("CountHistory", i), Query.Matches("CrawlID", "/Weibo/"),Query.GTE("FetchTime",new DateTime(2012,4,17)));
                var total = MongoItemAccess.Items.Count(query)*i;
                sum+= total;
            }
            MessageBox.Show(sum.ToString());

        }

        class TempData
        {
            public string CrawlID;
            public string CreateTime;
            public string Title;
            public string Serious;
            public string Type2;
            public string Category;
        }
        //private void AddTopic4Project()
        //{
        //    //初始数据
        //    Workbook book = new Workbook();
        //    var targetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "topics.xlsx");
        //    book.Open(targetPath);
        //    var dataSheet = book.Worksheets["sheet1"];
        //    int startPos = 0;
        //    List<TempData> list = new List<TempData>();
        //    while (!string.IsNullOrEmpty(dataSheet.Cells[startPos, 0].StringValue))
        //    {
        //        TempData data = new TempData();
        //        data.CrawlID = dataSheet.Cells[startPos, 0].StringValue.Trim();
        //        data.CreateTime = dataSheet.Cells[startPos, 1].StringValue.Trim();
        //        data.Title = dataSheet.Cells[startPos, 2].StringValue.Trim();
        //        data.Serious = dataSheet.Cells[startPos, 3].StringValue.Trim();
        //        data.Type2 = dataSheet.Cells[startPos, 4].StringValue.Trim();
        //        data.Category = dataSheet.Cells[startPos, 5].StringValue.Trim();
        //        list.Add(data);
        //        startPos++;
        //    }
        //    //匹配Crawl
        //    List<Topic4Project> resultList = new List<Topic4Project>();
        //    using (PalasDB db = new PalasDB())
        //    {
        //        foreach (var item in list)
        //        {
        //            Topic4Project topic4Project = new Topic4Project();
        //            topic4Project.CrawlID = item.CrawlID;
        //            topic4Project.Title = item.Title;
        //            topic4Project.Type2 = item.Type2;
        //            topic4Project.Serious = item.Serious;
        //            topic4Project.Category = item.Category;
        //            DateTime createTime;
        //            DateTime.TryParse(item.CreateTime, out createTime);
        //            topic4Project.CreateTime = createTime;


        //            try
        //            {
        //                var crawl = db.Crawl.FirstOrDefault(model => model.Name == item.Title);

        //                if (crawl != null)
        //                {

        //                    topic4Project.CrawlID = crawl.CrawlID;
        //                }
        //            }
        //            catch { }

        //            resultList.Add(topic4Project);
        //        }
        //    }
        //    //匹配Topic
        //    using (Crawler.Host.PalasDBOld.PalasDB34 db = new Crawler.Host.PalasDBOld.PalasDB34())
        //    {
        //        foreach (var item in resultList)
        //        {
        //            try
        //            {
        //                var topic = db.Topic.FirstOrDefault(model => model.Title == item.Title);
        //                if (topic !=null)
        //                {
        //                    item.TopicID = topic.TopicID;
        //                }
        //            }
        //            catch {}

        //        }
        //    }
        //    using (PalasDB db = new PalasDB())
        //    {
        //        foreach (var item in resultList)
        //        {
        //            db.Topic4Project.AddObject(item);
        //            db.SaveChanges();
        //        }
        //    }
            
        //}
        private void ImportDataToClient()
        {
             
        }
        private void TestESQuery()
        {
            SearchBuilder<Item> builder = new SearchBuilder<Item>();
            var query = builder.GetFullTextQuery(model => model.CleanText, "\"海联讯\"+300277+(章锋 邢文飙)");
            //var query = builder.GetFullTextQuery(model => model.CleanText, "\"凯越\" -(\"凯越酒店\" \"凯越国际\")");
            builder.Query.Must.AddFullQuery(query);
            var json = ESAccess.ExplainJson(builder);
        }
        private void TestESOperator()
        {




            Item item = new Item("abcdefg", "http://www.google.com");
            item.ItemID = "cdefg";
            var dateTime = DateTimeParser.Parser("2012-11-5");
            //dateTime = dateTime.ToLocalTime();
            var localTime = DateTime.SpecifyKind((DateTime) dateTime, DateTimeKind.Local);
            item.PubDate = localTime;
            
            item.AnalyzeData = new ItemAnalyzeData[] { new ItemAnalyzeData("Test", Enums.MergeMethod.FromTopic) };
            GeneralItemAccess.Delete(item);
            GeneralItemAccess.Index(item);
            //item.Url = "http://www.baidu.com";
            //GeneralItemAccess.Update(item);
            
            Item getItem = ESItemAccess.SearchByItemID("cdefg");
        }

        private void TestDelegate()
        {

        }



        private void UpdateXinHua()
        {
            var reg =
                @"src=""(?<Url>http://search\.news\.cn/.*?nodeid.*?)""";
            using (PalasDB db = new PalasDB())
            {
                var result =
                    db.Crawl.Where(
                        model =>
                        model.Name.StartsWith("新华网"));
                //db.ExecuteStoreQuery<Crawl>(@"SELECT * FROM crawl WHERE NAME LIKE '新华网%' AND IsDisabled =true and reviewtime = '2015-04-19'");
                foreach (Crawl crawl in result)
                {
                    var content = WebRequestProcessor.DownloadHTTPString(crawl.Url);
                    if (string.IsNullOrEmpty(content))
                    {
                        continue;

                    }
                    var match = Regex.Match(content, reg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (match.Success)
                    {

                        using (PalasDB updateDb = new PalasDB())
                        {
                            var updatedCrawl = updateDb.Crawl.FirstOrDefault(model => model.CrawlID == crawl.CrawlID);
                            updatedCrawl.Url = match.Groups["Url"].Value;
                            updateDb.SaveChanges();
                        }

                    }

                }


            }

        }

        private void AutoCreateCrawl()
        {
            using (PalasDB db = new PalasDB())
            {
                var result = db.ExecuteStoreQuery<Media>(
                    @"SELECT * FROM media WHERE (medianame LIKE '%人民网%' OR medianame LIKE '%新华网%' OR medianame LIKE '%新浪网%' OR medianame LIKE '%腾讯网%') 
AND mediaid NOT IN (SELECT mediaid FROM crawl)
",
                    null);

                int row = 0;
                foreach (var media in result)
                {
                    row++;
                    SaveCrawl(media);
                }
            }
            MessageBox.Show("创建完成");
        }
        private string GetSepString(string str)
        {
            try
            {
                var valStr = str.Split(':')[1];
                return valStr;

            }
            catch (Exception)
            {

                return "";
            }

        }
        private void MediaImport()
        {
            var files = Directory.GetFiles(@"d:\temp\mediaimport", "*.xlsx", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                Workbook book = new Workbook();
                book.Open(file);
                var sheet = book.Worksheets[0];
                bool isNewVersion = sheet.Cells[0, 6].StringValue == "媒体阶层";
                int colOffset = isNewVersion ? 0 : -1;
                int row = 1;
                string parentID = null;
                using (PalasDB db = new PalasDB())
                {
                    while (!string.IsNullOrEmpty(sheet.Cells[row, 0].StringValue))
                    {
                        var url = sheet.Cells[row, 0].StringValue;
                        var mediaName = sheet.Cells[row, 1].StringValue;
                        var channelName = sheet.Cells[row, 2].StringValue;
                        try
                        {

                            var mediaType = sbyte.Parse(GetSepString(sheet.Cells[row, 3].StringValue));
                            var mediaTendency = sbyte.Parse(GetSepString(sheet.Cells[row, 4].StringValue));
                            var mediaOrganType = sbyte.Parse(GetSepString(sheet.Cells[row, 5].StringValue));
                            sbyte mediaElitismType = 0;
                            if (isNewVersion)
                            {
                                mediaElitismType = sbyte.Parse(GetSepString(sheet.Cells[row, 6].StringValue));
                            }

                            var mediaWeight = sbyte.Parse(sheet.Cells[row, 7 + colOffset].StringValue);
                            var mediaStyle = sbyte.Parse(GetSepString(sheet.Cells[row, 8 + colOffset].StringValue));
                            var regionType = sbyte.Parse(GetSepString(sheet.Cells[row, 9 + colOffset].StringValue));
                            var province = sheet.Cells[row, 10 + colOffset].StringValue;
                            var city = sheet.Cells[row, 11 + colOffset].StringValue;
                            var district = sheet.Cells[row, 12 + colOffset].StringValue;
                            var industry = GetSepString(sheet.Cells[row, 13 + colOffset].StringValue);
                            var media = db.Media.CreateObject();
                            media.MediaID = CrawlEntity.NewGuid;
                            media.Url = url;
                            media.Channel = channelName;
                            media.MediaName = mediaName;
                            media.ParentMediaID = parentID;
                            if (parentID == null)
                            {
                                parentID = media.MediaID;
                            }
                            media.MediaType = mediaType;
                            media.MediaTendency = mediaTendency;
                            media.MediaOrganType = mediaOrganType;
                            media.MediaElitismType = mediaElitismType;
                            media.MediaWeight = mediaWeight;

                            media.MediaStyle = mediaStyle;
                            media.RegionType = regionType;
                            media.Nation = "中国";
                            media.Province = province;
                            media.City = city;
                            media.District = district;

                            media.IndustryIDs = industry;
                            media.CreateTime = DateTime.Now;
                            db.Media.AddObject(media);
                            db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                            SaveCrawl(media);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("导入媒体出错:媒体名={0},频道名={1},Url={2}", mediaName, channelName, url), ex);
                            
                        }


                        row++;
                    }
                }

            }
            MessageBox.Show("导入完成");
        }
        private void SaveCrawl(Media media)
        {
            using (PalasDB db = new PalasDB())
            {

                Crawl entity = db.Crawl.CreateObject();


                Site site = null;
                if (!string.IsNullOrEmpty(media.ParentMediaID))
                {
                    site = db.Site.FirstOrDefault(model => model.MediaID == media.ParentMediaID);
                }
                else
                {
                    site = db.Site.FirstOrDefault(model => model.MediaID == media.MediaID);
                }
                if (site == null)
                {
                    site = db.Site.CreateObject();
                    site.SiteID = CrawlEntity.NewGuid;
                    site.MediaID = media.MediaID;
                    site.UrlEncoding = "";
                    site.ParallelWithOtherCrawler = false;
                    site.TimeoutSecs = 10;
                    site.EncodingResponse = "";


                    site.LoginUseWebBrowser = false;
                    site.IsVisible = true;
                    site.ListItemCountPerPage = 20;
                    site.Name = media.MediaName;
                    site.ParseMethod = 1;//XPath抓取
                    site.CreateTime = DateTime.Now;
                    site.ListPattern = "";
                    site.ItemPattern = "";
                    site.ContentDetailLevel = 2;
                }







                entity.Site = site;
                entity.Name = media.MediaName;
                entity.AnalyzeFlag = 518111;
                entity.IssueID = "Common";
                entity.ListDrillMethod = 1;

                entity.CrawlType = 1;//Crawl List and Items
                entity.CrawlID = CrawlEntity.NewGuid;
                //Crawl Summary
                entity.Url = media.Url;
                entity.IntervalMins = 1440;
                entity.IntervalStrategy = 1;
                entity.ExistItemStrategy = 2;
                entity.MaxRetriveDays = 180;

                entity.RequiredCount = 20;
                entity.InitRequiredCount = 20;
                entity.CreateTime = DateTime.Now;
                entity.NextCrawlTime = DateTime.Now;
                entity.MediaType = media.MediaType;
                entity.MediaRegionType = media.RegionType;
                entity.MediaID = media.MediaID;
                entity.LastCrawlNewCount = 0;
                entity.FollowIntervalMins = 60;
                entity.FollowMinReplyLen = 6;
                entity.MediaMapToChannel = false;
                entity.MediaRecordNew = true;
                entity.SaveSummary = true;
                entity.SaveHtml = true;
                entity.SaveContent = true;
                entity.ReleaseAutoFormat = true;
                db.Crawl.AddObject(entity);


                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);


            }
        }
        private static bool IsFilterSurfix(string last)
        {
            string surfix = last.ToLower();
            switch (last)
            {
                case "com":
                case "net":
                case "biz":
                case "edu":
                case "org":
                    return true;
                default:
                    return false;

            }

        }
        private static string GetUrlDomain(string url)
        {
            string domain;

            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }
            Uri uri = new Uri(url);

            string host = uri.Host;
            var parts = new List<string>(host.Split('.'));
            if (parts.Last().Length == 2)
            {
                //process the culture surfix. e.g cn. us. etc
                parts.RemoveAt(parts.Count - 1);
            }
            if (parts.Last().Length == 3)
            {
                if (IsFilterSurfix(parts.Last()))
                {
                    parts.RemoveAt(parts.Count - 1);
                }

            }

            //int domaindot = 
            domain = parts.Last() + ".";
            return domain;
        }
        private void SetAllDuplicateCrawl()
        {
            Workbook book = new Workbook();
            book.Open(@"d:\temp\source.xls");
            var sheet = book.Worksheets[0];
            int row = 1;
            List<string> allDomainList = new List<string>();
            while (!string.IsNullOrEmpty(sheet.Cells[row, 1].StringValue))
            {
                var url = sheet.Cells[row, 1].StringValue;
                var domain = GetUrlDomain(url);
                if (allDomainList.Count(model => model == domain) == 0)
                {
                    allDomainList.Add(domain);
                }

                row++;
            }
            //更新数据库
            using (PalasDB db = new PalasDB())
            {
                foreach (string item in allDomainList)
                {
                    string querycmd = string.Format("select count(*) from crawl where url like '%{0}%'", item);
                    string cmd = string.Format("update crawl set reviewtime = null where url like '%{0}%'", item);
                    var cnt = db.ExecuteStoreQuery<int>(querycmd).FirstOrDefault();

                    if (cnt < 50)
                    {
                        if (cnt > 0)
                        {
                            db.ExecuteStoreCommand(cmd, null);
                        }

                    }
                    else
                    {
                        Logger.Error("可疑域名:" + item);
                    }

                }

            }
            MessageBox.Show("更新成功");
        }

        private void AnalyzeDetail()
        {
            //var query = Query.And(Query.EQ("CrawlID", "Weibo2"), Query.GTE("PubDate", new DateTime(2011, 10, 27, 16, 00, 00)));

            //var sort = SortBy.Ascending("PubDate");
            //var result = MongoItemAccess.Items.Find(query).SetFields("PubDate").SetSortOrder(sort).ToArray();

            //ResultGridView.DataSource = result;

            var query = Query.And(Query.EQ("CrawlID", "Weibo58"));

            var sort = SortBy.Ascending("PubDate");
            var result = MongoItemAccess.Items.Find(query).SetFields("ItemID", "Source", "CleanTitle", "AuthorName", "Url", "PubDate", "CurrentCount").SetSortOrder(sort).ToArray();
            var finalResult = from item in result
                              select
                                  new
                                      {
                                          item.ItemID,
                                          item.CleanTitle,
                                          item.AuthorName,
                                          item.Url,
                                          item.Source,
                                          PubDate = ((DateTime)(item.PubDate)).AddHours(8),
                                          DateTime = item.CurrentCount == null ? DateTime.MinValue : item.CurrentCount.FetchTime.AddHours(8),
                                          ForwardCount = item.CurrentCount == null ? 0 : item.CurrentCount.ForwardCount,
                                          ReplyCount = item.CurrentCount == null ? 0 : item.CurrentCount.ReplyCount
                                      };
            ResultGridView.DataSource = finalResult.ToArray();
        }
        private void AnalyzeAggr()
        {


            int startCnt = 10;
            int endCnt = 30;
            List<DetailResult> list = new List<DetailResult>();
            for (int i = startCnt; i < endCnt; i++)
            {
                var query = Query.And(Query.Size("CountHistory", i), Query.Matches("CrawlID", "/Weibo/"), Query.GT("FetchTime", new DateTime(2012, 4, 17)));


                //var sort = SortBy.Ascending("PubDate");
                var result = MongoItemAccess.Items.Find(query).SetFields("ItemID", "CountHistory", "CrawlID", "Url", "CleanTitle", "PubDate")
                    //.SetSortOrder(sort)
                    .ToArray();


                foreach (var item in result)
                {
                    Item item1 = item;
                    var subResult = from subItem in item.CountHistory
                                    select
                                        new DetailResult
                                        {
                                            CrawlID = item1.CrawlID,
                                            FetchTime = subItem.FetchTime,
                                            ForwardCount = subItem.ForwardCount,
                                            ItemID = item1.ItemID,
                                            ReplyCount = subItem.ReplyCount,
                                            CleanTitle = item1.CleanTitle,
                                            Url = item1.Url,
                                            ViewCount = subItem.ViewCount,
                                            PubDate = item1.PubDate ?? DateTime.MinValue
                                        };
                    list.AddRange(subResult);
                }
            }

            ResultGridView.DataSource = list.ToArray();



            //var query = Query.And(Query.EQ("CrawlID", TitleTxt.Text));

            //var sort = SortBy.Ascending("PubDate");
            //var result = MongoItemAccess.Items.Find(query).SetFields("PubDate", "CurrentCount").SetSortOrder(sort).ToArray();



            //var grouping = from item in result
            //               group item by ((DateTime)(item.PubDate)).ToString("yyyy-MM-dd HH:00")

            //                   into g
            //                   orderby g.Key
            //                   select g;
            //var pubDateResult = from g in grouping
            //                    select new StatResult { GroupTime = g.Key, Count = g.Count() };
            //var currentCountGroup = from item in result
            //                        where item.CurrentCount != null
            //                        group item by item.CurrentCount.FetchTime.ToString("yyyy-MM-dd HH:00")
            //                            into g
            //                            orderby g.Key
            //                            select g;

            ////var finalResult = from g in grouping
            ////                  select new { Key = g.Key, Count = g.Count() };
            //var statResult = from g in currentCountGroup
            //                 select new StatResult { GroupTime = g.Key, ForwardCount = g.Sum(model => model.CurrentCount.ForwardCount), ReplyCount = g.Sum(model => model.CurrentCount.ReplyCount) };

            ////Merge the samekey
            //List<StatResult> results = new List<StatResult>(pubDateResult);
            //foreach (var perStat in statResult)
            //{
            //    var findItem = results.FirstOrDefault(model => model.GroupTime == perStat.GroupTime);
            //    if (findItem == null)
            //    {
            //        results.Add(perStat);
            //    }
            //    else
            //    {
            //        findItem.ForwardCount = perStat.ForwardCount;
            //        findItem.ReplyCount = perStat.ReplyCount;
            //    }
            //}

            //var finalResult = results.ToArray();
            //ResultGridView.DataSource = finalResult;

        }
        private void GroupByTag()
        {
            Workbook book = new Workbook();
            string inputPath = "D:/WeiboReport/test.xlsx";
            book.Open(inputPath);
            Worksheet sheet = book.Worksheets["粉丝列表"];
            Worksheet resultSheet = book.Worksheets.Add("Tag统计");
            //Determined Column index
            int searchCol = -1;
            int tagCol = -1;
            for (int i = 0; i < 20 && !string.IsNullOrEmpty(sheet.Cells[0, i].StringValue); i++)
            {
                string currentTxt = sheet.Cells[0, i].StringValue.Trim();
                if (TagTxt.Text.Trim() == currentTxt)
                {
                    searchCol = i;
                    continue;

                }
                if ("Tag" == currentTxt)
                {
                    tagCol = i;
                    continue;
                }
                if (tagCol != -1 && searchCol != -1)
                {
                    break;
                }

            }
            //Determined row count
            int row = 1;
            while (!string.IsNullOrEmpty(sheet.Cells[row, searchCol].StringValue))
            {
                row++;
            }

            //Refine data
            int currentRow = 1;
            resultSheet.Cells[0, 0].PutValue(sheet.Cells[0, searchCol].StringValue);
            resultSheet.Cells[0, 1].PutValue("Tag");

            for (int i = 1; i < row; i++)
            {
                string searchPart = sheet.Cells[i, searchCol].StringValue;
                string rawTag = sheet.Cells[i, tagCol].StringValue;
                if (string.IsNullOrEmpty(rawTag))
                {
                    continue;
                }
                var tags = rawTag.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in tags)
                {
                    resultSheet.Cells[currentRow, 0].PutValue(searchPart);
                    resultSheet.Cells[currentRow, 1].PutValue(item);
                    currentRow++;
                }


            }
            book.Save(inputPath);


        }
        private void AnalyzeBtn_Click(object sender, EventArgs e)
        {
            var pageElement = new PageElement { Title = TitleTxt.Text };
            ItemPageXPaths outPath = null;
            List<SubItemElement> subList;
            string useless;
            string expMsg;
            string usedEncoding;
            Enums.CrawlResult uselessResult;
            string contentPageSource = WebRequestProcessor.DownloadHTTPString(UrlTxt.Text, out useless,
            out uselessResult, out expMsg, out usedEncoding, Encoding: "GB2312", ReEncodeByHTMLCharset: true);

            if (string.IsNullOrEmpty(contentPageSource))
            {
                return;
            }
            var oneResult = PageAutoAnalyzer.AnalyzeContent(contentPageSource, pageElement, RecogniseMode.News
                , new IdentityContentElement()

                , ref outPath, out subList
                );
            ResultGridView.DataSource = oneResult;

        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            //_host = WCFMonitorProxy.CreateServiceHost();
            //_host.Opened += new EventHandler((sd, evt) =>
            //{
            //    this.Invoke(new Action(() =>
            //    {
            //        this.Text = "监听中...";
            //    }));
            //});
            //_host.Open();
            
        }

        private int _testCount = 0;
        private void ConnectTestBtn_Click(object sender, EventArgs e)
        {
            Item item = new Item()
                            {
                                CleanTitle = _testCount++.ToString(),
                                Crawler = "MyTesterCrawler",
                            };
            var crawl = CrawlBusiness.GetByCrawlID("00014da10767401cb0e100c0b4ca81b1");
            var media = crawl.Media;
            var site = crawl.Site;
            _taskSendChannel = WCFUtility.CreateChannel<IAnalyzeTask>
                                           (
                                            "net.tcp://222.66.89.17:10229/AnalyzeData",
                                            new NetTcpBinding(SecurityMode.None)
                                            {

                                                MaxReceivedMessageSize = _maxRecvMessageSize,
                                                OpenTimeout = TimeSpan.FromDays(3),
                                                CloseTimeout = TimeSpan.FromDays(3),
                                                ReceiveTimeout = TimeSpan.FromDays(3),
                                                SendTimeout = TimeSpan.FromDays(3),
                                                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                                            });
            _taskSendChannel.SendAnalyzeTask(new AnalyzeData()
                                                 {
                                                     Crawl = crawl,
                                                     Item = item,
                                                     Media = media,
                                                     Site = site
                                                 });
           
            
        }


    }
    public class StatResult
    {
        public string GroupTime { get; set; }
        public int Count { get; set; }
        public int ForwardCount { get; set; }
        public int ReplyCount { get; set; }
    }
    public class WeiboReport
    {
        public string Url { get; set; }
        public string Weibo { get; set; }
        public DateTime PubDate { get; set; }
        public int Forward { get; set; }
        public int Comment { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is WeiboReport)
            {
                var comparedWeiboReport = obj as WeiboReport;
                return Url == comparedWeiboReport.Url;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
    }
    
}

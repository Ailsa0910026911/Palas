using Conductor.Core.Data;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Palas.Common;
using Palas.Common.Data;
using Palas.Common.DataAccess;
using Palas.Common.Utility;
using Palas.Framework;
using Palas.Framework.Analyze;
using Palas.Framework.Core;
using Palas.Framework.Parser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace WechatAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 1 }; //最大线程数100
            Parallel.For(0, 20, options, (i, loopState) =>
            {
                do
                {
                    WechatCrawler();
                }
                while (true);
            });
        }

        private static string itemsKey = "Items";
        private static RedisPool redisPool = RedisConnectionManager.Instance.GetRedisPool("Crawler", 0);
        private static string wechatSubsKey = "wechatSubs";

        private static void ItemCrawl()
        {
            using (var client = new RedisAccess(redisPool))
            {
                try
                {
                    //产生临时库（临时库和缓存总库取并集）
                    string wechatItem = client.PopItemWithHighestScoreFromSortedSet(itemsKey);
                    if (!string.IsNullOrEmpty(wechatItem))
                    {
                        Palas.Protocol.PFItemToAnalyze itemToAnalyze = JsonConvert.DeserializeObject<Palas.Protocol.PFItemToAnalyze>(wechatItem);

                        Palas.Protocol.PFItemToAnalyze analyzeResult = AnalyzeItem.Analyzer(itemToAnalyze, Palas.Protocol.PFAnalyzeFlag.Splite);

                        //6.使用AnalyzeFirst对文章进行第一次分析
                        MultriAnalyzeFlag analyzeFlags = AnalyzeItem.BuildAnalyzeFlag((Enums.AnalyzeFlag)analyzeResult.AnalyzeFlag);
                        Palas.Protocol.PFItemToAnalyze analyzeFirstResult = AnalyzeItem.AnalyzerFirst(analyzeResult, analyzeFlags);

                        //7.使用AnalyzeSecond对文章进行第二次分析
                        Palas.Protocol.PFItemToAnalyze analyzeSecondResult = AnalyzeItem.AnalyzeSecond(analyzeFirstResult, analyzeFlags);

                        //8.使用AnalyzeIssue对文章进行分Issue分析
                        Palas.Protocol.PFItemToAnalyze analyzeIssueResult = AnalyzeItem.IssueAnalyzer(analyzeSecondResult, analyzeFlags);

                        //9.使用IndexThenDup将文章去重索引到ES
                        if (analyzeIssueResult != null)
                        {
                            //此处对Item进行一次转换
                            Palas.Protocol.PFItem pfItem = analyzeIssueResult.Item;
                            Item _item = TypeExchangeUtility.ExchangeItem(pfItem);

                            _item = FilterIssue.FilterExcludeExpression(_item);

                            Enums.ProcessStatus result = Enums.ProcessStatus.Failed;
                            int retry = 0;
                            do
                            {
                                try
                                {
                                    //retry++;
                                    ESAccess.IndexOnly(_item);
                                    result = Enums.ProcessStatus.Succeeded;
                                    //result = DupThenIndexBusiness.DupThenIndexItem(_item);
                                    //if (result == Enums.ProcessStatus.Failed)
                                    //Thread.Sleep(new TimeSpan(0, 0, 30));
                                }
                                catch //(Exception ex)
                                {
                                    result = Enums.ProcessStatus.Failed;
                                    Thread.Sleep(new TimeSpan(0, 1, 00));
                                }
                            }
                            while (result == Enums.ProcessStatus.Failed && retry < 3);

                            //10.判断是否成功
                            //Assert.AreNotEqual(Enums.ProcessStatus.Failed, result);
                            Console.WriteLine(string.Format("Index Dup: {2} paper: {0}, Status: {1}", _item.ItemID, result.ToString(), _item.DuplicationID));
                        }

                    }
                    else
                    {
                        //if have nothing to emit, then sleep for a little while to release CPU
                        Thread.Sleep(100);
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static void DoSimpleCrawler()
        {
            //EntityManager.PrepareWithAutoRefreash();

            //1.通过TaskProvider获取Simple Crawler任务
            TaskProvider taskProvider = new TaskProvider();
            string lockCrawler = DES.Encrypt("NC31ASTORM", DataAccess.PalasKey);
            Palas.Protocol.PFTaskInfo taskInfo = taskProvider.GetTask(lockCrawler, ConfigurationManager.AppSettings["CrawlConductUrl"] + "Task/GetCrawlTask");
            Console.WriteLine(string.Format("GetTask: {0}", taskInfo.TaskId));

            //2.使用TaskResolve将任务进行分解
            TaskResolve taskResolve = new TaskResolve();
            List<Palas.Protocol.PFCrawlInfo> crawlInfos = taskResolve.ResolveTask(taskInfo);

            foreach (var crawlInfo in crawlInfos)
            {
                //3.使用HttpRequest获取网页信息
                HttpRequest httpRequest = new HttpRequest();
                Palas.Protocol.PFCrawlResponse crawlResponse = httpRequest.DoWebRequest(crawlInfo);

                //4.使用ParserHtml解析HTML信息
                Palas.Framework.Module.ParserHtml parserHtml = new Palas.Framework.Module.ParserHtml();
                List<string> papers = parserHtml.Parser(crawlResponse, taskInfo);

                Console.WriteLine(string.Format("Crawl paper count: {0}", papers.Count));

                foreach (string paper in papers)
                {
                    //5.使用ItemProcess对文章信息进行预处理
                    ItemProcess itemProcess = new ItemProcess();
                    string item = itemProcess.Process(paper, taskInfo.BaseInfo);

                    if (!string.IsNullOrEmpty(item))
                    {
                        Palas.Protocol.PFItemToAnalyze pfItemToAnalyze = new Palas.Protocol.PFItemToAnalyze();
                        pfItemToAnalyze.Item = JsonConvert.DeserializeObject<Palas.Protocol.PFItem>(item);
                        pfItemToAnalyze.CrawlRecode = taskInfo.BaseInfo;
                        pfItemToAnalyze.AnalyzeFlag = taskInfo.BaseInfo.AnalyzeFlag;

                        Palas.Protocol.PFItemToAnalyze analyzeResult = AnalyzeItem.Analyzer(pfItemToAnalyze, Palas.Protocol.PFAnalyzeFlag.Splite);

                        //6.使用AnalyzeFirst对文章进行第一次分析
                        MultriAnalyzeFlag analyzeFlags = AnalyzeItem.BuildAnalyzeFlag((Enums.AnalyzeFlag)analyzeResult.AnalyzeFlag);
                        Palas.Protocol.PFItemToAnalyze analyzeFirstResult = AnalyzeItem.AnalyzerFirst(analyzeResult, analyzeFlags);

                        //7.使用AnalyzeSecond对文章进行第二次分析
                        Palas.Protocol.PFItemToAnalyze analyzeSecondResult = AnalyzeItem.AnalyzeSecond(analyzeFirstResult, analyzeFlags);

                        //8.使用AnalyzeIssue对文章进行分Issue分析
                        Palas.Protocol.PFItemToAnalyze analyzeIssueResult = AnalyzeItem.IssueAnalyzer(analyzeSecondResult, analyzeFlags);

                        //9.使用IndexThenDup将文章去重索引到ES
                        if (analyzeIssueResult != null)
                        {
                            //此处对Item进行一次转换
                            Palas.Protocol.PFItem pfItem = analyzeIssueResult.Item;
                            Item _item = TypeExchangeUtility.ExchangeItem(pfItem);

                            _item = FilterIssue.FilterExcludeExpression(_item);

                            Enums.ProcessStatus result = Enums.ProcessStatus.Failed;
                            int retry = 0;
                            do
                            {
                                try
                                {
                                    //retry++;
                                    ESAccess.IndexOnly(_item);
                                    result = Enums.ProcessStatus.Succeeded;
                                    //result = DupThenIndexBusiness.DupThenIndexItem(_item);
                                    //if (result == Enums.ProcessStatus.Failed)
                                    //Thread.Sleep(new TimeSpan(0, 0, 30));
                                }
                                catch //(Exception ex)
                                {
                                    result = Enums.ProcessStatus.Failed;
                                    Thread.Sleep(new TimeSpan(0, 1, 00));
                                }
                            }
                            while (result == Enums.ProcessStatus.Failed && retry < 3);

                            //10.判断是否成功
                            //Assert.AreNotEqual(Enums.ProcessStatus.Failed, result);
                            Console.WriteLine(string.Format("Index paper: {0}, Status: {1}", _item.ItemID, result.ToString()));
                        }
                    }
                }
            }

        }

        private static void WechatCrawler()
        {
            using (var client = new RedisAccess(redisPool))
            {
                try
                {
                    //产生临时库（临时库和缓存总库取并集）
                    string wechatItem = client.PopItemWithHighestScoreFromSortedSet(wechatSubsKey);
                    if (!string.IsNullOrEmpty(wechatItem))
                    {
                        WeChatItemModel wechatItemModel = JsonConvert.DeserializeObject<WeChatItemModel>(wechatItem);

                        Palas.Protocol.PFItemToAnalyze pfItemToAnalyze = new Palas.Protocol.PFItemToAnalyze();
                        Palas.Protocol.PFItem pfItem = new Palas.Protocol.PFItem();

                        if (wechatItemModel != null && !string.IsNullOrEmpty(wechatItemModel.Id))
                        {
                            pfItem.AuthorID = wechatItemModel.WechatSubId;
                            pfItem.AuthorName = wechatItemModel.WechatSubName;
                            pfItem.ContentDetailLevel = Palas.Protocol.PFContentDetailLevel.FullContent;
                            pfItem.Crawler = "WechatSubs";
                            pfItem.CrawlID = wechatItemModel.WechatSubId;
                            pfItem.FetchTime = DateTimeUtility.GetUnixTimeStamp(DateTime.Now);
                            pfItem.ItemID = wechatItemModel.Id;
                            pfItem.MediaID = "WeChat";
                            pfItem.MediaName = "微信";
                            pfItem.PubDate = DateTimeUtility.GetUnixTimeStamp(wechatItemModel.PubDate);
                            pfItem.Url = wechatItemModel.Url.Replace("&amp;", "&");
                            pfItem.CleanTitle = wechatItemModel.Title;

                            if (wechatItemModel.Tags != null && wechatItemModel.Tags.Count > 0)
                            {
                                pfItem.Tag = string.Join(",", wechatItemModel.Tags);
                            }

                            string content = GetHtmlContent(pfItem.Url);
                            if (!string.IsNullOrEmpty(content))
                            {
                                pfItem.HTMLText = HtmlFormattor.FormatHtml(content, pfItem.Url);
                                pfItem.CleanText = HTMLCleaner.CleanHTML(pfItem.HTMLText, true);

                                pfItemToAnalyze.AnalyzeFlag = 312287;
                                pfItemToAnalyze.Item = pfItem;

                                pfItemToAnalyze.CrawlRecode = new Palas.Protocol.PFCrawlRecode(pfItem.Crawler, pfItem.CrawlID, "Common", "", "N", 312287, 1, "WeChatSync", 0, 0, 0, "", 0, 0, true, pfItem.MediaID, pfItem.MediaName, pfItem.MediaChannel);
                                pfItemToAnalyze.CrawlRecode.Tags = pfItem.Tag;

                                pfItemToAnalyze.CrawlRecode.JoinIssueIDs = string.Join(",", wechatItemModel.Customers);
                                pfItemToAnalyze.CrawlRecode.MediaType = 11;
                                pfItemToAnalyze.CrawlRecode.MediaWeight = 8;

                                // 临时追加

                                if (wechatItemModel.Tags != null && wechatItemModel.Tags.Count > 0)
                                {
                                    pfItem.Tag = string.Join(",", wechatItemModel.Tags);
                                    //pfItemToAnalyze.CrawlRecode.Tags = pfItem.Tag;
                                }
                                // 临时追加完毕

                                //pfItemToAnalyze.CrawlRecode = new Palas.Protocol.PFCrawlRecode(pfItem.Crawler, pfItem.CrawlID, "Common", "", "N", 312287, string.Join(",", wechatItemModel.Customers), 1, "WeChatSync", 0, 0, "", 0, "", "", 0, 0, "", true, 4, pfItem.MediaID, pfItem.MediaName, "", 11, 0, 8, 0, "", "",);

                                Palas.Protocol.PFItemToAnalyze analyzeResult = AnalyzeItem.Analyzer(pfItemToAnalyze, Palas.Protocol.PFAnalyzeFlag.Splite);

                                //6.使用AnalyzeFirst对文章进行第一次分析
                                MultriAnalyzeFlag analyzeFlags = AnalyzeItem.BuildAnalyzeFlag((Enums.AnalyzeFlag)analyzeResult.AnalyzeFlag);
                                Palas.Protocol.PFItemToAnalyze analyzeFirstResult = AnalyzeItem.AnalyzerFirst(analyzeResult, analyzeFlags);

                                //7.使用AnalyzeSecond对文章进行第二次分析
                                Palas.Protocol.PFItemToAnalyze analyzeSecondResult = AnalyzeItem.AnalyzeSecond(analyzeFirstResult, analyzeFlags);

                                //8.使用AnalyzeIssue对文章进行分Issue分析
                                Palas.Protocol.PFItemToAnalyze analyzeIssueResult = AnalyzeItem.IssueAnalyzer(analyzeSecondResult, analyzeFlags);

                                //9.使用IndexThenDup将文章去重索引到ES
                                if (analyzeIssueResult != null)
                                {
                                    //此处对Item进行一次转换
                                    Palas.Protocol.PFItem pfItem2 = analyzeIssueResult.Item;
                                    Item _item = TypeExchangeUtility.ExchangeItem(pfItem2);

                                    _item = FilterIssue.FilterExcludeExpression(_item);

                                    Enums.ProcessStatus result = Enums.ProcessStatus.Failed;
                                    int retry = 0;
                                    do
                                    {
                                        try
                                        {
                                            //retry++;
                                            ESAccess.IndexOnly(_item);
                                            result = Enums.ProcessStatus.Succeeded;
                                            //result = DupThenIndexBusiness.DupThenIndexItem(_item);
                                            //if (result == Enums.ProcessStatus.Failed)
                                            //Thread.Sleep(new TimeSpan(0, 0, 30));
                                        }
                                        catch //(Exception ex)
                                        {
                                            result = Enums.ProcessStatus.Failed;
                                            Thread.Sleep(new TimeSpan(0, 1, 00));
                                        }
                                    }
                                    while (result == Enums.ProcessStatus.Failed && retry < 3);

                                    //10.判断是否成功
                                    //Assert.AreNotEqual(Enums.ProcessStatus.Failed, result);
                                    Console.WriteLine(string.Format("Index paper: {0}, Status: {1}", _item.ItemID, result.ToString()));
                                }

                                return;
                            }
                        }

                    }
                    else
                    {
                        //if have nothing to emit, then sleep for a little while to release CPU
                        Thread.Sleep(50);
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Get the wechat content
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetHtmlContent(string Url)
        {
            string responseUrl;
            Enums.CrawlResult result;
            string html = HttpHelper.GetHttpContent(Url, out responseUrl, out result);

            if (!string.IsNullOrEmpty(html))
            {
                HtmlNode htmlNode = HtmlUtility.getSafeHtmlRootNode(html);

                try
                {
                    return htmlNode.SelectSingleNode("//div[@id=\"js_content\"]").InnerHtml;
                }
                catch { }
            }

            return null;
        }
    }
}

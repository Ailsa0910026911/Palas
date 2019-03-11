using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Crawler.Core;
using Palas.Common.Data;
using Palas.Common.DataAccess;

namespace SimpleCrawler
{
    public class ServiceMonitorClient:IStatusReportClient
    {
        #region Implementation of IStatusReportClient

        public CrawlStatusData GetCurrentCrawlStatus()
        {
            var infos =  CrawlerManager.CrawlerFactory.Pipelines.Select(model => model.Info).ToArray();
            var result = new CrawlStatusData()
                {
                    HostData = HostBasicData.GetHostData()

                };
            var data = from item in infos
                       select new CrawlData
                           {
                               CrawlID = item.CurrentJob_Name,
                               Message = item.Message,
                               JobCount = item.JobCount
                           };
            foreach (var crawlData in data)
            {
                using (PalasDB db = new PalasDB())
                {
                    try
                    {
                        var crawl = db.Crawl.FirstOrDefault(model => model.CrawlID == crawlData.CrawlID);
                        crawlData.LastCrawlTime = crawl.LastCrawlTime;
                        crawlData.Name = crawl.Name;
                        crawlData.Url = crawl.Url;
                    }
                    catch {}
                    
                }
            }
            result.CrawlData = data.ToArray();
            return result;
        }

        public string TestClientConnection(string testString)
        {
            return testString;

        }

        public HostStatusData GetCurrentResource()
        {
            throw new NotImplementedException();

        }

        public dynamic TestWCFDynamic(dynamic param)
        {
            if (param == 0)
            {
                return new CrawlStatusData();
            }
            if (param == 1)
            {
                return new RuntimeData();
            }
            return "Test";
        }

        public HostHourCountData GetHourCount(HostType hostType)
        {
            if (hostType != HostType.Crawler)
            {
                throw new NotSupportedException("不支持该类型操作");
            }
            var hostData = HostBasicData.GetHostData();
            HostHourCountData result = new HostHourCountData()
            {
                HostData = hostData
            };
            //获取HourCount
            HourCountData[] data = 
            {
                new HourCountData()
                {
                       Descrption = "运行总任务数",
                       HourCounter = CrawlerManager.ItemCount
                },
                
            };
            result.HourCounts = data;
            return result;
        }

        #endregion
    }
}

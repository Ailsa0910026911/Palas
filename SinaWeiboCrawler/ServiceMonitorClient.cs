using System;
using System.Linq;
using Palas.Common.Data;
using Palas.Common.DataAccess;

namespace SinaWeiboCrawler
{
    public class ServiceMonitorClient:IStatusReportClient
    {
        #region Implementation of IStatusReportClient

        public CrawlStatusData GetCurrentCrawlStatus()
        {
            throw new NotImplementedException();
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
            if (hostType != HostType.Weibo)
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
                       Descrption = Program.JobCounterDesc,
                       HourCounter = Program.JobCounter
                },
                
            };
            result.HourCounts = data;
            return result;
        }

        #endregion
    }
}

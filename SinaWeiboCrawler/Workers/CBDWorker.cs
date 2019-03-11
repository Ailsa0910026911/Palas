using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Module;
using Palas.Common.Data;
using SinaWeiboCrawler.DatabaseManager;
using System.Threading;
using NetDimension.Weibo;
using SinaWeiboCrawler.Utility;
using HooLab.Log;
using System.IO;
using Palas.Common.Utility;

namespace SinaWeiboCrawler.Workers
{
    /// <summary>
    /// 刷新CBD周围的人，即A2
    /// </summary>
    public class CBDWorker : IPipeline
    {
        private static string CrawlID = "WeiboCBD";

        HourCounter _CntData = new HourCounter();
        public HourCounter CntData
        {
            private set
            {
                _CntData = value;
            }
            get
            {
                return _CntData;
            }
        }

        PipelineInfo _Info = new PipelineInfo("WeiboCBD");
        public PipelineInfo Info
        {
            get
            {
                return _Info;
            }
        }

        public void SendMsg(string Msg)
        {
            Scheduler.SendMsg(this.Info, Msg);
        }

        static Scheduler _Scheduler;
        public Scheduler Scheduler
        {
            get { return _Scheduler; }
            set { _Scheduler = value; }
        }

        bool StopFlag = false;
        public void Stop()
        {
            StopFlag = true;
        }

        public int IntervalMS
        {
            get { return 60 * 1000; }
        }

        public CBDWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        public static void BackToOrigin() 
        {
            LocationDBManager.InitLocationJob();
        }

        private static object jobLock = new object();
        private Location GetNextJob() 
        {
            lock (jobLock)
            {
                SendMsg("正在获取下一任务");
                Location loc = LocationDBManager.GetNextCBDJob();
                if (loc == null)
                {
                    SendMsg("没有获取到任务，休息一会");
                    return null;
                }
                loc.RefreshStatus = Enums.CrawlStatus.Crawling;
                LocationDBManager.UpdateDB<Location>(loc, "PoID", new string[] { "RefreshStatus" }, MongoDB.Driver.SafeMode.True);
                SendMsg(string.Format("获取到{0}的任务", loc.Title));
                return loc;
            }
        }

        public string DoOneJob(IPipeline Pipeline) 
        {
            int SuccCount = 0, ErrCount = 0;
            DateTime nextWorkTime = Utilities.Epoch;
            

            while (!StopFlag) 
            {
                if (DateTime.Now > nextWorkTime)
                {
                    Location loc = GetNextJob();
                    if (loc != null)
                    {
                        try
                        {
                            DateTime lastTime = loc.LastRefreshTime;
                            loc.LastRefreshTime = DateTime.Now;
                            loc.NextRefreshTime = DateTime.Now.AddMinutes(loc.IntervalMins);
                            //获取CBD周围的人
                            List<dynamic> users = new List<dynamic>();
                            List<dynamic> statuses = new List<dynamic>();

                            try
                            {
                                SendMsg(string.Format("正在刷新{0}周围的位置动态", loc.Title));
                                WeiboAPI.GetUsersNearCBD(loc.Lon, loc.Lat, loc.Radius, Utilities.DateTime2UnixTime(lastTime), Utilities.DateTime2UnixTime(DateTime.Now), loc.LocationSampleMethode, users, statuses);
                            }
                            catch (IOException) 
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (WeiboException ex)
                            {
                                SendMsg("获取CBD周围动态时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("{0}的位置动态获取到{1}条，开始插入数据库", loc.Title, users.Count()));
                            for (int i = 0; i < users.Count; ++i)
                            {
                                if (i >= statuses.Count) break;
                                if (statuses[i] == null || users[i] == null) continue;
                                Item item = ItemDBManager.ConvertToItem(Enums.AuthorSource.LocationScan, CrawlID, statuses[i], users[i]);
                                if (item.PoID == null)
                                    item.PoID = item.PoIDSource = loc.CategoryID;
                                ItemDBManager.InsertOrUpdateItem(item);
                                Author author = AuthorDBManager.ConvertToAuthor(users[i], Enums.AuthorSource.LocationScan);
                                AuthorDBManager.InsertOrUpdateAuthorInfo(author);
                                CntData.Tick();
                            }

                            SuccCount++;
                            SendMsg(string.Format("{0}的任务完成", loc.Title));
                            continue;
                        }
                        catch (Exception ex)
                        {
                            ErrCount++;
                            nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            SendMsg(ex.ToString());
                            Logger.Error(ex.ToString());
                        }
                        finally 
                        {
                            loc.RefreshCount++;
                            loc.RefreshStatus = Enums.CrawlStatus.Normal;
                            LocationDBManager.PushbackLoationJob(loc);
                        }
                    }
                }
                SendMsg(string.Format("休息{0}秒", IntervalMS / 1000));
                Thread.Sleep(IntervalMS);
            }
            StopFlag = false;
            return SuccCount == 0 && ErrCount == 0 ? "Nothing to do" : string.Format("OneJob Done. Succ {0} Err {1}", SuccCount, ErrCount);
        }
    }
}

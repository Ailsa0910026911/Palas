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
using System.IO;
using HooLab.Log;
using Palas.Common.Utility;

namespace SinaWeiboCrawler.Workers
{
    /// <summary>
    /// 特定用户活动地点历史记录，即B3
    /// </summary>
    public class AuthorLocHistWorker : IPipeline
    {
        private static string CrawlID = "WeiboLocHistory";

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

        PipelineInfo _Info = new PipelineInfo("WeiboLocHistory");
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
            get { return 60 * 60 * 1000; }
        }

        public AuthorLocHistWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        private static object jobLock = new object();
        private Author GetNextJob() 
        {
            lock (jobLock)
            {
                SendMsg("正在获取下一任务");
                Author author = AuthorDBManager.GetNextLocHistJob();
                if (author == null)
                {
                    SendMsg("没有获取到任务，休息一会");
                    return null;
                }
                author.Location_RefreshStatus = Enums.CrawlStatus.Crawling;
                AuthorDBManager.UpdateDB<Author>(author, "AuthorID", new string[] { "Location_RefreshStatus" }, MongoDB.Driver.SafeMode.True);
                SendMsg(string.Format("获取到{0}的任务", author.AuthorName));
                return author;
            }
        }

        public static void BackToOrigin() 
        {
            AuthorDBManager.InitLocHistJob();
        }

        public string DoOneJob(IPipeline Pipeline)
        {
            int SuccCount = 0, ErrCount = 0;
            DateTime nextWorkTime = Utilities.Epoch;
            

            while (!StopFlag)
            {
                if (DateTime.Now > nextWorkTime)
                {
                    Author author = GetNextJob();
                    if (author != null)
                    {
                        try
                        {
                            //获取最新若干带有地点信息的微博
                            List<dynamic> result = new List<dynamic>();
                            try
                            {
                                SendMsg(string.Format("正在刷新{0}的位置动态", author.AuthorName));
                                WeiboAPI.GetUserStatusLocationHistory(author, result, author.LocationSampleMethode);
                            }
                            catch (IOException)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取位置动态时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                SendMsg(ex.ToString());
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("{0}的位置动态获取到{1}条，开始插入数据库", author.AuthorName, result.Count()));
                            for (int i = 0; i < result.Count; ++i)
                            {
                                Item item = ItemDBManager.ConvertToItem(author.AuthorSource, CrawlID, result[i], null, author);
                                if (item.PoID == null) continue;
                                ItemDBManager.InsertOrUpdateItem(item);
                                CntData.Tick();
                            }

                            SuccCount++;
                            SendMsg(string.Format("{0}的任务完成", author.AuthorName));
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
                            author.Location_RefreshStatus = Enums.CrawlStatus.Normal;
                            author.Location_UpdateTime = DateTime.Now;
                            author.Location_NextRefreshTime = DateTime.Now.AddDays(author.Location_IntervalDays);
                            author.Location_UpdateCount++;
                            AuthorDBManager.PushbackLocHistJob(author);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Module;
using SinaWeiboCrawler.DatabaseManager;
using Palas.Common.Lib.Entity;
using System.Threading;
using Palas.Common.Data;
using NetDimension.Weibo;
using SinaWeiboCrawler.Utility;
using Palas.Common.Lib.Business;
using HooLab.Log;
using System.IO;
using Palas.Common.Utility;

namespace SinaWeiboCrawler.Workers
{
    /// <summary>
    /// 刷新僵尸粉订阅的微博，即B1
    /// </summary>
    public class StatusSubscribeWorker : IPipeline
    {
        private static string CrawlID = "WeiboSub";

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

        PipelineInfo _Info = new PipelineInfo("WeiboSub");
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

        public StatusSubscribeWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        private static object jobLock = new object();
        private LoginAccountEntity GetNextJob() 
        {
            lock (jobLock)
            {
                SendMsg("正在获取下一任务");
                var acc =  AccountDBManager.GetNextSubscribeJob();
                if (acc != null)
                    SendMsg("获取到" + acc.UserName + "的任务");
                else SendMsg("没有获取到任务，休息一会");
                return acc;
            }
        }

        public static void BackToOrigin() 
        {
            AccountDBManager.SetAllSinceIDToNull();
        }

        public string DoOneJob(IPipeline Pipeline)
        {
            int SuccCount = 0, ErrCount = 0;
            DateTime nextWorkTime = Utilities.Epoch;
            

            while (!StopFlag)
            {
                if (DateTime.Now > nextWorkTime)
                {
                    LoginAccountEntity account = GetNextJob();
                    if (account != null)
                    {
                        try
                        {
                            //刷新订阅的微博
                            List<NetDimension.Weibo.Entities.status.Entity> result = new List<NetDimension.Weibo.Entities.status.Entity>();
                            try
                            {
                                SendMsg(string.Format("正在刷新{0}关注的最新微博", account.UserName));
                                WeiboAPI.FetchStatus(account, result);
                            }
                            catch (IOException)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取新微博时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                SendMsg(ex.ToString());
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("{0}关注的微博抓取到{1}条，开始插入", account.UserName, result.Count));
                            for (int i = 0; i < result.Count; ++i)
                            {
                                var item = ItemDBManager.ConvertToItem(result[i], Enums.AuthorSource.PublicLeader, CrawlID);
                                ItemDBManager.InsertOrUpdateItem(item);
                                CntData.Tick();
                            }

                            SendMsg(string.Format("{0}的关注微博刷新任务完成", account.UserName));
                            SuccCount++;
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
                            AccountDBManager.PushbackSubscribeJob(account);
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

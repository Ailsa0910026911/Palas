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

namespace SinaWeiboCrawler.Workers
{
    /// <summary>
    /// 单篇微博评论跟踪的工人，即C2
    /// </summary>
    public class ReplyTrackingWorker : IPipeline
    {
        PipelineInfo _Info = new PipelineInfo("ReplyTrackingWorker");
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
            get { return 3000; }
        }

        public ReplyTrackingWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        private Item GetNextJob()
        {
            Item item = ItemDBManager.GetNextReplyTrackingJob();
            if (item == null) return null;
            item.Tracking.FollowStatus = Enums.CrawlStatus.Crawling;
            ItemDBManager.UpdateDB<Item>(item, "ItemID", new Tuple<string, BsonValueType>[] { new Tuple<string, BsonValueType>("Tracking", BsonValueType.Document) }, MongoDB.Driver.SafeMode.False);
            return item;
        }

        public string DoOneJob(IPipeline Pipeline)
        {
            SendMsg("暂时不执行回复跟踪任务");
            StopFlag = true;
            int SuccCount = 0, ErrCount = 0;
            DateTime nextWorkTime = Utilities.Epoch;
            while (!StopFlag)
            {
                if (DateTime.Now > nextWorkTime)
                {
                    Item item = GetNextJob();
                    if (item != null)
                    {
                        try
                        {
                            //最新评论列表
                            List<NetDimension.Weibo.Entities.comment.Entity> result = new List<NetDimension.Weibo.Entities.comment.Entity>();
                            try
                            {
                                WeiboAPI.GetCommentsOfStatus(item, result);
                            }
                            catch (WeiboException ex)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                SendMsg(ex.ToString());
                            }
                            for (int i = 0; i < result.Count; ++i)
                            {
                                var reply = ItemReplyDBManager.ConvertToItemReply(result[i]);
                                ItemReplyDBManager.InsertItemReply(reply);
                            }

                            item.Tracking.ReplyCount += result.Count;
                            item.Tracking.FollowCount++;
                            if (WeiboUtilities.ShouldKeepFollow(item))
                                item.Tracking.FollowStatus = Enums.CrawlStatus.Normal;
                            else item.Tracking.FollowStatus = Enums.CrawlStatus.Stop;
                            SuccCount++;
                            continue;
                        }
                        catch (WeiboException ex)
                        {
                            item.Tracking.FollowStatus = Enums.CrawlStatus.Normal;
                            ErrCount++;
                            nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            SendMsg(ex.ToString());
                        }
                        finally 
                        {
                            item.Tracking.FollowNextTime = DateTime.Now.AddMinutes(DefaultSettings.ReplyTrackingInterval.TotalMinutes);
                            ItemDBManager.PushbackReplyTrackingJob(item);
                        }
                    }
                }
                Thread.Sleep(IntervalMS);
            }
            StopFlag = false;
            return SuccCount == 0 && ErrCount == 0 ? "Nothing to do" : string.Format("OneJob Done. Succ {0} Err {1}", SuccCount, ErrCount);
        }
    }
}

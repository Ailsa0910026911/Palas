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
    /// 微博转发跟踪的工人，即C2
    /// </summary>
    public class RepostTrackingWorker : IPipeline
    {
        private static string CrawlID = "RepostTrackingWorker";

        PipelineInfo _Info = new PipelineInfo("RepostTrackingWorker");
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

        public RepostTrackingWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        private Item GetNextJob()
        {
            Item item = ItemDBManager.GetNextRepostTrackingJob();
            if (item == null) return null;
            item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Crawling;
            ItemDBManager.UpdateDB<Item>(item, "ItemID", new Tuple<string, BsonValueType>[] { new Tuple<string, BsonValueType>("Tracking_Forward", BsonValueType.Document) }, MongoDB.Driver.SafeMode.False);
            return item;
        }

        public string DoOneJob(IPipeline Pipeline)
        {
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
                            #region 获取最新转发列表
                            List<NetDimension.Weibo.Entities.status.Entity> result = new List<NetDimension.Weibo.Entities.status.Entity>();
                            try
                            {
                                WeiboAPI.GetRepostOfStatus(item, result);
                            }
                            catch (WeiboException ex)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                SendMsg(ex.ToString());
                            }
                            for (int i = 0; i < result.Count; ++i)
                            {
                                var newItem = ItemDBManager.ConvertToItem(result[i], Enums.AuthorSource.TopicTrack, CrawlID);
                                ItemDBManager.InsertOrUpdateItem(newItem);
                            }
                            #endregion

                            #region 更新转发评论数的历史记录
                            try
                            {
                                var countData = WeiboAPI.GetRepostAndReplyCount(item.ClientItemID);
                                item.CurrentCount.FetchTime = DateTime.Now;
                                item.CurrentCount.ForwardCount = countData.Item1;
                                item.CurrentCount.ReplyCount = countData.Item2;
                            }
                            catch (WeiboException ex) 
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                SendMsg(ex.ToString());
                            }
                            
                            List<ItemCountData> count = null;
                            if (item.CountHistory == null)
                                count = new List<ItemCountData>();
                            else count = new List<ItemCountData>(item.CountHistory);
                            count.Add(item.CurrentCount);
                            item.CountHistory = count.ToArray();
                            #endregion

                            item.Tracking_Forward.FollowCount++;
                            if (WeiboUtilities.ShouldKeepFollow(item))
                                item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Normal;
                            else item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Stop;

                            SuccCount++;
                            continue;
                        }
                        catch (Exception ex)
                        {
                            item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Normal;
                            ErrCount++;
                            nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            SendMsg(ex.ToString());
                        }
                        finally 
                        {
                            item.Tracking_Forward.FollowNextTime = DateTime.Now.AddMinutes(DefaultSettings.RepostTrackingInterval.TotalMinutes);
                            ItemDBManager.PushbackRepostTrackingJob(item);
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

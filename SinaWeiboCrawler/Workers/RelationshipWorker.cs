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
    /// 关注/粉丝关系调查的工人，即C1
    /// </summary>
    public class RelationshipWorker : IPipeline
    {
        PipelineInfo _Info = new PipelineInfo("RelationshipWorker");

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

        public RelationshipWorker(string Name, Scheduler Scheduler)
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
                Author author = AuthorDBManager.GetNextRelationshipJob();
                if (author == null)
                {
                    SendMsg("没有获取到任务，休息一会");
                    return null;
                }
                author.Fans_RefreshStatus = Enums.CrawlStatus.Crawling;
                author.Fans_UpdateTime = DateTime.Now;
                AuthorDBManager.UpdateDB<Author>(author, "AuthorID", new string[] { "Fans_RefreshStatus", "Fans_UpdateTime" }, MongoDB.Driver.SafeMode.True);
                SendMsg("获取到" + author.AuthorName + "的任务");
                return author;
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
                    Author author = GetNextJob();
                    if (author != null)
                    {
                        try
                        {
                            //如果不是红人，那么只刷新一次就结束
                            if (WeiboUtilities.IsRedSkin(author.AuthorSource))
                                author.Fans_RefreshStatus = Enums.CrawlStatus.Normal;
                            else author.Fans_RefreshStatus = Enums.CrawlStatus.Stop;

                            #region 用户粉丝刷新
                            List<NetDimension.Weibo.Entities.user.Entity> users = new List<NetDimension.Weibo.Entities.user.Entity>();
                            try
                            {
                                SendMsg(string.Format("正在刷新{0}的粉丝", author.AuthorName));
                                WeiboAPI.GetFollowers(author.AuthorID, author.FollowerSampleMethode, users);
                            }
                            catch (IOException)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取粉丝列表时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                author.Fans_RefreshStatus = Enums.CrawlStatus.Normal;
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("{0}的粉丝抓取到{1}个，开始插入数据库", author.AuthorName, users.Count));
                            double avg = 0; //用户粉丝的粉丝平均数
                            for (int i = 0; i < users.Count; ++i)
                            {
                                var user = AuthorDBManager.ConvertToAuthor(users[i], Enums.AuthorSource.FansDiscover);
                                AuthorDBManager.InsertOrUpdateAuthorInfo(user);
                                CntData.Tick();
                                AuthorRelationDBManager.InsertOrUpdateRelation(user.AuthorID, author.AuthorID);
                                avg += (double)users[i].FollowersCount / (double)users.Count;
                            }
                            #endregion

                            #region 用户关注列表
                            try
                            {
                                IEnumerable<string> friends = null;
                                SendMsg(string.Format("{0}的粉丝插入完成，开始获取他的关注列表", author.AuthorName));
                                friends = WeiboAPI.GetFriendsIDs(author.AuthorID, author.FansSampleMethode);
                                if (friends != null)
                                {
                                    foreach (var user in friends)
                                        AuthorRelationDBManager.InsertOrUpdateRelation(user, author.AuthorID);
                                }
                            }
                            catch (IOException)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取关注列表时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                author.Fans_RefreshStatus = Enums.CrawlStatus.Normal;
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("{0}的关系刷新任务完成", author.AuthorName));
                            #endregion

                            author.AvgFansCountOfFans = (int)avg;
                            SuccCount++;
                            continue;
                        }
                        catch (Exception ex)
                        {
                            ErrCount++;
                            nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            author.Fans_RefreshStatus = Enums.CrawlStatus.Normal;
                            SendMsg(ex.ToString());
                            Logger.Error(ex.ToString());
                        }
                        finally 
                        {
                            author.Fans_UpdateCount++;
                            author.Fans_NextRefreshTime = DateTime.Now.AddDays(author.Fans_IntervalDays);
                            AuthorDBManager.PushbackRelationshipJob(author);
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

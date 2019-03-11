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
    /// 用户普查的工人，即B2工作
    /// </summary>
    public class AuthorCensusWorker : IPipeline
    {
        private static string CrawlID = "WeiboUserCensus";

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

        PipelineInfo _Info = new PipelineInfo("WeiboUserCensus");
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

        public AuthorCensusWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        public static void BackToOrigin() 
        {
            AuthorDBManager.InitCensusJob();
        }

        private static object jobLock = new object();
        private Author GetNextJob() 
        {
            lock (jobLock)
            {
                SendMsg("正在获取下一任务");
                Author author = AuthorDBManager.GetNextCensusJob();
                if (author == null)
                {
                    SendMsg("没有获取到任务，休息一会");
                    return null;
                }
                author.RefreshStatus = Enums.CrawlStatus.Crawling;
                AuthorDBManager.UpdateDB<Author>(author, "AuthorID", new string[] { "RefreshStatus" }, MongoDB.Driver.SafeMode.True);
                SendMsg("获取到" + author.AuthorName + "的普查任务");
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
                            author.RefreshStatus = Enums.CrawlStatus.Stop;

                            try
                            {
                                SendMsg(string.Format("正在更新用户{0}的个人信息", author.AuthorName));
                                //更新用户个人信息
                                NetDimension.Weibo.Entities.user.Entity user = WeiboAPI.GetAuthorInfo(author.AuthorID);
                                var author_new = AuthorDBManager.ConvertToAuthor(user, author.AuthorSource);
                                AuthorDBManager.InsertOrUpdateAuthorInfo(author);
                            }
                            catch (IOException)
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取个人信息时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                author.RefreshStatus = Enums.CrawlStatus.Normal;
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("用户{0}的个人信息更新完毕，开始刷新其最新微博", author.AuthorName));
                            List<NetDimension.Weibo.Entities.status.Entity> result = new List<NetDimension.Weibo.Entities.status.Entity>();
                            try
                            {
                                //获取最新若干微博
                                WeiboAPI.GetAuthorLatestStatus(author, result, author.PostSampleMethode);
                            }
                            catch (IOException) 
                            {
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            }
                            catch (Exception ex)
                            {
                                SendMsg("获取最新微博时发生错误，见日志");
                                ErrCount++;
                                nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                                author.RefreshStatus = Enums.CrawlStatus.Normal;
                                Logger.Error(ex.ToString());
                            }

                            SendMsg(string.Format("找到{0}的{1}条最新微博，开始插入数据库并统计相关信息", author.AuthorName, result.Count));
                            //同时更新平均转发数和评论数
                            double avgForward = 0, avgReply = 0;
                            for (int i = 0; i < result.Count; ++i)
                            {
                                avgForward += (double)result[i].RepostsCount / (double)result.Count;
                                avgReply += (double)result[i].CommentsCount / (double)result.Count;
                                var item = ItemDBManager.ConvertToItem(result[i], author.AuthorSource, CrawlID);
                                ItemDBManager.InsertOrUpdateItem(item);
                                CntData.Tick();
                            }

                            author.AvgForward = (int)avgForward;
                            author.AvgReply = (int)avgReply;
                            SuccCount++;
                            continue;
                        }
                        catch (Exception ex)
                        {
                            ErrCount++;
                            nextWorkTime = WeiboAPI.rateLimitStatus.ResetTime;
                            author.RefreshStatus = Enums.CrawlStatus.Normal;
                            SendMsg(ex.ToString());
                        }
                        finally 
                        {
                            author.UpdateCount++;
                            author.UpdateTime = DateTime.Now;
                            author.NextRefreshTime = author.UpdateTime.AddDays(author.IntervalDays);
                            AuthorDBManager.PushbackCensusJob(author);
                            SendMsg(string.Format("用户{0}的普查任务完成", author.AuthorName));
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

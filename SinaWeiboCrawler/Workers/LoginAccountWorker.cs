using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Module;
using SinaWeiboCrawler.Utility;
using System.Threading;
using SinaWeiboCrawler.DatabaseManager;
using HooLab.Log;
using System.IO;
using Palas.Common.Utility;

namespace SinaWeiboCrawler.Workers
{
    public class LoginAccountWorker : IPipeline
    {
        PipelineInfo _Info = new PipelineInfo("LoginAccountWorker");

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
            get { return 10 * 1000; }
        }

        public LoginAccountWorker(string Name, Scheduler Scheduler)
        {
            _Info = new PipelineInfo(Name);
            _Scheduler = Scheduler;
        }

        private string GetNextJob()
        {
            return AccountDBManager.GetNextFollowJob();
        }

        /// <summary>
        /// 由于每天每个账号至多订阅数百个用户，所以只需要开一个线程
        /// </summary>
        /// <param name="Pipeline"></param>
        /// <returns></returns>
        public string DoOneJob(IPipeline Pipeline)
        {
            int SuccCount = 0, ErrCount = 0;
            DateTime nextWorkTime = Utilities.Epoch;
            

            while (!StopFlag)
            {
                if (DateTime.Now > nextWorkTime)
                {
                    string authorID = GetNextJob();
                    if (authorID != null)
                    {
                        try
                        {
                            SendMsg("待关注用户: " + authorID);
                            var status = AccountDBManager.FollowUser(authorID);
                            if (status == AccountDBManager.FollowStatus.Succ)
                            {
                                SendMsg("关注用户成功");
                                CntData.Tick();
                                SuccCount++;
                            }
                            else
                            {
                                if (status == AccountDBManager.FollowStatus.Exception)
                                {
                                    SendMsg("出现异常，休息1小时后再尝试");
                                    nextWorkTime = DateTime.Now.AddHours(1);
                                }
                                else
                                {
                                    SendMsg("账号关注频率受限，1分钟后再试");
                                    Thread.Sleep(1000 * 60);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            nextWorkTime = DateTime.Now.AddHours(1);
                            Logger.Error(ex.ToString());
                            ErrCount++;
                        }
                    }
                    else SendMsg("所有任务已完成");
                }
                Thread.Sleep(IntervalMS);
            }
            StopFlag = false;
            return SuccCount == 0 && ErrCount == 0 ? "Nothing to do" : string.Format("OneJob Done. Succ {0} Err {1}", SuccCount, ErrCount);
        }
    }
}

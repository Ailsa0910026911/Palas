using HooLab.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vote.Common
{
    public class ValidateVote
    {
        /// <summary>
        /// 上次删除过期数据时间
        /// </summary>
        private static DateTime lastRefreashTime = DateTime.Now;
        /// <summary>
        /// 刷新间隔
        /// </summary>
        private static int ExpireMin = 30;

        private static object lockkey = new object();

        private static List<VoteInfo> _iPList = new List<VoteInfo>();

        public static List<VoteInfo> IPList
        {
            get
            {
                if (lastRefreashTime.AddMinutes(ExpireMin) < DateTime.Now)
                {
                    lock (lockkey)
                    {
                        if (lastRefreashTime.AddMinutes(ExpireMin) < DateTime.Now)
                        {
                            try
                            {
                                _iPList = _iPList.Where(o => o.ExpireTime > DateTime.Now).ToList();
                            }
                            catch
                            {
                                _iPList = new List<VoteInfo>();
                            }
                            lastRefreashTime = DateTime.Now;
                        }
                    }
                }
                return _iPList;
            }
            set
            {
                _iPList = value;
            }
        }

        /// <summary>
        /// 验证Vote是否被允许
        /// </summary>
        /// <param name="VoteId"></param>
        /// <param name="IP"></param>
        /// <param name="SessionId"></param>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public static bool VaildateVote(string VoteId, string IP, string SessionId, string UserAgent)
        {
            //Logger.Error(string.Format("Vaildate In {0} {1} {2} {3}", VoteId, IP, SessionId, UserAgent));
            //判断IP, UserAgent, SessionId是否为null
            if (string.IsNullOrEmpty(IP) || string.IsNullOrEmpty(SessionId) || string.IsNullOrEmpty(UserAgent))
                return false;

            try
            {
                //判断SessionId规定时间内是否已经投过了
                var session = IPList.Where(f => f.SessionId == SessionId);
                if (session != null && session.Count() > 0)
                    return false;

                //判断IP当日投票数是否大于500
                var IPDays = IPList.Where(f => f.IP == IP && f.VoteTime > DateTime.Now.AddDays(-1));
                if (IPDays != null && IPDays.Count() > 500)
                    return false;

                //判断IP一小时投票数是否大于100
                var IPHours = IPList.Where(f => f.IP == IP && f.VoteTime > DateTime.Now.AddHours(-1));
                if (IPHours != null && IPHours.Count() > 100)
                    return false;

                //判断相同IP和UserAgent投票数是否大于10
                var IPUserAgent = IPList.Where(f => f.IP == IP && f.UserAgent == UserAgent);
                if (IPUserAgent != null && IPUserAgent.Count() > 10)
                    return false;

                //相同IP与UserAgent只能对单个用户投一次票
                var IPUserAgentUser = IPList.Where(f => f.IP == IP && f.UserAgent == UserAgent && f.VoteId == VoteId);
                if (IPUserAgentUser != null && IPUserAgentUser.Count() > 0)
                    return false;

                //判断相同IP 30分钟内是否存在连续三次间隔在10s内，如果存在则返回false
                if (IPHours != null)
                {
                    bool isfalse = false;
                    int count = 0;
                    DateTime lastTime = DateTime.MinValue;
                    foreach (VoteInfo item in IPHours.OrderBy(f => f.VoteTime))
                    {
                        if (Math.Abs((item.VoteTime - lastTime).TotalSeconds) < 10)
                        {
                            count++;
                            if (count > 3)
                            {
                                isfalse = true;
                                break;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                        lastTime = item.VoteTime;
                    }

                    if (isfalse)
                        return false;
                }

                var IPStart = IPList.Where(f => f.VoteTime > DateTime.Now.AddMinutes(-20) && f.IP.StartsWith(IP.Substring(0, IP.LastIndexOf('.'))));

                //判断相同IP段内20分钟的投票数是否大于500
                if (IPStart != null && IPStart.Count() > 500)
                    return false;

                //判断相同IP段 20分钟内是否存在连续8次间隔在5s内，存在则返回false
                if (IPStart != null)
                {
                    bool isfalse = false;
                    int count = 0;
                    DateTime lastTime = DateTime.MinValue;
                    foreach (VoteInfo item in IPStart.OrderBy(f => f.VoteTime))
                    {
                        if (Math.Abs((item.VoteTime - lastTime).TotalSeconds) < 5)
                        {
                            count++;
                            if (count > 8)
                            {
                                isfalse = true;
                                break;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                        lastTime = item.VoteTime;
                    }

                    if (isfalse)
                        return false;
                }


                IPList.Add(new VoteInfo() { VoteId = VoteId, IP = IP, SessionId = SessionId, UserAgent = UserAgent, VoteTime = DateTime.Now, ExpireTime = DateTime.Now.AddDays(1) });

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
            //Logger.Error("Has IP LIST COUNT :" + IPList.Count);

            return true;
        }
    }

    public class VoteInfo
    {
        public string VoteId { set; get; }
        public string IP { set; get; }
        public string SessionId { set; get; }
        public string UserAgent { set; get; }
        public DateTime VoteTime { set; get; }
        public DateTime ExpireTime { set; get; }
    }
}

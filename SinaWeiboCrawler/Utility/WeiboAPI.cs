using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetDimension.Weibo;
using System.Threading;
using System.Diagnostics;
using Palas.Common.Lib.Entity;
using Palas.Common.Data;
using SinaWeiboCrawler.DatabaseManager;
using System.IO;
using NetDimension.Json;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 微博API（内部调用）
    /// </summary>
    class WeiboAPI
    {
        private static NetDimension.Weibo.Entities.RateLimitStatus _rateLimitStatus = null;
        private static readonly object permissionLock = new object();
        private static DateTime lastLimitUpdateTime = new DateTime(0);
        /// <summary>
        /// 本地保存的请求限制信息
        /// </summary>
        public static NetDimension.Weibo.Entities.RateLimitStatus rateLimitStatus
        {
            get 
            {
                if (_rateLimitStatus == null || DateTime.Now > _rateLimitStatus.ResetTime || DateTime.Now - lastLimitUpdateTime > DefaultSettings.IPStatusUpdateInterval)
                {
                    lock (permissionLock) 
                    {
                        if (_rateLimitStatus == null || DateTime.Now > _rateLimitStatus.ResetTime || DateTime.Now - lastLimitUpdateTime > DefaultSettings.IPStatusUpdateInterval)
                        {
                            var bak = _rateLimitStatus;
                            try
                            {
                                Interlocked.Exchange(ref _rateLimitStatus, defaultClient.API.Entity.Account.RateLimitStatus());
                            }
                            catch (Exception) 
                            {
                                Interlocked.Exchange(ref _rateLimitStatus, bak);
                            }
                            lastLimitUpdateTime = DateTime.Now;
                        }
                    }
                }
                return _rateLimitStatus;
            }
        }

        /// <summary>
        /// 本地进行请求次数限制
        /// </summary>
        /// <param name="requireCnt">需要请求的访问次数</param>
        private static void RequireAccPermission(int requireCnt) 
        {
            if (rateLimitStatus.RemainingIPHits < requireCnt)
                throw new WeiboException("10022", string.Empty, string.Empty);
            else
                Interlocked.Add(ref rateLimitStatus.RemainingIPHits, -requireCnt);
        }

        private static Client _client = null;
        private static readonly object syncObj = new object();
        /// <summary>
        /// 默认使用的Client，用来进行不需要指定僵尸的任务
        /// </summary>
        private static Client defaultClient
        {
            get
            {
                if (_client != null)
                {
                    return _client;
                }
                lock (syncObj)
                {
                    if (_client == null)
                    {
                        var oauth = new NetDimension.Weibo.OAuth(DefaultSettings.AppKey, DefaultSettings.AppSecret, string.Empty);
                        var result = oauth.ClientLogin(DefaultSettings.defaultZombieName, DefaultSettings.defaultZombiePwd);
                        if (result)
                        {
                            Client current = new Client(oauth);
                            Interlocked.Exchange(ref _client, current);
                        }
                        else
                        {
                            throw new Exception("验证不成功");
                        }
                    }
                }
                return _client;
            }
        }

        private static object clientCacheLock = new object();
        /// <summary>
        /// 僵尸账号登录缓存
        /// </summary>
        private static Dictionary<string, Client> clientCache = new Dictionary<string, Client>();
        /// <summary>
        /// 获取特定的僵尸以执行任务，如订阅、关注等
        /// </summary>
        /// <param name="username">僵尸用户名</param>
        /// <returns>特定僵尸Client</returns>
        private static Client GetSpecificClient(string username) 
        {
            lock (clientCacheLock)
            {
                if (clientCache.ContainsKey(username))
                    return clientCache[username];
                string password = AccountDBManager.GetPassword(username);
                RequireAccPermission(3);
                OAuth o = new OAuth(DefaultSettings.AppKey, DefaultSettings.AppSecret, String.Empty);
                o.ClientLogin(username, password);
                Client client = new Client(o);
                clientCache.Add(username, client);
                return client;
            }
        }

        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <returns>个人信息</returns>
        public static NetDimension.Weibo.Entities.user.Entity GetAuthorInfo(string AuthorID) 
        {
            RequireAccPermission(1);
            return defaultClient.API.Entity.Users.Show(AuthorID);
        }

        /// <summary>
        /// 获取用户名对应的ID(不消耗请求次数)
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>AuthorID</returns>
        public static string GetAuthorID(string username) 
        {
            return GetSpecificClient(username).API.Entity.Account.GetUID();
        }

        /// <summary>
        /// 获取用户至多5000个粉丝
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <param name="sampleMethod">采样数量</param>
        /// <param name="result">返回结果</param>
        public static void GetFollowers(string AuthorID, Enums.SampleMethod sampleMethod, List<NetDimension.Weibo.Entities.user.Entity> result) 
        {
            int target = Utilities.GetAimCount(sampleMethod);
            int cur = 1;
            do
            {
                RequireAccPermission(1);
                var info = defaultClient.API.Entity.Friendships.Followers(AuthorID, null, 200, cur);
                foreach (var user in info.Users) 
                    result.Add(user);
                cur = int.Parse(info.NextCursor);
            } while (cur != 0 && result.Count < target);
        }

        /// <summary>
        /// 获取CBD附近发微博的人
        /// </summary>
        /// <param name="longitude">CBD经度</param>
        /// <param name="latitude">CBD纬度</param>
        /// <param name="range">CBD辐射半径</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>该位置附近特定时间段发微博的人及其所发微博ID</returns>
        public static void GetUsersNearCBD(double longitude, double latitude, int range, Int64 startTime, Int64 endTime, Enums.SampleMethod sampleMethod, List<dynamic> users, List<dynamic> statuses) 
        {
            int cur = 1, target = Utilities.GetAimCount(sampleMethod);
            do
            {
                try
                {
                    RequireAccPermission(1);
                    dynamic userlist = defaultClient.API.Dynamic.Place.NearByUsers((float)latitude, (float)longitude, range, 50, cur++, startTime, endTime);
                    try
                    {
                        if (userlist == null || userlist.users == null) break;
                        foreach (dynamic user in userlist.users)
                        {
                            users.Add(user);
                            try
                            {
                                dynamic tmp = user.status;
                                statuses.Add(tmp);
                            }
                            catch (Exception) { statuses.Add(null); }
                        }
                    }
                    catch (Exception) { break; }
                }
                catch (JsonReaderException) 
                {
                    break;
                }
                catch (WeiboException ex)
                {
                    //如果返回内容为空则代表结束
                    if (ex.ErrorCode == "00000" || ex.ErrorCode == "23201")
                        break;
                    else throw ex;
                }
            } while (users.Count < target);
        }

        /// <summary>
        /// 获取用户所关注的人的ID列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sampleMethod">采样数量</param>
        /// <returns>ID列表</returns>
        public static IEnumerable<string> GetFriendsIDs(string userID, Enums.SampleMethod sampleMethod) 
        {
            if (sampleMethod == Enums.SampleMethod.None) return null;
            int target = Utilities.GetAimCount(sampleMethod);
            target = Math.Min(target, 5000);
            RequireAccPermission(1);
            var users = defaultClient.API.Entity.Friendships.FriendIDs(userID, null, target);
            return users.Users;
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="username">账号名</param>
        /// <param name="targetAuthorID">需要取消的被关注者ID</param>
        public static void UnFollowUser(string username, string targetAuthorID) 
        {
            RequireAccPermission(1);
            try
            {
                GetSpecificClient(username).API.Entity.Friendships.Destroy(targetAuthorID);
            }
            catch (WeiboException ex) 
            {
                if (ex.ErrorCode != "20003") throw ex;
            }
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="username">关注出发者（僵尸）</param>
        /// <param name="AuthorID">被关注用户的ID</param>
        public static void FollowUser(string username, string AuthorID) 
        {
            RequireAccPermission(1);
            GetSpecificClient(username).API.Entity.Friendships.Create(AuthorID);
        }

        /// <summary>
        /// 获取用户最新的若干微博
        /// </summary>
        /// <param name="account">用来订阅的僵尸账号</param>
        /// <param name="result">返回结果</param>
        public static void FetchStatus(LoginAccountEntity account, List<NetDimension.Weibo.Entities.status.Entity> result) 
        {
            int cur = 1;
            string sinceID = account.SubscribeSinceID;
            do
            {
                try
                {
                    RequireAccPermission(1);
                    var statuslist = GetSpecificClient(account.UserName).API.Entity.Statuses.HomeTimeline(sinceID, "", 50, cur++);
                    if (statuslist == null || statuslist.Statuses == null) break;
                    foreach (var status in statuslist.Statuses)
                    {
                        result.Add(status);
                        try
                        {
                            //更新sinceID
                            if (string.IsNullOrEmpty(account.SubscribeSinceID) || Int64.Parse(status.ID) > Int64.Parse(account.SubscribeSinceID))
                                account.SubscribeSinceID = status.ID;
                        }
                        catch (Exception) { throw new WeiboException("ID数字格式错误"); }
                    }
                    if (statuslist.NextCursor == "0" || statuslist.NextCursor == null) break;
                }
                catch (WeiboException ex) 
                {
                    if (ex.ErrorCode == "00000")
                        break;
                    else throw ex;
                }
            } while (true);
        }

        /// <summary>
        /// 获取用户前若干条微博
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sampleMethod">采样方法</param>
        /// <returns>微博集合</returns>
        public static void GetAuthorLatestStatus(Author author, List<NetDimension.Weibo.Entities.status.Entity> result, Enums.SampleMethod sampleMethod) 
        {
            if (sampleMethod == Enums.SampleMethod.None) return;
            int page = 1, target = Utilities.GetAimCount(sampleMethod), totalNum = target;
            string sinceID = author.LastSinceID;
            bool finished = false;
            do
            {
                RequireAccPermission(1);
                var statuslist = defaultClient.API.Entity.Statuses.UserTimeline(author.AuthorID, null, sinceID, null, 50, page++);
                if (statuslist == null || statuslist.Statuses == null) break;  //抓取结束条件
                totalNum = Math.Min(totalNum, statuslist.TotalNumber);
                finished = true;
                foreach (var status in statuslist.Statuses)
                {
                    result.Add(status);
                    try
                    {
                        if (string.IsNullOrEmpty(author.LastSinceID) || Int64.Parse(status.ID) > Int64.Parse(author.LastSinceID))
                            author.LastSinceID = status.ID;
                    }
                    catch (Exception) { throw new WeiboException("ID数字格式错误"); };
                    finished = false;
                }
            } while (!finished && result.Count < target && result.Count < totalNum);
        }

        /// <summary>
        /// 获取用户带有地点信息的微博
        /// </summary>
        /// <param name="author">用户ID</param>
        /// <param name="result">返回结果</param>
        /// <param name="sampleMethod">采样数量</param>
        public static void GetUserStatusLocationHistory(Author author, List<dynamic> result, Enums.SampleMethod sampleMethod)
        {
            if (sampleMethod == Enums.SampleMethod.None) return;
            int cur = 1, target = Utilities.GetAimCount(sampleMethod) ;
            string sinceID = author.Location_LastSinceID;
            do
            {
                try
                {
                    RequireAccPermission(1);
                    dynamic locationlist = defaultClient.API.Dynamic.Place.UserTimeline(author.AuthorID, sinceID, null, 50, cur++);
                    try
                    {
                        if (locationlist == null || locationlist.statuses == null) return;
                    }
                    catch (Exception) { return; }
                    foreach (dynamic status in locationlist.statuses)
                    {
                        result.Add(status);
                        try
                        {
                            if (string.IsNullOrEmpty(author.Location_LastSinceID) || Int64.Parse(status.id) > Int64.Parse(author.Location_LastSinceID))
                                author.Location_LastSinceID = status.id;
                        }
                        catch (Exception) { throw new WeiboException("ID数字格式错误"); }
                    }
                }
                catch (WeiboException ex)
                {
                    if (ex.ErrorCode == "00000" || ex.ErrorCode == "23201")
                        break;
                    else throw ex;
                }
            } while (result.Count < target);
        }

        public static void SetPOIInfo(Location loc, string clientPOI) 
        {
            RequireAccPermission(1);
            try
            {
                dynamic info = defaultClient.API.Dynamic.Place.POIShow(clientPOI);
                try
                {
                    loc.Address = info.address;
                }
                catch (Exception) { }
                try
                {
                    loc.Lon = float.Parse(info.lon);
                    loc.Lat = float.Parse(info.lat);
                }
                catch (Exception) 
                {
                    try
                    {
                        loc.Lon = (float)info.lon;
                        loc.Lat = (float)info.lat;
                    }
                    catch (Exception) { }
                }
                try
                {
                    loc.CategoryID = info.category;
                }
                catch (Exception) { }
                try
                {
                    loc.CategoryName = info.category_name;
                }
                catch (Exception) { }
                try
                {
                    loc.IconUrl = info.icon;
                }
                catch (Exception) { }
                try
                {
                    loc.Title = info.title;
                }
                catch (Exception) { }
                try
                {
                    loc.RegionID = WeiboUtilities.GetPOIRegionID(info.city);
                }
                catch (Exception) { }
                try
                {
                    loc.CheckInCount = int.Parse(info.checkin_num);
                }
                catch (Exception) 
                {
                    try
                    {
                        loc.CheckInCount = info.checkin_num;
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 获取单条微博的评论
        /// </summary>
        /// <param name="item">待获取评论的微博</param>
        /// <param name="result">返回结果</param>
        public static void GetCommentsOfStatus(Item item, List<NetDimension.Weibo.Entities.comment.Entity> result) 
        {
            int page = 1;
            string since_id = item.Tracking.LastReply_SinceID;
            bool finished = false;
            do
            {
                var commentlist = defaultClient.API.Entity.Comments.Show(item.ClientItemID, since_id, null, 50, page++);
                finished = true;
                foreach (var comment in commentlist.Comments) 
                {
                    result.Add(comment);
                    try
                    {
                        if (string.IsNullOrEmpty(item.Tracking.LastReply_SinceID) || Int64.Parse(comment.ID) > Int64.Parse(item.Tracking.LastReply_SinceID))
                        {
                            item.Tracking.LastReply_SinceID = comment.ID;
                            item.Tracking.LastReplyAuthorID = comment.User.ID;
                            item.Tracking.LastReplyAuthorName = comment.User.Name;
                            item.Tracking.LastReplyDate = Utilities.ParseToDateTime(comment.CreatedAt);
                        }
                    }
                    catch (Exception) { throw new WeiboException("ID数字格式错误"); }
                    finished = false;
                }
            } while (!finished);
        }
        
        /// <summary>
        /// 获取单条微博的转发列表
        /// </summary>
        /// <param name="item">待查询的微博</param>
        /// <param name="result">返回结果</param>
        public static void GetRepostOfStatus(Item item, List<NetDimension.Weibo.Entities.status.Entity> result) 
        {
            string since_id = item.Tracking_Forward.LastReply_SinceID;
            int page = 1;
            bool finished = false;
            do
            {
                RequireAccPermission(1);
                var weibolist = defaultClient.API.Entity.Statuses.RepostTimeline(item.ClientItemID, since_id, null, 50, page++);
                finished = true;
                foreach (var weibo in weibolist.Statuses)
                {
                    result.Add(weibo);
                    try
                    {
                        if (string.IsNullOrEmpty(item.Tracking_Forward.LastReply_SinceID) || Int64.Parse(weibo.ID) > Int64.Parse(item.Tracking_Forward.LastReply_SinceID))
                            item.Tracking_Forward.LastReply_SinceID = weibo.ID;
                    }
                    catch (Exception) { throw new WeiboException("ID数字格式错误"); }
                    finished = false;
                }
            } while (!finished);
        }

        /// <summary>
        /// 获取单条微博的转发评论数
        /// </summary>
        /// <param name="weiboID">微博ID</param>
        /// <returns>返回结果，item1为转发数，item2为评论数</returns>
        public static Tuple<int, int> GetRepostAndReplyCount(string weiboID) 
        {
            RequireAccPermission(1);
            var status = defaultClient.API.Entity.Statuses.Show(weiboID);
            NetDimension.Weibo.Entities.status.Count result = new NetDimension.Weibo.Entities.status.Count();
            return new Tuple<int, int>(status.RepostsCount, status.CommentsCount);
        }
    }
}

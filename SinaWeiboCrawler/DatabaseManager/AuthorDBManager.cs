using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Palas.Common.Data;
using System.Collections.Concurrent;
using MongoDB.Driver.Builders;
using SinaWeiboCrawler.Utility;
using HooLab.Log;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// Author类与Mongo数据库交互的类
    /// </summary>
    class AuthorDBManager : MongoDBManager
    {
        /// <summary>
        /// 更新数据库中用户信息
        /// </summary>
        /// <param name="author">待更新用户</param>
        private static void UpdateUserInfo(Author author) 
        {
            //待更新的字段
            string[] parameters = new string[10];
            parameters[0] = "Certification";
            parameters[1] = "CertificationInfo";
            parameters[2] = "RegionID";
            parameters[3] = "Description";
            parameters[4] = "Homepage";
            parameters[5] = "FansCount";
            parameters[6] = "FollowCount";
            parameters[7] = "CloseFriendsCount";
            parameters[8] = "PostCount";
            parameters[9] = "AuthorImg";
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        /// <summary>
        /// 插入一个新用户，如果该用户已经存在，那么更新其信息
        /// </summary>
        /// <param name="author">待操作的用户</param>
        public static void InsertOrUpdateAuthorInfo(Author author)
        {
            if (!WeiboUtilities.ShouldFetchAuthor(author)) return;
            var query = Query.EQ("AuthorID", author.AuthorID);
            if (Exists<Author>(query))
                UpdateUserInfo(author);
            else InsertOrReplace<Author>(author, "AuthorID", SafeMode.True);
        }

        /// <summary>
        /// 设置用户抓取信息
        /// </summary>
        /// <param name="author">用户</param>
        /// <param name="source">用户来源</param>
        private static void SetAuthorCrawlInfo(Author author, Enums.AuthorSource source) 
        {
            #region 抓取信息
            author.AuthorSource = source;
            author.CreateTime = DateTime.Now;

            #region 基本信息和微博刷新
            WeiboUtilities.SetAuthorPostSampleRule(author, source);
            author.UpdateTime = Utilities.Epoch;
            author.UpdateCount = 0;
            author.NextRefreshTime = Utilities.Epoch;
            #endregion

            #region 刷新粉丝和关注
            author.Fans_UpdateTime = Utilities.Epoch;
            author.Fans_UpdateCount = 0;
            author.Fans_NextRefreshTime = Utilities.Epoch;
            WeiboUtilities.SetAuthorFansAndFollowersSampleRule(author, source);
            #endregion

            #region 刷新地理列表
            author.Location_UpdateTime = Utilities.Epoch;
            author.Location_UpdateCount = 0;
            author.Location_NextRefreshTime = Utilities.Epoch;
            WeiboUtilities.SetAuthorLocationSampleRule(author, source);
            #endregion

            switch (author.AuthorSource) 
            {
                case Enums.AuthorSource.ListedTop:
                case Enums.AuthorSource.Partner:
                case Enums.AuthorSource.PublicLeader:
                    author.InternalSubscribeID = DefaultSettings.ToBeFollowed;
                    break;
                default:
                    author.InternalSubscribeID = null;
                    break;
            }

            #endregion
        }

        /// <summary>
        /// 将新浪返回的动态类转换为Author类
        /// </summary>
        /// <param name="user">动态类用户</param>
        /// <param name="source">用户来源</param>
        /// <returns>转换后的用户</returns>
        public static Author ConvertToAuthor(dynamic user, Enums.AuthorSource source) 
        {
            Author author = new Author();

            #region 注册信息
            try
            {
                author.AuthorID = user.id;
                author.AuthorName = user.screen_name;    //是昵称还是？
                author.RealName = user.name;
                author.Certification = Utilities.GetCertificationType(user.verified_type, user.verified);
                author.CertificationInfo = user.verified_reason;
                author.Gender = Utilities.GetGender(user.gender);
                author.RegisterTime = Utilities.ParseToDateTime(user.created_at);
                author.RegionID = RegionDBManager.GetRegionID(user.location);
                author.Description = user.description;
                author.AuthorImg = user.profile_image_url;
                author.Homepage = user.profile_url;
                if (!string.IsNullOrEmpty(author.Homepage) && !author.Homepage.Contains("http:"))
                {
                    if (author.Homepage.Contains('/'))
                        author.Homepage = "http://weibo.com/" + author.Homepage;
                    else author.Homepage = "http://blog.sina.com.cn/" + author.Homepage;
                }
            }
            catch (Exception) { }
            #endregion

            #region 行为数据
            try
            {
                author.FansCount = int.Parse(user.followers_count);
                author.FollowCount = int.Parse(user.friends_count);
                author.CloseFriendsCount = int.Parse(user.bi_followers_count);
                author.PostCount = int.Parse(user.statuses_count);
                author.AvgForward = author.AvgReply = author.AvgFansCountOfFans;
            }
            catch (Exception) { }
            #endregion

            SetAuthorCrawlInfo(author, source);

            return author;
        }

        /// <summary>
        /// 将新浪返回的用户类型转换为Author类型
        /// </summary>
        /// <param name="user">新浪返回的用户</param>
        /// <param name="source">用户来源</param>
        /// <returns>转换后的用户</returns>
        public static Author ConvertToAuthor(NetDimension.Weibo.Entities.user.Entity user, Enums.AuthorSource source) 
        {
            Author author = new Author();

            #region 注册信息
            author.AuthorID = user.ID;
            author.AuthorName = user.ScreenName;
            author.RealName = user.Name;
            author.Certification = Utilities.GetCertificationType(user.VerifiedType, user.Verified);
            author.CertificationInfo = user.VerifiedReason;
            author.Gender = Utilities.GetGender(user.Gender);
            author.RegionID = RegionDBManager.GetRegionID(user.Location);
            author.Description = user.Description;
            author.AuthorImg = user.ProfileImageUrl;
            author.Homepage = user.ProfileUrl;
            if (!string.IsNullOrEmpty(author.Homepage) && !author.Homepage.Contains("http:"))
            {
                if (author.Homepage.Contains('/'))
                    author.Homepage = "http://weibo.com/" + author.Homepage;
                else author.Homepage = "http://blog.sina.com.cn/" + author.Homepage;
            }
            author.RegisterTime = Utilities.ParseToDateTime(user.CreatedAt);
            #endregion

            #region 行为数据
            author.FansCount = user.FollowersCount;
            author.FollowCount = user.FriendsCount;
            author.CloseFriendsCount = user.BIFollowersCount;
            author.PostCount = user.StatusesCount;
            author.AvgForward = author.AvgReply = author.AvgFansCountOfFans = 0;
            #endregion

            SetAuthorCrawlInfo(author, source);

            return author;
        }

        /// <summary>
        /// 从数据库中通过用户ID获取一个用户
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <returns>对应用户，如果没有则返回null</returns>
        protected static Author GetAuthorByID(string AuthorID) 
        {
            var query = Query.EQ("AuthorID", AuthorID);
            return GetOneEntityByQuery<Author>(query);
        }

        /// <summary>
        /// 获取一个还没被关注的要关注的用户ID
        /// </summary>
        /// <returns></returns>
        public static string GetAuthorToBeFollowed() 
        {
            var query = Query.EQ("InternalSubscribeID", DefaultSettings.ToBeFollowed);
            var author = GetOneEntityByQuery<Author>(query);
            if (author != null)
                return author.AuthorID;
            else return null;
        }

        /// <summary>
        /// 获取当前关注某个用户的僵尸账号
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <returns>僵尸账号用户名</returns>
        public static string GetUsernameFollowing(string AuthorID)
        {
            var query = Query.EQ("AuthorID", AuthorID);
            var author = GetOneEntityByQuery<Author>(query);
            if (author != null)
                return author.InternalSubscribeID;
            else return null;
        }

        /// <summary>
        /// 取消关注某个用户，改变数据库中相关信息
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        public static void UnSubscribeAuthor(string AuthorID)
        {
            var author = GetAuthorByID(AuthorID);
            if (author == null) return;
            author.InternalSubscribeID = null;
            string[] parameters = new string[] { "InternalSubscribeID" };
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        /// <summary>
        /// 设置关注用户的标识以等待工人执行
        /// </summary>
        /// <param name="AuthorID"></param>
        public static void WaitToSubscribeAuthor(string AuthorID) 
        {
            var author = GetAuthorByID(AuthorID);
            if (author == null) return;
            author.InternalSubscribeID = DefaultSettings.ToBeFollowed;
            string[] parameters = new string[] { "InternalSubscribeID" };
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        /// <summary>
        /// 关注某个用户，改变数据库中相关信息
        /// </summary>
        /// <param name="username">用来关注的僵尸用户名</param>
        /// <param name="AuthorID">被关注的用户ID</param>
        public static void SubscribeAuthor(string username, string AuthorID) 
        {
            var author = GetAuthorByID(AuthorID);
            if (author == null) return;
            author.InternalSubscribeID = username;
            string[] parameters = new string[] { "InternalSubscribeID" };
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        /// <summary>
        /// 获取下一个普查的工作
        /// </summary>
        /// <returns>待普查的用户</returns>
        public static Author GetNextCensusJob() 
        {
            var query = Query.And(Query.LT("NextRefreshTime", DateTime.Now), Query.EQ("RefreshStatus", (sbyte)Enums.CrawlStatus.Normal));
            return GetOneEntityByQuery<Author>(query);
        }

        /// <summary>
        /// 普查工作结束后写回数据库
        /// </summary>
        /// <param name="author">被普查的用户</param>
        public static void PushbackCensusJob(Author author) 
        {
            string[] parameters = new string[7];
            parameters[0] = "UpdateTime";
            parameters[1] = "UpdateCount";
            parameters[2] = "NextRefreshTime";
            parameters[3] = "RefreshStatus";
            parameters[4] = "LastSinceID";
            parameters[5] = "AvgForward";
            parameters[6] = "AvgReply";
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        public static void InitCensusJob() 
        {
            var query = Query.NE("InternalSubscribeID", MongoDB.Bson.BsonNull.Value);
            var collection = GetCollections<Author>();
            var authors = collection.FindAs<Author>(query);
            int cnt = 0;
            foreach (var author in authors) 
            {
                Console.WriteLine("init {0}", author.AuthorName);
                if (WeiboUtilities.IsRedSkin(author.AuthorSource))
                {
                    cnt++;
                    author.RefreshStatus = Enums.CrawlStatus.Normal;
                    author.UpdateTime = Utilities.Epoch;
                    author.UpdateCount = 0;
                    author.NextRefreshTime = Utilities.Epoch;
                    author.LastSinceID = null;

                    string[] parameters = new string[5];
                    parameters[0] = "RefreshStatus";
                    parameters[1] = "UpdateTime";
                    parameters[2] = "UpdateCount";
                    parameters[3] = "NextRefreshTime";
                    parameters[4] = "LastSinceID";

                    UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
                }
            }
            Console.WriteLine(cnt);
        }

        /// <summary>
        /// 获取下一个地点历史跟踪任务
        /// </summary>
        /// <returns>被跟踪的用户</returns>
        public static Author GetNextLocHistJob() 
        {
            var query = Query.And(Query.LT("Location_NextRefreshTime", DateTime.Now), Query.EQ("Location_RefreshStatus", (sbyte)Enums.CrawlStatus.Normal));
            return GetOneEntityByQuery<Author>(query);
        }

        /// <summary>
        /// 写回一个被地点历史跟踪的用户
        /// </summary>
        /// <param name="author">被地点历史跟踪的用户</param>
        public static void PushbackLocHistJob(Author author) 
        {
            string[] parameters = new string[5];
            parameters[0] = "Location_LastSinceID";
            parameters[1] = "Location_NextRefreshTime";
            parameters[2] = "Location_RefreshStatus";
            parameters[3] = "Location_UpdateCount";
            parameters[4] = "Location_UpdateTime";
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }

        public static void InitLocHistJob() 
        {
            var query = Query.NE("InternalSubscribeID", MongoDB.Bson.BsonNull.Value);
            var collection = GetCollections<Author>();
            var authors = collection.FindAs<Author>(query);
            int cnt = 0;
            foreach (var author in authors)
            {
                Console.WriteLine("init {0}", author.AuthorName);
                if (WeiboUtilities.IsRedSkin(author.AuthorSource))
                {
                    cnt++;
                    author.Location_RefreshStatus = Enums.CrawlStatus.Normal;
                    author.Location_LastSinceID = null;
                    author.Location_NextRefreshTime = Utilities.Epoch;
                    author.Location_UpdateCount = 0;
                    author.Location_UpdateTime = Utilities.Epoch;

                    string[] parameters = new string[5];
                    parameters[0] = "Location_LastSinceID";
                    parameters[1] = "Location_NextRefreshTime";
                    parameters[2] = "Location_UpdateCount";
                    parameters[3] = "Location_UpdateTime";
                    parameters[4] = "Location_RefreshStatus";
                    UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
                }
            }
            Console.WriteLine(cnt);
        }

        /// <summary>
        /// 获取下一个关系普查任务
        /// </summary>
        /// <returns>待关系调查的用户</returns>
        public static Author GetNextRelationshipJob() 
        {
            var query = Query.And(Query.LT("Fans_NextRefreshTime", DateTime.Now), Query.EQ("Fans_RefreshStatus", (sbyte)Enums.CrawlStatus.Normal));
            return GetOneEntityByQuery<Author>(query);
        }

        /// <summary>
        /// 写回一个执行了关注和粉丝调查的用户
        /// </summary>
        /// <param name="author">被关系调查的用户</param>
        public static void PushbackRelationshipJob(Author author)
        {
            string[] parameters = new string[5];
            parameters[0] = "Fans_RefreshStatus";
            parameters[1] = "Fans_UpdateCount";
            parameters[2] = "Fans_UpdateTime";
            parameters[3] = "Fans_NextRefreshTime";
            parameters[4] = "AvgFansCountOfFans";
            UpdateDB<Author>(author, "AuthorID", parameters, SafeMode.True);
        }
    }
}

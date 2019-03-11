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
using System.Configuration;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// Item与数据库交互的类
    /// </summary>
    class ItemDBManager : MongoDBManager
    {
        /// <summary>
        /// 插入一条item，如果已经存在则更新之
        /// </summary>
        /// <param name="item"></param>
        public static void InsertOrUpdateItem(Item item) 
        {
            if (item.ItemID == null) return;
            Conductor.Core.MQCrawl.CrawlWorker.UploadItemToAnalyzer(new ItemToAnalyze(item, null, item.CrawlID));

            //var query = Query.EQ("ItemID", item.ItemID);
            //if (!Exists<Item>(query))
            //    InsertOrReplace<Item>(item, "ItemID", SafeMode.False);
            //else if (item.PoID != null)
            //{
            //    string[] parameters = new string[4];
            //    parameters[0] = "PoID";
            //    parameters[1] = "PoIDSource";
            //    parameters[2] = "Lon";
            //    parameters[3] = "Lat";
            //    UpdateDB<Item>(item, "ItemID", parameters, SafeMode.False);
            //}
        }

        /// <summary>
        /// 将新浪返回的动态微博类型转换为本地微博类型（Item）
        /// </summary>
        /// <param name="status">动态类型微博</param>
        /// <param name="source">微博来源</param>
        /// <returns></returns>
        public static Item ConvertToItem(Enums.AuthorSource source, string CrawlID, dynamic status, dynamic user = null, Author author = null) 
        {
            Item item = new Item();
            item.CrawlID = CrawlID;
            item.Crawler = ConfigurationManager.AppSettings["ServerLocation"] + CrawlID;

            //设置媒体信息
            WeiboUtilities.SetItemMediaInfo(item);

            try
            {
                #region 抓取任务数据

                item.FetchTime = DateTime.Now;
                item.UpdateTime = null;
                item.ContentDetailLevel = Enums.ContentDetailLevel.Weibo;

                #endregion 抓取任务数据

                #region 基础数据
                if (user == null)
                {
                    if (author != null)
                        item.Url = Utilities.GetItemUrl(author.AuthorID, status.mid);
                    else item.Url = Utilities.GetItemUrl(status.user.id, status.mid);
                }
                else item.Url = Utilities.GetItemUrl(user.id, status.mid);
                if (item.Url == null)
                    item.ItemID = null;
                else item.ItemID = Palas.Common.Utility.MD5Helper.getMd5Hash(item.Url);
                item.ClientItemID = status.id;
                item.CleanTitle = status.text;
                item.PubDate = Utilities.ParseToDateTime(status.created_at);
                item.Location = null;
                string checkinUrl = null;
                try
                {
                     checkinUrl = Utilities.GetCheckInUrl(status.text);
                }
                catch (Exception) { }

                LocationDBManager.SetPoIDAndCoordinate(item, status, checkinUrl);

                if (user == null)
                {
                    if (author != null)
                    {
                        item.AuthorName = author.AuthorName;
                        item.AuthorID = author.AuthorID;
                        item.AuthorCertificated = author.Certification;
                        item.Source = status.source;
                        item.AuthorImg = author.AuthorImg;
                    }
                    else
                    {
                        item.AuthorName = status.user.name;
                        item.AuthorID = status.user.id;
                        item.AuthorCertificated = Utilities.GetCertificationType(status.user.verified_type, status.user.verified);
                        item.Source = status.source;
                        item.AuthorImg = status.user.profile_image_url;
                    }
                }
                else
                {
                    item.AuthorName = user.name;
                    item.AuthorID = user.id;
                    item.AuthorCertificated = Utilities.GetCertificationType(user.verified_type, user.verified);
                    item.Source = status.source;
                    item.AuthorImg = user.profile_image_url;
                }
                try
                {
                    item.AttachImg = status.original_pic;
                }
                catch (Exception) { }
                #endregion

                #region Item跟踪
                item.CurrentCount = new ItemCountData(DateTime.Now);
                try
                {
                    item.CurrentCount.ForwardCount = int.Parse(status.reposts_count);
                    item.CurrentCount.ReplyCount = int.Parse(status.comments_count);
                }
                catch (Exception) { }
                item.CountHistory = new ItemCountData[1];
                item.CountHistory[0] = item.CurrentCount;
                WeiboUtilities.SetItemTrackingRule(item, source);
                #endregion Item跟踪
                try
                {
                    if (status.retweeted_status != null)
                    {
                        Item tmp = ConvertToItem(status.retweeted_status, source, CrawlID);
                        if (tmp.ItemID != null)
                            InsertOrUpdateItem(tmp);
                        item.ParentItemID = tmp.ItemID;
                    }
                }
                catch (Exception) { }
            }
            catch (Exception) {  }
            return item;
        }

        /// <summary>
        /// 将新浪返回的微博转换为本地微博类型（Item）
        /// </summary>
        /// <param name="status">新浪返回的微博</param>
        /// <param name="source">微博来源</param>
        /// <returns></returns>
        public static Item ConvertToItem(NetDimension.Weibo.Entities.status.Entity status, Enums.AuthorSource source, string CrawlID) 
        {
            Item item = new Item();
            item.CrawlID = CrawlID;
            item.Crawler = ConfigurationManager.AppSettings["ServerLocation"] + CrawlID;

            //设置媒体信息
            WeiboUtilities.SetItemMediaInfo(item);

            #region 抓取任务数据

            item.FetchTime = DateTime.Now;
            item.UpdateTime = null;
            item.ContentDetailLevel = Enums.ContentDetailLevel.Weibo;

            #endregion 抓取任务数据
            try
            {
                #region 基础数据
                item.Url = Utilities.GetItemUrl(status.User.ID, status.MID);
                item.ItemID = Palas.Common.Utility.MD5Helper.getMd5Hash(item.Url);
                item.ClientItemID = status.ID;
                item.CleanTitle = status.Text;
                item.PubDate = Utilities.ParseToDateTime(status.CreatedAt);
                item.Location = null;
                item.PoID = null;
                item.AuthorName = status.User.Name;
                item.AuthorID = status.User.ID;
                item.AuthorImg = status.User.ProfileImageUrl;
                item.AuthorCertificated = Utilities.GetCertificationType(status.User.VerifiedType, status.User.Verified);
                item.Source = status.Source;
                item.AttachImg = status.OriginalPictureUrl;
                #endregion

                #region Item跟踪

                item.CurrentCount = new ItemCountData(DateTime.Now);
                item.CurrentCount.ForwardCount = status.RepostsCount;
                item.CurrentCount.ReplyCount = status.CommentsCount;

                item.CountHistory = new ItemCountData[1];
                item.CountHistory[0] = item.CurrentCount;

                WeiboUtilities.SetItemTrackingRule(item, source);
                #endregion Item跟踪

                if (status.RetweetedStatus != null)
                {
                    Item tmp = ConvertToItem(status.RetweetedStatus, source, CrawlID);
                    if (tmp.ItemID != null)
                        InsertOrUpdateItem(tmp);
                    item.ParentItemID = tmp.ItemID;
                }
            }
            catch (Exception) { }
            return item;
        }

        /// <summary>
        /// 获取下一个待转发跟踪的微博
        /// </summary>
        /// <returns></returns>
        public static Item GetNextRepostTrackingJob() 
        {
            var query = Query.And(Query.EQ("Tracking_Forward.FollowStatus", (sbyte)Enums.CrawlStatus.Normal), Query.LT("Tracking_Forward.FollowNextTime", DateTime.Now));
            return GetOneEntityByQuery<Item>(query);
        }

        /// <summary>
        /// 转发跟踪完毕后写回数据库
        /// </summary>
        /// <param name="item"></param>
        public static void PushbackRepostTrackingJob(Item item) 
        {
            Tuple<string, BsonValueType>[] parameters = new Tuple<string, BsonValueType>[3];
            parameters[0] = new Tuple<string, BsonValueType>("Tracking_Forward", BsonValueType.Document);
            parameters[1] = new Tuple<string, BsonValueType>("CurrentCount", BsonValueType.Document);
            parameters[2] = new Tuple<string, BsonValueType>("CountHistory", BsonValueType.Array);
            UpdateDB<Item>(item, "ItemID", parameters, SafeMode.False);
        }

        /// <summary>
        /// 获取下一个待跟踪评论的微博
        /// </summary>
        /// <returns></returns>
        public static Item GetNextReplyTrackingJob() 
        {
            var query = Query.And(Query.EQ("Tracking.FollowStatus", (sbyte)Enums.CrawlStatus.Normal), Query.LT("Tracking.FollowNextTime", DateTime.Now));
            return GetOneEntityByQuery<Item>(query);
        }

        /// <summary>
        /// 评论跟踪完毕后写回数据库
        /// </summary>
        /// <param name="item"></param>
        public static void PushbackReplyTrackingJob(Item item) 
        {
            Tuple<string, BsonValueType>[] parameters = new Tuple<string, BsonValueType>[1];
            parameters[0] = new Tuple<string, BsonValueType>("Tracking", BsonValueType.Document);
            UpdateDB<Item>(item, "ItemID", parameters, SafeMode.False);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Palas.Common.Data;
using System.Collections.Concurrent;
using MongoDB.Driver.Builders;
using SinaWeiboCrawler.Utility;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// ItemReply与数据库交互的类
    /// </summary>
    class ItemReplyDBManager : MongoDBManager
    {
        /// <summary>
        /// 将新浪返回的评论类型转换为ItemReply类型
        /// </summary>
        /// <param name="comment">新浪返回的comment</param>
        /// <returns></returns>
        public static ItemReply ConvertToItemReply(NetDimension.Weibo.Entities.comment.Entity comment) 
        {
            ItemReply reply = new ItemReply();
            reply.ItemID = comment.Status.ID;
            reply.CleanText = comment.Text;
            reply.FetchTime = DateTime.Now;
            reply.PubDate = Utilities.ParseToDateTime(comment.CreatedAt);
            reply.AuthorName = comment.User.Name;
            reply.AuthorID = comment.User.ID;
            reply.AuthorImg = comment.User.AvatarLarge;
            reply.AuthorCertificated = Utilities.GetCertificationType(comment.User.VerifiedType, comment.User.Verified);
            reply.Location = comment.User.Location;
            reply.Source = comment.Source;
            return reply;
        }

        /// <summary>
        /// 插入一条评论，因为ItemReply没有主键所以就不判重了
        /// </summary>
        /// <param name="reply"></param>
        public static void InsertItemReply(ItemReply reply) 
        {
            Insert<ItemReply>(reply, SafeMode.False);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Palas.Common.Data;
using System.Collections.Concurrent;
using MongoDB.Driver.Builders;
using HooLab.Log;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// AuthorRelation与数据库交互的类
    /// </summary>
    class AuthorRelationDBManager : MongoDBManager
    {
        /// <summary>
        /// 插入或者更新一条关注关系
        /// </summary>
        /// <param name="FollowerID">关注关系出发者ID</param>
        /// <param name="TargetID">被关注的人ID</param>
        public static void InsertOrUpdateRelation(string FollowerID, string TargetID) 
        {
            AuthorRelation relation = new AuthorRelation();
            relation.FollowerID = FollowerID;
            relation.TargetID = TargetID;
            relation.CreateTime = DateTime.Now;
            relation.UpdateTime = DateTime.Now;
            var query = Query.And(Query.EQ("FollowerID", FollowerID), Query.EQ("TargetID", TargetID));
            if (Exists<AuthorRelation>(query))
                UpdateDB<AuthorRelation>(relation, new string[] { "FollowerID", "TargetID" }, new string[] { "UpdateTime" }, SafeMode.False);
            else InsertOrReplace<AuthorRelation>(relation, new string[] { "FollowerID", "TargetID" }, SafeMode.False);
        }
    }
}

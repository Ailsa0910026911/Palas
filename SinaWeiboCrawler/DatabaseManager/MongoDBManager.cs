using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MongoDB.Driver;
using Palas.Common.Data;
using System.Collections.Concurrent;
using MongoDB.Driver.Builders;
using SinaWeiboCrawler.Utility;
using HooLab.Log;

namespace SinaWeiboCrawler.DatabaseManager
{
    public enum BsonValueType 
    {
        Default = 0,
        Document = 1,
        Array = 2
    }
    /// <summary>
    /// 与MongoDB交互的类，包含用泛型和反射实现的插入删除更新方法
    /// </summary>
    class MongoDBManager
    {
        protected static string _connectStr = null;
        protected static readonly object connectStringLock = new object();
        ///<summary>
        /// 获取Mongo连接字符串
        ///</summary>
        protected static string ConnectString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectStr))
                {
                    lock (connectStringLock)
                    {
                        if (string.IsNullOrEmpty(_connectStr))
                            _connectStr = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
                    }
                }
                return _connectStr;
            }
        }

        ///<summary>
        /// 数据库名称
        ///</summary>
        protected const string DatabaseName = "Palas";

        protected static MongoDatabase _db = null;
        protected static readonly object DBLock = new object();
        ///<summary>
        /// 获取当前数据库
        ///</summary>
        protected static MongoDatabase DB
        {
            get
            {
                if (_db == null)
                {
                    lock (DBLock)
                    {
                        if (_db == null)
                            _db = Server.GetDatabase(DatabaseName);
                    }
                }
                return _db;
            }
        }

        protected static MongoServer _server = null;
        protected static readonly object serverLock = new object();
        ///<summary>
        /// 获取Mongo服务器
        ///</summary>
        protected static MongoServer Server
        {
            get
            {
                if (_server == null)
                {
                    lock (serverLock)
                    {
                        if (_server == null)
                            _server = MongoServer.Create(ConnectString);
                    }
                }
                return _server;
            }
        }

        private static ConcurrentDictionary<string, MongoCollection> _mongoCollections = new ConcurrentDictionary<string, MongoCollection>();
        /// <summary>
        /// 调试用，将Item和ItemReply存在新的集合中
        /// </summary>
        private static Dictionary<string, string> typenameMap = new Dictionary<string, string> { { "Item", "ItemNew" }, {"ItemReply", "ItemReplyNew"} };
        /// <summary>
        /// 获取类对应的集合名
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <returns></returns>
        protected static MongoCollection GetCollections<T>() 
        {
            string className = typeof(T).Name;
             //调试用，将Item和ItemReply存在新的集合中
            //if (typenameMap.ContainsKey(className))
            //    className = typenameMap[className]; 
             
            return _mongoCollections.GetOrAdd(className, DB.GetCollection<T>);
        }

        /// <summary>
        /// 根据查询语句删去特定条目
        /// </summary>
        /// <typeparam name="T">待删的集合类型</typeparam>
        /// <param name="query"></param>
        private static void Remove<T>(IMongoQuery query, SafeMode safemode) 
        {
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            collection.Remove(query, safemode);
        }

        /// <summary>
        /// 插入一个类到对应数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        protected static void Insert<T>(T item, SafeMode safemode) 
        {
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            collection.Insert(item, safemode);
        }

        /// <summary>
        /// 获取主键的询问
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="idName">主键名</param>
        /// <returns></returns>
        private static IMongoQuery GetIDQuery<T>(T item, string idName) 
        {
            return Query.EQ(idName, MongoDB.Bson.BsonValue.Create(typeof(T).GetProperty(idName).GetValue(item, null)));
        }

        /// <summary>
        /// 获取主键的询问
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="idNames">主键集合</param>
        /// <returns></returns>
        private static IMongoQuery GetIDQuery<T>(T item, string[] idNames) 
        {
            QueryComplete[] param = new QueryComplete[idNames.Length];
            for (int i = 0; i < idNames.Length; ++i)
                param[i] = Query.EQ(idNames[i], MongoDB.Bson.BsonValue.Create(typeof(T).GetProperty(idNames[i]).GetValue(item, null)));
            return Query.And(param);
        }

        /// <summary>
        /// 插入或者替换已有条目
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="idName">主键名</param>
        protected static void InsertOrReplace<T>(T item, string idName, SafeMode safemode) 
        {
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            var query = GetIDQuery<T>(item, idName);
            Remove<T>(query, safemode);
            collection.Insert(item, safemode);
        }

        /// <summary>
        /// 插入或者替换已有条目
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="idNames">主键集合</param>
        protected static void InsertOrReplace<T>(T item, string[] idNames, SafeMode safemode) 
        {
            if (idNames == null || idNames.Length == 0) return;
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            var query = GetIDQuery<T>(item, idNames);
            Remove<T>(query, safemode);
            collection.Insert(item, safemode);
        }

        /// <summary>
        /// 判断在数据库中是否存在该条目
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="query">查询语句</param>
        /// <returns></returns>
        protected static bool Exists<T>(IMongoQuery query) 
        {
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            T result = collection.FindOneAs<T>(query);
            if (result != null)
                return true;
            return false;
        }

        /// <summary>
        /// 根据查询语句和更新参数更新数据库
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">所需要更新的字段</param>
        private static void UpdateDB<T>(T item, IMongoQuery query, string[] parameters, SafeMode safemode) 
        {
            if (parameters == null || parameters.Length == 0) return;
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            UpdateBuilder update = new UpdateBuilder();
            for (int i = 0; i < parameters.Length; ++i)
            {
                var obj = typeof(T).GetProperty(parameters[i]).GetValue(item, null);
                if (obj != null)
                    update = update.Set(parameters[i], MongoDB.Bson.BsonValue.Create(obj));
                else update = update.Set(parameters[i], MongoDB.Bson.BsonNull.Value);
            }
            collection.Update(query, update, safemode);
        }

        /// <summary>
        /// 根据查询语句和更新参数更新数据库
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="item">对象</param>
        /// <param name="query">查询语句</param>
        /// <param name="parameters">所需要更新的字段以及字段对应类型</param>
        private static void UpdateDB<T>(T item, IMongoQuery query, Tuple<string, BsonValueType>[] parameters, SafeMode safemode)
        {
            if (parameters == null || parameters.Length == 0) return;
            string className = typeof(T).Name;
            var collection = GetCollections<T>();
            UpdateBuilder update = new UpdateBuilder();
            for (int i = 0; i < parameters.Length; ++i)
            {
                var obj = typeof(T).GetProperty(parameters[i].Item1).GetValue(item, null);
                if (obj != null)
                {
                    switch (parameters[i].Item2)
                    {
                        case BsonValueType.Document: update = update.Set(parameters[i].Item1, Utilities.ToMongoDocument(obj)); break;
                        case BsonValueType.Array: update = update.Set(parameters[i].Item1, Utilities.ToMongoArray(obj)); break;
                        default: update = update.Set(parameters[i].Item1, MongoDB.Bson.BsonValue.Create(obj)); break;
                    }
                }
                else update = update.Set(parameters[i].Item1, MongoDB.Bson.BsonNull.Value);
            }
            collection.Update(query, update, safemode);
        }

        /// <summary>
        /// 根据主键和更新参数更新数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="idName"></param>
        /// <param name="parameters"></param>
        public static void UpdateDB<T>(T item, string idName, Tuple<string, BsonValueType>[] parameters, SafeMode safemode) 
        {
            var query = GetIDQuery<T>(item, idName);
            UpdateDB<T>(item, query, parameters, safemode);
        }

        /// <summary>
        /// 根据主键和更新参数更新数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="idName"></param>
        /// <param name="parameters"></param>
        public static void UpdateDB<T>(T item, string idName, string[] parameters, SafeMode safemode) 
        {
            var query = GetIDQuery<T>(item, idName);
            UpdateDB<T>(item, query, parameters, safemode);
        }

        /// <summary>
        /// 根据主键和更新参数更新数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="idNames"></param>
        /// <param name="parameters"></param>
        public static void UpdateDB<T>(T item, string[] idNames, string[] parameters, SafeMode safemode) 
        {
            var query = GetIDQuery<T>(item, idNames);
            UpdateDB<T>(item, query, parameters, safemode);
        }

        /// <summary>
        /// 根据查询语句获得单个实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">查询语句</param>
        /// <returns></returns>
        public static T GetOneEntityByQuery<T>(IMongoQuery query) 
        {
            var collection = GetCollections<T>();
            return collection.FindOneAs<T>(query);
        }
    }
}

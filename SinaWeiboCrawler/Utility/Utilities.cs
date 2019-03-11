using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetDimension.Weibo;
using Palas.Common.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using SinaWeiboCrawler.DatabaseManager;
using HooLab.Log;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 一些全局默认设置
    /// </summary>
    class DefaultSettings 
    {
        public static string AppKey = "1371841863";
        public static string AppSecret = "48268c18e397b5fbdb286d78c5274af2";
        public static string defaultZombieName = "antv.isionantsphere@gmail.com";         //默认僵尸账号，用以处理不需要特定账号的任务
        public static string defaultZombiePwd = "antsphere";

        public static TimeSpan IPStatusUpdateInterval = new TimeSpan(0, 1, 0);            //每隔多长时间刷新一下当前请求次数限制

        public static int CelebrityPageCnt = 40;                                          //名人榜一共有的页数

        public static int MaxFriendsCnt = 2000;                                           //每个僵尸最多订阅用户数量
        public static int FollowLimitPerDay = 230;                                        //每个僵尸每天至多订阅用户的数量

        public static TimeSpan specialStartTime = new TimeSpan(2, 0, 0);
        public static TimeSpan specialEndTime = new TimeSpan(6, 0, 0);                    //某些工作只能在特定时段进行
        public static TimeSpan RepostTrackingInterval = new TimeSpan(0, 15, 0);           //单条微博转发跟踪间隔
        public static TimeSpan ReplyTrackingInterval = new TimeSpan(0, 15, 0);            //单条微博评论跟踪间隔

        public const string ToBeFollowed = "ToBeFollowed";

        public static int MinReply = 20;
        public static int MinForward = 50;

        public static int EndReplyCount = 2;
        public static int EndForwardCount = 2;

        //账号最多失效的次数，超过此次数账号就fail
        public static int MaxFailureCount = 3;

        //以下标准设定了筛掉僵尸账号的阈值
        public static int MinUserPostCnt = 5;
        public static int MinUserFollowingCnt = 20;
        public static int MinUserFansCnt = 20;
    }

    /// <summary>
    /// 工具类（内部调用）
    /// </summary>
    class Utilities
    {
        /// <summary>
        /// Unix时间的0时刻，使用UTC时间。
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 将东八区当地时间转换为Unix时间
        /// </summary>
        /// <param name="datetime">东八区时间</param>
        /// <returns>Unix时间</returns>
        public static Int64 DateTime2UnixTime(DateTime datetime) 
        {
            TimeSpan span = (datetime - Epoch).Subtract(new TimeSpan(8, 0, 0));
            return (Int64)span.TotalSeconds;
        }

        private static Dictionary<string, int> eng2num = new Dictionary<string, int> { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
        private static Regex reg = new Regex("(\\w+) (\\w+) (.+) (.+):(.+):(.+) (.+) (.+)");
        /// <summary>
        /// 将新浪时间字符串转换为标准DateTime
        /// </summary>
        /// <param name="datetime">新浪时间</param>
        /// <returns>标准DateTime</returns>
        public static DateTime ParseToDateTime(string datetime) 
        {
            try
            {
                Match match = reg.Match(datetime);
                DateTime result = new DateTime(int.Parse(match.Groups[8].Value), eng2num[match.Groups[2].Value], int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value));
                return result;
            }
            catch (Exception) 
            {
                return Epoch;
            }
        }

        /// <summary>
        /// 获取用户验证类型
        /// </summary>
        /// <param name="VerifiedType">新浪验证类型字符串</param>
        /// <returns>验证类型</returns>
        public static Enums.CertificatedType GetCertificationType(string VerifiedType, bool isVerified) 
        {
            if (!isVerified) return Enums.CertificatedType.NoneCertificated;
            if (VerifiedType == "0")
                return Enums.CertificatedType.Personal;
            else return Enums.CertificatedType.Organization;
        }

        /// <summary>
        /// 获取用户性别
        /// </summary>
        /// <param name="gender">性别字符串</param>
        /// <returns>性别</returns>
        public static Enums.Gender GetGender(string gender) 
        {
            if (gender == "m") return Enums.Gender.Male;
            if (gender == "f") return Enums.Gender.Female;
            if (gender == "n") return Enums.Gender.Unkown;
            return Enums.Gender.Other;
        }

        /// <summary>
        /// 获取采样方法对应的采样数量
        /// </summary>
        /// <param name="sampleMethod">采样方法</param>
        /// <returns>目标获取数量</returns>
        public static int GetAimCount(Enums.SampleMethod sampleMethod) 
        {
            switch (sampleMethod) 
            {
                case Enums.SampleMethod.All: return Int32.MaxValue;
                case Enums.SampleMethod.First100: return 100;
                case Enums.SampleMethod.First5000: return 5000;
                default: return 0;
            }
        }

        private static string[] str62keys = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
	                                         "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", 
	                                         "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", 
	                                         "u", "v", "w", "x", "y", "z",
	                                         "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", 
	                                         "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", 
	                                         "U", "V", "W", "X", "Y", "Z"};
        private static string int10to62(Int64 int10) 
        {
            string s62 = "";
            Int64 r = 0;
            while (int10 != 0) 
            {
                r = int10 % 62;
                s62 = str62keys[r] + s62;
                int10 /= 62;
            }
            return s62;
        }
        /// <summary>
        /// 通过用户ID和微博MID转换成微博url
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <param name="MID">微博MID</param>
        /// <returns>微博url</returns>
        public static string GetItemUrl(string AuthorID, string MID)
        {
            try
            {
                string result = "";
                for (int i = MID.Length - 7; i > -7; i -= 7)
                {
                    int offset1 = i < 0 ? 0 : i;
                    int offset2 = i + 7;
                    Int64 num = Int64.Parse(MID.Substring(offset1, offset2 - offset1));
                    string tmp = int10to62(num);
                    if (offset2 - offset1 == 7) 
                    {
                        while (tmp.Length < 4)
                            tmp = '0' + tmp;
                    }
                    result = tmp + result;
                }
                result = @"http://weibo.com/" + AuthorID + "/" + result;
                return result;
            }
            catch (Exception ex) { Logger.Error(ex.ToString()); return null; }
        }

        private static Regex checkinurlreg = new Regex("(http://t.cn/\\w+)");
        public static string GetCheckInUrl(string text) 
        {
            string url = null;
            MatchCollection matches = checkinurlreg.Matches(text);
            foreach (Match match in matches) 
            {
                url = match.Groups[1].Value;
            }
            if (url != null)
                return WebRequestProcessor.GetRedirectedUrl(url);
            else 
            {
                Logger.Info("正则未命中的签到信息：" + text);
                return null;
            }
        }

        private static Regex wpinfoUrlReg = new Regex("lng=([^\\s]+)&lat=([^\\s]+)");
        public static Tuple<float, float> GetCoordinateViaWPInfo(dynamic wpinfo) 
        {
            try
            {
                string url = wpinfo.url;
                Match match = wpinfoUrlReg.Match(url);
                if (match.Success && match.Groups[1].Success && match.Groups[2].Success)
                    return new Tuple<float, float>(float.Parse(match.Groups[1].Value), float.Parse(match.Groups[2].Value));
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Tuple<float, float> GetCoordinateViaGEO(dynamic geo) 
        {
            try
            {
                float tmp = -1, lat = -1, lon = -1;
                foreach (var v in geo.coordinates) 
                {
                    try
                    {
                        tmp = float.Parse(v);
                    }
                    catch (Exception) 
                    {
                        try
                        {
                            tmp = (float)v;
                        }
                        catch (Exception) { }
                    }
                    if (lat < 0) lat = tmp; else lon = tmp;
                }
                if (lon < 0 || lat < 0) return null;
                return new Tuple<float, float>(lon, lat);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Regex coorUrlReg = new Regex("//place.weibo.com/xy/([-\\d\\.]+),([-\\d\\.]+)");
        public static Tuple<float, float> GetCoordinateViaUrl(string url) 
        {
            try
            {
                Match match = coorUrlReg.Match(url);
                if (match.Success && match.Groups[1].Success && match.Groups[2].Success)
                    return new Tuple<float, float>(float.Parse(match.Groups[1].Value), float.Parse(match.Groups[2].Value));
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 将类转换为BsonDocument以放入MongoDB，类中每个字段都可转换为BsonValue
        /// </summary>
        /// <param name="obj">待转换的对象</param>
        /// <returns>转换后的BsonDocument对象</returns>
        public static MongoDB.Bson.BsonDocument ToMongoDocument(object obj)
        {
            MongoDB.Bson.BsonDocument document = new MongoDB.Bson.BsonDocument();

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                object propertyValue = property.GetValue(obj, null);
                if (propertyValue == null)
                    document.Add(property.Name, MongoDB.Bson.BsonNull.Value);
                else
                    document.Add(property.Name, MongoDB.Bson.BsonValue.Create(propertyValue));
            }
            return document;
        }

        /// <summary>
        /// 将数组转换为BsonArray以放入MongoDB，数组中每个元素都可转换为BsonDocument
        /// </summary>
        /// <param name="obj">待转换的数组对象</param>
        /// <returns>转换后的BsonArray对象</returns>
        public static MongoDB.Bson.BsonArray ToMongoArray(object obj) 
        {
            object[] objs = (object[])obj;
            MongoDB.Bson.BsonArray array = new MongoDB.Bson.BsonArray();
            for (int i = 0; i < objs.Length; ++i)
                array.Add(ToMongoDocument(objs[i]));
            return array;
        }
    }
}

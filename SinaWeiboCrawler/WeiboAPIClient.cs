using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Data;
using SinaWeiboCrawler.DatabaseManager;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using HooLab.Log;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 外部用户调用的类
    /// </summary>
    public class WeiboAPIClient
    {
        /// <summary>
        /// 添加一堆存放在文本文件中的CBD，每行一个CBD，从左往右分别为名字、纬度、经度
        /// </summary>
        /// <param name="filename"></param>
        public static void AddNewCBDs(string filename) 
        {
            string[] cbds = File.ReadAllLines(filename, Encoding.Default);
            for (int i = 0; i < cbds.Length; ++i) 
            {
                Location loc = new Location();

                string[] info = cbds[i].Split();
                loc.Title = info[0];
                loc.Lat = float.Parse(info[1]);
                loc.Lon = float.Parse(info[2]);

                loc.CategoryID = "CBD";
                loc.CategoryName = "CBD";
                loc.IntervalMins = 15;
                loc.LocationSampleMethode = Enums.SampleMethod.All;
                loc.PoID = Guid.NewGuid().ToString("N");
                loc.Radius = 600;
                loc.RegionID = RegionDBManager.GetRegionID(info[3] + " " + info[4]);
                loc.RefreshStatus = Enums.CrawlStatus.Normal;
                if (loc.RegionID == null)
                    loc.RegionID = RegionDBManager.GetRegionID(info[3]);
                AddNewCBD(loc);
            }
        }

        /// <summary>
        /// 添加一个CBD
        /// </summary>
        /// <param name="loc"></param>
        public static void AddNewCBD(Location loc)
        {
            loc.LastRefreshTime = Utilities.Epoch;
            loc.NextRefreshTime = Utilities.Epoch;
            loc.RefreshCount = 0;
            LocationDBManager.AddNewLocation(loc);
        }

        /// <summary>
        /// 手工添加一个用户到库内（使用用户ID）
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <param name="source">用户来源</param>
        public static void AddNewAuthorWithID(string AuthorID, Enums.AuthorSource source) 
        {
            var user = WeiboAPI.GetAuthorInfo(AuthorID);
            var author = AuthorDBManager.ConvertToAuthor(user, source);
            AuthorDBManager.InsertOrUpdateAuthorInfo(author);
        }

        private static Regex userIDReg = new Regex("href=\"/(.+)/fans?");
        /// <summary>
        /// 手工添加一个用户到库内（使用用户微博主页地址）
        /// </summary>
        /// <param name="url">用户微博主页</param>
        /// <param name="source">用户来源</param>
        public static void AddNewAuthorWithUrl(string url, Enums.AuthorSource source) 
        {
            string html = HTMLJobProcessor.GetHTMLViaRequest(url);
            Match match = userIDReg.Match(html);
            if (match.Groups[1].Success)
                AddNewAuthorWithID(match.Groups[1].Value, source);
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="AuthorID">用户ID</param>
        /// <returns></returns>
        public static void FollowAuthor(string AuthorID) 
        {
            AuthorDBManager.WaitToSubscribeAuthor(AuthorID);
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="AuthorID"></param>
        public static void UnFollowAuthor(string AuthorID) 
        {
            AccountDBManager.UnFollowUser(AuthorID);
        }

        private static Regex idreg = new Regex("<img src=\"http://tp\\d\\.sinaimg\\.cn/(\\d+).+\"  class=\"photo_zw\"/>");
        private static string urlPattern = @"http://data.weibo.com/top/hot/famous?page={0}class=0001";
        /// <summary>
        /// 刷新新浪微博名人排行榜，并更新数据库，即A1
        /// </summary>
        /// <returns>是否刷新成功</returns>
        public static bool UpdateCelebrityList()
        {
            try
            {
                int cnt = 0;
                for (int i = 1; i <= DefaultSettings.CelebrityPageCnt; ++i)
                {
                    string url = string.Format(urlPattern, i);
                    string html = HTMLJobProcessor.GetHTMLViaRequest(url);
                    Console.WriteLine("one web page fetched");
                    MatchCollection matches = idreg.Matches(html);
                    foreach (Match match in matches)
                    {
                        string AuthorID = match.Groups[1].Value;
                        var user = WeiboAPI.GetAuthorInfo(AuthorID);
                        Console.WriteLine("got one user");
                        var author = AuthorDBManager.ConvertToAuthor(user, Palas.Common.Data.Enums.AuthorSource.ListedTop);
                        Console.WriteLine("converted to Author class");
                        Console.WriteLine("inserting...");
                        AuthorDBManager.InsertOrUpdateAuthorInfo(author);
                        Console.WriteLine("insert successfully");
                        Console.WriteLine(++cnt);
                    }
                }
                return true;
            }
            catch (Exception ex) 
            {
                Logger.Error(ex.ToString());
                return false; 
            }
        }
    }
}

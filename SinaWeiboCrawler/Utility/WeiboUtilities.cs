using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Data;
using SinaWeiboCrawler.DatabaseManager;
using HooLab.Log;
using System.IO;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 与微博逻辑相关的工具类，暂时只有默认设置，需要进一步设置
    /// </summary>
    class WeiboUtilities
    {
        /// <summary>
        /// 签到URL来源是否在白名单中
        /// </summary>
        /// <param name="poiSource"></param>
        /// <returns></returns>
        public static bool IsPOISourceInWhiteList(string poiSource) 
        {
            if (poiSource == null) return false;
            return whitelisthash.Contains(poiSource);
        }

        private static HashSet<string> whitelisthash = new HashSet<string>();
        public static void InitPOISourceWhiteList() 
        {
            string[] s = File.ReadAllLines("poisourcewhitelist.txt", Encoding.Default);
            whitelisthash = new HashSet<string>();
            foreach (var ss in s)
                if (!whitelisthash.Contains(ss))
                    whitelisthash.Add(ss);
        }
        /// <summary>
        /// 根据新浪返回的城市代码获取Region表中的ID
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public static string GetPOIRegionID(string cityCode) 
        {
            if (cityregionCodeDict.ContainsKey(cityCode))
                return cityregionCodeDict[cityCode];
            return null;
        }

        private static Dictionary<string, string> cityregionCodeDict = new Dictionary<string, string>();
        /// <summary>
        /// 从文件初始化新浪城市代码和Region表ID对应表
        /// </summary>
        public static void InitSinaCityTable() 
        {
            string[] s = File.ReadAllLines("citycode.txt", Encoding.Default);
            cityregionCodeDict = new Dictionary<string, string>();
            foreach (var ss in s) 
            {
                string[] st = ss.Split(new char[] { ':', ' ' });
                cityregionCodeDict.Add(st[1], st[2]);
            }
        }

        public static string GetPoIDSource(string url) 
        {
            if (url == null) return null;
            try
            {
                Uri uri = new Uri(url);
                return uri.Host;
            }
            catch (Exception) { return "unknown"; }
        }

        /// <summary>
        /// 判断该用户是否需要入库，用于筛除僵尸
        /// </summary>
        /// <param name="author">待查用户</param>
        /// <returns></returns>
        public static bool ShouldFetchAuthor(Author author) 
        {
            if (author.Certification != Enums.CertificatedType.NoneCertificated) return true;
            if (author.PostCount < DefaultSettings.MinUserPostCnt) return false;
            if (author.FansCount < DefaultSettings.MinUserFansCnt) return false;
            if (author.FollowCount < DefaultSettings.MinUserFollowingCnt) return false;
            return true;
        }

        /// <summary>
        /// 设置用户普查的规则，默认为每7天普查前100条微博
        /// </summary>
        /// <param name="author">用户</param>
        /// <param name="source">用户来源</param>
        public static void SetAuthorPostSampleRule(Author author, Enums.AuthorSource source) 
        {
            switch (source)
            {
                case Enums.AuthorSource.ListedTop:
                case Enums.AuthorSource.Partner:
                case Enums.AuthorSource.PublicLeader:
                    author.RefreshStatus = Enums.CrawlStatus.Normal;
                    break;
                default:
                    author.RefreshStatus = Enums.CrawlStatus.Stop;
                    break;
            }
            switch (source){
                case Enums.AuthorSource.ListedTop: author.PostSampleMethode = Enums.SampleMethod.All; break;
                case Enums.AuthorSource.PublicLeader: author.PostSampleMethode = Enums.SampleMethod.All; break;
                default:
                    author.PostSampleMethode = Enums.SampleMethod.First100;
                    break;
            }
            author.IntervalDays = 7;
        }

        /// <summary>
        /// 设置用户粉丝和关注的跟踪规则，默认为每7天抓取用户前5000个粉丝和关注
        /// </summary>
        /// <param name="author">用户</param>
        /// <param name="source">用户来源</param>
        public static void SetAuthorFansAndFollowersSampleRule(Author author, Enums.AuthorSource source) 
        {
            switch (source) 
            {
                case Enums.AuthorSource.ListedTop:
                case Enums.AuthorSource.Partner:
                case Enums.AuthorSource.PublicLeader:
                    author.Fans_RefreshStatus = Enums.CrawlStatus.Normal;
                    break;
                default:
                    author.Fans_RefreshStatus = Enums.CrawlStatus.Stop;
                    break;
            }
            author.FollowerSampleMethode = Enums.SampleMethod.First5000;
            author.FansSampleMethode = Enums.SampleMethod.First5000;
            author.Fans_IntervalDays = 7;
        }

        /// <summary>
        /// 设置用户活动地点跟踪规则，默认为每7天抓取用户最新所有微博
        /// </summary>
        /// <param name="author">用户</param>
        /// <param name="source">用户来源</param>
        public static void SetAuthorLocationSampleRule(Author author, Enums.AuthorSource source) 
        {
            switch (source) 
            {
                case Enums.AuthorSource.ListedTop:
                case Enums.AuthorSource.Partner:
                case Enums.AuthorSource.PublicLeader:
                    author.Location_RefreshStatus = Enums.CrawlStatus.Normal;
                    break;
                default:
                    author.Location_RefreshStatus = Enums.CrawlStatus.Stop;
                    break;
            }
            author.LocationSampleMethode = Enums.SampleMethod.All;
            author.Location_IntervalDays = 7;
        }

        /// <summary>
        /// 设置单条微博的转发和跟踪规则，默认为每15分钟刷一次转发和评论
        /// </summary>
        /// <param name="item">微博</param>
        /// <param name="source">微博来源</param>
        public static void SetItemTrackingRule(Item item, Enums.AuthorSource source)
        {
            item.Tracking = null;

            item.Tracking_Forward = new ItemTracking();
            item.Tracking_Forward.FollowNextTime = Utilities.Epoch;
            if (ItemCountData.ShouldFollow(item.CurrentCount, -1, DefaultSettings.MinReply, DefaultSettings.MinForward))
                item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Normal;
            else item.Tracking_Forward.FollowStatus = Enums.CrawlStatus.Stop;
            item.Tracking_Forward.FollowPriority = 0;
        }

        public static void SetItemMediaInfo(Item item) 
        {
            item.MediaID = "Weibo";
            item.MediaName = "新浪微博";
            item.MediaChannel = null;
            item.Rank = 0;
            item.MediaType = Enums.MediaType.Weibo;
            item.MediaRegionType = Enums.MediaRegionType.National;
            item.MediaWeight = 9;
            item.MediaOrganType = Enums.MediaOrganType.TradePaper;
            item.MediaStyle = Enums.MediaStyle.Opinion;
            item.MediaTendency = 0;
            item.MediaIndustryIDs = null;
            item.ProxyZone = 0;
            item.ReproducedMediaID = null;
            item.ReproducedMediaName = null;
            item.ReproducedUrl = null;
        }

        /// <summary>
        /// 判断单条微博是否需要继续跟踪
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ShouldKeepFollow(Item item) 
        {
            if (item.CountHistory == null || item.CountHistory.Length == 0) return true;
            ItemCountData LastCount = null;
            if (item.CountHistory.Length > 1)
                LastCount = item.CountHistory[item.CountHistory.Length - 2];
            return item.CountHistory[item.CountHistory.Length - 1].ShouldKeepFollow(LastCount, 0, DefaultSettings.EndReplyCount, DefaultSettings.EndForwardCount);
        }

        /// <summary>
        /// 判断该用户来源是否属于红人
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsRedSkin(Enums.AuthorSource source) 
        {
            switch (source) 
            {
                case Enums.AuthorSource.ListedTop:
                case Enums.AuthorSource.Partner:
                case Enums.AuthorSource.PublicLeader:
                    return true;
                default:
                    return false;
            }
        }
    }
}

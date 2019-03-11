using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Palas.Common.Data;
using System.Collections.Concurrent;
using MongoDB.Driver.Builders;
using HooLab.Log;
using SinaWeiboCrawler.Utility;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// Location与数据库交互的类
    /// </summary>
    class LocationDBManager : MongoDBManager
    {
        /// <summary>
        /// 添加一个新的地点
        /// </summary>
        /// <param name="loc"></param>
        public static void AddNewLocation(Location loc) 
        {
            InsertOrReplace<Location>(loc, "PoID", SafeMode.True);
        }

        public static Tuple<string, float, float> AddNewLocation(dynamic place, string url) 
        {
            Location loc = new Location();
            loc.PoIDSource = WeiboUtilities.GetPoIDSource(url);
            loc.Url = url;
            loc.IntervalMins = 15;
            loc.LocationSampleMethode = Enums.SampleMethod.All;
            loc.PoID = Guid.NewGuid().ToString("N");
            loc.Radius = 600;
            loc.RefreshStatus = Enums.CrawlStatus.Stop;
            try
            {
                loc.Lat = float.Parse(place.lat);
                loc.Lon = float.Parse(place.lon);
            }
            catch (Exception)
            {
                try
                {
                    loc.Lat = (float)place.lat;
                    loc.Lon = (float)place.lon;
                }
                catch (Exception) { }
            }
            loc.Title = place.title;
            loc.ClientID = place.poiid;
            try
            {
                if (loc.ClientID != null)
                    WeiboAPI.SetPOIInfo(loc, loc.ClientID);
            }
            catch (Exception) { }
            LocationDBManager.AddNewLocation(loc);
            return new Tuple<string, float, float>(loc.PoID, loc.Lon, loc.Lat);
        }

        public static Tuple<string, float, float> AddNewLocation(Tuple<float, float> coordinate, string url) 
        {
            Location loc = new Location();
            loc.PoIDSource = "_" + WeiboUtilities.GetPoIDSource(url);
            loc.CheckInCount = 0;
            loc.Url = url;
            loc.IntervalMins = 15;
            loc.LocationSampleMethode = Enums.SampleMethod.All;
            loc.PoID = Guid.NewGuid().ToString("N");
            loc.Radius = 600;
            loc.RefreshStatus = Enums.CrawlStatus.Stop;
            if (coordinate != null) 
            {
                loc.Lon = coordinate.Item1;
                loc.Lat = coordinate.Item2;
            }
            loc.CategoryID = "unknown";
            LocationDBManager.AddNewLocation(loc);
            return new Tuple<string, float, float>(loc.PoID, loc.Lon, loc.Lat);
        }

        public static string AddNewLocation(Tuple<float, float> coordinate) 
        {
            Location loc = new Location();
            loc.PoIDSource = WeiboUtilities.GetPoIDSource(null);
            loc.CheckInCount = 0;
            loc.Url = null;
            loc.IntervalMins = 15;
            loc.LocationSampleMethode = Enums.SampleMethod.All;
            loc.PoID = Guid.NewGuid().ToString("N");
            loc.Radius = 600;
            loc.RefreshStatus = Enums.CrawlStatus.Stop;
            loc.Lon = coordinate.Item1;
            loc.Lat = coordinate.Item2;
            loc.CategoryID = "unknown";
            LocationDBManager.AddNewLocation(loc);
            return loc.PoID;
        }

        public static Tuple<string,float,float> GetPoIDAndCoordinateViaClientID(string clientID) 
        {
            var query = Query.EQ("ClientID", clientID);
            var loc = GetOneEntityByQuery<Location>(query);
            if (loc != null)
                return new Tuple<string, float, float>(loc.PoID, loc.Lon, loc.Lat);
            return null;
        }

        public static Tuple<string, float, float> GetPoIDAndCoordinateViaUrl(string url) 
        {
            var query = Query.EQ("Url", url);
            var loc = GetOneEntityByQuery<Location>(query);
            if (loc != null)
                return new Tuple<string, float, float>(loc.PoID, loc.Lon, loc.Lat);
            return null;
        }

        public static string GetPoIDViaCoordinate(float longitude, float latitude) 
        {
            var query = Query.And(Query.EQ("Lat", latitude), Query.EQ("Lon", longitude));
            var loc = GetOneEntityByQuery<Location>(query);
            if (loc != null)
                return loc.PoID;
            return null;
        }

        public static void SetPoIDAndCoordinate(Item item, dynamic status, string checkinUrl) 
        {
            item.PoIDSource = WeiboUtilities.GetPoIDSource(checkinUrl);
            if (!WeiboUtilities.IsPOISourceInWhiteList(item.PoIDSource)) 
            {
                item.PoID = item.PoIDSource = null;
                return;
            }
            //情形一，有POID
            try
            {
                foreach (var anno in status.annotations)
                {
                    Tuple<string,float,float> tupe = LocationDBManager.GetPoIDAndCoordinateViaClientID(anno.place.poiid);
                    if (tupe == null)
                        tupe = AddNewLocation(anno.place, checkinUrl);
                    if (tupe != null)
                    {
                        item.PoID = tupe.Item1;
                        item.Lon = tupe.Item2;
                        item.Lat = tupe.Item3;
                        return;
                    }
                }
            }
            catch (Exception) { }

            #region 尝试获取坐标
            Tuple<float, float> coordinate = null;
            try
            {
                foreach (var anno in status.annotations)
                {
                    //从wpinfo获取坐标
                    coordinate = Utilities.GetCoordinateViaWPInfo(anno.wpinfo);
                }
            }
            catch (Exception) { }
            if (coordinate == null) 
            {
                try
                {
                    //从GEO获取坐标
                    coordinate = Utilities.GetCoordinateViaGEO(status.geo);
                }
                catch (Exception) { }
            }
            if (coordinate == null) 
            {
                try
                {
                    //尝试解析签到链接获取url
                    coordinate = Utilities.GetCoordinateViaUrl(checkinUrl);
                }
                catch (Exception) { }
            }
            #endregion

            if (coordinate != null) 
            {
                item.Lon = coordinate.Item1;
                item.Lat = coordinate.Item2;
            }

            //有checkinUrl
            if (checkinUrl != null)
            {
                var tupe = GetPoIDAndCoordinateViaUrl(checkinUrl);
                if (tupe == null)
                    tupe = AddNewLocation(coordinate, checkinUrl);
                if (tupe != null) 
                {
                    item.PoID = tupe.Item1;
                    item.Lon = tupe.Item2;
                    item.Lat = tupe.Item3;
                }
                return;
            }

            //只有坐标
            if (coordinate != null) 
            {
                item.PoID = GetPoIDViaCoordinate(coordinate.Item1, coordinate.Item2);
                if (item.PoID == null)
                    item.PoID = AddNewLocation(coordinate);
                return;
            }

            //什么都没有
            item.PoID = item.PoIDSource = null;
        }

        /// <summary>
        /// 获取下一个CBD跟踪任务
        /// </summary>
        /// <returns></returns>
        public static Location GetNextCBDJob() 
        {
            var query = Query.And(Query.EQ("RefreshStatus", (sbyte)Enums.CrawlStatus.Normal), Query.LT("NextRefreshTime", DateTime.Now));
            return GetOneEntityByQuery<Location>(query);
        }

        /// <summary>
        /// CBD跟踪完成后写回数据库
        /// </summary>
        /// <param name="loc"></param>
        public static void PushbackLoationJob(Location loc) 
        {
            string[] parameters = new string[4];
            parameters[0] = "RefreshStatus";
            parameters[1] = "LastRefreshTime";
            parameters[2] = "RefreshCount";
            parameters[3] = "NextRefreshTime";
            UpdateDB<Location>(loc, "PoID", parameters, SafeMode.True);
        }

        public static void InitLocationJob()
        {
            var query = Query.EQ("CategoryID", "CBD");
            var collection = GetCollections<Location>();
            var cbds = collection.FindAs<Location>(query);
            int cnt = 0;
            foreach (var loc in cbds)
            {
                cnt++;
                collection.Remove(Query.EQ("PoID", loc.PoID), SafeMode.True);
                loc.RefreshCount = 0;
                if (loc.RefreshStatus == Enums.CrawlStatus.Crawling)
                    loc.RefreshStatus = Enums.CrawlStatus.Normal;
                loc.LastRefreshTime = Utilities.Epoch;
                loc.NextRefreshTime = Utilities.Epoch;
                collection.Insert<Location>(loc, SafeMode.True);
            }
            Console.WriteLine(cnt);
        }
    }
}

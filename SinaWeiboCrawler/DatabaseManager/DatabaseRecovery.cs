using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SinaWeiboCrawler.Utility;
using System.IO;

namespace SinaWeiboCrawler.DatabaseManager
{
    class DatabaseRecovery : MongoDBManager
    {
        public static void Work() 
        {
            var collection = GetCollections<Location>();
            var result = collection.FindAllAs<Location>();
            Dictionary<string, int> cnt = new Dictionary<string, int>();
            int nullcnt = 0;
            foreach (var loc in result)
            {
                Console.WriteLine(loc.PoID);
                if (loc.PoIDSource == null)
                {
                    nullcnt++;
                    continue;
                }
                if (cnt.ContainsKey(loc.PoIDSource))
                    cnt[loc.PoIDSource] += loc.CheckInCount;
                else cnt.Add(loc.PoIDSource, loc.CheckInCount);
            }
            File.AppendAllText("result.txt", nullcnt.ToString() + "\n");
            foreach (var tupe in cnt) 
            {
                File.AppendAllText("result.txt", tupe.Key + " " + tupe.Value + "\n");
            }
            Console.WriteLine("finished");
        }

        public static void RecoverInternalSubscribeID() 
        {

        }

        public static int Count() 
        {
            var query = Query.And(Query.LT("NextRefreshTime", DateTime.Now), Query.EQ("RefreshStatus", (sbyte)Enums.CrawlStatus.Normal));
            return GetCollections<Author>().Count(query);
        }
    }
}

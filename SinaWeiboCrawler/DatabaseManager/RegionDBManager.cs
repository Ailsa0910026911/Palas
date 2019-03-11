using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palas.Common.Lib.DAL;
using HooLab.Log;

namespace SinaWeiboCrawler.DatabaseManager
{
    /// <summary>
    /// Region与数据库交互的类
    /// </summary>
    class RegionDBManager
    {
        /// <summary>
        /// 获取地点对应的RegionID
        /// </summary>
        /// <param name="location">地址</param>
        /// <returns>最接近的Region的RegionID</returns>
        public static string GetRegionID(string location)
        {
            if (string.IsNullOrEmpty(location)) return null;
            string[] segs = location.Split();

            if (segs[0] == "其他") return null;

            string Where = string.Format("Nation='{0}' OR Province='{0}'", segs[0]);
            if (segs[0] == "海外")
            {
                if (segs.Length == 1) return null;
                Where = string.Format("Nation='{0}'", segs[1]);
                segs = segs.Skip(1).ToArray();
            }

            string FirstWhere = Where;

            if (segs.Length > 1 && segs[1] != "其他")
                Where += string.Format(" AND (City='{0}' OR District='{0}')", segs[1]);

            string[] IDs = RegionMySqlDAL.GetIDsByWhere(Where, null, "District,Street", 0, 1);
            if (IDs != null && IDs.Length >= 1)
                return IDs[0];
            else
                if (segs.Length > 1)
                {
                    IDs = RegionMySqlDAL.GetIDsByWhere(FirstWhere, null, RegionMySqlDAL.OrderColumn.Default, 0, 1);
                    if (IDs != null && IDs.Length >= 1)
                        return IDs[0];
                    else
                    {
                        Logger.Info(location);
                        return null;
                    }
                }
                else
                {
                    Logger.Info(location);
                    return null;
                }
        }
    }
}

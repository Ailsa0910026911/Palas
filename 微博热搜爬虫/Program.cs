using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CsQuery;
using Newtonsoft.Json.Linq;
using Palas.Common.DataAccess;
using Palas.Common.Data;
using HtmlAgilityPack;

namespace WeiboHotKeyCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient hc = new HttpClient();
            WeiboHotKey(hc);
            Console.WriteLine("微博抓取完成");
            BaiduHotKey(hc);
            Console.WriteLine("百度抓取完成");
            SoHotKey(hc);
            Console.WriteLine("360抓取完成");
            Console.WriteLine("全部抓取完成");
        }

        /// <summary>
        /// 新浪微博热词抓取
        /// </summary>
        /// <param name="hc"></param>
        public static void WeiboHotKey(HttpClient hc)
        {
            #region 微博
            var _t = hc.GetStringAsync("http://s.weibo.com/top/summary?cate=realtimehot");
            _t.Wait();
            string value = _t.Result;
            CQ DOM = new CQ(value);
            var libs = DOM["script"];
            foreach (var lib in libs)
            {
                var mainLib = CQ.Create(lib);
                if (mainLib.Html().Contains("\"pid\":\"pl_top_realtimehot\""))
                {
                    List<WeiboHotKey> weiboHotKey = new List<WeiboHotKey>();
                    string jsonValue = mainLib.Html().Replace("STK && STK.pageletM && STK.pageletM.view(", "");
                    jsonValue = jsonValue.Remove(jsonValue.Length - 1);
                    var jobject = JsonConvert.DeserializeObject<JToken>(jsonValue);
                    string htmlValue = jobject["html"].ToString();
                    CQ hotDOM = new CQ(htmlValue);
                    var hotLibs = hotDOM["tbody tr"];
                    foreach (var hotlib in hotLibs)
                    {
                        var hotLibItem = CQ.Create(hotlib);
                        WeiboHotKey hotkey = new WeiboHotKey()
                        {
                            title = hotLibItem["div.rank_content p a"].Text(),
                            type = hotLibItem["div.rank_content p i"].Text(),
                            HotValue = GetInt(hotLibItem["p.star_num span"].Text())
                        };
                        weiboHotKey.Add(hotkey);
                    }
                   
                    using (var client = new RedisAccess(RedisConnect.WeiboHotKeyRedisPool))
                    {
                        client.Set("Weibo", weiboHotKey);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 百度搜索热词抓取
        /// </summary>
        /// <param name="hc"></param>
        public static void BaiduHotKey(HttpClient hc)
        {
            var _t = hc.GetByteArrayAsync("http://top.baidu.com/buzz?b=341&c=513&fr=topbuzz&qq-pf-to=pcqq.c2c");
            _t.Wait();
            var value = _t.Result;
            string html = Encoding.GetEncoding("gb2312").GetString(value);
            HtmlDocument docu = new HtmlDocument();
            docu.LoadHtml(html);
            var node = docu.DocumentNode.SelectNodes("//table[@class='list-table']/tr[position()>1] ");
            List<WeiboHotKey> weiboHotKey = new List<WeiboHotKey>();
            foreach (var item in node)
            {
                var title = item.SelectSingleNode("./td[@class='keyword']/a[@class='list-title']")?.InnerText.ToString();
                var count = GetInt(item.SelectSingleNode("./td[@class='last']/span[@class='icon-rise']")?.InnerText.ToString());
                var type = item.SelectSingleNode("./td[@class='keyword']/span")?.GetAttributeValue("class", "").ToString();

                weiboHotKey.Add(new WeiboHotKey()
                {
                    title = title,
                    HotValue = count,
                    type = !string.IsNullOrEmpty(type) ? "新" : ""
                });
            }

            using (var client = new RedisAccess(RedisConnect.WeiboHotKeyRedisPool))
            {
                client.Set("Baidu", weiboHotKey);
            }
        }
        /// <summary>
        /// 360搜索热词抓取
        /// </summary>
        /// <param name="hc"></param>
        public static void SoHotKey(HttpClient hc)
        {
            var _t = hc.GetStringAsync("https://trends.so.com/top/realtime");
            _t.Wait();
            var value = _t.Result;
            List<WeiboHotKey> weiboHotKey = new List<WeiboHotKey>();
            JToken jvalue = JsonConvert.DeserializeObject<JToken>(value);
            foreach (var item in jvalue["data"]["result"])
            {
                string isHot = item["update"].ToObject<int>() == 1 ? "新" : "";
                string hot = item["heat"]?.ToString();
                weiboHotKey.Add(new WeiboHotKey()
                {
                    title = item["query"].ToString(),
                    HotValue = !string.IsNullOrEmpty(hot) ? GetInt(hot) : 0,
                    type = isHot
                });
            }
            using (var client = new RedisAccess(RedisConnect.WeiboHotKeyRedisPool))
            {
                client.Set("360", weiboHotKey);
            }
        }

        static int GetInt(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
    }
    class WeiboHotKey
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 热/新
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 搜索热度
        /// </summary>
        public int HotValue { get; set; }
    }
}

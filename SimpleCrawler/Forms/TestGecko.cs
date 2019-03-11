using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Crawler.Core;
using Crawler.Core.Manager;
using Crawler.Core.RequestProcessor;
using Crawler.Core.Utility;
using Gecko;
using Palas.Common.Data;


namespace Crawler.Host
{
    public partial class TestGecko : Form
    {
        private static string _CachePath
            ;

        public TestGecko()
        {
            Xpcom.Initialize(CrawlerManager.XULRunnerPath);                                   //设置xpcom等组件的目录
            if (Directory.Exists(CachePath))                                                  //设置缓存目录
                Xpcom.ProfileDirectory = CachePath;
            else
            {
                Directory.CreateDirectory(CachePath);
                Xpcom.ProfileDirectory = CachePath;
            }
            //var cookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
            //cookieMan = Xpcom.QueryInterface<nsICookieManager>(cookieMan);

            //Gecko.GeckoPreferences.User["permissions.default.image"] = 2;            //禁用图片
            //Gecko.GeckoPreferences.User["network.image.imageBehavior"] = 2;          //禁用图片  
            //Gecko.GeckoPreferences.User["permissions.default.stylesheet"] = 2;       //禁用css
            //Gecko.GeckoPreferences.User["browser.link.open_newwindow.restriction"] = 1;       //不打开任何新的窗口
            

            //Gecko.GeckoPreferences.User["browser.history_expire_days"] = 3;          //浏览历史过期天数
            //Gecko.GeckoPreferences.User["browser.history_expire_days_min"] = 3;
            //Gecko.GeckoPreferences.User["browser.cache.memory.capacity"] = 65536;    //FF的内存缓存
            //Gecko.GeckoPreferences.User["browser.sessionhistory.max_total_viewers"] = 0;    //禁用“上一页”

            //Gecko.GeckoPreferences.User["network.http.pipelining"] = true;    
            //启用Pipeline
            InitializeComponent();
        }
        public static string CachePath
        {
            get
            {
                if (_CachePath == null)
                {
                    //创建一个新的临时文件夹
                    _CachePath = Path.Combine(CrawlerManager.XULRunnerRootCachePath, Guid.NewGuid().ToString());
                    //删除今天以前未使用的旧临时文件夹
                    foreach (DirectoryInfo di in new DirectoryInfo(CrawlerManager.XULRunnerRootCachePath).GetDirectories())
                    {
                        if (di.LastAccessTime < DateTime.Now.AddDays(-1))
                            di.Delete(true);
                    }
                }
                return _CachePath;
            }
        }
        private void TestGecko_Load(object sender, EventArgs e)
        {


            //geckoWebBrowser1.DocumentCompleted += new EventHandler(geckoWebBrowser1_DocumentCompleted);
            //geckoWebBrowser1.Visible = true;
            //geckoWebBrowser1.Navigate("http://www.weibo.com");
            //geckoWebBrowser1.Dispose();
        }
        private Point GetAbsolutePosition(GeckoHtmlElement element)
        {
            var left = 0;
            var top = 0;
            var obj = element;
            
            while(obj.TagName.ToLower() != "body")
            {
                left += obj.OffsetLeft;
                top += obj.OffsetTop;
                obj = obj.OffsetParent;
            }
            return new Point(left,top);
        }
        void geckoWebBrowser1_DocumentCompleted(object sender, EventArgs e)
        {
            
            var html = geckoWebBrowser1.Document.Body.InnerHtml;
            var nodes = geckoWebBrowser1.Document.EvaluateXPath("//div").GetNodes();
            //GeckoElement htmlnode =  (GeckoElement) geckoWebBrowser1.Document.GetNodes("/html").FirstOrDefault();

            foreach (var geckoNode in nodes)
            {
               
                var element = (GeckoHtmlElement) geckoNode;
                var pos = GetAbsolutePosition(element);
                var x = pos.X;
                var y = pos.Y;



            }

        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            var cssValue = EnableCssChk.Checked ? 1 : 2;
            Gecko.GeckoPreferences.User["permissions.default.stylesheet"] = cssValue;

            var value = Gecko.GeckoPreferences.User["permissions.default.stylesheet"];
            
            geckoWebBrowser1.Navigate(UrlTxt.Text.Trim(),GeckoLoadFlags.StopContent);
            geckoWebBrowser1.DocumentCompleted+=geckoWebBrowser1_DocumentCompleted;
            
            //string ResponseUrl, ExceptionMsg,UsingEncoding;
            //Enums.CrawlResult Status;
            //CookieContainer cookieContainer = new CookieContainer();
            //var cookieStr = geckoWebBrowser1.Document.Cookie;
            //var pair = cookieStr.Split(';');
            //foreach (string s in pair)
            //{
            //    var rst = s.Split('=');
            //    var name = HttpUtility.UrlEncode(rst[0],Encoding.UTF8);
            //    var value = HttpUtility.UrlEncode(rst[1], Encoding.UTF8);
            //    Cookie cookie = new Cookie(name, value);

            //    cookie.Domain = ".weibo.com";
            //    cookie.Path = "/";
            //    cookieContainer.Add(cookie);
            //}

            //string Content = WebRequestProcessor.DownloadHTTPString(UrlTxt.Text.Trim(), out ResponseUrl, out Status, out ExceptionMsg, out UsingEncoding,  5 , "UTF-8", "",cookieContainer,false);


        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            var weiboBrowser = new WeiboBrowser(geckoWebBrowser1);
            weiboBrowser.ToHome()
                        .ContinueWith(task => weiboBrowser.Login("15244082618", "qazwsxedc"))
                        .ContinueWith(task => weiboBrowser.FollowWeibo("3547072390944856","转发微博"));
            
        }
    }
}

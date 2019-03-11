using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HooLab.Log;
using Palas.Common.Data;
using Palas.Common.Lib.Entity;
using System.IO.Compression;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 提供WebRequest使用的相关静态方法
    /// </summary>
    public static class WebRequestProcessor
    {
        private static string GetFullUrl(string baseUrl, string currentUrl)
        {
            Uri uri = new Uri(new Uri(baseUrl),currentUrl);
            return uri.AbsoluteUri;
        }
        /// <summary>
        /// 通过HttpRequest对象访问网络
        /// </summary>
        public static string GetRedirectedUrl(string url)
        {
            string result = url;

            //初始化
            string ResponseUrl;
            Enums.CrawlResult Status;
            string ExceptionMsg;
            
            //发出请求
            string UsingEncoding;
            string Content = DownloadHTTPString(url, out ResponseUrl, out Status, out ExceptionMsg, out UsingEncoding);
            result = ResponseUrl;
            //处理Meta转向
            for(int i=0;i<5;i++)
            {
                string metaUrl = ExtractMetaRedirectUrl(Content);
                if (!string.IsNullOrEmpty(metaUrl))
                {
                    string redirectUrl = GetFullUrl(url, metaUrl);
                    result = redirectUrl;
                    Content = DownloadHTTPString(redirectUrl, out ResponseUrl, out Status, out ExceptionMsg, out UsingEncoding);
                    continue;
                }
                break;
            }
            
            //请求成功则处理转向
            if (Status == Enums.CrawlResult.Succ)
            {
                for (sbyte i = 0; i < 10; i++)
                {
                    string RedirectUrl = ExtractJsRedirectUrl(Content);
                    if (string.IsNullOrEmpty(RedirectUrl))
                    {
                        break;
                    }
                    RedirectUrl = GetFullUrl(url, RedirectUrl);
                    Content = DownloadHTTPString(RedirectUrl, out ResponseUrl, out Status, out ExceptionMsg, out UsingEncoding);
                    result = RedirectUrl;
                    if (Status != Enums.CrawlResult.Succ) break;
                }
            }
            
            return result;
        }

        private static string DownloadHTTPString(string Url, int TimeoutSecs = 40, CookieContainer Cookie = null)
        {
            string useLessUrl;
            string useLessExp;
            string useLessEncoding;
            Enums.CrawlResult useLessResult;
            return DownloadHTTPString(Url, TimeoutSecs:TimeoutSecs, Cookie: Cookie, ResponseUrl: out useLessUrl, Status: out useLessResult, ExceptionMsg: out useLessExp, Proxy: null, UsingEncoding: out useLessEncoding, ReEncodeByHTMLCharset: true);
        }

        private const string CharsetReg = @"(meta.*?charset=""?(?<Charset>[^\s""]+)""?)|(xml.*?encoding=""?(?<Charset>[^\s""]+)""?)";
        /// <summary>
        /// 获得HTTP页面URI的HTTP返回字符串(get方法)
        /// </summary>
        /// <param name="Url">原始请求Url</param>
        /// <param name="ResponseUrl">跳转后的Url</param>
        /// <param name="Status">状态信息</param>
        /// <param name="ExceptionMsg">异常信息</param>
        /// <param name="UsingEncoding">实际使用的Encoding</param>
        /// <param name="TimeoutSecs"></param>
        /// <param name="Encoding">指定编码</param>
        /// <param name="Proxy"></param>
        /// <param name="Cookie">输入和输出</param>
        /// <param name="ReEncodeByHTMLCharset">根据HTML头Charset标志决定编码(utf8,gb2312,gbk之一)，忽略指定编码，并将结果返回到UsingEncoding</param>
        /// <returns>结果字符</returns>
        private static string DownloadHTTPString(string Url, out string ResponseUrl, out Enums.CrawlResult Status, out string ExceptionMsg, out string UsingEncoding, int TimeoutSecs = 40, string Encoding = "UTF-8", string Proxy = null, CookieContainer Cookie = null, bool ReEncodeByHTMLCharset = true)
        {
            //todo:low httpdownload 支持header 的 tag检查，未更新页面不下载
            //todo:low httpdownload DNS缓存

            #region 初始化设置
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(Url));
            Request.AllowAutoRedirect = true;
            Request.MaximumAutomaticRedirections = 5;
            //Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0.2) Gecko/20100101 Firefox/6.0.2";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/15.0.849.0 Safari/535.1";
            Request.KeepAlive = true;
            Request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            Request.Referer = "http://" + Request.RequestUri.Host;

            if (Cookie != null)
                Request.CookieContainer = Cookie;
            else
                Request.CookieContainer = new CookieContainer();
            if (TimeoutSecs > 0)
                Request.Timeout = TimeoutSecs * 1000;
            if (!string.IsNullOrEmpty(Proxy))
                Request.Proxy = new WebProxy(Proxy);

            #endregion 初始化设置

            //建立连接
            System.Net.HttpWebResponse Response;
            try
            {
                Response = (HttpWebResponse)Request.GetResponse();
            }
            catch (Exception ex)
            {
                Status = DetermineResultStatus(ex);
                ExceptionMsg = ex.Message;
                ResponseUrl = Url;
                UsingEncoding = null;
                return null;
            }

            string Content = null;
            //参数默认值GB2312,如果设为null,则这里初始化为Encoding.Default
            
            Encoding Encode = string.IsNullOrEmpty(Encoding) ? System.Text.Encoding.GetEncoding("UTF-8") : System.Text.Encoding.GetEncoding(Encoding);
            if (ReEncodeByHTMLCharset)
            {
                #region 根据Html头判断

                //缓冲区长度
                const int N_CacheLength = 10000;
                //头部预读取缓冲区，字节形式
                List<byte> bytes=new List<byte>();
                int count = 0;
                //头部预读取缓冲区，字符串
                string cache = "";

                //创建流对象并解码
                System.IO.Stream ResponseStream;
                switch (Response.ContentEncoding.ToUpperInvariant())
                {
                    case "GZIP":
                        ResponseStream = new GZipStream(Response.GetResponseStream(), CompressionMode.Decompress);
                        break;
                    case "DEFLATE":
                        ResponseStream = new DeflateStream(Response.GetResponseStream(), CompressionMode.Decompress);
                        break;

                    default:
                        ResponseStream = Response.GetResponseStream();
                        break;
                }

                try
                {
                    while (!(cache.EndsWith("</head>",StringComparison.OrdinalIgnoreCase) || count >= N_CacheLength))
                    {
                        byte b = (byte)ResponseStream.ReadByte();
                        if (b < 0)  //end of stream
                            break;
                        bytes.Add(b);
                       
                        count++;
                        cache += (char)b;
                    }
                    
                    var match = Regex.Match(cache, CharsetReg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (match.Success)
                    {
                        try
                        {
                            var charset = match.Groups["Charset"].Value;
                            Encode = System.Text.Encoding.GetEncoding(charset);
                        }
                        catch {
                        
                        }
                    }
                    else
                    {
                        try
                        {
                            if (Response.CharacterSet == "ISO-8859-1")
                            {
                                Encode = System.Text.Encoding.GetEncoding("GB2312");
                            }
                            else
                            {
                                Encode = System.Text.Encoding.GetEncoding(Response.CharacterSet);
                            }
                        }
                        catch {}
                    }
                    

                    //缓冲字节重新编码，然后再把流读完
                    StreamReader Reader = new StreamReader(ResponseStream, Encode);
                    Content = Encode.GetString(bytes.ToArray(), 0, count) + Reader.ReadToEnd();
                    Reader.Close();
                }
                catch (Exception ex)
                {
                    Status = DetermineResultStatus(ex);
                    ExceptionMsg = ex.Message;
                    ResponseUrl = Response.ResponseUri.AbsoluteUri;
                    UsingEncoding = null;
                    return null;
                }
                finally
                {
                    Response.Close();
                }

                #endregion 根据Html头判断
            }
            else
            {
                #region 直接读取流

                //不根据Html头来判断，直接读取流
                System.IO.StreamReader Reader = new System.IO.StreamReader(Response.GetResponseStream(), Encode);
                try
                {
                    Content = Reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    Status = DetermineResultStatus(ex);
                    ExceptionMsg = ex.Message;
                    ResponseUrl = Response.ResponseUri.AbsoluteUri;
                    UsingEncoding = null;
                    return null;
                }
                finally
                {
                    Reader.Close();
                }

                #endregion 直接读取流
            }

            Status = Enums.CrawlResult.Succ;
            ExceptionMsg = null;
            ResponseUrl = Response.ResponseUri.AbsoluteUri;
            Encoding = Encode.WebName;
            UsingEncoding = Encode.WebName;
            return Content;
        }

        /// <summary>
        /// 根据异常内容判断状态类型
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        static Enums.CrawlResult DetermineResultStatus(Exception ex)
        {
            string Msg = ex.Message.ToLower();
            if (Msg.Contains("超时") || Msg.Contains("timeout") || Msg.Contains("timed out") || Msg.Contains("502"))
                return Enums.CrawlResult.Timeout;
            if (Msg.Contains("dns") || Msg.Contains("resolved"))
                return Enums.CrawlResult.DNSProblem;
            if (Msg.Contains("404"))
                return Enums.CrawlResult.HTTP404;
            if (Msg.Contains("500"))
                return Enums.CrawlResult.HTTP50X;
            //if (Msg.Contains("connect") || Msg.Contains("connection") || Msg.Contains("302"))
            //    return Enums.CrawlResult.GeneralConnectError;

            return Enums.CrawlResult.GeneralConnectError;
        }

        static string ExtractMetaRedirectUrl(string HTML)
        {
            if (string.IsNullOrEmpty(HTML))
            {
                return null;
            }
            Regex metaex = new Regex(@"<META.*?HTTP-EQUIV=""?Refresh""?.*?url=(?<Url>.+?)""", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match metam = metaex.Match(HTML);
            if (metam.Success)
            {
                return metam.Groups["Url"].Value;
            }
            return null;
        }
        /// <summary>
        /// 获取页面JS重定向Url（抓取过程中用）
        /// </summary>
        /// <param name="HTML">初步页面代码</param>
        /// <returns></returns>
        static string ExtractJsRedirectUrl(String HTML)
        {
            
            
            Regex ex = new Regex(@"location\.replace\(.*\);");
            Match m = ex.Match(HTML);
            if (m.Success)
            {
                int st = m.Value.IndexOf("\"") + 1, ed = m.Value.IndexOf(")") - 2;
                return m.Value.Substring(st, ed - st + 1);
            }


            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.IO;
using HooLab.Log;

namespace SinaWeiboCrawler.Utility
{
    /// <summary>
    /// 用来页面抓取名人榜的HTML内容
    /// </summary>
    class HTMLJobProcessor
    {
        private static int TimeoutSecs = 10;
        private static String ReadResponseStream(WebResponse Response)
        {
            const int N_CacheLength = 1000;                                                   //缓冲区长度
            byte[] bytes = new byte[N_CacheLength];                                           //头部预读取缓冲区，字节形式
            int count = 0;
            string cache = "";                                                                //头部预读取缓冲区，字符串
            Encoding encode = Encoding.UTF8;                                                  //初始化默认encode

            System.IO.Stream ResponseStream = Response.GetResponseStream();

            while (!(cache.EndsWith("</head>") || cache.EndsWith("charset=") || count >= N_CacheLength))
            {
                byte b = (byte)ResponseStream.ReadByte();
                if (b < 0)  //end of stream
                    break;
                bytes[count] = b;
                count++;
                cache += (char)b;
            }

            if (cache.EndsWith("charset="))
            {
                do
                {
                    byte b = (byte)ResponseStream.ReadByte();
                    bytes[count] = b;
                    count++;
                    cache += (char)b;
                    switch ((char)b)
                    {
                        case 'G':
                        case 'g':
                            encode = Encoding.GetEncoding("GB2312");
                            break;
                        case ' ':
                            continue;
                    }
                } while (false);
            }

            try
            {
                System.IO.StreamReader Reader = new StreamReader(ResponseStream, encode);
                string Result = encode.GetString(bytes, 0, count) + Reader.ReadToEnd();
                Reader.Close();
                Response.Close();
                return Result;
            }
            catch (Exception) { return null; };
        }

        private static HttpWebRequest CreateWebRequest(String URL)
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(URL));
            Request.CookieContainer = new CookieContainer();
            if (TimeoutSecs > 0)
                Request.Timeout = TimeoutSecs * 1000;
            return Request;
        }

        public static string GetHTMLViaRequest(string url)
        {
            try
            {
                WebRequest Request = CreateWebRequest(url);
                WebResponse Response = null;
                Response = Request.GetResponse();
                return ReadResponseStream(Response);
            }
            catch (Exception ex) { Logger.Error(ex.ToString()); return null; }
        }
    }
}

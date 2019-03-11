using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Aspose.Cells;
using Crawler.Core.RequestProcessor;
using Crawler.Core.Utility;
using Palas.Common.DataAccess;
using Palas.Common.Utility;
using Palas.Common.Data;

namespace Crawler.Host
{
    public partial class TestBaidu : Form
    {
        private string baiduRegex =
            @"<td class=f>[\s\S]*?<a.*?href=""(?<Url>.+?)"".*?>(?<Title>.+?)(?:\.\.\.)?</a>[\s\S]*?<font size=-1>\s*(?<Text>.+?)<br>\s*?<span class=""g"">.*?(?<PubDate>(\d+-\d+-\d+)|(\d+.{1,3}前))";
            //@"<td class=f>[\s\S]*?<a[^>]*href=""(?<Url>[^""]+)""[^>]*>(?<Title>.+?)(?:\.\.\.)?</a>[\s\S]*?<font size=-1>\s*(?:<font class=c>文件格式:</font>[^<]+<a[^>]+>HTML版</a><br>|<span class=""m"">(?:[^>]+)</span>)?(?<Text>.+?)(?:\.\.\.)?<br><span class=""g"">.*?\s(?<PubDate>(\d+-\d+-\d+)|(\d+.+前))";
        private string DsgUrl = @"http://www.baidu.com/s?q1=DSG&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CE%AC%C8%A8+%CE%CA%CC%E2+%C8%B1%CF%DD+%B6%B6%B6%AF+%C2%A9%B6%B4+%C9%C1%CB%B8+%D5%D9%BB%D8&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv";
        private string[] poloUrls = new string[]
                                        {
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7+%D2%EC%CF%EC&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                            @"http://www.baidu.com/s?ct=1&tn=baiduadv&rn=100&lm=1&bs=title%3A+%28polo+%28%C6%DB%D5%A9+%7C+%C5%E2%B3%A5+%7C+%CD%CB%BB%F5+%7C+%CD%BB%C8%BB+%7C+%B1%DC%D5%F0+%7C+%CE%DE%D3%EF+%7C+%B7%C0%B6%B3%D2%BA+%7C+%CB%CA%B3%B5+%7C+%C1%EF%C6%C2&f=8&rsv_bp=1&wd=title%3A+%28polo+%28%C6%DB%D5%A9+%7C+%C5%E2%B3%A5+%7C+%CD%BB%C8%BB+%7C+%B1%DC%D5%F0+%7C+%CE%DE%D3%EF+%7C+%B7%C0%B6%B3%D2%BA+%7C+%CB%CA%B3%B5+%7C+%C1%EF%C6%C2%29%29&inputT=9562",
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB+%D3%B0%CF%EC+%C3%FB%D3%FE+%D1%CF%D6%D8+%B8%F4%D2%F4+%C7%F3%D6%FA+%C6%F4%B6%AF&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7+%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2+%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                            @"http://www.baidu.com/s?q1=polo&q2=&q3=%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4+%D0%C4%BA%AE+%C9%FA%C6%F8&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv"
                                        };
        private string[] langyiUrls = new string[]
                                          {
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4+%D0%C4%BA%AE+%C9%FA%C6%F8&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2+%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7+%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB+%D3%B0%CF%EC+%C3%FB%D3%FE+%D1%CF%D6%D8+%B8%F4%D2%F4+%C7%F3%D6%FA+%C6%F4%B6%AF&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                              @"http://www.baidu.com/s?ct=1&tn=baiduadv&rn=100&lm=1&bs=title%3A+%28%C0%CA%D2%DD+%28%D6%C6%B6%AF+%7C+%CA%A7%CD%FB+%7C+%BF%D3%C8%CB+%7C+%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF%29%29&f=8&rsv_bp=1&wd=title%3A+%28%C0%CA%D2%DD+%28%C6%DB%D5%A9+%7C+%C5%E2%B3%A5+%7C+%CD%BB%C8%BB+%7C+%B1%DC%D5%F0+%7C+%CE%DE%D3%EF+%7C+%B7%C0%B6%B3%D2%BA+%7C+%CB%CA%B3%B5+%7C+%C1%EF%C6%C2%29%29&rsv_n=2&inputT=1125",
                                              @"http://www.baidu.com/s?q1=%C0%CA%D2%DD&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7+%D2%EC%CF%EC+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",

                                          };
        private string[] pasateUrls = new string[]
                                          {
                                              @"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%D2%EC%CF%EC+%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%D0%C4%BA%AE+%C9%FA%C6%F8+%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA+%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5+%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA+%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?ct=1&tn=baiduadv&rn=100&lm=1&bs=title%3A+%28%C5%C1%C8%F8%CC%D8+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+4S+%7C+%D6%C6%C0%E4%29%29&f=8&rsv_bp=1&wd=title%3A+%28%C5%C1%C8%F8%CC%D8+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+%C6%DB%D5%A9+%7C+%C5%E2%B3%A5%29%29&inputT=6203",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%B5%B5%CE%BB+%CD%BB%C8%BB+%B1%DC%D5%F0+%CE%DE%D3%EF+%BE%D9%B1%A8+%CB%CA%B3%B5+%C1%EF%C6%C2&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C5%C1%C8%F8%CC%D8&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv"

                                          };
        private string[] santanaUrls = new string[]
                                           {
                                               @"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%D2%EC%CF%EC+%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%D0%C4%BA%AE+%C9%FA%C6%F8+%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA+%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5+%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA+%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?ct=1&tn=baiduadv&rn=100&lm=1&bs=title%3A+%28%C9%A3%CB%FE%C4%C9+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+4S+%7C+%D6%C6%C0%E4%29%29&f=8&rsv_bp=1&wd=title%3A+%28%C9%A3%CB%FE%C4%C9+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+%C6%DB%D5%A9+%7C+%C5%E2%B3%A5%29%29&rsv_n=2&inputT=1344",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%B5%B5%CE%BB+%CD%BB%C8%BB+%B1%DC%D5%F0+%CE%DE%D3%EF+%BE%D9%B1%A8+%CB%CA%B3%B5+%C1%EF%C6%C2&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%C9%A3%CB%FE%C4%C9&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
};

        private string[] turanUrls = new string[]
                                         {
                                             @"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7+%D2%EC%CF%EC&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4+%D0%C4%BA%AE+%C9%FA%C6%F8&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2+%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7+%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB+%D3%B0%CF%EC+%C3%FB%D3%FE+%D1%CF%D6%D8+%B8%F4%D2%F4+%C7%F3%D6%FA+%C6%F4%B6%AF&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B0%B2&q2=&q3=%C6%DB%D5%A9+%C5%E2%B3%A5+%CD%BB%C8%BB+%B1%DC%D5%F0+%CE%DE%D3%EF+%B7%C0%B6%B3%D2%BA+%CB%CA%B3%B5+%C1%EF%C6%C2&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
};
        private string[] tuguanUrls = new string[]
                                          {
                                              @"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%B9%CA%D5%CF+%C8%B1%CF%DD+%CA%A7%C1%E9+%D7%D4%C8%BC+%CF%A8%BB%F0+%B6%B6%B6%AF+%C2%A9%B6%B4+%B1%AC%B8%D7+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%D2%EC%CF%EC+%CE%CA%CC%E2+%CE%AC%C8%A8+%CD%B6%CB%DF+%C6%D8%B9%E2+%D2%FE%BB%BC+%C2%A9%CB%AE+%BC%F5%C5%E4+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%D0%C4%BA%AE+%C9%FA%C6%F8+%B1%A8%BE%AF+%B3%B5%BB%F6+%D7%B2%B3%B5+%CA%C2%B9%CA+%C6%F0%BB%F0+%CB%F7%C5%E2+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%BF%D3%B5%F9+%CD%A3%B2%FA+%BC%F5%B2%FA+%D3%B2%C9%CB+%E8%A6%B4%C3+%B1%AC%C1%CF+%C3%AB%B2%A1+%BC%D3%BC%DB+&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%CD%C6%CE%AF+%EC%CE%C5%AA+%B5%CD%C1%D3+%CD%D1%B5%B5+%CD%D0%B5%B5+%CD%CF%B5%B5+%BC%F2%C5%E4+%C0%EB%C6%D7&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%BB%CE%B6%AF+%D1%CF%D6%D8+%CB%B3%B3%A9+%D3%CD%BA%C4+%BA%F3%BB%DA+%D6%C6%B6%AF+%CA%A7%CD%FB+%BF%D3%C8%CB&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
@"http://www.baidu.com/s?ct=1&tn=baiduadv&rn=100&lm=1&bs=title%3A+%28%CD%BE%B9%DB+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+4S+%7C+%D6%C6%C0%E4%29%29&f=8&rsv_bp=1&wd=title%3A+%28%CD%BE%B9%DB+%28%D3%B0%CF%EC+%7C+%C3%FB%D3%FE+%7C+%D1%CF%D6%D8+%7C+%B8%F4%D2%F4+%7C+%C7%F3%D6%FA+%7C+%C6%F4%B6%AF+%7C+%C6%DB%D5%A9+%7C+%C5%E2%B3%A5%29%29&rsv_n=2&inputT=2125",
@"http://www.baidu.com/s?q1=%CD%BE%B9%DB&q2=&q3=%B5%B5%CE%BB+%CD%BB%C8%BB+%B1%DC%D5%F0+%CE%DE%D3%EF+%BE%D9%B1%A8+%CB%CA%B3%B5+%C1%EF%C6%C2&q4=&rn=100&lm=1&ct=1&ft=&q5=1&q6=&tn=baiduadv",
                                          };
        public TestBaidu()
        {
            InitializeComponent();
        }
        
        private void CrawlBtn_Click(object sender, EventArgs e)
        {

            //ImportMedia();
            //return;
            //Dsg Report generate
            var content = WebRequestProcessor.DownloadHTTPString(DsgUrl);
            
            var matches = Regex.Matches(content, baiduRegex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Workbook book = new Workbook();
            book.Open(@"D:\dailyreport\DSG.xlsx");
            var worksheet = book.Worksheets[0];
            int dsgStartRow = 7;
            foreach (Match match in matches)
            {
                if (match.Groups["PubDate"].Value.Contains("前"))
                {
                    worksheet.Cells.InsertRow(dsgStartRow);
                }
                
            }

            
                foreach (Match match in matches)
                {
                    if (!match.Groups["PubDate"].Value.Contains("前"))
                    {
                        continue;
                    }

                    
                    var resultUrl = match.Groups["Url"].Value;
                    try
                    {

                        Uri uri = new Uri(resultUrl);
                        var domain = GetUrlDomain(uri.Host);
                        //匹配媒体名
                        worksheet.Cells[dsgStartRow, 1].PutValue(domain);
                    }
                    catch (Exception)
                    {

                    }
                    var title = TextCleaner.FullClean(match.Groups["Title"].Value) + Environment.NewLine + TextCleaner.FullClean(match.Groups["Text"].Value);
                    var currentExcelRow = dsgStartRow + 1;
                    worksheet.Cells[dsgStartRow, 0].PutValue(resultUrl);
                    worksheet.Cells[dsgStartRow, 5].Formula = "=VLOOKUP(B" + currentExcelRow + ",Sheet2!A:B,2,FALSE)";
                    worksheet.Cells[dsgStartRow, 6].PutValue(title);

                    worksheet.Hyperlinks.Add(dsgStartRow, 6, 1, 1, match.Groups["Url"].Value);
                    worksheet.Cells[dsgStartRow, 7].PutValue(DateTime.Now.ToString("yyyy-MM-dd"));
                    worksheet.Cells[dsgStartRow, 8].PutValue("负面舆情");
                    dsgStartRow++;
                }
            
            book.Save(@"D:\dailyreport\DSG.xlsx");

            //Polo Report generate
            Workbook dailybook = new Workbook();
            dailybook.Open(@"D:\dailyreport\日报.xlsx");
            var dailyWorksheet = dailybook.Worksheets[0];
            int dailyStartRow = 6;

            string categoryName = "大众-POLO";
            var categoryUrls = poloUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

            categoryName = "大众-朗逸";
            categoryUrls = langyiUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

            categoryName = "大众-途安";
            categoryUrls = turanUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

            categoryName = "大众-帕萨特";
            categoryUrls = pasateUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

            categoryName = "大众-桑塔纳";
            categoryUrls = santanaUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

            categoryName = "大众-途观";
            categoryUrls = tuguanUrls;
            CrawlDailyReport(dailyWorksheet, dailybook, ref dailyStartRow, categoryName, categoryUrls);

           

            MessageBox.Show("抓取完成");


        }

        private void ImportMedia()
        {
            Workbook dailybook = new Workbook();
            dailybook.Open(@"D:\dailyreport\日报.xlsx");
            var dailyWorksheet = dailybook.Worksheets[1];
            int dailyStartRow = 0;
            using(PalasDB db = new PalasDB())
            {
                
                    var result = db.Media.Where(model => model.ParentMediaID == null || model.ParentMediaID == "");
                foreach (var item in result)
                {
                    try
                    {
                        var url = item.Url;
                        Uri uri = new Uri(url);
                        var host = GetUrlDomain(uri.Host);
                        dailyWorksheet.Cells[dailyStartRow, 0].PutValue(host);
                        dailyWorksheet.Cells[dailyStartRow, 1].PutValue(item.MediaName);
                        dailyStartRow++;
                    }
                    catch (Exception)
                    {
                        
                        
                    }
                    

                }
                
            }
            dailybook.Save(@"D:\dailyreport\日报.xlsx");

        }

        private static bool IsFilterSurfix(string last)
        {
            string surfix = last.ToLower();
            switch (last)
            {
                case "com":
                case "net":
                case "biz":
                case "edu":
                case "org":
                    return true;
                default:
                    return false;

            }

        }
        private static string GetUrlDomain(string url)
        {
            try
            {
                string domain = "";
                if (string.IsNullOrEmpty(url))
                {
                    url = "http://nourl.com";
                }
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    url = "http://" + url;
                }
                Uri uri = new Uri(url);

                string host = uri.Host;
                var parts = new List<string>(host.Split('.'));
                if (parts.Last().Length == 2)
                {
                    //process the culture surfix. e.g cn. us. etc
                    parts.RemoveAt(parts.Count - 1);
                }
                if (parts.Last().Length == 3)
                {
                    if (IsFilterSurfix(parts.Last()))
                    {
                        parts.RemoveAt(parts.Count - 1);
                    }

                }

                //int domaindot = 
                domain = parts.Last();
                return domain;
            }
            catch (Exception)
            {
                return null;

            }

        }
        private void CrawlDailyReport(Worksheet dailyWorksheet, Workbook dailybook, ref int dailyStartRow, string categoryName,
                                      string[] categoryUrls)
        {
            bool isFirst = true;
            foreach (string url in categoryUrls)
            {
                var dailycontent = WebRequestProcessor.DownloadHTTPString(url);
                Thread.Sleep(2000);
                var dailyMatches = Regex.Matches(dailycontent, baiduRegex,
                                                 RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match dailyMatch in dailyMatches)
                {
                    if (!dailyMatch.Groups["PubDate"].Value.Contains("前"))
                    {
                        continue;
                    }
                    if (isFirst)
                    {
                        dailyWorksheet.Cells[dailyStartRow, 2].PutValue(categoryName);
                        isFirst = false;
                    }
                    var resultUrl = dailyMatch.Groups["Url"].Value;
                    try
                    {
                        
                        Uri uri = new Uri(resultUrl);
                        var domain = GetUrlDomain(uri.Host);
                        //匹配媒体名
                        dailyWorksheet.Cells[dailyStartRow,1].PutValue(domain);
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    var title = TextCleaner.FullClean(dailyMatch.Groups["Title"].Value) + Environment.NewLine +
                                TextCleaner.FullClean(dailyMatch.Groups["Text"].Value);
                    var colorstyle = dailyWorksheet.Cells[dailyStartRow, 6].GetDisplayStyle();
                    colorstyle.Font.Color = Color.Blue;
                    var currentExcelRow = dailyStartRow + 1;
                    dailyWorksheet.Cells[dailyStartRow,0].PutValue(resultUrl);
                    dailyWorksheet.Cells[dailyStartRow, 5].Formula = "=VLOOKUP(B"+currentExcelRow+",Sheet2!A:B,2,FALSE)";
                    
                    
                    dailyWorksheet.Cells[dailyStartRow, 6].SetStyle(colorstyle);
                    dailyWorksheet.Cells[dailyStartRow, 6].PutValue(title);


                    dailyWorksheet.Hyperlinks.Add(dailyStartRow, 6, 1, 1, resultUrl);
                    dailyWorksheet.Cells[dailyStartRow, 7].PutValue(DateTime.Now.ToString("yyyy-MM-dd"));
                    dailyWorksheet.Cells[dailyStartRow,8].PutValue("负面舆情");
                    dailyStartRow++;
                }
            }
            dailybook.Save(@"D:\dailyreport\日报.xlsx");
        }
    }
}

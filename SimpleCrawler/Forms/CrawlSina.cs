using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Threading.Tasks;
using Aspose.Cells;
using Crawler.Core.Manager;
using Crawler.Core.Parser;
using Crawler.Core.Utility;
using HooLab.Log;
using Newtonsoft.Json;
using Palas.Common.Lib.Business;
using Palas.Common.Data;
using Crawler.Core.RequestProcessor;
using Crawler.Core.Data;
using Palas.Common.Lib.Entity;
using Palas.Common.Utility;

namespace Crawler.Host
{
    public partial class CrawlSina : Form
    {
        public CrawlSina()
        {
            InitializeComponent();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            UrlList.Items.Add(FamousTxt.Text.Trim());
        }

        private void CrawlBtn_Click(object sender, EventArgs e)
        {
            if (!Login())
            {
                MessageBox.Show("登入错误，请重试");
                return;
            }
            
            var tmp = UrlList.Items.Cast<string>();
            var task = from item in tmp
                       where !string.IsNullOrEmpty(item)
                       select item;
            int i = 1;
            foreach (string url in task)
            {
                CrawlTask(url);
            }
            //ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = 5;

            //Parallel.ForEach(task, options, item =>
            //                                    {
                                                    
                                                    
            //                                        CrawlTask(item);
                                                    
            //                                    });
           
            MessageBox.Show("运行完成");

        }
        private CrawlRequest BuildRequest(string url,string mustRegex = null)
        {

            return new CrawlRequest(Enums.PageType.List, "Test", "Test", "Weibo", url, null, 0, "", true, true, 0,MustRegex:mustRegex);

        }

        
        public bool NeedCrawlComment
        {
            get { return CommentChk.Checked; }
            set { CommentChk.Checked = value; }
        }
        private int sumGecko = 0;
        private string _currentUrl = "";
        private UserTweet CrawlTask(string entryUrl)
        {
            
            _currentUrl = entryUrl;
            var Site = SiteBusiness.GetBySiteID("Weibo");
            var Request = BuildRequest(entryUrl,RegexContent);
            Site.TimeoutSecs = 60;
            CrawlResponse Response = null;
            try
            {
                Response = GeckoRequestProcessor.DoRequest(Request, Site, null, null);
                AggrSum();

            }
            catch
            {
                
            }
            
            
            
            if (Response.Status != Enums.CrawlResult.Succ)
            {
                Logger.Info("访问页面错误:Url = " + Response.Url);
                
            }
            var content = Response.Content;
            //First page
            UserTweet result = new UserTweet();
            result.Url = entryUrl;
            try
            {
                FillUserInfo(content, result);
            }
            catch
            {
                return result;
            }
            
            var endId = DeterminedMid(content, MidType.EndId);
            var maxId = DeterminedMid(content, MidType.MaxId);
            //var name = Regex.Match(content,)
            int currentPage = 1;
            int maxPage = 50;
            string rootPath = @"D:/output/" + result.Name + "/";
            Workbook outputBook = new Workbook();
            Worksheet sheet = null;
            int currentLine = 4;
            int pos = 1; //表示第几次滚屏(一页中共3次),0:第一次;1:第二次;2:第三次
            bool isContinue = false;
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            if (File.Exists(rootPath + result.Name + ".xls"))
            {
                outputBook.Open(rootPath+result.Name + ".xls");
                sheet = outputBook.Worksheets[result.Name];
                int endrow = currentLine;
                while (!string.IsNullOrEmpty(sheet.Cells[endrow, 0].StringValue))
                {
                    endrow++;
                }
                currentLine = endrow;
                currentPage = (int)(currentLine / 45d) + 1;
                isContinue = true;

            }
            else
            {
                
                sheet = outputBook.Worksheets.Add(result.Name);
                

            }
            //Save to excel
           
            //Initialize column
            sheet.Cells[0, 0].PutValue("姓名");
            sheet.Cells[0, 1].PutValue("网址");
            sheet.Cells[0, 2].PutValue("粉丝数");
            sheet.Cells[0, 3].PutValue("关注数");
            sheet.Cells[0, 4].PutValue("微博数");

            sheet.Cells[1, 0].PutValue(result.Name);
            sheet.Cells[1, 1].PutValue(result.Url);
            sheet.Cells[1, 2].PutValue(result.Follower);
            sheet.Cells[1, 3].PutValue(result.Follow);
            sheet.Cells[1, 4].PutValue(result.TweetNum);

            sheet.Cells[3, 0].PutValue("微博内容");
            sheet.Cells[3, 1].PutValue("发布时间");
            sheet.Cells[3, 2].PutValue("转发数");
            sheet.Cells[3, 3].PutValue("评论数");
            sheet.Cells[3, 4].PutValue("原帖地址");
            sheet.Cells[3, 5].PutValue("来源");
            sheet.Cells[3, 6].PutValue("具体评论");
           

            
            
            if (isContinue)
            {
                pos = 0;
                var url = BuildTweetJsonUrl(result, endId, maxId, currentPage, pos);
                Request = BuildRequest(url);
                Response = GeckoRequestProcessor.DoRequest(Request, Site, null, null);
                AggrSum();
                JsonResponse tmpResult =
                    JsonConvert.DeserializeObject<JsonResponse>(Response.Content.Trim("</pre>".ToArray()));
                Response.Content = HttpUtility.HtmlDecode(tmpResult.data);
                pos++;
                var firstTweet = FillUserTweet(result, Response.Content);
                var firstUrl = firstTweet.FirstOrDefault().Url;
                result.Tweets.Clear();
                int endrow = currentLine - 1;
                while(endrow > 3)
                {
                    if (sheet.Cells[endrow,4].StringValue==firstUrl)
                    {
                        currentLine = endrow;
                        break;
                    }
                    endrow--;
                }
                
                


            }
            //Crawl with json
            while (Regex.IsMatch(Response.Content.Trim(), RegexContent, RegexOptions.Multiline | RegexOptions.IgnoreCase) && Response.Status == Enums.CrawlResult.Succ)
            {
                content = Response.Content.Trim();
                var currentTweet = FillUserTweet(result, content);
                foreach (Tweet tweet in currentTweet)
                {
                    string fileName = tweet.Mid + ".xls";
                    //检查是否是失败后的已经存在的评论
                    if (NeedCrawlComment)
                    {
                        if (!File.Exists(rootPath + fileName))
                        {
                            FillTweetComment(tweet, Site);
                            if (tweet.Comments.Count > 0)
                            {
                                SaveComment(rootPath, tweet, fileName);
                            }
                        }
                        

                    }
                   
                  
                    
                    sheet.Cells[currentLine, 0].PutValue(tweet.Content);
                    sheet.Cells[currentLine, 1].PutValue(tweet.PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sheet.Cells[currentLine, 2].PutValue(tweet.Forward);
                    sheet.Cells[currentLine, 3].PutValue(tweet.Comment);
                    sheet.Cells[currentLine, 4].PutValue(tweet.Url);
                    sheet.Cells[currentLine, 5].PutValue(tweet.Source);

                    //link comment
                    if (File.Exists(rootPath + fileName))
                    {
                        sheet.Cells[currentLine, 6].PutValue("点击查看");
                        //string linkPath = result.Name + "/" + fileName;
                        string linkPath = fileName;
                        sheet.Hyperlinks.Add(currentLine, 6, 1, 1, linkPath);
                    }
                    outputBook.Save(rootPath + result.Name + ".xls");
                    StatusLbl.Text = string.Format("正在读取名人:{0}的第{1}条微博", result.Name, currentLine - 3);
                    Application.DoEvents();
                    currentLine++;

                }
                
              
                

                var url = BuildTweetJsonUrl(result, endId, maxId, currentPage,pos);
                Request = BuildRequest(url);
                for (int i = 0; i < 5;i++ )
                {
                    
                    try
                    {
                        Response = GeckoRequestProcessor.DoRequest(Request, Site, null, null);
                        AggrSum();
                    }
                    catch
                    {

                    }
                    if (Response.Status != Enums.CrawlResult.Succ)
                    {
                        Logger.Info("访问页面错误:Url = " + Response.Url);
                        
                    }
                    else
                    {
                        break;
                    }
                }
                    
                try
                {
                    JsonResponse tmpResult =
                    JsonConvert.DeserializeObject<JsonResponse>(Response.Content.Trim("</pre>".ToArray()));
                    Response.Content = HttpUtility.HtmlDecode(tmpResult.data);
                }
                catch
                {
                    try
                    {
                        CommentJsonResponse tmpResult =
                        JsonConvert.DeserializeObject<CommentJsonResponse>(Response.Content.Trim("</pre>".ToArray()));
                        Response.Content = HttpUtility.HtmlDecode(tmpResult.data.html);
                    }
                    catch 
                    {
                        
                    }
                    

                }
                
                pos = (pos + 1)%3;
                if (pos == 0)
                {
                    currentPage++;
                }
                maxId = result.Tweets.Last().Mid;


            }
            

            
            
            return result;

        }

        private void AggrSum()
        {
            sumGecko++;
            SumLbl.Text = "累计发了" + sumGecko + "请求";
            Application.DoEvents();
            if (sumGecko % 900 == 0)
            {
                //Restart
                File.AppendAllLines("restart.txt",new string[]{NeedCrawlComment.ToString(),_currentUrl});
                Process.Start("Crawler.Host.exe");
                Thread.Sleep(10000);
                Application.Exit();
            }
        }

        private static void SaveComment(string rootPath, Tweet tweet, string fileName)
        {
            Workbook commentBook = new Workbook();
            Worksheet commentSheet = commentBook.Worksheets.Add(tweet.Mid);
            //Initialize column
            commentSheet.Cells[0, 0].PutValue("评论人地址");
            commentSheet.Cells[0, 1].PutValue("评论人");
            commentSheet.Cells[0, 2].PutValue("评论内容");
            commentSheet.Cells[0, 3].PutValue("评论时间");
            int commentLine = 1;
            //string commentDir = @"D:/output/" + result.Name + "/";
            //if (!Directory.Exists(commentDir))
            //{
            //    Directory.CreateDirectory(commentDir);
            //}

            foreach (Comment comment in tweet.Comments)
            {
                commentSheet.Cells[commentLine, 0].PutValue(comment.AuthorUrl);
                commentSheet.Cells[commentLine, 1].PutValue(comment.Author);
                commentSheet.Cells[commentLine, 2].PutValue(comment.Content);
                commentSheet.Cells[commentLine, 3].PutValue(comment.PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
                commentLine++;
            }
            //Save link


            commentBook.Save(rootPath + fileName);
        }

        

        private void FillTweetComment(Tweet tweet,SiteEntity site)
        {
            if (tweet.Comment == 0)
            {
                return;
            }
            int currentPage = 1;
            string mid = tweet.Mid;
            try
            {
                while (true)
                {
                    string url = string.Format(CommentUrlFormat, mid, currentPage);

                    var request = BuildRequest(url);

                    CrawlResponse response = null;
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            response = GeckoRequestProcessor.DoRequest(request, site, null, null);
                            AggrSum();
                        }
                        catch{}
                       
                        if (response.Status != Enums.CrawlResult.Succ)
                        {
                            Logger.Info("访问页面错误:Url = " + response.Url);
                            
                        }
                        else
                        {
                            break;
                        }
                    }
                    CommentJsonResponse tmpResult =
                        JsonConvert.DeserializeObject<CommentJsonResponse>(response.Content.Trim("</pre>".ToArray()));
                    response.Content = HttpUtility.HtmlDecode(tmpResult.data.html);
                    var pageMatch = Regex.Match(response.Content, RegexCommentPage,
                                                RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (currentPage != 1 &&
                        (!pageMatch.Success ||
                         pageMatch.Groups["CurrentPageNum"].Value != currentPage.ToString(CultureInfo.InvariantCulture)))
                    {
                        return;
                    }
                    //Fill Tweet
                    var matches = Regex.Matches(response.Content, RegexComment,
                                                RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    foreach (Match match in matches)
                    {
                        Comment comment = new Comment();
                        comment.Author = match.Groups["Author"].Value;
                        comment.AuthorUrl = RegexParser.AbsoluteUrl(match.Groups["AuthorUrl"].Value, tweet.Url, true);
                        comment.Content = TextCleaner.FullClean(match.Groups["Content"].Value);
                        comment.PubDate = DateTimeParser.Parser(match.Groups["PubDate"].Value) ?? DateTime.MinValue;
                        tweet.Comments.Add(comment);
                    }

                    currentPage++;
                    
                }
            }
            catch {
                
                
            }
            





        }

        private Tweet[] FillUserTweet(UserTweet result, string content)
        {
            var matches = Regex.Matches(content, RegexContent, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            List<Tweet> tweetList = new List<Tweet>();
            try
            {
                foreach (Match match in matches)
                {
                    Tweet tweet = new Tweet();
                    int comment;
                    int.TryParse(match.Groups["Reply"].Value, out comment);
                    int forward;
                    int.TryParse(match.Groups["Forward"].Value, out forward);
                    tweet.Comment = comment;
                    tweet.Content = TextCleaner.FullClean(match.Groups["Content"].Value); 
                    tweet.Mid = match.Groups["Mid"].Value;
                    tweet.Forward = forward;
                    tweet.Source = match.Groups["Source"].Value;
                    tweet.PubDate = DateTimeParser.Parser(match.Groups["PubDate"].Value) ?? DateTime.MinValue;
                    tweet.Url = RegexParser.AbsoluteUrl(match.Groups["Url"].Value, result.Url, true);
                    result.Tweets.Add(tweet);
                    tweetList.Add(tweet);

                }
            }
            catch {}
            
            return tweetList.ToArray();

        }

        private string BuildTweetJsonUrl(UserTweet userTweet, string endId, string maxId, int page, int pos)
        {
            string url = prefixWeiboUrl + "&max_id="+maxId + "&end_id="+endId+"&uid="+userTweet.Uid;
            switch (pos)
            {
                case 0:
                    {
                        //第一次滚屏
                        url = url + "&count=50&page=" + page;
                        int prepage = page > 1 ? page - 1 : page;
                        url = url + "&pre_page=" + prepage;
                        break;
                    }
                case 1:
                case 2:
                    {
                        //第二次，第三次滚屏逻辑一样
                        url = url + "&count=15&page=" + page + "&pre_page=" + page;
                        int pagebar = pos - 1;
                        url = url + "&pagebar=" + pagebar;

                        break;
                    }
                default:
                    {
                        throw new Exception("滚屏次数不对，目前新浪只支持每页3次滚屏");
                    }
            }
            return url;
        }

        private void FillUserInfo(string content, UserTweet userTweet)
        {
            var match = Regex.Match(content, RegexInfo, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            userTweet.Follow = int.Parse(match.Groups["Follow"].Value);
            userTweet.Follower = int.Parse(match.Groups["Follower"].Value);
            userTweet.TweetNum = int.Parse(match.Groups["TweetNum"].Value);
            userTweet.Name = HTMLCleaner.CleanHTML(match.Groups["Name"].Value,true);
            userTweet.Uid = match.Groups["Uid"].Value;

        }

        private string prefixWeiboUrl = "http://weibo.com/aj/mblog/mbloglist?_t=0";
        private string CommentUrlFormat = "http://www.weibo.com/aj/comment/big?_t=0&id={0}&page={1}";
        public const string RegexContent =
            @"<dl action-type=""feed_list.*?mid=""(?<Mid>\d*)""[\s\S]*?<dd class=""content"">[\s\S]*?<p node-type=""feed_list_content"">(?<Content>.+?)</p>[\s\S]*?<p class=""info W_linkb W_textb"">.*?转发(?:\((?<Forward>\d+)\))?</a>[\s\S]+?评论(?:\((?<Reply>\d+)\))?</a>.*?<a title=""(?<PubDate>.*?)"" .*?href=""(?<Url>[^""]+?)"".*?>.*?来自\s*<a.*?>(?<Source>.+?)</a>";

        public const string RegexInfo =
            @"<div class=""left"">(?<Name>[\s\S]+?)<[\s\S]*?uid=(?<Uid>\d*)&[\s\S]*?<strong node-type=""follow"">(?<Follow>\d*)<[\s\S]*?<strong node-type=""fans"">(?<Follower>\d*)<[\s\S]*?<strong node-type=""weibo"">(?<TweetNum>\d*)<";

        public const string RegexComment = @"<dd>[\s\S]*?<a href=""(?<AuthorUrl>[^""]+?)"" title=""(?<Author>[^""]+?)"".*?：(?<Content>[\s\S]+?)<span class=""W_textb"">\((?<PubDate>.*?)\)";
        public const string RegexCommentPage = @"<a action-data=.*?class=""current"".*?>(?<CurrentPageNum>\d+)</a>";
        private string DeterminedMid(string content, MidType type)
        {
            var matches = Regex.Matches(content, RegexContent, RegexOptions.Multiline | RegexOptions.IgnoreCase).Cast<Match>();
            switch (type)
            {
                case MidType.MaxId:
                default:
                    {
                        var result = matches.LastOrDefault();
                        return result == null ? "" : result.Groups["Mid"].Value;
                        break;
                    }
                case MidType.EndId:
                    {
                        var result = matches.FirstOrDefault();
                        return result == null ? "" : result.Groups["Mid"].Value;
                        break;
                    }

            }


        }

        private bool Login()
        {

            //换一个账号
            var Site = SiteBusiness.GetBySiteID("Weibo");
            //AccountManager.NeedChangeAccount(Site);
            //var GotAccout = AccountManager.ChangeAccount(Site.SiteID, Site.AccountLimitReqs);

            ////获得可用帐号，开始登录
            //AccountExtend CurrentAccount = AccountManager.GetCurrentAccount(Site.SiteID);
            
            //var AccountID = CurrentAccount.AccountID;
            //执行登出登录任务并返回其执行结果           
            var CurrentAccount = LoginAccountBusiness.GetByAccountID("Weibo1200");
            var Response = Crawler.Core.RequestProcessor.Processor.DoLogin(Site, CurrentAccount);

            if (Response != null && Response.Status == Enums.CrawlResult.Succ)
            {
                //登录成功
                return true;
            }
            else
            {
                return false;
            }

            //检查代理获取是否正常:null表示异常，string.Empty表示不用代理


        }
        private void CrawlSina_Load(object sender, EventArgs e)
        {
            StatusLbl.Text = "";
            SumLbl.Text = "";
            //Check restart
         
        }

        private void CrawlSina_Shown(object sender, EventArgs e)
        {
            InitialTimer.Enabled = true;
        }

        private void InitialTimer_Tick(object sender, EventArgs e)
        {
            InitialTimer.Enabled = false;
            if (File.Exists("restart.txt"))
            {
                //restart
                
                string[] args = File.ReadAllLines("restart.txt");
                NeedCrawlComment = bool.Parse(args[0]);
                File.Delete("restart.txt");
                string url = args[1];
                Login();
                
                CrawlTask(url);

            }
        }
    }

    public enum MidType : int
    {
        EndId = 0,
        MaxId = 1,
    }
    public class UserTweet
    {
        public string Uid { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int Follower { get; set; }
        public int Follow { get; set; }
        public int TweetNum { get; set; }
        public List<Tweet> Tweets { get; set; } 
        public UserTweet()
        {
            Tweets = new List<Tweet>();

        }
    }
    public class JsonResponse
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }
    public class CommentJsonResponse
    {
        public string code { get; set; }
        public string msg { get; set; }
        public CommentHtml data { get; set; }
    }
public class CommentHtml
{
    public string html { get; set; }
}
    public class Tweet
    {
        public string Mid { get; set; }
        public int Forward { get; set; }
        public int Comment { get; set; }
        public string Url { get; set; }
        public DateTime PubDate { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
        public List<Comment> Comments { get; set; } 
        public Tweet()
        {
            Comments = new List<Comment>();
        }
    }

    public class Comment
    {
        public string AuthorUrl { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime PubDate { get; set; }
    }
}

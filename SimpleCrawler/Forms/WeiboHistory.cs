using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using MongoDB.Driver.Builders;
using Palas.Common;
using Palas.Common.Data;
using Palas.Common.DataAccess;

namespace Crawler.Host
{
    public partial class WeiboHistory : Form
    {
        private string _outputPath = "";
        public WeiboHistory()
        {
            InitializeComponent();
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            var leaderInfoMapping = GetLeaderInfo();
            var eventInfoMapping = GetEventInfo();
            var result = GetHistoryData(StartDatePicker.Value,EndDatePicker.Value,leaderInfoMapping,eventInfoMapping,(int) MinUpdown.Value, (int) MaxUpdown.Value);
            CalcField(result);
            //照itemID排序

            
            //var targetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "时点数.xlsx");
            //var builder = new ExcelOutputFormatBuilder();
            
           
            var groupedResult = from item in result
                                group item by item.CrawlID
                                into g
                                select g;
            foreach (var groupItem in groupedResult)
            {
                var itemGroup = from item in groupItem
                                group item by item.ItemID
                                into g
                                select g;
                var targetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), groupItem.Key + ".csv");
                var builder = new FilterFieldCsvOutputFormatBuilder(targetPath,",");
               
                int pos = 1;
                foreach (var itemDetail in itemGroup)
                {
                    foreach (var item in itemDetail)
                    {
                        item.ItemID = pos.ToString();
                        BuildOneItem(item, builder);
                    }
                    pos++;

                }
                builder.Output();

                
                
                
                using (PalasDB db = new PalasDB())
                {
                    
                    var printCountPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), groupItem.Key + "search.csv");
                    var logBuilder = new CsvOutputFormatBuilder(printCountPath, ",");
                    
                   
                    
                    IGrouping<string, DetailResult> item = groupItem;
                    var crawlLogResult = db.CrawlLog.Where(model => model.CrawlID == item.Key && model.CrawlTime >= StartDatePicker.Value && model.CrawlTime <= EndDatePicker.Value).OrderBy(model => model.CrawlTime);
                    foreach (var crawlLog in crawlLogResult)
                    {
                        logBuilder.Build("PrintCount", crawlLog.PrintCount);
                        logBuilder.Build("CrawlTime",crawlLog.CrawlTime.ToString("yyyyMMddHHmmss"));
                        logBuilder.NewLine();
                    }
                    logBuilder.Output();

                }
            }
            /*
            builder.Initialize(targetPath);
            foreach (var item in result)
            {
                BuildOneItem(item, builder);
            }
            builder.Output();
             //*/
            
            MessageBox.Show("导出成功");
        }

        private static void BuildOneItem(DetailResult item, OutputFormatBuilder builder)
        {
            builder.Build("ItemID", item.ItemID);
            builder.Build("ParentItemID", item.ParentItemID);
            builder.Build("CrawlID", item.CrawlID);
            builder.Build("CleanTitle", item.CleanTitle);
            builder.Build("CleanText", item.CleanText);
            builder.Build("Url", item.Url);
            builder.Build("PubDate", item.PubDate);
            builder.Build("FetchTime", item.PubDate);
            builder.Build("ReplyCount", item.ReplyCount);
            builder.Build("ForwardCount", item.ForwardCount);
            builder.Build("ViewCount", item.ViewCount);
            builder.Build("CurrentHistoryFetchTime", item.CurrentHistoryFetchTime);
            builder.Build("KeywordQuery", item.KeywordQuery);
            builder.Build("MediaID", item.MediaID);
            builder.Build("ParentMediaID", item.ParentMediaID);
            builder.Build("MediaName", item.MediaName);
            builder.Build("Channel", item.Channel);
            builder.Build("MediaType", item.MediaType);
            builder.Build("MediaTendency", item.MediaTendency);
            builder.Build("MediaOrganType", item.MediaOrganType);
            builder.Build("MediaElitismType", item.MediaElitismType);
            builder.Build("MediaWeight", item.MediaWeight);
            builder.Build("MediaStyle", item.MediaStyle);
            builder.Build("RegionType", item.RegionType);
            builder.Build("AuthorName", item.AuthorName);
            builder.Build("AuthorID", item.AuthorID);
            builder.Build("AuthorCertificated", item.AuthorCertificated);
            builder.Build("Source", item.Source);
            builder.Build("IsPublicLeader", item.IsPublicLeader);
            LeaderInfo leaderInfo = item.LeaderInfo ?? new LeaderInfo();
            builder.Build("Rank", leaderInfo.Rank);
            builder.Build("Gender", leaderInfo.Gender);
            builder.Build("Job", leaderInfo.Job);
            builder.Build("Age", leaderInfo.Age);
            builder.Build("Follower", leaderInfo.Follower);
            builder.Build("Following", leaderInfo.Following);
            builder.Build("TweetCount", ""); //目前没有对应的微博数
            builder.Build("AvgFoward", leaderInfo.AvgFoward);
            builder.Build("AvgComment", leaderInfo.AvgComment);
            builder.Build("AvgFollowersFollower", leaderInfo.AvgFollowersFollower);
            builder.Build("HasAttachUrl", item.HasAttachUrl);
            builder.Build("AttachUrl", item.AttachUrl);
            builder.Build("GovProcess", item.GovProcess);
            builder.Build("ArticleHot", item.ArticleHot);
            builder.Build("CrawlName", item.CrawlName);

            builder.NewLine();
        }

        private void CalcField(DetailResult[] result)
        {
            //对计算字段赋值
            var grouping = from item in result
                           group item by item.CrawlID
                           into g
                           select g;
            foreach (var grouped in grouping)
            {
                var subGrouping = (from item in grouped
                                  group item by item.ItemID
                                  into g
                                  select g).ToArray();
                var newestItems = (from item in subGrouping
                                   orderby item.Max(model => model.CurrentHistoryFetchTime)
                                   select item.First());
                var compareItem = newestItems.OrderByDescending(model => model.ForwardCount).ToArray();
                var cnt = compareItem.Count();

                int total30Percent = (int) (cnt*0.3);
                int total10Percent = (int)(cnt * 0.1);
                int total5Percent = (int)(cnt * 0.05);
                var total5ItemID = compareItem.Take(total5Percent).Select(model => model.ItemID);
                var total10ItemID = compareItem.Skip(total5Percent).Take(total10Percent - total5Percent).Select(model=>model.ItemID);
                var total30ItemID =
                    compareItem.Skip(total5Percent + total10Percent).Take(total30Percent - total10Percent -
                                                                          total5Percent).Select(model => model.ItemID);

                SetArticleHot(subGrouping, total5ItemID,3);
                SetArticleHot(subGrouping,total10ItemID,2);
                SetArticleHot(subGrouping,total30ItemID,1);
            }


        }

        private static void SetArticleHot(IGrouping<string, DetailResult>[] source, IEnumerable<string> lookupItemID,int value)
        {

            foreach (var itemID in lookupItemID)
            {
                string id = itemID;
                var target = source.FirstOrDefault(model => model.Key == id);
                foreach (var item in target)
                {
                    item.ArticleHot = value;
                }
            }
        }

        private Dictionary<string, FocusEvent> GetEventInfo()
        {
            var targetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "辅助表.xlsx");
            Workbook book = new Workbook();
            book.Open(targetPath);
            var dataSheet = book.Worksheets["Sheet2"];
            int startPos = 1;
            Dictionary<string, FocusEvent> result = new Dictionary<string, FocusEvent>();
            while (!string.IsNullOrEmpty(dataSheet.Cells[startPos, 0].StringValue))
            {
                FocusEvent info = new FocusEvent();
                info.EventName = dataSheet.Cells[startPos, 0].StringValue;
                result.Add(info.EventName, info);
                startPos++;
            }
            return result;

        }

        private Dictionary<string,LeaderInfo> GetLeaderInfo()
        {
            var targetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "辅助表.xlsx");
            Workbook book = new Workbook();
            book.Open(targetPath);
            var dataSheet = book.Worksheets["Sheet1"];
            int startPos = 1;
            Dictionary<string,LeaderInfo> result = new Dictionary<string, LeaderInfo>();
            while(!string.IsNullOrEmpty(dataSheet.Cells[startPos,0].StringValue))
            {
                
                LeaderInfo info = new LeaderInfo();
                info.RealName = dataSheet.Cells[startPos, 0].StringValue;
                info.Gender = dataSheet.Cells[startPos, 1].StringValue;
                info.Age = ParseFrom<int>(dataSheet.Cells[startPos, 2].StringValue);
                info.NickName = dataSheet.Cells[startPos, 3].StringValue;
                info.Job = dataSheet.Cells[startPos, 4].StringValue;
                info.Follower = ParseFrom<int>(dataSheet.Cells[startPos, 5].StringValue);
                info.Following = ParseFrom<int>(dataSheet.Cells[startPos, 6].StringValue);
                info.AvgFoward = ParseFrom<double>(dataSheet.Cells[startPos, 7].StringValue);
                info.AvgComment = ParseFrom<double>(dataSheet.Cells[startPos, 8].StringValue);
                info.AvgFollowersFollower = ParseFrom<double>(dataSheet.Cells[startPos, 9].StringValue);
                info.Rank = ParseFrom<double>(dataSheet.Cells[startPos, 10].StringValue);
                if (!string.IsNullOrEmpty(info.NickName))
                {
                    result.Add(info.NickName, info);
                }
                
                                    
                

                startPos++;
            }
            return result;
        }
        private T? ParseFrom<T>(string source) where T:struct 
        {
            if (typeof(T) == typeof(int))
            {
                int tmpInt;
                if (int.TryParse(source,out tmpInt))
                {
                    object obj = tmpInt;//Warn:装箱发生,效率会悲剧,不可用于对效率有需求的情况;
                   return (T?)obj;
                }
                else
                {
                    return null;
                }
            }
            else if(typeof(T) == typeof(double))
            {
                double tmpInt;
                if (double.TryParse(source, out tmpInt))
                {
                    object obj = tmpInt;//Warn:装箱发生,效率会悲剧,不可用于对效率有需求的情况;
                    return (T?)obj;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw  new Exception(string.Format("不支持当前类型的转型:{0}",typeof(T).FullName));
            }
        }
        private DetailResult[] GetHistoryData(DateTime startDate,DateTime endDate,Dictionary<string, LeaderInfo> leaderInfoMapping,Dictionary<string,FocusEvent> focusEventMapping,int startCnt = 10, int endCnt = 30  )
        {
            
            List<DetailResult> list = new List<DetailResult>();
            string[] crawlIDs;
            using (PalasDB db = new PalasDB())
            {
                crawlIDs = db.Crawl.Where(model => model.CrawlID.StartsWith("Weibo")).Select(model=>model.CrawlID).ToArray();
            }
            crawlIDs = crawlIDs.Where(model => model != "WeiboSubscribe").ToArray();
            foreach(var crawlID in crawlIDs)
            {
                for (int i = startCnt; i < endCnt; i++)
                {
                    //var crawlQuery = Query.Matches("CrawlID", "/Weibo/");
                    var crawlQuery = Query.EQ("CrawlID", crawlID);
                    var query = Query.And(crawlQuery,Query.GTE("FetchTime", startDate), Query.LTE("FetchTime", endDate),Query.Size("CountHistory", i)
                    
                        );


                    //var sort = SortBy.Ascending("PubDate");
                    Item[] result = null;
                    try
                    {
                        result =
                            MongoItemAccess.Items.Find(query).SetFields("ItemID", "CountHistory", "CrawlID", "Url", "CleanTitle", "MediaID",
                                                                        "PubDate", "ParentItemID", "CleanText", "AuthorName", "AuthorID", "AuthorCertificated", "Source", "AttachUrl")
                                //.SetSortOrder(sort)
                                .ToArray();
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                
                

                
                    foreach (var item in result)
                    {
                    
                   
                        DetailResult  dest = new DetailResult();
                        item.CopyTo(ref dest);  //赋值Item中字段
                        //取Media字段
                        using (PalasDB db =new PalasDB())
                        {
                            var media = db.Media.FirstOrDefault(model => model.MediaID == item.MediaID);
                            if (media!=null)
                            {
                                media.CopyTo(ref dest);
                            }
                            var crawl = db.Crawl.FirstOrDefault(model => model.CrawlID == item.CrawlID);
                            if (crawl!=null)
                            {
                                dest.KeywordQuery = crawl.KeywordQuery;
                                dest.CrawlName = crawl.Name;
                            }
                        }
                    
                        //匹配意见领袖
                        LeaderInfo info;
                        bool isPublicLeader = leaderInfoMapping.TryGetValue(dest.AuthorName ?? "", out info);
                        dest.IsPublicLeader  =isPublicLeader ?1:0;
                        if (isPublicLeader)
                        {
                            dest.LeaderInfo = info;
                        }
                        //匹配事件
                        FocusEvent focus;

                        bool isGovProcess = focusEventMapping.TryGetValue(dest.CrawlName ?? "",out focus);
                        dest.GovProcess = isGovProcess? 1 : 0;

                        dest.HasAttachUrl = !string.IsNullOrEmpty(dest.AttachUrl) ? 1 : 0;
                        //赋值不同的嵌套字段
                        foreach (var itemCountData in item.CountHistory)
                        {
                            var clonedDest = dest.SwallowClone();
                            clonedDest.CurrentHistoryFetchTime = itemCountData.FetchTime;
                            clonedDest.ViewCount = itemCountData.ViewCount;
                            clonedDest.ReplyCount = itemCountData.ReplyCount;
                            clonedDest.ForwardCount = itemCountData.ForwardCount;
                            list.Add(clonedDest);
                        }
                   
                    
                    }
                
                }
            }
            return list.ToArray();
        }




        private void WeiboHistory_Load(object sender, EventArgs e)
        {
           
            StartDatePicker.Value = DateTime.Now.AddDays(-7);
            EndDatePicker.Value = DateTime.Now;
            
        }
    }
    internal class DetailResult
    {
        public string CrawlName { get; set; }
        public string KeywordQuery { get; set; }
        public string ParentItemID { get; set; }
        public string CrawlID { get; set; }
        public string ItemID { get; set; }
        public string Url { get; set; }
        public DateTime FetchTime { get; set; }
        public int ViewCount { get; set; }
        public int ReplyCount { get; set; }
        public int ForwardCount { get; set; }
        public string CleanTitle { get; set; }
        public string CleanText { get; set; }
        public DateTime PubDate { get; set; }
        public DateTime CurrentHistoryFetchTime { get; set; }
        public string MediaID { get; set; }
        public string ParentMediaID { get; set; }
        public string MediaName { get; set; }
        public string Channel { get; set; }
        public sbyte MediaType { get; set; }
        public sbyte MediaTendency { get; set; }
        public sbyte MediaOrganType { get; set; }
        public sbyte MediaElitismType { get; set; }
        public int MediaWeight { get; set; }
        public sbyte MediaStyle { get; set; }
        public sbyte RegionType { get; set; }
        public string AuthorName { get; set; }
        public string AuthorID { get; set; }
        public sbyte AuthorCertificated { get; set; }
        public string Source { get; set; }
        public int IsPublicLeader { get; set; }
        public int HasAttachUrl { get; set; }
        public string AttachUrl { get; set; }
        public int GovProcess { get; set; }
        public int ArticleHot { get; set; }
        public LeaderInfo LeaderInfo { get; set; }
        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns>拷贝后的副本</returns>
        public DetailResult SwallowClone()
        {
            return (DetailResult)MemberwiseClone();
        }
    }
    internal class LeaderInfo
    {
        public string RealName { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public string NickName { get; set; }
        public string Job { get; set; }
        public int? Follower { get; set; }
        public int? Following { get; set; }
        public double? AvgFoward { get; set; }
        public double? AvgComment { get; set; }
        public double? AvgFollowersFollower { get; set; }
        public double? Rank { get; set; }
    }
    internal class FocusEvent
    {
        public string EventName { get; set; }
    }

    /// <summary>
    /// 用于将数据转换到目标格式的抽象类
    /// </summary>
    internal abstract class OutputFormatBuilder
    {
        /// <summary>
        /// 目标保存路径
        /// </summary>
        protected string TargetPath;
        /// <summary>
        /// 当前的行号
        /// </summary>
        protected int RowPos = 0;
        /// <summary>
        /// 当前的列号
        /// </summary>
        protected int ColPos = 0;

        
        public OutputFormatBuilder(string targetPath)
        {
            TargetPath = targetPath;
        }
        

        /// <summary>
        ///  全部Builder完之后用于保存到特定路径的方法
        ///  </summary>
        public abstract void Output();

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public abstract void Build(string fieldName, string fieldValue);

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public virtual void Build(string fieldName, DateTime? fieldValue)
        {
            Build(fieldName,fieldValue==null?"":fieldValue.ToString());
        }

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public virtual void Build(string fieldName, int? fieldValue)
        {
            Build(fieldName, fieldValue == null ? "" : fieldValue.ToString());
        }

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public virtual void Build(string fieldName, double? fieldValue)
        {
            Build(fieldName, fieldValue == null ? "" : fieldValue.ToString());
        }

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public virtual void Build(string fieldName, decimal? fieldValue)
        {
            Build(fieldName, fieldValue == null ? "" : fieldValue.ToString());
        }
        /// <summary>
        ///  用于表示新输出行
        ///  </summary><param name="rowNum">需要换行的次数</param>
        public virtual void NewLine(int rowNum = 1)
        {
            RowPos = RowPos + rowNum;
        }

        /// <summary>
        ///  用于控制跳过的列数
        ///  </summary><param name="colNum">需要跳过的列的次数</param>
        public virtual void SkipColumn(int colNum = 1)
        {
            ColPos = ColPos + colNum;
        }

        /// <summary>
        ///  用于跳转到指定的行
        ///  </summary><param name="rowPos">需要跳转的行号</param>
        public virtual void SeekRow(int rowPos)
        {
            RowPos = rowPos;
        }

        /// <summary>
        ///  用于跳转到指定的列
        ///  </summary><param name="colPos">需要跳转的列号</param>
        public virtual void SeekColumn(int colPos)
        {
            ColPos = colPos;
        }
    }

    internal class ExcelOutputFormatBuilder:OutputFormatBuilder
    {
        #region 私有字段

        
        private Workbook _book;
        
        private Worksheet _dataSheet;

        #endregion
        #region Implementation of IOutputFormatBuilder

        public ExcelOutputFormatBuilder(string targetPath) : base(targetPath)
        {
            _book = new Workbook();
            _book.Open(targetPath);
            _dataSheet = _book.Worksheets["Sheet1"];
            RowPos = 1;
            //清除数据
            while (!string.IsNullOrEmpty(_dataSheet.Cells[RowPos, 0].StringValue))
            {
                RowPos++;
            }
            _dataSheet.Cells.DeleteRows(1, RowPos, true);
            //重置行数
            RowPos = 1;
        }

        

        /// <summary>
        /// 全部Builder完之后用于保存到特定路径的方法
        /// </summary>
        public override void Output()
        {
            
            _book.Save(TargetPath);
        }

        /// <summary>
        /// 用于构建一个字段的输出
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        public override void Build(string fieldName, string fieldValue)
        {
            _dataSheet.Cells[RowPos, ColPos++].PutValue(fieldValue); 
        }
        
        

        #endregion
    }
    internal class CsvOutputFormatBuilder:OutputFormatBuilder
    {
        protected string SepStr = ",";
        protected StreamWriter Stream;

        #region Overrides of OutputFormatBuilder

        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetPath">目标路径</param>
        /// <param name="sepStr">分隔符</param>
        public CsvOutputFormatBuilder(string targetPath, string sepStr)
            : base(targetPath)
        {
            SepStr = sepStr;
            var fs = new FileStream(targetPath, FileMode.Create);
            Stream = new StreamWriter(fs);
        }
        
        /// <summary>
        ///  全部Builder完之后用于保存到特定路径的方法
        ///  </summary>
        public override void Output()
        {
            Stream.Flush();   
            Stream.Close();
        }

        /// <summary>
        ///  用于构建一个字段的输出
        ///  </summary><param name="fieldName">字段名</param><param name="fieldValue">字段值</param>
        public override void Build(string fieldName, string fieldValue)
        {
            Stream.Write(fieldValue);
            Stream.Write(SepStr);
        }
        public override void NewLine(int rowNum = 1)
        {
            base.NewLine(rowNum);
            for (int i = 0; i < rowNum;i++ )
            {
                Stream.WriteLine();
            }
                
        }
        public override void SeekColumn(int colPos)
        {
            throw new NotSupportedException("流媒体暂时不支持此操作");
        }
        
        public override void SeekRow(int rowPos)
        {
            throw new NotSupportedException("流媒体暂时不支持此操作");
        }
        public override void SkipColumn(int colNum = 1)
        {
            base.SkipColumn(colNum);
            for (int i = 0; i < colNum; i++)
            {
                Stream.Write(SepStr);
            }
        }
        #endregion
    }
    /// <summary>
    /// 仅用于将特定字段导入Csv的输出类
    /// </summary>
    internal class FilterFieldCsvOutputFormatBuilder:CsvOutputFormatBuilder
    {

        private readonly string[] validFieldName = { "ItemID", "CurrentHistoryFetchTime", "ForwardCount", "ReplyCount" };

        public FilterFieldCsvOutputFormatBuilder(string targetPath, string sepStr) : base(targetPath, sepStr)
        {

        }

        public override void Build(string fieldName, string fieldValue)
        {
            if (ValidField(fieldName))
            {
                base.Build(fieldName,fieldValue);
            }


        }

        private bool ValidField(string fieldName)
        {
            return validFieldName.Contains(fieldName);

        }

        public override void Build(string fieldName, DateTime? fieldValue)
        {
            string resultStr = "";
            if (fieldValue !=null)
            {
                resultStr = ((DateTime)fieldValue).ToString("yyyyMMddHHmmss");
            }
            Build(fieldName,resultStr);
        }
        
    }

    
}

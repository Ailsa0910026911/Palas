using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using Crawler.Core.Data;
using Palas.Common.Data;
using Palas.Common.DataAccess;
using Palas.Common.Module;

namespace Crawler.Host
{
    public partial class CrawlTool : Form
    {

        public CrawlTool()
        {
            InitializeComponent();
        }
        string _msgFormat = "正在抓取{0},关键词:{1},第{2}页,第{3}条数据";
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            StatusLbl.Text = "开始抓取";
            tabControl1.Enabled = false;
            Application.DoEvents();


            var siteid = SearchForumList.SelectedValue.ToString();
            var keyword = keywordTxt.Text.Trim();
            var startDate = startDateTime.Value;
            var maxCnt = (int)maxPageNum.Value;
            var resultList = CrawlOneKeyword(siteid, keyword, startDate, maxCnt,
                (name, word, currentPageNum, currentCnt) =>
                {
                    string showMsg = string.Format(_msgFormat, name, word, currentPageNum, currentCnt);
                    StatusLbl.Text = showMsg;
                    Application.DoEvents();
                });

            //绑定数据
            var bindingData = (from item in resultList
                               orderby item.Data.PubDate descending
                               select
                                   new
                                       {

                                           Url = item.Data == null ? "" : item.Data.Url,
                                           MediaName = item.Data == null ? item.SiteName : string.IsNullOrEmpty(item.Data.ReproducedMediaName) ? item.SiteName : item.Data.ReproducedMediaName,
                                           Title = item.Data == null ? "" : item.Data.CleanTitle,
                                           PubDate = item.Data == null ? (DateTime?)null : item.Data.PubDate,
                                           Author = item.Data == null ? "" : item.Data.AuthorName,
                                           View = item.Count == null ? (int?)null : item.Count.ViewCount,
                                           Comment = item.Count == null ? (int?)null : item.Count.ReplyCount
                                       }).ToArray();
            ResultDataGridView.DataSource = bindingData;
            tabControl1.Enabled = true;
            Application.DoEvents();
            MessageBox.Show("抓取完成");



        }

        private List<ResultResponse> CrawlOneKeyword(string siteID, string keyword, DateTime startDate, int maxCnt, Action<string, string, int, int> CallBack)
        {
            Crawler.Core.Crawler.SimpleCrawler oneJob = new Core.Crawler.SimpleCrawler("test",
                                                                           new Scheduler("nothing", new SchedulerSetting()));

            var crawlid = "";
            var pageCnt = 0;
            var siteName = "";
            using (PalasDB db = new PalasDB())
            {
                var crawl = db.Crawl.FirstOrDefault(model => model.SiteID == siteID);
                crawlid = crawl.CrawlID;
                pageCnt = crawl.Site.ListItemCountPerPage;
                siteName = crawl.Site.Name;
            }
            List<ResultResponse> resultList = new List<ResultResponse>();
            var needPage = (int)(maxCnt / pageCnt) + 1;

            for (int pageNum = 0; pageNum < needPage; pageNum++)
            {
                var result = oneJob.CrawlOnePageList(crawlid, keyword, pageNum, "", true);
                if (result != null && result.Items != null)
                {
                    var resultItem = from item in result.Items
                                     select
                                         new ResultResponse { Count = item.DataItemCount, Data = item.DataItem, SiteName = siteName,Keyword = keyword};
                    //如果时间已经过了特定时间段则直接返回
                    resultItem = resultItem.Where(model => model.Data != null).ToArray();
                    var expireCnt = resultItem.Count(model => model.Data.PubDate < startDate);
                    if (expireCnt > 0)
                    {
                        //如果存在过期项目则特殊处理
                        resultItem = resultItem.Where(model => model.Data.PubDate >= startDate);
                        resultList.AddRange(resultItem);
                        return resultList;
                    }
                    resultList.AddRange(resultItem);
                }
                if (CallBack != null)
                {
                    CallBack(siteName, keyword, pageNum + 1, pageNum * pageCnt);
                }
                
                

               
            }
           
           
            return resultList;
        }

        private void CrawlTool_Load(object sender, EventArgs e)
        {
            Initailize();

        }

        private void Initailize()
        {
            //绑定搜索列表
            using (PalasDB db = new PalasDB())
            {
                var result = db.Site.Where(model => model.Media.MediaType == (sbyte)Enums.MediaType.SearchForum).ToArray();

                SearchForumList.DataSource = result;
                SearchForumList.DisplayMember = "Name";
                SearchForumList.ValueMember = "SiteID";

                SearchChkList.DataSource = result;
                SearchChkList.DisplayMember = "Name";
                SearchChkList.ValueMember = "SiteID";
            }
            //初始化Grid
            ResultDataGridView.AutoGenerateColumns = false;
            SearchResultDataGridView.AutoGenerateColumns = false;
            //初始化限定时间
            startDateTime.Value = DateTime.Now.AddMonths(-1);
            BatchStartDateTime.Value = DateTime.Now.AddMonths(-1);
            //暂时移除第二个选项卡
            //tabControl1.TabPages.RemoveAt(1);
        }

        private void ResultDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                if (ResultDataGridView.Rows[e.RowIndex].DataBoundItem != null)
                {
                    dynamic data = ResultDataGridView.Rows[e.RowIndex].DataBoundItem;
                    var url = data.Url;
                    Process.Start(url);
                }


            }
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (ResultDataGridView.Rows.Count == 0 || ResultDataGridView.Rows[0].DataBoundItem == null)
            {
                MessageBox.Show("请先搜索才能导出");
                return;
            }
            var path = Application.ExecutablePath;
            var folderPath = Path.GetDirectoryName(path);
            var filename = ((dynamic)SearchForumList.SelectedItem).Name + keywordTxt.Text.Trim() + ".xlsx";
            var outputPath = Path.Combine(folderPath, filename);
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            //设置表头
            sheet.Cells[0, 0].PutValue("日期");
            sheet.Cells[0, 1].PutValue("媒体");
            sheet.Cells[0, 2].PutValue("标题");
            sheet.Cells[0, 3].PutValue("作者");
            sheet.Cells[0, 4].PutValue("点击");
            sheet.Cells[0, 5].PutValue("评论");
            //设置数据
            int rowPos = 1;
            foreach (DataGridViewRow row in ResultDataGridView.Rows)
            {
                if (row.DataBoundItem != null)
                {
                    dynamic data = row.DataBoundItem;
                    sheet.Cells[rowPos, 0].PutValue(data.PubDate.ToString());
                    sheet.Cells[rowPos, 1].PutValue(data.MediaName);
                    sheet.Cells[rowPos, 2].PutValue(data.Title);
                    sheet.Hyperlinks.Add(rowPos, 2, 1, 1, data.Url);
                    var style = sheet.Cells[rowPos, 2].GetStyle();
                    style.Font.Underline = FontUnderlineType.Single;
                    style.Font.Color = Color.Blue;
                    sheet.Cells[rowPos, 2].SetStyle(style);
                    sheet.Cells[rowPos, 3].PutValue(data.Author);
                    sheet.Cells[rowPos, 4].PutValue(data.View);
                    sheet.Cells[rowPos, 5].PutValue(data.Comment);
                    rowPos++;
                    //显示信息
                    StatusLbl.Text = "正在处理第" + rowPos + "条数据";
                    Application.DoEvents();
                }
            }
            book.Save(outputPath);
            MessageBox.Show("导出成功,文件存储在当前程序目录下");

        }
        private void ParallelCrawl()
        {
            this.Invoke(new Action(()=>
                                       {
                                           BatchStatusLbl.Text = "开始抓取";
                                           tabControl1.Enabled = false;
                                           SearchResultDataGridView.DataSource = null;
                                       }));
            
            List<ResultResponse> resultData = new List<ResultResponse>();
            var selectedSites = SearchChkList.CheckedItems.Cast<Site>().ToArray();

            Parallel.ForEach(selectedSites, site =>
            {
                foreach (string line in BatchKeywordTxt.Lines.Where(model => !string.IsNullOrEmpty(model.Trim())))
                {
                    var siteid = site.SiteID;
                    var keyword = line.Trim();
                    var startDate = BatchStartDateTime.Value;
                    var maxCnt = (int)BatchLimitNum.Value;
                    var resultList = CrawlOneKeyword(siteid, keyword, startDate, maxCnt,
                        (name, word, currentPageNum, currentCnt) =>
                        {
                            string showMsg = string.Format(_msgFormat, name, word, currentPageNum, currentCnt);

                            this.Invoke(new Action(() =>
                            {
                                BatchStatusLbl.Text = showMsg; Application.DoEvents();
                            }));


                        });
                    lock (_syncObj)
                    {
                        resultData.AddRange(resultList);
                    }



                }
            });

            //绑定数据

            var bindingData = (from item in resultData
                               orderby item.Data.PubDate descending
                               select
                                   new
                                   {

                                       Url = item.Data == null ? "" : item.Data.Url,
                                       MediaName = item.Data == null ? item.SiteName : string.IsNullOrEmpty(item.Data.ReproducedMediaName) ? item.SiteName : item.Data.ReproducedMediaName,
                                       Title = item.Data == null ? "" : item.Data.CleanTitle,
                                       PubDate = item.Data == null ? (DateTime?)null : item.Data.PubDate,
                                       Author = item.Data == null ? "" : item.Data.AuthorName,
                                       View = item.Count == null ? (int?)null : item.Count.ViewCount,
                                       Comment = item.Count == null ? (int?)null : item.Count.ReplyCount,
                                       Keyword = item.Keyword,
                                       SearchEngine = item.SiteName
                                   }).ToArray();
            this.Invoke(new Action(()=>
                                       {
                                           SearchResultDataGridView.DataSource = bindingData;
                                           tabControl1.Enabled = true;
                                           tabControl2.SelectTab(1);
                                           MessageBox.Show("抓取完成");
                                       }));
            
        }
        private readonly object _syncObj = new object();
        private void BatchSearchBtn_Click(object sender, EventArgs e)
        {
            
            Thread thread = new Thread(ParallelCrawl);
            thread.Start();
            


        }

        private void BatchExportBtn_Click(object sender, EventArgs e)
        {
            if (SearchResultDataGridView.Rows.Count == 0 || SearchResultDataGridView.Rows[0].DataBoundItem == null)
            {
                MessageBox.Show("请先搜索才能导出");
                return;
            }
            var path = Application.ExecutablePath;
            var folderPath = Path.GetDirectoryName(path);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "综合搜索" + ".xlsx";
            var outputPath = Path.Combine(folderPath, filename);
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            //设置表头
            sheet.Cells[0, 0].PutValue("日期");
            sheet.Cells[0, 1].PutValue("媒体");
            sheet.Cells[0, 2].PutValue("标题");
            sheet.Cells[0, 3].PutValue("作者");
            sheet.Cells[0, 4].PutValue("点击");
            sheet.Cells[0, 5].PutValue("评论");
            sheet.Cells[0,6].PutValue("关键词");
            sheet.Cells[0,7].PutValue("搜索引擎");
            //设置数据
            int rowPos = 1;
            foreach (DataGridViewRow row in SearchResultDataGridView.Rows)
            {
                if (row.DataBoundItem != null)
                {
                    dynamic data = row.DataBoundItem;
                    sheet.Cells[rowPos, 0].PutValue(data.PubDate.ToString());
                    sheet.Cells[rowPos, 1].PutValue(data.MediaName);
                    sheet.Cells[rowPos, 2].PutValue(data.Title);
                    sheet.Hyperlinks.Add(rowPos, 2, 1, 1, data.Url);
                    var style = sheet.Cells[rowPos, 2].GetStyle();
                    style.Font.Underline = FontUnderlineType.Single;
                    style.Font.Color = Color.Blue;
                    sheet.Cells[rowPos, 2].SetStyle(style);
                    sheet.Cells[rowPos, 3].PutValue(data.Author);
                    sheet.Cells[rowPos, 4].PutValue(data.View);
                    sheet.Cells[rowPos, 5].PutValue(data.Comment);
                    sheet.Cells[rowPos,6].PutValue(data.Keyword);
                    sheet.Cells[rowPos,7].PutValue(data.SearchEngine);
                    rowPos++;
                    //显示信息
                    BatchStatusLbl.Text = "正在处理第" + rowPos + "条数据";
                    Application.DoEvents();
                }
            }
            book.Save(outputPath);
            MessageBox.Show("导出成功,文件存储在当前程序目录下");
        }
    }
    public class ResultResponse
    {

        public string SiteName { get; set; }
        public RawItem Data { get; set; }
        public ItemCountData Count { get; set; }
        public string Keyword { get; set; }
    }
}

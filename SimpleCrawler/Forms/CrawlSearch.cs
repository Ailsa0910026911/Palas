using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using Crawler.Core.Data;
using Crawler.Core.Utility;
using Palas.Common.Data;
using Palas.Common.Lib.Business;
using Palas.Common.Lib.Entity;
using Palas.Common.Utility;

namespace Crawler.Host
{
    public partial class CrawlSearch : Form
    {
        [Serializable]
        private class AnalyzeData
        {
            public Item[] Items { get; set; }
            public string CrawlID { get; set; }
        }

        private class KeywordQuery
        {
            public string Keyword { get; set; }
            public int StartPage { get; set; }
            public int EndPage { get; set; }

            public override string ToString()
            {
                return string.Format("{0}({1}页----{2}页)", Keyword, StartPage, EndPage);
            }
        }

        public CrawlSearch()
        {
            InitializeComponent();
        }

        private void Search(SiteEntity siteEntity, KeywordQuery keywordQuery, List<AnalyzeData> resultDataList, int ProgressPercStart, int ProgressPercEnd)
        {
            var firstCrawl = CrawlBusiness.GetTopBySiteID(siteEntity.SiteID, "");
            var crawlID = firstCrawl.CrawlID;
            string lastItemID = null;
            var keyword = keywordQuery.Keyword;
            var keywordExclude = "";
            var startPage = keywordQuery.StartPage;
            var endPage = keywordQuery.EndPage;
            var crawl = CrawlBusiness.GetByCrawlID(crawlID);
            var site = SiteBusiness.GetBySiteID(crawl.SiteID);
            ListResponse result = null;
            for (int currentPage = startPage; currentPage <= endPage; currentPage++)
            {
                CrawlRequest request = CrawlRequest.GetQueryUrl(crawlID, keyword, currentPage, keywordExclude, "", "");
                crawl.KeywordQuery = keyword;
                crawl.KeywordAny = "";
                crawl.KeywordNot = keywordExclude;
                crawl.KeywordSite = "";
                var response = Core.Crawler.SimpleCrawler.CrawlList_Single(request, crawl, site, true, null);
                var currentItems = response.ExtractItems(crawl);
                if (!(currentItems == null || !currentItems.Any() || currentItems.Last().ItemID == lastItemID))
                {
                    lastItemID = currentItems.Last().ItemID;
                    //bool stopCrawl;
                    //ExistCheck.ExistCheck_List(
                    //                           response, null,
                    //                           (Enums.ExistItemStrategy) crawl.ExistItemStrategy,
                    //                           (Enums.ContentDetailLevel) site.ContentDetailLevel, crawl.IssueID,
                    //                           crawl.CrawlID, out stopCrawl);
                    if (result == null)
                        result = response;
                    else
                        result.CombineList(response);
                }

                backgroundWorker1.ReportProgress(ProgressPercStart + (ProgressPercEnd - ProgressPercStart) * (currentPage + 1) / (endPage - startPage + 1));
            }

            //Get Item
            if (result != null)
            {
                if (DetailChk.Checked)
                {
                    this.Text = Site.Name + " 抓取Items";
                    Core.Crawler.SimpleCrawler.CrawlItem_Multi(
                                                     result, crawl, site, msg =>
                                                     {
                                                     });
                    this.Text = @"Palas搜索工具";
                }
                
                var items = result.ExtractItems(crawl, null);
                AnalyzeData data = new AnalyzeData()
                                       {
                                           Items = items,
                                           CrawlID = crawl.CrawlID
                                       };
                resultDataList.Add(data);
            }
        }

        private void AddKeywordBtn_Click(object sender, EventArgs e)
        {
            var keyword = KeywordTxt.Text.Trim();
            var startPage = int.Parse(StartPageTxt.Text);
            var endPage = int.Parse(EndPageTxt.Text);
            KeywordQuery query = new KeywordQuery()
                                     {
                                         EndPage = endPage,
                                         Keyword = keyword,
                                         StartPage = startPage
                                     };
            KeywordListbox.Items.Add(query);
            KeywordTxt.Text = "";
        }

        private void AnalyzeBtn_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("RawItem.bry", FileMode.Open);
            var dataList = bf.Deserialize(fs) as AnalyzeData[];
            foreach (var data in dataList)
            {
                var items = data.Items;
                var crawlID = data.CrawlID;

                var crawl = CrawlBusiness.GetByCrawlID(crawlID);
                foreach (var item in items)
                {
                    Analyzer.Core.Analyzer.AnalyzeItem(item, null, crawl);
                }
            }
        }

        private void CrawlBtn_Click(object sender, EventArgs e)
        {
            //this.Enabled = false;
            progressBar1.Visible = true;

            backgroundWorker1.RunWorkerAsync();
        }

        private void CrawlSearch_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件|*.xlsx|Excel 2003文件|*.xls";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = saveFileDialog.FileName;
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream("RawItem.bry", FileMode.Open);
                var dataList = bf.Deserialize(fs) as AnalyzeData[];
                var excelItems = from list in dataList
                                 from item in list.Items
                                 select
                                     new
                                         {
                                             Title = item.CleanTitle,
                                             Text = item.CleanText,
                                             MediaName = item.ReproducedMediaName,
                                             Url=item.Url,
                                             PubDate = item.PubDate,
                                             Forward = item.CurrentCount == null?0:item.CurrentCount.ForwardCount,
                                             Reply = item.CurrentCount == null ? 0 : item.CurrentCount.ReplyCount,
                                             View = item.CurrentCount == null ? 0 : item.CurrentCount.ViewCount,
                                         };
                Workbook book =new Workbook();
                var sheet = book.Worksheets.Add("结果");
                ExcelUtility.FillCollection(sheet,excelItems);
                book.Save(fileName);
                fs.Dispose();
            }
            MessageBox.Show("导出成功");
        }

        private void Init()
        {
            var sites = SiteBusiness.GetByWhere("(ContentType=4 OR ContentType=5) AND IsVisible=True", null, "", 0, 1000);
            //var filterSites = from site in sites
            //                  let media = site.Media
            //                  where media.MediaType > 20
            //                  select site;
            SearchEngineChkList.DataSource = sites.OrderBy(s => s.Name).ToArray();
            SearchEngineChkList.DisplayMember = "Name";
            SearchEngineChkList.ValueMember = "SiteID";

            StartDate.Value = DateTime.Now.AddDays(-180);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var checkSites = SearchEngineChkList.CheckedItems;
            List<AnalyzeData> resultDataList = new List<AnalyzeData>();

            var keywordQueryList = KeywordListbox.Items.Cast<KeywordQuery>().ToArray();

            int JobCount = 0;
            foreach (SiteEntity siteEntity in checkSites)
            {
                foreach (var keywordQuery in keywordQueryList)
                {
                    Search(siteEntity, keywordQuery, resultDataList, JobCount * 100 / checkSites.Count, (JobCount + 1) * 100 / checkSites.Count);
                    JobCount++;
                }
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("RawItem.bry", FileMode.Create);
            var dataArray = resultDataList.ToArray();
            bf.Serialize(fs, dataArray);
            fs.Dispose();
            MessageBox.Show("全部运行完成");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(string.Format("发生错误:{0}", e.Error.Message));
            
            //this.Enabled = true;
            progressBar1.Visible = false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;            
        }
    }
}
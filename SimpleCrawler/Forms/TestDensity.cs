using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Crawler.Core.Parser;
using Crawler.Core.RequestProcessor;
using Palas.Common.Lib.Entity;
using Crawler.Core.Data;
using Palas.Common.Data;
using Aspose.Cells;

namespace Crawler.Host
{
    public partial class TestDensity : Form
    {
        public TestDensity()
        {
            InitializeComponent();
        }

        private void ParseListBtn_Click(object sender, EventArgs e)
        {
            string url = InputUrlTxt.Text;
            string content = "";
            RecogniseMode mode = DeterminedMode();
            var xpath = new ListPageXPaths();
            PageElement[] result;
            if (GeckoDownRd.Checked)
            {
                //result = new GeckoParser().AnalyzeArticleList(url,mode,out xpath,86400);
                CrawlResponse resp = GeckoRequestProcessor.DoRequest(BuildFakeRequest(url), BuildFakeSiteEntity(), null, null, null, true, 1000);
                content = resp.Content;
                var ret = PageAutoAnalyzer.AnalyzeArticleList(resp.Url, content, mode, new IdentityPageElement(), ref xpath, 86400);
                result = ret == null ? null : ret.List;
            }
            else if (HttpdownRd.Checked)
            {
                content = WebRequestProcessor.DownloadHTTPString(url, 30);
                var ret = PageAutoAnalyzer.AnalyzeArticleList(url, content, mode, new IdentityPageElement(), ref xpath, 86400);
                result = ret == null ? null : ret.List;
            }
            else
            {
                throw new NotSupportedException("不支持当前项抓取");
            }
            
            
             
            if (result == null)
            {
                MessageBox.Show("解析不出数据");
                return;
            }
            foreach (var pageElement in result)
            {

                pageElement.Url = HtmlUtility.ExpandRelativePath(url, pageElement.Url);
            }
            ListGridView.DataSource = result;
        }

        private RecogniseMode DeterminedMode()
        {
            RecogniseMode mode;
            if (NewsRd.Checked)
            {
                mode = RecogniseMode.News;
            }
            else
            {
                mode = RecogniseMode.Forum;
            }
            return mode;
        }

        private void ParsePageBtn_Click(object sender, EventArgs e)
        {
            string url = InputUrlTxt.Text;
            string title = InputTitleTxt.Text;
            ParsePage(title,url);
        }

        

        private void ListGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ListGridView.Columns["ParsePageCol"].Index)
            {
                string url =  ListGridView.Rows[e.RowIndex].Cells["Url"].Value.ToString();
                string title = ListGridView.Rows[e.RowIndex].Cells["Title"].Value.ToString();
                var pageElement = (PageElement)ListGridView.Rows[e.RowIndex].DataBoundItem;
                ParsePage(title, url,pageElement);
            }

        }

        private void ParsePage(string title, string url,PageElement pageElement= null)
        {
            if (pageElement == null)
            {
                pageElement = new PageElement {Title = title, Url = url};
            }
            
            var xpath = new ItemPageXPaths();
            List<SubItemElement> subList;
            DateTime startTime = DateTime.Now;
            PageElement result;
            if (GeckoDownRd.Checked)
            {
                //result = new GeckoParser().GetArticleContent(url, title, DeterminedMode(), out xpath);
                CrawlResponse resp = GeckoRequestProcessor.DoRequest(BuildFakeRequest(url), BuildFakeSiteEntity(), null, null, null, true, 1000);
                string content = resp.Content;
                result = PageAutoAnalyzer.AnalyzeContent(content, pageElement,
                                                                     DeterminedMode(), new IdentityContentElement(), ref xpath,
                                                                     out subList, 86400, ExcludeTxt.Text);
            }
            else if (HttpdownRd.Checked)
            {
                string content = WebRequestProcessor.DownloadHTTPString(url, 30);
                result = PageAutoAnalyzer.AnalyzeContent(content, pageElement,
                                                                     DeterminedMode(), new IdentityContentElement(), ref xpath,
                                                                     out subList, 86400, ExcludeTxt.Text);
            }
            else
            {
                throw new Exception("不支持该方式分析正文");
            }
            
            
            TimeSpan usedTime = DateTime.Now - startTime;

            if (result == null)
            {
                return;
            }
            PageUrlTxt.Text = HtmlUtility.ExpandRelativePath(url, result.Url); 
            TitleTxt.Text = result.Title;
            ContentTxt.Text = result.Content;
            ViewTxt.Text = result.View.ToString();
            ReplyTxt.Text = result.Reply.ToString();
            PubdateTxt.Text = result.Pubdate == null ? "" : result.Pubdate.ToString();
            AuthorTxt.Text = result.Author;
            MediaTxt.Text = result.MediaName;
            ElementXPathTxt.Text = result.ElementXPath;
            ElementBlockTxt.Text = result.ElementBlock;
            NextpageXPathTxt.Text = result.NextPageXPath;
        }

        private SiteEntity BuildFakeSiteEntity()
        {
            SiteEntity site = new SiteEntity();
            site.SiteID = "TestSiteChris";
            site.Status = 0;
            site.IntervalMSBtwReqs = 0;
            site.TimeoutSecs = 30;
            site.ListUrlMethod = 0;

            return site;
        }

        private CrawlRequest BuildFakeRequest(String url)
        {
            CrawlRequest req = new CrawlRequest(
                Enums.PageType.List,
                "Test",
                "TestCrawlChris",
                "TestSiteChris",
                url,
                null, 4, null, true);

            //req.MustMatchRegex = "外经贸企业违规将上黑名单";
            //req.MustMatchRegex = mustmatch;

            return req;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.CheckPathExists = true;
            save.OverwritePrompt = true;
            save.Filter = "Excel (*.xlsx)|*.xlsx";

            var ok = save.ShowDialog(this);

            if (ok == System.Windows.Forms.DialogResult.OK)
            {
                IEnumerable<PageElement> list = ListGridView.DataSource as IEnumerable<PageElement>;

                if (list != null)
                {
                    ExportToExcel(save.FileName, list);
                }
                else
                {
                    MessageBox.Show("Data source cast error!");
                }
            }
        }

        private void ExportToExcel(String file, IEnumerable<PageElement> list)
        {
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];

            Int32 index = 0;
            foreach (var pe in list)
            {
                sheet.Cells[index, 0].PutValue(pe.Title);
                sheet.Cells[index, 1].PutValue(pe.Url);
                index++;
            }

            book.Save(file);
        }
    }
}

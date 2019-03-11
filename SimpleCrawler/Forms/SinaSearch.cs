using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using Crawler.Core.Utility;
using Palas.Common.Data;
using Palas.Common.Utility;
using System.Threading;
using HooLab.Log;

namespace Crawler.Host
{
    public partial class SinaSearch : Form
    {
        public SinaSearch()
        {
            InitializeComponent();
        }
        internal class WeiboSearchQuery
        {
            public string Keyword { get; set; }
            public int StartPage { get; set; }
            public int EndPage { get; set; }
            public bool? SearchAll { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool? isOrigin { get; set; }
            public bool? isVip { get; set; }
            public SearchWeiboOption? Option { get; set; }
            public string AddtionQuery { get; set; }
            public override string ToString()
            {
                string listStr = SearchAll == true ?
                    String.Format("关键词:{0},抓取所有", Keyword) :
                    string.Format("关键词:{0},页码:{1}---{2}",Keyword,StartPage,EndPage);
                if (StartDate != null && EndDate != null)
                {
                    listStr += string.Format(
                                             ",日期:{0}---{1}", StartDate.Value.ToString("yyyy-MM-dd-HH"),
                                             EndDate.Value.ToString("yyyy-MM-dd-HH"));
                }
                if (isOrigin == true)
                {
                    listStr += ",原创";
                }
                if (isVip == true)
                {
                    listStr += ",认证用户";
                }
                if (Option != null)
                {
                    switch (Option)
                    {
                            case SearchWeiboOption.Hot:
                            {
                                listStr += ",热门排序";
                                break;
                            }
                            case SearchWeiboOption.RealTime:
                            {
                                listStr += ",实时排序";
                                break;
                            }
                    }
                }
                return listStr;
            }
        }
        internal class PeopleSearchQuery
        {
            
            public string Keyword { get; set; }
            public int StartPage { get; set; }
            public int EndPage { get; set; }
            
            public SearchCert Cert { get; set; }
            public SearchAge Age { get; set; }
            public SearchGender Gender { get; set; }
            public SearchAttr Attr { get; set; }
            public string AddtionQuery { get; set; }
            public override string ToString()
            {
                string listStr = string.Format("关键词:{0},页码:{1}---{2}", Keyword, StartPage, EndPage);
                
                
                return listStr;
            }
            
        }
        private void AddQueryBtn_Click(object sender, EventArgs e)
        {
            var query = new WeiboSearchQuery()
                            {
                                Keyword = KeywordTxt.Text.Trim(),
                                StartPage = (int) StartPageNum.Value,
                                EndPage = (int) EndPageNum.Value,
                                isVip = VipChk.Checked,
                                isOrigin = OriginChk.Checked,
                                SearchAll = searchAllChk.Checked,
                            };
            DateTime? startDate = null;
            DateTime? endDate = null;

            try
            {
                if (!string.IsNullOrEmpty(StartDateTxt.Text) && !string.IsNullOrEmpty(EndDateTxt.Text))
                {
                    startDate = DateTime.Parse(StartDateTxt.Text);
                    endDate = DateTime.Parse(EndDateTxt.Text);
                }
                
            }
            catch (Exception)
            {

                MessageBox.Show("日期格式错误");
                return;
            }
            query.StartDate = startDate;
            query.EndDate = endDate;
            
            if (query.SearchAll == false)
            {
                var selectedSort = SortDropDown.SelectedItem as String;
                switch (selectedSort)
                {
                    case "实时":
                        {
                            query.Option = SearchWeiboOption.RealTime;
                            break;
                        }
                    case "热门":
                        {
                            query.Option = SearchWeiboOption.Hot;
                            break;
                        }
                    default:
                        {
                            query.Option = SearchWeiboOption.Default;
                            break;
                        }
                }
            }
            else
            {
                query.Option = SearchWeiboOption.RealTime;
            }
            query.AddtionQuery = AddtionQueryTxt.Text;
            WeiboSearchKeywordListbox.Items.Add(query);
            KeywordTxt.Text = "";
        }

        private void SinaSearch_Load(object sender, EventArgs e)
        {
            SortDropDown.SelectedIndex = 0;
        }

        private void SearchWeiboBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            Int32 saveCountDown = 1;

            dialog.Filter = "Excel文件|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Workbook book = new Workbook();
                    var querys = WeiboSearchKeywordListbox.Items.Cast<WeiboSearchQuery>().ToArray();
                    foreach (var query in querys)
                    {
                        int count = 0;
                        Worksheet sheet = book.Worksheets.Add(query.Keyword);

                        var composer = QueryComposerFactory.GetQueryComposer(query);
                        Boolean appendExcel = false;

                        while (!composer.Empty)
                        {
                            var weiboSearchQuery = composer.Next();

                            List<Item> _items = new List<Item>(); 
                            Item[] items = null;
                            // 如果请求失败则继续请求
                            while (true)
                            {
                                try
                                {
                                    List<string> args = new List<string>();
                                    args.Add("&age=39y");
                                    args.Add("&age=40y");

                                    List<string> gender = new List<string>();
                                    gender.Add("&gender=man");
                                    gender.Add("&gender=women");

                                    foreach(string str1 in args)
                                    {
                                        foreach (string str2 in gender)
                                        {

                                            Item[] temp1 = Crawler.Core.Utility.WeiboUtility.Search(
                                                                                                   weiboSearchQuery.Keyword, weiboSearchQuery.StartPage,
                                                                                                   weiboSearchQuery.EndPage, out count, weiboSearchQuery.Option,
                                                                                                   true, weiboSearchQuery.isOrigin,
                                                                                                   weiboSearchQuery.StartDate, weiboSearchQuery.EndDate,
                                                                                                   weiboSearchQuery.AddtionQuery + str1 + str2);

                                            Item[] temp2 = Crawler.Core.Utility.WeiboUtility.Search(
                                                        weiboSearchQuery.Keyword, weiboSearchQuery.StartPage,
                                                        weiboSearchQuery.EndPage, out count, weiboSearchQuery.Option,
                                                        false , weiboSearchQuery.isOrigin,
                                                        weiboSearchQuery.StartDate, weiboSearchQuery.EndDate,
                                                        weiboSearchQuery.AddtionQuery + str1 + str2);

                                            _items.AddRange(temp1);
                                            _items.AddRange(temp2);


                                        }

                                    }


                                    items = _items.ToArray();

                                    
                                    break;
                                }
                                catch (Exception ee)
                                {
                                    Logger.Warn("Weibo Search Request failed " + ee.ToString());
                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                }
                            }

                            composer.ReportStatus(ref items, count);

                            var projectionItem
                                = items.Select(
                                               item => new
                                               {
                                                   Title = item.CleanTitle,
                                                   Text = item.CleanText,
                                                   MediaName = item.ReproducedMediaName,
                                                   Url = item.Url,
                                                   PubDate = item.PubDate,
                                                   Forward = item.CurrentCount == null
                                                   ? 0
                                                   : item.CurrentCount.ForwardCount,
                                                   Reply = item.CurrentCount == null
                                                   ? 0
                                                   : item.CurrentCount.ReplyCount,
                                                   View = item.CurrentCount == null
                                                   ? 0
                                                   : item.CurrentCount.ViewCount,
                                                   Source = item.Source,
                                                   Author = item.AuthorName,
                                                   Cert = item.AuthorCertificated.ToString(),
                                                   Region = (int)item.Lat,
                                               });
                            ExcelUtility.FillCollection(sheet, projectionItem, appendExcel);
                            appendExcel = true;
                            saveCountDown--;
                            if (saveCountDown <= 0)
                            {
                                saveCountDown = 1;
                                book.Save(dialog.FileName);
                            }
                        }
                    }

                    book.Save(dialog.FileName);
                    MessageBox.Show("导出成功");
                }
                catch (Exception unexpected)
                {
                    Logger.Error("SinaSearch failed " + unexpected.ToString());
                    throw;
                }
            }
        }

        private void AddPeopleQueryBtn_Click(object sender, EventArgs e)
        {
            var query = new PeopleSearchQuery
                            {
                                AddtionQuery = PeopleAddtionQueryTxt.Text,
                                Keyword = WeiboPeopleKeywordTxt.Text,
                                StartPage = (int) WeiboPeopleStartPageNum.Value,
                                EndPage = (int) WeiboPeopleEndPageNum.Value,
                                
                            };
            query.Age = EnumsExtension.FromDesc<SearchAge>(AgeDropdown.SelectedText);
            query.Cert = EnumsExtension.FromDesc<SearchCert>(CertDropdown.SelectedText);
            query.Gender = EnumsExtension.FromDesc<SearchGender>(GenderDropdown.SelectedText);
            query.Attr = EnumsExtension.FromDesc<SearchAttr>(AttrDropdown.SelectedText);

            WeiboPeopleSearchListbox.Items.Add(query);
            KeywordTxt.Text = "";
        }

        private void SearchPeopleBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel文件|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Workbook book = new Workbook();

                List<SinaPeople> _items = new List<SinaPeople>();
                var querys = WeiboPeopleSearchListbox.Items.Cast<PeopleSearchQuery>().ToArray();
                foreach (var peopleSearchQuery in querys)
                {                
                    List<string> args = new List<string>();
                    args.Add("&age=39y");
                    args.Add("&age=40y");

                    List<string> gender = new List<string>();
                    gender.Add("&gender=man");
                    gender.Add("&gender=women");

                    foreach (string str1 in args)
                    {
                        foreach (string str2 in gender)
                        {

                            SinaPeople[] temp1 = Crawler.Core.Utility.WeiboUtility.SearchPeople(
                    peopleSearchQuery.Keyword, peopleSearchQuery.StartPage,
                    peopleSearchQuery.EndPage, peopleSearchQuery.AddtionQuery + str1 + str2,
                    SearchCert.Cert, peopleSearchQuery.Age,
                    peopleSearchQuery.Gender, peopleSearchQuery.Attr);

                            SinaPeople[] temp2 = Crawler.Core.Utility.WeiboUtility.SearchPeople(
                    peopleSearchQuery.Keyword, peopleSearchQuery.StartPage,
                    peopleSearchQuery.EndPage, peopleSearchQuery.AddtionQuery + str1 + str2,
                    SearchCert.Regular, peopleSearchQuery.Age,
                    peopleSearchQuery.Gender, peopleSearchQuery.Attr);


                            _items.AddRange(temp1);
                            _items.AddRange(temp2);
                        }

                        Worksheet sheet = book.Worksheets.Add(peopleSearchQuery.Keyword + str1);
                        ExcelUtility.FillCollection(sheet, _items.ToArray());
                        _items.Clear();
                        book.Save(dialog.FileName);

                    }
                }
                book.Save(dialog.FileName);
                MessageBox.Show("导出成功");
            }
        }
    }
}

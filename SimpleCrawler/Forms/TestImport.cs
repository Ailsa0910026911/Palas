using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using Crawler.Core.Data;
using Crawler.Core.Parser;
using HooLab.Log;
using Palas.Common.Data;
using Palas.Common.DataAccess;
using Palas.Common.Lib;
using Palas.Common.Lib.DTO;
using Palas.Common.Lib.Entity;

namespace Crawler.Host
{
    public partial class TestImport : Form
    {
        public TestImport()
        {
            InitializeComponent();
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            Workbook workbook  = new Workbook();
            workbook.Open(@"D:\output\input.xlsx");
            Worksheet sheet = workbook.Worksheets["输入源"];
            int currentLine = 1;
            while(!string.IsNullOrEmpty(sheet.Cells[currentLine,0].StringValue))
            {
                try
                {
                    string mediaChannel = sheet.Cells[currentLine, 0].StringValue;
                    string url = ProcessUrl(sheet.Cells[currentLine, 1].StringValue);
                    string type = sheet.Cells[currentLine, 2].StringValue;
                    string subType = sheet.Cells[currentLine, 3].StringValue;
                    string name = sheet.Cells[currentLine, 4].StringValue;
                    string webName = sheet.Cells[currentLine, 5].StringValue;
                    string webUrl = ProcessUrl(sheet.Cells[currentLine, 6].StringValue);
                    string region = sheet.Cells[currentLine, 7].StringValue;
                    string industry = sheet.Cells[currentLine, 8].StringValue;
                    string orgLevel = sheet.Cells[currentLine, 9].StringValue;
                    string socialSystem = sheet.Cells[currentLine, 10].StringValue;
                    int mediaType = 3;
                    const string economyIndustryID = "J";
                    string departmentID = null;
                    string orgID = null;
                    string industryID = null;
                    string regionID = GetRegionID(region);
                    switch (type)
                    {
                        //需要填入行业信息
                        case "行业协会":
                        case "行业信息":
                            {
                                using (PalasDB db = new PalasDB())
                                {

                                    industryID = string.IsNullOrEmpty(industry) ? null : industry.Substring(0, 1);
                                    if (subType == "行业协会")
                                    {
                                        //Add org

                                        Organization org = db.Organization.FirstOrDefault(model => model.FullName == name);
                                        if (org == null)
                                        {
                                            org = db.Organization.CreateObject();
                                            org.OrganizationID = CrawlDTO.NewGuid;
                                            org.FullName = name;
                                            org.ShortName = name;
                                            org.OrgLevel = (sbyte)Enums.OrgLevel.Province;//省部
                                            org.RegionID = regionID ?? "10000";
                                            org.RegionLevel = (sbyte)Enums.RegionLevel.Province; //省级
                                            org.SocialSystem = (sbyte)Enums.SocialSystem.CompanyNational; //国企
                                            org.DepartmentID = null;
                                            org.IndustryID = industryID;
                                            org.StockListing = (sbyte)Enums.StockListing.NonIPO;
                                            //org.Homepage = webUrl;
                                            db.Organization.AddObject(org);
                                            db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);

                                        }
                                        orgID = org.OrganizationID;


                                    }



                                }
                                break;
                            }
                        //需要填入部门信息
                        case "机构":
                        case "上市公司":
                            {
                                industryID = economyIndustryID;
                                using (PalasDB db = new PalasDB())
                                {
                                    var department = db.Department.FirstOrDefault(model => model.DepartmentName == subType);
                                    if (department == null)
                                    {
                                        department = db.Department.CreateObject();
                                        department.DepartmentName = subType;
                                        department.Order = 0;
                                        department.DepartmentID = CrawlDTO.NewGuid;
                                        db.Department.AddObject(department);
                                        db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                                    }
                                    departmentID = department.DepartmentID;
                                    var org = db.Organization.FirstOrDefault(model => model.FullName == name);
                                    if (org == null)
                                    {
                                        org = db.Organization.CreateObject();
                                        org.OrganizationID = CrawlDTO.NewGuid;
                                        org.FullName = name;
                                        org.ShortName = name;
                                        org.OrgLevel = (sbyte)Enums.OrgLevel.Province;//省部
                                        org.RegionID = regionID ?? "10000";
                                        org.RegionLevel = (sbyte)Enums.RegionLevel.Province; //省级
                                        org.SocialSystem = (sbyte)Enums.SocialSystem.CompanyNational; //国企
                                        org.DepartmentID = departmentID;
                                        org.IndustryID = industryID;
                                        if (type == "上市公司")
                                        {
                                            org.StockListing = (sbyte)Enums.StockListing.Shanghai_A;
                                        }
                                        else
                                        {
                                            org.StockListing = (sbyte)Enums.StockListing.NonIPO;
                                        }

                                        //org.Homepage = webUrl;
                                        db.Organization.AddObject(org);
                                        db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                                    }
                                    orgID = org.OrganizationID;

                                }

                                break;
                            }
                        //需要填入组织级别
                        case "政府机构":
                            {
                                industryID = null;
                                using (PalasDB db = new PalasDB())
                                {
                                    var department = db.Department.FirstOrDefault(model => model.DepartmentName == subType);
                                    if (department == null)
                                    {
                                        department = db.Department.CreateObject();
                                        department.DepartmentName = subType;
                                        department.Order = 0;
                                        department.DepartmentID = CrawlDTO.NewGuid;
                                        db.Department.AddObject(department);
                                        db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                                    }
                                    departmentID = department.DepartmentID;
                                    var org = db.Organization.FirstOrDefault(model => model.FullName == name);
                                    if (org == null)
                                    {
                                        org = db.Organization.CreateObject();
                                        org.OrganizationID = CrawlDTO.NewGuid;
                                        org.FullName = name;
                                        org.ShortName = name;
                                        org.OrgLevel = sbyte.Parse(orgLevel.Substring(0, 1));//省部
                                        org.RegionID = regionID ?? "10000";
                                        org.RegionLevel = (sbyte)(region == "全国 " ? Enums.RegionLevel.National : Enums.RegionLevel.Province);
                                        org.SocialSystem = (sbyte)Enums.SocialSystem.Government; //政府

                                        org.IndustryID = industryID;

                                        org.StockListing = (sbyte)Enums.StockListing.NonIPO;
                                        org.DepartmentID = departmentID;

                                        //org.Homepage = webUrl;
                                        db.Organization.AddObject(org);
                                        db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                                    }
                                    orgID = org.OrganizationID;

                                }
                                break;
                            }

                        //普通方式
                        default:
                            {
                                if (subType == "专业财经 ")
                                {
                                    industryID = economyIndustryID;
                                }
                                //决定MediaType
                                mediaType = (int)DeterminedMediaType(type);
                                break;
                            }
                    }
                    using (PalasDB db = new PalasDB())
                    {
                        //添加Media
                        var media = db.Media.FirstOrDefault(model => model.MediaName == name);
                        if (media == null)
                        {
                            media = db.Media.CreateObject();
                            media.MediaID = CrawlDTO.NewGuid;
                            media.MediaName = name;
                            media.Url = webUrl;
                            media.Channel = mediaChannel;
                            media.MediaType = (sbyte)mediaType;
                            media.MediaTendency = 0;
                            media.MediaOrganType = 0;
                            media.MediaWeight = 0;
                            media.MediaStyle = 1;
                            media.RegionType = 1;
                            media.IndustryIDs = industryID;
                            media.OrganizationIDs = orgID;
                            media.DepartmentIDs = departmentID;
                            media.ProxyZone = 0;
                            media.CreateTime = DateTime.Now;
                            db.Media.AddObject(media);
                            db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                        }
                        var mediaID = media.MediaID;

                        //添加Site
                        var site = db.Site.FirstOrDefault(model => model.Name == name);
                        if (site == null)
                        {
                            site = db.Site.CreateObject();
                            site.SiteID = CrawlDTO.NewGuid;
                            site.MediaID = mediaID;
                            site.UrlEncoding = "UTF-8";
                            site.ParallelWithOtherCrawler = false;
                            site.TimeoutSecs = 10;
                            site.EncodingResponse = "UTF-8";
                            //site.EncodingResponse = "UTF-8";


                            site.LoginUseWebBrowser = false;
                            site.IsVisible = true;
                            site.ListItemCountPerPage = 20;
                            site.Name = media.MediaName;
                            site.ParseMethod = 1;//XPath抓取
                            site.CreateTime = DateTime.Now;
                            site.ListPattern = "";
                            site.ItemPattern = "";
                            site.ContentDetailLevel = 2;
                            db.Site.AddObject(site);
                            db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                        }
                        var siteID = site.SiteID;
                        //添加Crawl
                        var crawl = db.Crawl.FirstOrDefault(model => model.Url == url);
                        if (crawl == null)
                        {
                            crawl = db.Crawl.CreateObject();
                            crawl.CrawlID = CrawlDTO.NewGuid;
                        }
                        crawl.SiteID = siteID;

                        crawl.Name = name;

                        crawl.IssueID = "ECO";
                        crawl.ListDrillMethod = 1;

                        crawl.CrawlType = 1;//Crawl List and Items

                        //Crawl Summary
                        crawl.Url = url;
                        crawl.IntervalMins = 1440;
                        crawl.IntervalStrategy = 1;
                        crawl.ExistItemStrategy = 2;
                        crawl.MaxRetriveDays = 180;

                        crawl.RequiredCount = 20;
                        crawl.InitRequiredCount = 20;
                        crawl.CreateTime = DateTime.Now;
                        crawl.NextCrawlTime = DateTime.Now;
                        crawl.MediaType = media.MediaType;
                        crawl.MediaRegionType = media.RegionType;
                        crawl.MediaID = mediaID;
                        crawl.LastCrawlNewCount = 0;
                        crawl.FollowIntervalMins = 60;
                        crawl.FollowMinReplyLen = 6;
                        crawl.MediaMapToChannel = false;
                        crawl.MediaRecordNew = true;
                        crawl.SaveSummary = true;
                        crawl.SaveHtml = true;
                        crawl.SaveContent = true;
                        crawl.ReleaseAutoFormat = true;
                        crawl.OrganizationIDs = orgID;
                        crawl.DepartmentIDs = departmentID;
                        crawl.IndustryIDs = industryID;

                        db.Crawl.AddObject(crawl);
                        db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);


                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("行"+currentLine+"错误",ex);
                }
                

                Statuslbl.Text = "当前正导入第" + currentLine + "条记录";
                Application.DoEvents();
                currentLine++;
            }
        }

        private Enums.MediaType DeterminedMediaType(string type)
        {
            switch (type)
            {
                case "平面媒体":
                    {
                        return Enums.MediaType.PaperMagazine;
                        break;
                    }
                case "视听媒体":
                    {
                        return Enums.MediaType.Video;
                        break;
                    }
                default:
                    {
                        return Enums.MediaType.WebNews;
                    }
            }

        }

        private string GetRegionID(string region)
        {
            using (PalasDB db = new PalasDB())
            {
                if (string.IsNullOrEmpty(region))
                {
                    return null;
                }
                string province = region.Substring(0, 2);
                var regionEntity = db.Region.FirstOrDefault(model => model.Province == province && model.City==null);
                if (regionEntity == null)
                {
                    return null;

                }
                return regionEntity.RegionID;
            }

        }

        private string ProcessUrl(string rawUrl)
        {
            string url = rawUrl;
            if (!rawUrl.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            return url;
        }
    }
}

using Analyzer.Core.Algorithm;
using Crawler.Core.RequestProcessor;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crawler.Core.Parser;
using Crawler.Core.Crawler;

using Palas.Common.Data;
using System.Collections.Generic;
using Palas.Common.Utility;

namespace SimpleCrawler
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.Run(new CrawlHost());
        }
    }
}
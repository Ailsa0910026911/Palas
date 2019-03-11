using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Crawler.Host
{
    public partial class TestIEWebbrowser : Form
    {
        public TestIEWebbrowser()
        {
            InitializeComponent();
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(UrlTxt.Text);
        }
    }
}

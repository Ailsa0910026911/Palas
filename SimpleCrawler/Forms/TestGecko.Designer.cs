using System.IO;
using Crawler.Core;


namespace Crawler.Host
{
    partial class TestGecko
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UrlTxt = new System.Windows.Forms.TextBox();
            this.NavBtn = new System.Windows.Forms.Button();
            this.geckoWebBrowser1 = new Gecko.GeckoWebBrowser();
            this.EnableCssChk = new System.Windows.Forms.RadioButton();
            this.DisableCssChk = new System.Windows.Forms.RadioButton();
            this.CookieChk = new System.Windows.Forms.CheckBox();
            this.TestBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UrlTxt
            // 
            this.UrlTxt.Location = new System.Drawing.Point(13, 13);
            this.UrlTxt.Name = "UrlTxt";
            this.UrlTxt.Size = new System.Drawing.Size(836, 21);
            this.UrlTxt.TabIndex = 2;
            // 
            // NavBtn
            // 
            this.NavBtn.Location = new System.Drawing.Point(855, 11);
            this.NavBtn.Name = "NavBtn";
            this.NavBtn.Size = new System.Drawing.Size(75, 23);
            this.NavBtn.TabIndex = 3;
            this.NavBtn.Text = "导航至";
            this.NavBtn.UseVisualStyleBackColor = true;
            this.NavBtn.Click += new System.EventHandler(this.NavBtn_Click);
            // 
            // geckoWebBrowser1
            // 
            //this.geckoWebBrowser1.DisableWmImeSetContext = false;
            this.geckoWebBrowser1.Location = new System.Drawing.Point(13, 85);
            this.geckoWebBrowser1.Name = "geckoWebBrowser1";
            this.geckoWebBrowser1.Size = new System.Drawing.Size(1024, 745);
            this.geckoWebBrowser1.TabIndex = 5;
            this.geckoWebBrowser1.UseHttpActivityObserver = false;
            // 
            // EnableCssChk
            // 
            this.EnableCssChk.AutoSize = true;
            this.EnableCssChk.Checked = true;
            this.EnableCssChk.Location = new System.Drawing.Point(13, 41);
            this.EnableCssChk.Name = "EnableCssChk";
            this.EnableCssChk.Size = new System.Drawing.Size(65, 16);
            this.EnableCssChk.TabIndex = 6;
            this.EnableCssChk.TabStop = true;
            this.EnableCssChk.Text = "启用Css";
            this.EnableCssChk.UseVisualStyleBackColor = true;
            // 
            // DisableCssChk
            // 
            this.DisableCssChk.AutoSize = true;
            this.DisableCssChk.Location = new System.Drawing.Point(84, 40);
            this.DisableCssChk.Name = "DisableCssChk";
            this.DisableCssChk.Size = new System.Drawing.Size(65, 16);
            this.DisableCssChk.TabIndex = 7;
            this.DisableCssChk.Text = "禁用Css";
            this.DisableCssChk.UseVisualStyleBackColor = true;
            // 
            // CookieChk
            // 
            this.CookieChk.AutoSize = true;
            this.CookieChk.Checked = true;
            this.CookieChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CookieChk.Location = new System.Drawing.Point(12, 63);
            this.CookieChk.Name = "CookieChk";
            this.CookieChk.Size = new System.Drawing.Size(84, 16);
            this.CookieChk.TabIndex = 8;
            this.CookieChk.Text = "启用Cookie";
            this.CookieChk.UseVisualStyleBackColor = true;
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(855, 40);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(75, 23);
            this.TestBtn.TabIndex = 9;
            this.TestBtn.Text = "测试";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // TestGecko
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 742);
            this.Controls.Add(this.TestBtn);
            this.Controls.Add(this.CookieChk);
            this.Controls.Add(this.DisableCssChk);
            this.Controls.Add(this.EnableCssChk);
            this.Controls.Add(this.geckoWebBrowser1);
            this.Controls.Add(this.NavBtn);
            this.Controls.Add(this.UrlTxt);
            this.Name = "TestGecko";
            this.Text = "TestGecko";
            this.Load += new System.EventHandler(this.TestGecko_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UrlTxt;
        private System.Windows.Forms.Button NavBtn;
        private Gecko.GeckoWebBrowser geckoWebBrowser1;
        private System.Windows.Forms.RadioButton EnableCssChk;
        private System.Windows.Forms.RadioButton DisableCssChk;
        private System.Windows.Forms.CheckBox CookieChk;
        private System.Windows.Forms.Button TestBtn;

    }
}
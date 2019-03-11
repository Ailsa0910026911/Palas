namespace Crawler.Host
{
    partial class TestIEWebbrowser
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.NavBtn = new System.Windows.Forms.Button();
            this.UrlTxt = new System.Windows.Forms.TextBox();
            this.TestBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.webBrowser1);
            this.panel1.Location = new System.Drawing.Point(12, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 342);
            this.panel1.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(800, 342);
            this.webBrowser1.TabIndex = 0;
            // 
            // NavBtn
            // 
            this.NavBtn.Location = new System.Drawing.Point(737, 6);
            this.NavBtn.Name = "NavBtn";
            this.NavBtn.Size = new System.Drawing.Size(75, 23);
            this.NavBtn.TabIndex = 1;
            this.NavBtn.Text = "浏览";
            this.NavBtn.UseVisualStyleBackColor = true;
            this.NavBtn.Click += new System.EventHandler(this.NavBtn_Click);
            // 
            // UrlTxt
            // 
            this.UrlTxt.Location = new System.Drawing.Point(13, 6);
            this.UrlTxt.Name = "UrlTxt";
            this.UrlTxt.Size = new System.Drawing.Size(708, 21);
            this.UrlTxt.TabIndex = 2;
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(13, 383);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(75, 23);
            this.TestBtn.TabIndex = 3;
            this.TestBtn.Text = "Test";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // TestIEWebbrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 484);
            this.Controls.Add(this.TestBtn);
            this.Controls.Add(this.UrlTxt);
            this.Controls.Add(this.NavBtn);
            this.Controls.Add(this.panel1);
            this.Name = "TestIEWebbrowser";
            this.Text = "TestIEWebbrowser";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button NavBtn;
        private System.Windows.Forms.TextBox UrlTxt;
        private System.Windows.Forms.Button TestBtn;
    }
}
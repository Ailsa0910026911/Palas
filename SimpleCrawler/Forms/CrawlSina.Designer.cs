namespace Crawler.Host
{
    partial class CrawlSina
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.FamousTxt = new System.Windows.Forms.TextBox();
            this.AddBtn = new System.Windows.Forms.Button();
            this.UrlList = new System.Windows.Forms.ListBox();
            this.CommentChk = new System.Windows.Forms.CheckBox();
            this.CrawlBtn = new System.Windows.Forms.Button();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.SumLbl = new System.Windows.Forms.Label();
            this.InitialTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名人Url:";
            // 
            // FamousTxt
            // 
            this.FamousTxt.Location = new System.Drawing.Point(71, 6);
            this.FamousTxt.Name = "FamousTxt";
            this.FamousTxt.Size = new System.Drawing.Size(272, 21);
            this.FamousTxt.TabIndex = 1;
            // 
            // AddBtn
            // 
            this.AddBtn.Location = new System.Drawing.Point(349, 6);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(64, 23);
            this.AddBtn.TabIndex = 2;
            this.AddBtn.Text = "添加";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // UrlList
            // 
            this.UrlList.FormattingEnabled = true;
            this.UrlList.ItemHeight = 12;
            this.UrlList.Location = new System.Drawing.Point(14, 36);
            this.UrlList.Name = "UrlList";
            this.UrlList.Size = new System.Drawing.Size(399, 124);
            this.UrlList.TabIndex = 3;
            // 
            // CommentChk
            // 
            this.CommentChk.AutoSize = true;
            this.CommentChk.Location = new System.Drawing.Point(14, 167);
            this.CommentChk.Name = "CommentChk";
            this.CommentChk.Size = new System.Drawing.Size(72, 16);
            this.CommentChk.TabIndex = 4;
            this.CommentChk.Text = "抓取评论";
            this.CommentChk.UseVisualStyleBackColor = true;
            // 
            // CrawlBtn
            // 
            this.CrawlBtn.Location = new System.Drawing.Point(166, 191);
            this.CrawlBtn.Name = "CrawlBtn";
            this.CrawlBtn.Size = new System.Drawing.Size(75, 23);
            this.CrawlBtn.TabIndex = 5;
            this.CrawlBtn.Text = "开始抓取";
            this.CrawlBtn.UseVisualStyleBackColor = true;
            this.CrawlBtn.Click += new System.EventHandler(this.CrawlBtn_Click);
            // 
            // StatusLbl
            // 
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(14, 220);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(71, 12);
            this.StatusLbl.TabIndex = 6;
            this.StatusLbl.Text = "[StatusLbl]";
            // 
            // SumLbl
            // 
            this.SumLbl.AutoSize = true;
            this.SumLbl.Location = new System.Drawing.Point(14, 245);
            this.SumLbl.Name = "SumLbl";
            this.SumLbl.Size = new System.Drawing.Size(53, 12);
            this.SumLbl.TabIndex = 7;
            this.SumLbl.Text = "[SumLbl]";
            // 
            // InitialTimer
            // 
            this.InitialTimer.Interval = 5000;
            this.InitialTimer.Tick += new System.EventHandler(this.InitialTimer_Tick);
            // 
            // CrawlSina
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 276);
            this.Controls.Add(this.SumLbl);
            this.Controls.Add(this.StatusLbl);
            this.Controls.Add(this.CrawlBtn);
            this.Controls.Add(this.CommentChk);
            this.Controls.Add(this.UrlList);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.FamousTxt);
            this.Controls.Add(this.label1);
            this.Name = "CrawlSina";
            this.Text = "CrawlSina";
            this.Load += new System.EventHandler(this.CrawlSina_Load);
            this.Shown += new System.EventHandler(this.CrawlSina_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FamousTxt;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.ListBox UrlList;
        private System.Windows.Forms.CheckBox CommentChk;
        private System.Windows.Forms.Button CrawlBtn;
        private System.Windows.Forms.Label StatusLbl;
        private System.Windows.Forms.Label SumLbl;
        private System.Windows.Forms.Timer InitialTimer;
    }
}
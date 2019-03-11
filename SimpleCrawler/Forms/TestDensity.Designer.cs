namespace Crawler.Host
{
    partial class TestDensity
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
            this.InputUrlTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NewsRd = new System.Windows.Forms.RadioButton();
            this.ForumRd = new System.Windows.Forms.RadioButton();
            this.ParseListBtn = new System.Windows.Forms.Button();
            this.ParsePageBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ListGridView = new System.Windows.Forms.DataGridView();
            this.ParsePageCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ContentTxt = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.NextpageXPathTxt = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ElementXPathTxt = new System.Windows.Forms.TextBox();
            this.ElementBlockTxt = new System.Windows.Forms.TextBox();
            this.TitleTxt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.MediaTxt = new System.Windows.Forms.TextBox();
            this.AuthorTxt = new System.Windows.Forms.TextBox();
            this.ReplyTxt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ViewTxt = new System.Windows.Forms.TextBox();
            this.PubdateTxt = new System.Windows.Forms.TextBox();
            this.PageUrlTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.InputTitleTxt = new System.Windows.Forms.TextBox();
            this.HttpdownRd = new System.Windows.Forms.RadioButton();
            this.GeckoDownRd = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.ExcludeTxt = new System.Windows.Forms.TextBox();
            this.Export = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ListGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputUrlTxt
            // 
            this.InputUrlTxt.Location = new System.Drawing.Point(47, 10);
            this.InputUrlTxt.Name = "InputUrlTxt";
            this.InputUrlTxt.Size = new System.Drawing.Size(247, 21);
            this.InputUrlTxt.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Url:";
            // 
            // NewsRd
            // 
            this.NewsRd.AutoSize = true;
            this.NewsRd.Checked = true;
            this.NewsRd.Location = new System.Drawing.Point(9, 3);
            this.NewsRd.Name = "NewsRd";
            this.NewsRd.Size = new System.Drawing.Size(47, 16);
            this.NewsRd.TabIndex = 2;
            this.NewsRd.TabStop = true;
            this.NewsRd.Text = "News";
            this.NewsRd.UseVisualStyleBackColor = true;
            // 
            // ForumRd
            // 
            this.ForumRd.AutoSize = true;
            this.ForumRd.Location = new System.Drawing.Point(62, 3);
            this.ForumRd.Name = "ForumRd";
            this.ForumRd.Size = new System.Drawing.Size(53, 16);
            this.ForumRd.TabIndex = 3;
            this.ForumRd.Text = "Forum";
            this.ForumRd.UseVisualStyleBackColor = true;
            // 
            // ParseListBtn
            // 
            this.ParseListBtn.Location = new System.Drawing.Point(687, 10);
            this.ParseListBtn.Name = "ParseListBtn";
            this.ParseListBtn.Size = new System.Drawing.Size(75, 23);
            this.ParseListBtn.TabIndex = 4;
            this.ParseListBtn.Text = "ParseList";
            this.ParseListBtn.UseVisualStyleBackColor = true;
            this.ParseListBtn.Click += new System.EventHandler(this.ParseListBtn_Click);
            // 
            // ParsePageBtn
            // 
            this.ParsePageBtn.Location = new System.Drawing.Point(768, 10);
            this.ParsePageBtn.Name = "ParsePageBtn";
            this.ParsePageBtn.Size = new System.Drawing.Size(75, 23);
            this.ParsePageBtn.TabIndex = 5;
            this.ParsePageBtn.Text = "ParsePage";
            this.ParsePageBtn.UseVisualStyleBackColor = true;
            this.ParsePageBtn.Click += new System.EventHandler(this.ParsePageBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ListGridView);
            this.groupBox1.Location = new System.Drawing.Point(14, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 396);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "List";
            // 
            // ListGridView
            // 
            this.ListGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ListGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParsePageCol});
            this.ListGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListGridView.Location = new System.Drawing.Point(3, 17);
            this.ListGridView.Name = "ListGridView";
            this.ListGridView.RowTemplate.Height = 23;
            this.ListGridView.Size = new System.Drawing.Size(415, 376);
            this.ListGridView.TabIndex = 0;
            this.ListGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ListGridView_CellContentClick);
            // 
            // ParsePageCol
            // 
            this.ParsePageCol.HeaderText = "ParsePage";
            this.ParsePageCol.Name = "ParsePageCol";
            this.ParsePageCol.ReadOnly = true;
            this.ParsePageCol.Text = "ParsePage";
            this.ParsePageCol.ToolTipText = "ParsePage";
            this.ParsePageCol.UseColumnTextForButtonValue = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ContentTxt);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.NextpageXPathTxt);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.ElementXPathTxt);
            this.groupBox2.Controls.Add(this.ElementBlockTxt);
            this.groupBox2.Controls.Add(this.TitleTxt);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.MediaTxt);
            this.groupBox2.Controls.Add(this.AuthorTxt);
            this.groupBox2.Controls.Add(this.ReplyTxt);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.ViewTxt);
            this.groupBox2.Controls.Add(this.PubdateTxt);
            this.groupBox2.Controls.Add(this.PageUrlTxt);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(441, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 398);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Page";
            // 
            // ContentTxt
            // 
            this.ContentTxt.Location = new System.Drawing.Point(8, 262);
            this.ContentTxt.MaxLength = 10000000;
            this.ContentTxt.Multiline = true;
            this.ContentTxt.Name = "ContentTxt";
            this.ContentTxt.ReadOnly = true;
            this.ContentTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ContentTxt.Size = new System.Drawing.Size(380, 130);
            this.ContentTxt.TabIndex = 26;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 247);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 25;
            this.label12.Text = "Content:";
            // 
            // NextpageXPathTxt
            // 
            this.NextpageXPathTxt.Location = new System.Drawing.Point(96, 223);
            this.NextpageXPathTxt.Name = "NextpageXPathTxt";
            this.NextpageXPathTxt.ReadOnly = true;
            this.NextpageXPathTxt.Size = new System.Drawing.Size(280, 21);
            this.NextpageXPathTxt.TabIndex = 24;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 226);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 23;
            this.label11.Text = "NextPageXPath:";
            // 
            // ElementXPathTxt
            // 
            this.ElementXPathTxt.Location = new System.Drawing.Point(96, 199);
            this.ElementXPathTxt.Name = "ElementXPathTxt";
            this.ElementXPathTxt.ReadOnly = true;
            this.ElementXPathTxt.Size = new System.Drawing.Size(280, 21);
            this.ElementXPathTxt.TabIndex = 22;
            // 
            // ElementBlockTxt
            // 
            this.ElementBlockTxt.Location = new System.Drawing.Point(96, 176);
            this.ElementBlockTxt.Name = "ElementBlockTxt";
            this.ElementBlockTxt.ReadOnly = true;
            this.ElementBlockTxt.Size = new System.Drawing.Size(280, 21);
            this.ElementBlockTxt.TabIndex = 21;
            // 
            // TitleTxt
            // 
            this.TitleTxt.Location = new System.Drawing.Point(66, 153);
            this.TitleTxt.Name = "TitleTxt";
            this.TitleTxt.ReadOnly = true;
            this.TitleTxt.Size = new System.Drawing.Size(310, 21);
            this.TitleTxt.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 202);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "ElementXPath:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 179);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "ElementBlock:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "Title:";
            // 
            // MediaTxt
            // 
            this.MediaTxt.Location = new System.Drawing.Point(66, 131);
            this.MediaTxt.Name = "MediaTxt";
            this.MediaTxt.ReadOnly = true;
            this.MediaTxt.Size = new System.Drawing.Size(309, 21);
            this.MediaTxt.TabIndex = 16;
            // 
            // AuthorTxt
            // 
            this.AuthorTxt.Location = new System.Drawing.Point(66, 108);
            this.AuthorTxt.Name = "AuthorTxt";
            this.AuthorTxt.ReadOnly = true;
            this.AuthorTxt.Size = new System.Drawing.Size(309, 21);
            this.AuthorTxt.TabIndex = 15;
            // 
            // ReplyTxt
            // 
            this.ReplyTxt.Location = new System.Drawing.Point(66, 85);
            this.ReplyTxt.Name = "ReplyTxt";
            this.ReplyTxt.ReadOnly = true;
            this.ReplyTxt.Size = new System.Drawing.Size(309, 21);
            this.ReplyTxt.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "Media:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Author:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "Reply:";
            // 
            // ViewTxt
            // 
            this.ViewTxt.Location = new System.Drawing.Point(66, 61);
            this.ViewTxt.Name = "ViewTxt";
            this.ViewTxt.ReadOnly = true;
            this.ViewTxt.Size = new System.Drawing.Size(310, 21);
            this.ViewTxt.TabIndex = 10;
            // 
            // PubdateTxt
            // 
            this.PubdateTxt.Location = new System.Drawing.Point(66, 38);
            this.PubdateTxt.Name = "PubdateTxt";
            this.PubdateTxt.ReadOnly = true;
            this.PubdateTxt.Size = new System.Drawing.Size(310, 21);
            this.PubdateTxt.TabIndex = 9;
            // 
            // PageUrlTxt
            // 
            this.PageUrlTxt.Location = new System.Drawing.Point(66, 15);
            this.PageUrlTxt.Name = "PageUrlTxt";
            this.PageUrlTxt.ReadOnly = true;
            this.PageUrlTxt.Size = new System.Drawing.Size(310, 21);
            this.PageUrlTxt.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "View:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Pubdate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Url:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(300, 13);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 9;
            this.label13.Text = "Title:";
            // 
            // InputTitleTxt
            // 
            this.InputTitleTxt.Location = new System.Drawing.Point(347, 10);
            this.InputTitleTxt.Name = "InputTitleTxt";
            this.InputTitleTxt.Size = new System.Drawing.Size(213, 21);
            this.InputTitleTxt.TabIndex = 10;
            // 
            // HttpdownRd
            // 
            this.HttpdownRd.AutoSize = true;
            this.HttpdownRd.Checked = true;
            this.HttpdownRd.Location = new System.Drawing.Point(17, 37);
            this.HttpdownRd.Name = "HttpdownRd";
            this.HttpdownRd.Size = new System.Drawing.Size(71, 16);
            this.HttpdownRd.TabIndex = 12;
            this.HttpdownRd.TabStop = true;
            this.HttpdownRd.Text = "直接抓取";
            this.HttpdownRd.UseVisualStyleBackColor = true;
            // 
            // GeckoDownRd
            // 
            this.GeckoDownRd.AutoSize = true;
            this.GeckoDownRd.Location = new System.Drawing.Point(94, 37);
            this.GeckoDownRd.Name = "GeckoDownRd";
            this.GeckoDownRd.Size = new System.Drawing.Size(107, 16);
            this.GeckoDownRd.TabIndex = 13;
            this.GeckoDownRd.Text = "浏览器模拟抓取";
            this.GeckoDownRd.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.NewsRd);
            this.panel1.Controls.Add(this.ForumRd);
            this.panel1.Location = new System.Drawing.Point(566, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(115, 28);
            this.panel1.TabIndex = 14;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(207, 39);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 12);
            this.label14.TabIndex = 15;
            this.label14.Text = "排除字段:";
            // 
            // ExcludeTxt
            // 
            this.ExcludeTxt.Location = new System.Drawing.Point(273, 38);
            this.ExcludeTxt.Name = "ExcludeTxt";
            this.ExcludeTxt.Size = new System.Drawing.Size(162, 21);
            this.ExcludeTxt.TabIndex = 16;
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(768, 37);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(75, 22);
            this.Export.TabIndex = 17;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestDensity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 465);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.ExcludeTxt);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.GeckoDownRd);
            this.Controls.Add(this.HttpdownRd);
            this.Controls.Add(this.InputTitleTxt);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ParsePageBtn);
            this.Controls.Add(this.ParseListBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputUrlTxt);
            this.Name = "TestDensity";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestDensity";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ListGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputUrlTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton NewsRd;
        private System.Windows.Forms.RadioButton ForumRd;
        private System.Windows.Forms.Button ParseListBtn;
        private System.Windows.Forms.Button ParsePageBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView ListGridView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox ElementXPathTxt;
        private System.Windows.Forms.TextBox ElementBlockTxt;
        private System.Windows.Forms.TextBox TitleTxt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox MediaTxt;
        private System.Windows.Forms.TextBox AuthorTxt;
        private System.Windows.Forms.TextBox ReplyTxt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ViewTxt;
        private System.Windows.Forms.TextBox PubdateTxt;
        private System.Windows.Forms.TextBox PageUrlTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NextpageXPathTxt;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox ContentTxt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox InputTitleTxt;
        private System.Windows.Forms.DataGridViewButtonColumn ParsePageCol;
        private System.Windows.Forms.RadioButton HttpdownRd;
        private System.Windows.Forms.RadioButton GeckoDownRd;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox ExcludeTxt;
        private System.Windows.Forms.Button Export;
    }
}
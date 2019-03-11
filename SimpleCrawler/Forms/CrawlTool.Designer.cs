namespace Crawler.Host
{
    partial class CrawlTool
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
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ResultDataGridView = new System.Windows.Forms.DataGridView();
            this.PubDateCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MediaCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleCol = new System.Windows.Forms.DataGridViewLinkColumn();
            this.AuthorCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ViewCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommentCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ExportBtn = new System.Windows.Forms.Button();
            this.maxPageNum = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.startDateTime = new System.Windows.Forms.DateTimePicker();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.SearchForumList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.keywordTxt = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.SearchChkList = new System.Windows.Forms.CheckedListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.BatchKeywordTxt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.SearchResultDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewLinkColumn1 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeywordCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchEngineCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.BatchStatusLbl = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.BatchExportBtn = new System.Windows.Forms.Button();
            this.BatchLimitNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.BatchStartDateTime = new System.Windows.Forms.DateTimePicker();
            this.BatchSearchBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultDataGridView)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxPageNum)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultDataGridView)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BatchLimitNum)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(825, 41);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(528, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "本程序用于从外部搜索引擎(如谷歌、天涯等)搜索及导出结果 Palas©2012";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 41);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(825, 474);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ResultDataGridView);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(817, 448);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "单词模式";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ResultDataGridView
            // 
            this.ResultDataGridView.AllowUserToOrderColumns = true;
            this.ResultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PubDateCol,
            this.MediaCol,
            this.TitleCol,
            this.AuthorCol,
            this.ViewCol,
            this.CommentCol});
            this.ResultDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultDataGridView.Location = new System.Drawing.Point(2, 63);
            this.ResultDataGridView.Name = "ResultDataGridView";
            this.ResultDataGridView.RowTemplate.Height = 23;
            this.ResultDataGridView.Size = new System.Drawing.Size(813, 383);
            this.ResultDataGridView.TabIndex = 10;
            this.ResultDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultDataGridView_CellClick);
            // 
            // PubDateCol
            // 
            this.PubDateCol.DataPropertyName = "PubDate";
            this.PubDateCol.HeaderText = "日期";
            this.PubDateCol.Name = "PubDateCol";
            this.PubDateCol.ReadOnly = true;
            // 
            // MediaCol
            // 
            this.MediaCol.DataPropertyName = "MediaName";
            this.MediaCol.HeaderText = "媒体";
            this.MediaCol.Name = "MediaCol";
            this.MediaCol.ReadOnly = true;
            // 
            // TitleCol
            // 
            this.TitleCol.DataPropertyName = "Title";
            this.TitleCol.HeaderText = "标题";
            this.TitleCol.Name = "TitleCol";
            this.TitleCol.ReadOnly = true;
            this.TitleCol.Width = 350;
            // 
            // AuthorCol
            // 
            this.AuthorCol.DataPropertyName = "Author";
            this.AuthorCol.HeaderText = "作者";
            this.AuthorCol.Name = "AuthorCol";
            this.AuthorCol.ReadOnly = true;
            this.AuthorCol.Width = 70;
            // 
            // ViewCol
            // 
            this.ViewCol.DataPropertyName = "View";
            this.ViewCol.HeaderText = "点击";
            this.ViewCol.Name = "ViewCol";
            this.ViewCol.ReadOnly = true;
            this.ViewCol.Width = 70;
            // 
            // CommentCol
            // 
            this.CommentCol.DataPropertyName = "Comment";
            this.CommentCol.HeaderText = "评论";
            this.CommentCol.Name = "CommentCol";
            this.CommentCol.ReadOnly = true;
            this.CommentCol.Width = 70;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.StatusLbl);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.ExportBtn);
            this.panel2.Controls.Add(this.maxPageNum);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.startDateTime);
            this.panel2.Controls.Add(this.SearchBtn);
            this.panel2.Controls.Add(this.SearchForumList);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.keywordTxt);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(813, 61);
            this.panel2.TabIndex = 8;
            // 
            // StatusLbl
            // 
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(72, 37);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(0, 12);
            this.StatusLbl.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 17;
            this.label9.Text = "抓取状态;";
            // 
            // ExportBtn
            // 
            this.ExportBtn.Location = new System.Drawing.Point(770, 7);
            this.ExportBtn.Margin = new System.Windows.Forms.Padding(2);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(38, 24);
            this.ExportBtn.TabIndex = 16;
            this.ExportBtn.Text = "导出";
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // maxPageNum
            // 
            this.maxPageNum.Location = new System.Drawing.Point(538, 10);
            this.maxPageNum.Margin = new System.Windows.Forms.Padding(2);
            this.maxPageNum.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.maxPageNum.Name = "maxPageNum";
            this.maxPageNum.Size = new System.Drawing.Size(40, 21);
            this.maxPageNum.TabIndex = 15;
            this.maxPageNum.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(487, 14);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "限定条数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(584, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "限定时间";
            // 
            // startDateTime
            // 
            this.startDateTime.Location = new System.Drawing.Point(634, 10);
            this.startDateTime.Margin = new System.Windows.Forms.Padding(2);
            this.startDateTime.Name = "startDateTime";
            this.startDateTime.Size = new System.Drawing.Size(126, 21);
            this.startDateTime.TabIndex = 12;
            // 
            // SearchBtn
            // 
            this.SearchBtn.Location = new System.Drawing.Point(420, 7);
            this.SearchBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(56, 24);
            this.SearchBtn.TabIndex = 11;
            this.SearchBtn.Text = "搜 索";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // SearchForumList
            // 
            this.SearchForumList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SearchForumList.FormattingEnabled = true;
            this.SearchForumList.Items.AddRange(new object[] {
            "谷歌网页",
            "天涯"});
            this.SearchForumList.Location = new System.Drawing.Point(327, 10);
            this.SearchForumList.Margin = new System.Windows.Forms.Padding(2);
            this.SearchForumList.Name = "SearchForumList";
            this.SearchForumList.Size = new System.Drawing.Size(87, 20);
            this.SearchForumList.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "关键词";
            // 
            // keywordTxt
            // 
            this.keywordTxt.Location = new System.Drawing.Point(51, 10);
            this.keywordTxt.Margin = new System.Windows.Forms.Padding(2);
            this.keywordTxt.Name = "keywordTxt";
            this.keywordTxt.Size = new System.Drawing.Size(270, 21);
            this.keywordTxt.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tabControl2);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(817, 448);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "批量模式";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(2, 65);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(813, 381);
            this.tabControl2.TabIndex = 10;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.SearchChkList);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.BatchKeywordTxt);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage3.Size = new System.Drawing.Size(805, 355);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "关键词组";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // SearchChkList
            // 
            this.SearchChkList.CheckOnClick = true;
            this.SearchChkList.FormattingEnabled = true;
            this.SearchChkList.Items.AddRange(new object[] {
            "谷歌",
            "天涯"});
            this.SearchChkList.Location = new System.Drawing.Point(376, 33);
            this.SearchChkList.Margin = new System.Windows.Forms.Padding(2);
            this.SearchChkList.Name = "SearchChkList";
            this.SearchChkList.Size = new System.Drawing.Size(214, 308);
            this.SearchChkList.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(376, 14);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "搜索引擎";
            // 
            // BatchKeywordTxt
            // 
            this.BatchKeywordTxt.Location = new System.Drawing.Point(15, 33);
            this.BatchKeywordTxt.Margin = new System.Windows.Forms.Padding(2);
            this.BatchKeywordTxt.Multiline = true;
            this.BatchKeywordTxt.Name = "BatchKeywordTxt";
            this.BatchKeywordTxt.Size = new System.Drawing.Size(331, 308);
            this.BatchKeywordTxt.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 14);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "关键词组(一行一个)";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.SearchResultDataGridView);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage4.Size = new System.Drawing.Size(805, 355);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "搜索结果";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // SearchResultDataGridView
            // 
            this.SearchResultDataGridView.AllowUserToOrderColumns = true;
            this.SearchResultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SearchResultDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewLinkColumn1,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.KeywordCol,
            this.SearchEngineCol});
            this.SearchResultDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchResultDataGridView.Location = new System.Drawing.Point(2, 2);
            this.SearchResultDataGridView.Name = "SearchResultDataGridView";
            this.SearchResultDataGridView.RowTemplate.Height = 23;
            this.SearchResultDataGridView.Size = new System.Drawing.Size(801, 351);
            this.SearchResultDataGridView.TabIndex = 11;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "PubDate";
            this.dataGridViewTextBoxColumn1.HeaderText = "日期";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "MediaName";
            this.dataGridViewTextBoxColumn2.HeaderText = "媒体";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewLinkColumn1
            // 
            this.dataGridViewLinkColumn1.DataPropertyName = "Title";
            this.dataGridViewLinkColumn1.HeaderText = "标题";
            this.dataGridViewLinkColumn1.Name = "dataGridViewLinkColumn1";
            this.dataGridViewLinkColumn1.ReadOnly = true;
            this.dataGridViewLinkColumn1.Width = 350;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Author";
            this.dataGridViewTextBoxColumn3.HeaderText = "作者";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 70;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "View";
            this.dataGridViewTextBoxColumn4.HeaderText = "点击";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 70;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Comment";
            this.dataGridViewTextBoxColumn5.HeaderText = "评论";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 70;
            // 
            // KeywordCol
            // 
            this.KeywordCol.DataPropertyName = "Keyword";
            this.KeywordCol.HeaderText = "关键词";
            this.KeywordCol.Name = "KeywordCol";
            this.KeywordCol.ReadOnly = true;
            // 
            // SearchEngineCol
            // 
            this.SearchEngineCol.DataPropertyName = "SearchEngine";
            this.SearchEngineCol.HeaderText = "搜索引擎";
            this.SearchEngineCol.Name = "SearchEngineCol";
            this.SearchEngineCol.ReadOnly = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.BatchStatusLbl);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.BatchExportBtn);
            this.panel3.Controls.Add(this.BatchLimitNum);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.BatchStartDateTime);
            this.panel3.Controls.Add(this.BatchSearchBtn);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(2, 2);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(813, 63);
            this.panel3.TabIndex = 9;
            // 
            // BatchStatusLbl
            // 
            this.BatchStatusLbl.AutoSize = true;
            this.BatchStatusLbl.Location = new System.Drawing.Point(72, 40);
            this.BatchStatusLbl.Name = "BatchStatusLbl";
            this.BatchStatusLbl.Size = new System.Drawing.Size(0, 12);
            this.BatchStatusLbl.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "抓取状态:";
            // 
            // BatchExportBtn
            // 
            this.BatchExportBtn.Location = new System.Drawing.Point(357, 5);
            this.BatchExportBtn.Margin = new System.Windows.Forms.Padding(2);
            this.BatchExportBtn.Name = "BatchExportBtn";
            this.BatchExportBtn.Size = new System.Drawing.Size(38, 24);
            this.BatchExportBtn.TabIndex = 16;
            this.BatchExportBtn.Text = "导出";
            this.BatchExportBtn.UseVisualStyleBackColor = true;
            this.BatchExportBtn.Click += new System.EventHandler(this.BatchExportBtn_Click);
            // 
            // BatchLimitNum
            // 
            this.BatchLimitNum.Location = new System.Drawing.Point(56, 7);
            this.BatchLimitNum.Margin = new System.Windows.Forms.Padding(2);
            this.BatchLimitNum.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.BatchLimitNum.Name = "BatchLimitNum";
            this.BatchLimitNum.Size = new System.Drawing.Size(40, 21);
            this.BatchLimitNum.TabIndex = 15;
            this.BatchLimitNum.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 11);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "限定条数";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(101, 11);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "限定时间";
            // 
            // BatchStartDateTime
            // 
            this.BatchStartDateTime.Location = new System.Drawing.Point(152, 7);
            this.BatchStartDateTime.Margin = new System.Windows.Forms.Padding(2);
            this.BatchStartDateTime.Name = "BatchStartDateTime";
            this.BatchStartDateTime.Size = new System.Drawing.Size(126, 21);
            this.BatchStartDateTime.TabIndex = 12;
            // 
            // BatchSearchBtn
            // 
            this.BatchSearchBtn.Location = new System.Drawing.Point(292, 5);
            this.BatchSearchBtn.Margin = new System.Windows.Forms.Padding(2);
            this.BatchSearchBtn.Name = "BatchSearchBtn";
            this.BatchSearchBtn.Size = new System.Drawing.Size(56, 24);
            this.BatchSearchBtn.TabIndex = 11;
            this.BatchSearchBtn.Text = "搜 索";
            this.BatchSearchBtn.UseVisualStyleBackColor = true;
            this.BatchSearchBtn.Click += new System.EventHandler(this.BatchSearchBtn_Click);
            // 
            // CrawlTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 515);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CrawlTool";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Palas外部搜索工具";
            this.Load += new System.EventHandler(this.CrawlTool_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultDataGridView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxPageNum)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SearchResultDataGridView)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BatchLimitNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ExportBtn;
        private System.Windows.Forms.NumericUpDown maxPageNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker startDateTime;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.ComboBox SearchForumList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox keywordTxt;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckedListBox SearchChkList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox BatchKeywordTxt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button BatchExportBtn;
        private System.Windows.Forms.NumericUpDown BatchLimitNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker BatchStartDateTime;
        private System.Windows.Forms.Button BatchSearchBtn;
        private System.Windows.Forms.Label StatusLbl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView ResultDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn PubDateCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MediaCol;
        private System.Windows.Forms.DataGridViewLinkColumn TitleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn AuthorCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ViewCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommentCol;
        private System.Windows.Forms.DataGridView SearchResultDataGridView;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label BatchStatusLbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewLinkColumn dataGridViewLinkColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeywordCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchEngineCol;
    }
}
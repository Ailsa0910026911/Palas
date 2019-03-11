namespace Crawler.Host
{
    partial class CrawlSearch
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
            this.Label2 = new System.Windows.Forms.Label();
            this.KeywordTxt = new System.Windows.Forms.TextBox();
            this.CrawlBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.StartPageTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.EndPageTxt = new System.Windows.Forms.TextBox();
            this.AnalyzeBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchEngineChkList = new System.Windows.Forms.CheckedListBox();
            this.AddKeywordBtn = new System.Windows.Forms.Button();
            this.KeywordListbox = new System.Windows.Forms.ListBox();
            this.ExportToExcel = new System.Windows.Forms.Button();
            this.DetailChk = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(386, 251);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(71, 15);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Keyword:";
            // 
            // KeywordTxt
            // 
            this.KeywordTxt.Location = new System.Drawing.Point(466, 246);
            this.KeywordTxt.Margin = new System.Windows.Forms.Padding(4);
            this.KeywordTxt.Name = "KeywordTxt";
            this.KeywordTxt.Size = new System.Drawing.Size(216, 25);
            this.KeywordTxt.TabIndex = 3;
            // 
            // CrawlBtn
            // 
            this.CrawlBtn.Location = new System.Drawing.Point(22, 246);
            this.CrawlBtn.Margin = new System.Windows.Forms.Padding(4);
            this.CrawlBtn.Name = "CrawlBtn";
            this.CrawlBtn.Size = new System.Drawing.Size(100, 29);
            this.CrawlBtn.TabIndex = 4;
            this.CrawlBtn.Text = "抓取";
            this.CrawlBtn.UseVisualStyleBackColor = true;
            this.CrawlBtn.Click += new System.EventHandler(this.CrawlBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(386, 294);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "PageNum:";
            // 
            // StartPageTxt
            // 
            this.StartPageTxt.Location = new System.Drawing.Point(465, 289);
            this.StartPageTxt.Margin = new System.Windows.Forms.Padding(4);
            this.StartPageTxt.Name = "StartPageTxt";
            this.StartPageTxt.Size = new System.Drawing.Size(30, 25);
            this.StartPageTxt.TabIndex = 6;
            this.StartPageTxt.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(498, 294);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "—";
            // 
            // EndPageTxt
            // 
            this.EndPageTxt.Location = new System.Drawing.Point(523, 289);
            this.EndPageTxt.Margin = new System.Windows.Forms.Padding(4);
            this.EndPageTxt.Name = "EndPageTxt";
            this.EndPageTxt.Size = new System.Drawing.Size(30, 25);
            this.EndPageTxt.TabIndex = 8;
            this.EndPageTxt.Text = "9";
            // 
            // AnalyzeBtn
            // 
            this.AnalyzeBtn.Location = new System.Drawing.Point(222, 289);
            this.AnalyzeBtn.Margin = new System.Windows.Forms.Padding(4);
            this.AnalyzeBtn.Name = "AnalyzeBtn";
            this.AnalyzeBtn.Size = new System.Drawing.Size(100, 29);
            this.AnalyzeBtn.TabIndex = 9;
            this.AnalyzeBtn.Text = "分析";
            this.AnalyzeBtn.UseVisualStyleBackColor = true;
            this.AnalyzeBtn.Click += new System.EventHandler(this.AnalyzeBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Engine:";
            // 
            // SearchEngineChkList
            // 
            this.SearchEngineChkList.FormattingEnabled = true;
            this.SearchEngineChkList.Location = new System.Drawing.Point(22, 42);
            this.SearchEngineChkList.Margin = new System.Windows.Forms.Padding(4);
            this.SearchEngineChkList.Name = "SearchEngineChkList";
            this.SearchEngineChkList.Size = new System.Drawing.Size(324, 184);
            this.SearchEngineChkList.TabIndex = 11;
            // 
            // AddKeywordBtn
            // 
            this.AddKeywordBtn.Location = new System.Drawing.Point(575, 287);
            this.AddKeywordBtn.Margin = new System.Windows.Forms.Padding(4);
            this.AddKeywordBtn.Name = "AddKeywordBtn";
            this.AddKeywordBtn.Size = new System.Drawing.Size(107, 29);
            this.AddKeywordBtn.TabIndex = 12;
            this.AddKeywordBtn.Text = "添加到列表";
            this.AddKeywordBtn.UseVisualStyleBackColor = true;
            this.AddKeywordBtn.Click += new System.EventHandler(this.AddKeywordBtn_Click);
            // 
            // KeywordListbox
            // 
            this.KeywordListbox.FormattingEnabled = true;
            this.KeywordListbox.ItemHeight = 15;
            this.KeywordListbox.Location = new System.Drawing.Point(382, 42);
            this.KeywordListbox.Margin = new System.Windows.Forms.Padding(4);
            this.KeywordListbox.Name = "KeywordListbox";
            this.KeywordListbox.Size = new System.Drawing.Size(300, 184);
            this.KeywordListbox.TabIndex = 13;
            // 
            // ExportToExcel
            // 
            this.ExportToExcel.Location = new System.Drawing.Point(22, 289);
            this.ExportToExcel.Margin = new System.Windows.Forms.Padding(4);
            this.ExportToExcel.Name = "ExportToExcel";
            this.ExportToExcel.Size = new System.Drawing.Size(168, 29);
            this.ExportToExcel.TabIndex = 14;
            this.ExportToExcel.Text = "导出到Excel";
            this.ExportToExcel.UseVisualStyleBackColor = true;
            this.ExportToExcel.Click += new System.EventHandler(this.ExportToExcel_Click);
            // 
            // DetailChk
            // 
            this.DetailChk.AutoSize = true;
            this.DetailChk.Checked = true;
            this.DetailChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DetailChk.Location = new System.Drawing.Point(139, 252);
            this.DetailChk.Margin = new System.Windows.Forms.Padding(4);
            this.DetailChk.Name = "DetailChk";
            this.DetailChk.Size = new System.Drawing.Size(194, 19);
            this.DetailChk.TabIndex = 15;
            this.DetailChk.Text = "文章二次抓取（很慢哦）";
            this.DetailChk.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(464, 11);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(218, 23);
            this.progressBar1.TabIndex = 16;
            this.progressBar1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(382, 16);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "已添加词:";
            // 
            // StartDate
            // 
            this.StartDate.CustomFormat = "yyyy-M-d";
            this.StartDate.Enabled = false;
            this.StartDate.Location = new System.Drawing.Point(464, 332);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(164, 25);
            this.StartDate.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(386, 337);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "After:";
            // 
            // CrawlSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 373);
            this.ControlBox = false;
            this.Controls.Add(this.StartDate);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.DetailChk);
            this.Controls.Add(this.ExportToExcel);
            this.Controls.Add(this.KeywordListbox);
            this.Controls.Add(this.AddKeywordBtn);
            this.Controls.Add(this.SearchEngineChkList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AnalyzeBtn);
            this.Controls.Add(this.EndPageTxt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StartPageTxt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CrawlBtn);
            this.Controls.Add(this.KeywordTxt);
            this.Controls.Add(this.Label2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CrawlSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CrawlSearch";
            this.Load += new System.EventHandler(this.CrawlSearch_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.TextBox KeywordTxt;
        private System.Windows.Forms.Button CrawlBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox StartPageTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox EndPageTxt;
        private System.Windows.Forms.Button AnalyzeBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox SearchEngineChkList;
        private System.Windows.Forms.Button AddKeywordBtn;
        private System.Windows.Forms.ListBox KeywordListbox;
        private System.Windows.Forms.Button ExportToExcel;
        private System.Windows.Forms.CheckBox DetailChk;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.Label label6;
    }
}
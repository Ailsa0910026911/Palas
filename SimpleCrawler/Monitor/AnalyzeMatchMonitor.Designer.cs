namespace SimpleCrawler
{
    partial class AnalyzeMatchMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalyzeMatchMonitor));
            this.MonitorGridView = new System.Windows.Forms.DataGridView();
            this.IDCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentUrlCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalCrawlCnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartDateCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.MonitorGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MonitorGridView
            // 
            this.MonitorGridView.AllowUserToAddRows = false;
            this.MonitorGridView.AllowUserToDeleteRows = false;
            this.MonitorGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.MonitorGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.MonitorGridView.BackgroundColor = System.Drawing.Color.White;
            this.MonitorGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MonitorGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDCol,
            this.CurrentUrlCol,
            this.TotalCrawlCnt,
            this.StartDateCol});
            this.MonitorGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MonitorGridView.Location = new System.Drawing.Point(0, 0);
            this.MonitorGridView.MultiSelect = false;
            this.MonitorGridView.Name = "MonitorGridView";
            this.MonitorGridView.ReadOnly = true;
            this.MonitorGridView.RowHeadersVisible = false;
            this.MonitorGridView.RowTemplate.Height = 23;
            this.MonitorGridView.Size = new System.Drawing.Size(581, 382);
            this.MonitorGridView.TabIndex = 2;
            // 
            // IDCol
            // 
            this.IDCol.DataPropertyName = "ID";
            this.IDCol.Frozen = true;
            this.IDCol.HeaderText = "实体类型";
            this.IDCol.Name = "IDCol";
            this.IDCol.ReadOnly = true;
            this.IDCol.Width = 80;
            // 
            // CurrentUrlCol
            // 
            this.CurrentUrlCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CurrentUrlCol.DataPropertyName = "Url";
            this.CurrentUrlCol.HeaderText = "当前任务";
            this.CurrentUrlCol.Name = "CurrentUrlCol";
            this.CurrentUrlCol.ReadOnly = true;
            this.CurrentUrlCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // TotalCrawlCnt
            // 
            this.TotalCrawlCnt.DataPropertyName = "TotalCnt";
            this.TotalCrawlCnt.HeaderText = "总分析次数";
            this.TotalCrawlCnt.Name = "TotalCrawlCnt";
            this.TotalCrawlCnt.ReadOnly = true;
            this.TotalCrawlCnt.Width = 92;
            // 
            // StartDateCol
            // 
            this.StartDateCol.DataPropertyName = "StartDate";
            this.StartDateCol.HeaderText = "启动时间";
            this.StartDateCol.Name = "StartDateCol";
            this.StartDateCol.ReadOnly = true;
            this.StartDateCol.Width = 80;
            // 
            // AnalyzeMatchMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(581, 382);
            this.Controls.Add(this.MonitorGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AnalyzeMatchMonitor";
            this.Text = "分析服务监控";
            this.Load += new System.EventHandler(this.AnalyzeMatchMonitor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MonitorGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView MonitorGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentUrlCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCrawlCnt;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartDateCol;
    }
}
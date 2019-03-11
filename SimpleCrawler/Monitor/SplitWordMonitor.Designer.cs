namespace SimpleCrawler
{
    partial class SplitWordMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitWordMonitor));
            this.MonitorGridView = new System.Windows.Forms.DataGridView();
            this.TotalCrawlCnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartDateCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListenUrlCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.MonitorGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MonitorGridView
            // 
            this.MonitorGridView.AllowUserToAddRows = false;
            this.MonitorGridView.AllowUserToDeleteRows = false;
            this.MonitorGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MonitorGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TotalCrawlCnt,
            this.StartDateCol,
            this.ListenUrlCol});
            this.MonitorGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MonitorGridView.Location = new System.Drawing.Point(0, 0);
            this.MonitorGridView.Name = "MonitorGridView";
            this.MonitorGridView.ReadOnly = true;
            this.MonitorGridView.RowHeadersVisible = false;
            this.MonitorGridView.RowTemplate.Height = 23;
            this.MonitorGridView.Size = new System.Drawing.Size(551, 341);
            this.MonitorGridView.TabIndex = 0;
            // 
            // TotalCrawlCnt
            // 
            this.TotalCrawlCnt.DataPropertyName = "TotalCnt";
            this.TotalCrawlCnt.HeaderText = "总抓取次数";
            this.TotalCrawlCnt.Name = "TotalCrawlCnt";
            this.TotalCrawlCnt.ReadOnly = true;
            // 
            // StartDateCol
            // 
            this.StartDateCol.DataPropertyName = "StartDate";
            this.StartDateCol.HeaderText = "启动时间";
            this.StartDateCol.Name = "StartDateCol";
            this.StartDateCol.ReadOnly = true;
            // 
            // ListenUrlCol
            // 
            this.ListenUrlCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ListenUrlCol.DataPropertyName = "ListenUrl";
            this.ListenUrlCol.HeaderText = "侦听地址";
            this.ListenUrlCol.Name = "ListenUrlCol";
            this.ListenUrlCol.ReadOnly = true;
            // 
            // SplitWordMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 341);
            this.Controls.Add(this.MonitorGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplitWordMonitor";
            this.Text = "分词监控";
            this.Load += new System.EventHandler(this.SpliteWordMonitor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MonitorGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView MonitorGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCrawlCnt;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartDateCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListenUrlCol;
    }
}
using System.Diagnostics;
namespace SimpleCrawler
{
    partial class CrawlHost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrawlHost));
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.PipeGridView = new System.Windows.Forms.DataGridView();
            this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentJobCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentStatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JobCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MessageCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ContentTxt = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.PNetworkLbl = new System.Windows.Forms.Label();
            this.PCPULbl = new System.Windows.Forms.Label();
            this.PErrorCntLbl = new System.Windows.Forms.Label();
            this.PItemCntLbl = new System.Windows.Forms.Label();
            this.PJobCntLbl = new System.Windows.Forms.Label();
            this.PJobStartTimeLbl = new System.Windows.Forms.Label();
            this.PJobLbl = new System.Windows.Forms.Label();
            this.PStatusLbl = new System.Windows.Forms.Label();
            this.PNextRuntimeLbl = new System.Windows.Forms.Label();
            this.PStartTimeLbl = new System.Windows.Forms.Label();
            this.PNameLbl = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SContentTxt = new System.Windows.Forms.TextBox();
            this.SStartTimeLbl = new System.Windows.Forms.Label();
            this.SErrorCntLbl = new System.Windows.Forms.Label();
            this.SCrawlCntLbl = new System.Windows.Forms.Label();
            this.SJobCntLbl = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PipeGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Enabled = true;
            this.RefreshTimer.Interval = 1000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // PipeGridView
            // 
            this.PipeGridView.AllowUserToAddRows = false;
            this.PipeGridView.AllowUserToDeleteRows = false;
            this.PipeGridView.AllowUserToOrderColumns = true;
            this.PipeGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PipeGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.PipeGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.PipeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PipeGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameCol,
            this.CurrentJobCol,
            this.CurrentStatusCol,
            this.JobCol,
            this.ErrorCol,
            this.MessageCol});
            this.PipeGridView.Location = new System.Drawing.Point(11, 173);
            this.PipeGridView.Name = "PipeGridView";
            this.PipeGridView.ReadOnly = true;
            this.PipeGridView.RowHeadersVisible = false;
            this.PipeGridView.RowTemplate.Height = 23;
            this.PipeGridView.Size = new System.Drawing.Size(761, 174);
            this.PipeGridView.TabIndex = 0;
            this.PipeGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PipeGridView_CellClick);
            // 
            // NameCol
            // 
            this.NameCol.DataPropertyName = "Name";
            this.NameCol.HeaderText = "名称";
            this.NameCol.Name = "NameCol";
            this.NameCol.ReadOnly = true;
            this.NameCol.Width = 56;
            // 
            // CurrentJobCol
            // 
            this.CurrentJobCol.DataPropertyName = "CurrentJob_Name";
            this.CurrentJobCol.HeaderText = "当前任务";
            this.CurrentJobCol.Name = "CurrentJobCol";
            this.CurrentJobCol.ReadOnly = true;
            this.CurrentJobCol.Width = 80;
            // 
            // CurrentStatusCol
            // 
            this.CurrentStatusCol.DataPropertyName = "Status";
            this.CurrentStatusCol.HeaderText = "当前状态";
            this.CurrentStatusCol.Name = "CurrentStatusCol";
            this.CurrentStatusCol.ReadOnly = true;
            this.CurrentStatusCol.Width = 80;
            // 
            // JobCol
            // 
            this.JobCol.DataPropertyName = "JobCount";
            this.JobCol.HeaderText = "任务";
            this.JobCol.Name = "JobCol";
            this.JobCol.ReadOnly = true;
            this.JobCol.Width = 56;
            // 
            // ErrorCol
            // 
            this.ErrorCol.DataPropertyName = "ErrorCount";
            this.ErrorCol.HeaderText = "出错";
            this.ErrorCol.Name = "ErrorCol";
            this.ErrorCol.ReadOnly = true;
            this.ErrorCol.Width = 56;
            // 
            // MessageCol
            // 
            this.MessageCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MessageCol.DataPropertyName = "Message";
            this.MessageCol.HeaderText = "最新消息";
            this.MessageCol.Name = "MessageCol";
            this.MessageCol.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 353);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 242);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "管道详情";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.ContentTxt);
            this.groupBox3.Location = new System.Drawing.Point(405, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(348, 210);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "消息列表:";
            // 
            // ContentTxt
            // 
            this.ContentTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContentTxt.BackColor = System.Drawing.Color.White;
            this.ContentTxt.Location = new System.Drawing.Point(7, 22);
            this.ContentTxt.Multiline = true;
            this.ContentTxt.Name = "ContentTxt";
            this.ContentTxt.ReadOnly = true;
            this.ContentTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ContentTxt.Size = new System.Drawing.Size(335, 182);
            this.ContentTxt.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.PNetworkLbl);
            this.groupBox2.Controls.Add(this.PCPULbl);
            this.groupBox2.Controls.Add(this.PErrorCntLbl);
            this.groupBox2.Controls.Add(this.PItemCntLbl);
            this.groupBox2.Controls.Add(this.PJobCntLbl);
            this.groupBox2.Controls.Add(this.PJobStartTimeLbl);
            this.groupBox2.Controls.Add(this.PJobLbl);
            this.groupBox2.Controls.Add(this.PStatusLbl);
            this.groupBox2.Controls.Add(this.PNextRuntimeLbl);
            this.groupBox2.Controls.Add(this.PStartTimeLbl);
            this.groupBox2.Controls.Add(this.PNameLbl);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(7, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 210);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基本信息:";
            // 
            // PNetworkLbl
            // 
            this.PNetworkLbl.AutoSize = true;
            this.PNetworkLbl.Location = new System.Drawing.Point(315, 169);
            this.PNetworkLbl.Name = "PNetworkLbl";
            this.PNetworkLbl.Size = new System.Drawing.Size(29, 13);
            this.PNetworkLbl.TabIndex = 21;
            this.PNetworkLbl.Text = "[Cnt]";
            // 
            // PCPULbl
            // 
            this.PCPULbl.AutoSize = true;
            this.PCPULbl.Location = new System.Drawing.Point(315, 141);
            this.PCPULbl.Name = "PCPULbl";
            this.PCPULbl.Size = new System.Drawing.Size(29, 13);
            this.PCPULbl.TabIndex = 20;
            this.PCPULbl.Text = "[Cnt]";
            // 
            // PErrorCntLbl
            // 
            this.PErrorCntLbl.AutoSize = true;
            this.PErrorCntLbl.Location = new System.Drawing.Point(315, 85);
            this.PErrorCntLbl.Name = "PErrorCntLbl";
            this.PErrorCntLbl.Size = new System.Drawing.Size(29, 13);
            this.PErrorCntLbl.TabIndex = 19;
            this.PErrorCntLbl.Text = "[Cnt]";
            // 
            // PItemCntLbl
            // 
            this.PItemCntLbl.AutoSize = true;
            this.PItemCntLbl.Location = new System.Drawing.Point(315, 53);
            this.PItemCntLbl.Name = "PItemCntLbl";
            this.PItemCntLbl.Size = new System.Drawing.Size(29, 13);
            this.PItemCntLbl.TabIndex = 18;
            this.PItemCntLbl.Text = "[Cnt]";
            // 
            // PJobCntLbl
            // 
            this.PJobCntLbl.AutoSize = true;
            this.PJobCntLbl.Location = new System.Drawing.Point(315, 23);
            this.PJobCntLbl.Name = "PJobCntLbl";
            this.PJobCntLbl.Size = new System.Drawing.Size(29, 13);
            this.PJobCntLbl.TabIndex = 17;
            this.PJobCntLbl.Text = "[Cnt]";
            // 
            // PJobStartTimeLbl
            // 
            this.PJobStartTimeLbl.AutoSize = true;
            this.PJobStartTimeLbl.Location = new System.Drawing.Point(126, 169);
            this.PJobStartTimeLbl.Name = "PJobStartTimeLbl";
            this.PJobStartTimeLbl.Size = new System.Drawing.Size(29, 13);
            this.PJobStartTimeLbl.TabIndex = 16;
            this.PJobStartTimeLbl.Text = "[Cnt]";
            // 
            // PJobLbl
            // 
            this.PJobLbl.AutoSize = true;
            this.PJobLbl.Location = new System.Drawing.Point(74, 114);
            this.PJobLbl.Name = "PJobLbl";
            this.PJobLbl.Size = new System.Drawing.Size(29, 13);
            this.PJobLbl.TabIndex = 15;
            this.PJobLbl.Text = "[Cnt]";
            // 
            // PStatusLbl
            // 
            this.PStatusLbl.AutoSize = true;
            this.PStatusLbl.Location = new System.Drawing.Point(74, 51);
            this.PStatusLbl.Name = "PStatusLbl";
            this.PStatusLbl.Size = new System.Drawing.Size(29, 13);
            this.PStatusLbl.TabIndex = 14;
            this.PStatusLbl.Text = "[Cnt]";
            // 
            // PNextRuntimeLbl
            // 
            this.PNextRuntimeLbl.AutoSize = true;
            this.PNextRuntimeLbl.Location = new System.Drawing.Point(95, 141);
            this.PNextRuntimeLbl.Name = "PNextRuntimeLbl";
            this.PNextRuntimeLbl.Size = new System.Drawing.Size(29, 13);
            this.PNextRuntimeLbl.TabIndex = 13;
            this.PNextRuntimeLbl.Text = "[Cnt]";
            // 
            // PStartTimeLbl
            // 
            this.PStartTimeLbl.AutoSize = true;
            this.PStartTimeLbl.Location = new System.Drawing.Point(74, 85);
            this.PStartTimeLbl.Name = "PStartTimeLbl";
            this.PStartTimeLbl.Size = new System.Drawing.Size(29, 13);
            this.PStartTimeLbl.TabIndex = 12;
            this.PStartTimeLbl.Text = "[Cnt]";
            // 
            // PNameLbl
            // 
            this.PNameLbl.AutoSize = true;
            this.PNameLbl.Location = new System.Drawing.Point(74, 23);
            this.PNameLbl.Name = "PNameLbl";
            this.PNameLbl.Size = new System.Drawing.Size(29, 13);
            this.PNameLbl.TabIndex = 11;
            this.PNameLbl.Text = "[Cnt]";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(256, 169);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "网络使用:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(262, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "CPU使用:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "当前状态:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "下次运行时间:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "开始时间:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "名称:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(232, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "累计出错次数:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(208, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "累计抓取的条目数:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "累计的任务数:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "当前任务开始时间:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "当前任务:";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.SContentTxt);
            this.groupBox4.Controls.Add(this.SStartTimeLbl);
            this.groupBox4.Controls.Add(this.SErrorCntLbl);
            this.groupBox4.Controls.Add(this.SCrawlCntLbl);
            this.groupBox4.Controls.Add(this.SJobCntLbl);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Location = new System.Drawing.Point(13, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(759, 163);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "全局信息";
            // 
            // SContentTxt
            // 
            this.SContentTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SContentTxt.BackColor = System.Drawing.Color.White;
            this.SContentTxt.Location = new System.Drawing.Point(11, 42);
            this.SContentTxt.Multiline = true;
            this.SContentTxt.Name = "SContentTxt";
            this.SContentTxt.ReadOnly = true;
            this.SContentTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SContentTxt.Size = new System.Drawing.Size(741, 112);
            this.SContentTxt.TabIndex = 9;
            // 
            // SStartTimeLbl
            // 
            this.SStartTimeLbl.AutoSize = true;
            this.SStartTimeLbl.Location = new System.Drawing.Point(387, 26);
            this.SStartTimeLbl.Name = "SStartTimeLbl";
            this.SStartTimeLbl.Size = new System.Drawing.Size(29, 13);
            this.SStartTimeLbl.TabIndex = 8;
            this.SStartTimeLbl.Text = "[Cnt]";
            // 
            // SErrorCntLbl
            // 
            this.SErrorCntLbl.AutoSize = true;
            this.SErrorCntLbl.Location = new System.Drawing.Point(280, 26);
            this.SErrorCntLbl.Name = "SErrorCntLbl";
            this.SErrorCntLbl.Size = new System.Drawing.Size(29, 13);
            this.SErrorCntLbl.TabIndex = 7;
            this.SErrorCntLbl.Text = "[Cnt]";
            // 
            // SCrawlCntLbl
            // 
            this.SCrawlCntLbl.AutoSize = true;
            this.SCrawlCntLbl.Location = new System.Drawing.Point(177, 26);
            this.SCrawlCntLbl.Name = "SCrawlCntLbl";
            this.SCrawlCntLbl.Size = new System.Drawing.Size(29, 13);
            this.SCrawlCntLbl.TabIndex = 6;
            this.SCrawlCntLbl.Text = "[Cnt]";
            // 
            // SJobCntLbl
            // 
            this.SJobCntLbl.AutoSize = true;
            this.SJobCntLbl.Location = new System.Drawing.Point(56, 26);
            this.SJobCntLbl.Name = "SJobCntLbl";
            this.SJobCntLbl.Size = new System.Drawing.Size(29, 13);
            this.SJobCntLbl.TabIndex = 5;
            this.SJobCntLbl.Text = "[Cnt]";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(322, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(58, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "开始时间:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(227, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "出错数:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(124, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "抓取数:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "任务数:";
            // 
            // CrawlHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 609);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.PipeGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CrawlHost";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "爬虫状态监控";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CrawlHost_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PipeGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.DataGridView PipeGridView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label SStartTimeLbl;
        private System.Windows.Forms.Label SErrorCntLbl;
        private System.Windows.Forms.Label SCrawlCntLbl;
        private System.Windows.Forms.Label SJobCntLbl;
        private System.Windows.Forms.Label PNetworkLbl;
        private System.Windows.Forms.Label PCPULbl;
        private System.Windows.Forms.Label PErrorCntLbl;
        private System.Windows.Forms.Label PItemCntLbl;
        private System.Windows.Forms.Label PJobCntLbl;
        private System.Windows.Forms.Label PJobStartTimeLbl;
        private System.Windows.Forms.Label PJobLbl;
        private System.Windows.Forms.Label PStatusLbl;
        private System.Windows.Forms.Label PNextRuntimeLbl;
        private System.Windows.Forms.Label PStartTimeLbl;
        private System.Windows.Forms.Label PNameLbl;
        private System.Windows.Forms.TextBox ContentTxt;
        private System.Windows.Forms.TextBox SContentTxt;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentJobCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentStatusCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn JobCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrorCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MessageCol;
    }
}
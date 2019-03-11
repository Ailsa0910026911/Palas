namespace Crawler.Host
{
    partial class TestForm
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
            this.TestBtn = new System.Windows.Forms.Button();
            this.ResultGridView = new System.Windows.Forms.DataGridView();
            this.AnalyzeBtn = new System.Windows.Forms.Button();
            this.UrlTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TitleTxt = new System.Windows.Forms.TextBox();
            this.TagTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.ConnectTestBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(24, 13);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(75, 23);
            this.TestBtn.TabIndex = 0;
            this.TestBtn.Text = "Test";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // ResultGridView
            // 
            this.ResultGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultGridView.Location = new System.Drawing.Point(13, 98);
            this.ResultGridView.Name = "ResultGridView";
            this.ResultGridView.RowTemplate.Height = 23;
            this.ResultGridView.Size = new System.Drawing.Size(654, 263);
            this.ResultGridView.TabIndex = 1;
            // 
            // AnalyzeBtn
            // 
            this.AnalyzeBtn.Location = new System.Drawing.Point(445, 11);
            this.AnalyzeBtn.Name = "AnalyzeBtn";
            this.AnalyzeBtn.Size = new System.Drawing.Size(105, 23);
            this.AnalyzeBtn.TabIndex = 2;
            this.AnalyzeBtn.Text = "分析下载页面";
            this.AnalyzeBtn.UseVisualStyleBackColor = true;
            this.AnalyzeBtn.Click += new System.EventHandler(this.AnalyzeBtn_Click);
            // 
            // UrlTxt
            // 
            this.UrlTxt.Location = new System.Drawing.Point(174, 10);
            this.UrlTxt.Name = "UrlTxt";
            this.UrlTxt.Size = new System.Drawing.Size(265, 21);
            this.UrlTxt.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Url:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Title:";
            // 
            // TitleTxt
            // 
            this.TitleTxt.Location = new System.Drawing.Point(174, 39);
            this.TitleTxt.Name = "TitleTxt";
            this.TitleTxt.Size = new System.Drawing.Size(265, 21);
            this.TitleTxt.TabIndex = 6;
            // 
            // TagTxt
            // 
            this.TagTxt.Location = new System.Drawing.Point(174, 67);
            this.TagTxt.Name = "TagTxt";
            this.TagTxt.Size = new System.Drawing.Size(147, 21);
            this.TagTxt.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "根据下面列分组Tag";
            // 
            // StatusLbl
            // 
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(22, 48);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(59, 12);
            this.StatusLbl.TabIndex = 9;
            this.StatusLbl.Text = "StatusLbl";
            // 
            // ConnectTestBtn
            // 
            this.ConnectTestBtn.Location = new System.Drawing.Point(556, 13);
            this.ConnectTestBtn.Name = "ConnectTestBtn";
            this.ConnectTestBtn.Size = new System.Drawing.Size(75, 23);
            this.ConnectTestBtn.TabIndex = 10;
            this.ConnectTestBtn.Text = "连接";
            this.ConnectTestBtn.UseVisualStyleBackColor = true;
            this.ConnectTestBtn.Click += new System.EventHandler(this.ConnectTestBtn_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 373);
            this.Controls.Add(this.ConnectTestBtn);
            this.Controls.Add(this.StatusLbl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TagTxt);
            this.Controls.Add(this.TitleTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UrlTxt);
            this.Controls.Add(this.AnalyzeBtn);
            this.Controls.Add(this.ResultGridView);
            this.Controls.Add(this.TestBtn);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TestBtn;
        private System.Windows.Forms.DataGridView ResultGridView;
        private System.Windows.Forms.Button AnalyzeBtn;
        private System.Windows.Forms.TextBox UrlTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TitleTxt;
        private System.Windows.Forms.TextBox TagTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label StatusLbl;
        private System.Windows.Forms.Button ConnectTestBtn;

    }
}
namespace Crawler.Host
{
    partial class TestBaidu
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
            this.CrawlBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CrawlBtn
            // 
            this.CrawlBtn.Location = new System.Drawing.Point(75, 28);
            this.CrawlBtn.Name = "CrawlBtn";
            this.CrawlBtn.Size = new System.Drawing.Size(75, 23);
            this.CrawlBtn.TabIndex = 0;
            this.CrawlBtn.Text = "抓取";
            this.CrawlBtn.UseVisualStyleBackColor = true;
            this.CrawlBtn.Click += new System.EventHandler(this.CrawlBtn_Click);
            // 
            // TestBaidu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 82);
            this.Controls.Add(this.CrawlBtn);
            this.Name = "TestBaidu";
            this.Text = "TestBaidu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CrawlBtn;
    }
}
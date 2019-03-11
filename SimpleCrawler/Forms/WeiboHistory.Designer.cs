namespace Crawler.Host
{
    partial class WeiboHistory
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
            this.ExportBtn = new System.Windows.Forms.Button();
            this.StartDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.EndDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MaxUpdown = new System.Windows.Forms.NumericUpDown();
            this.MinUpdown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinUpdown)).BeginInit();
            this.SuspendLayout();
            // 
            // ExportBtn
            // 
            this.ExportBtn.Location = new System.Drawing.Point(117, 101);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(75, 23);
            this.ExportBtn.TabIndex = 2;
            this.ExportBtn.Text = "导出";
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // StartDatePicker
            // 
            this.StartDatePicker.Location = new System.Drawing.Point(77, 3);
            this.StartDatePicker.Name = "StartDatePicker";
            this.StartDatePicker.Size = new System.Drawing.Size(200, 21);
            this.StartDatePicker.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "开始时间:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "结束时间:";
            // 
            // EndDatePicker
            // 
            this.EndDatePicker.Location = new System.Drawing.Point(77, 32);
            this.EndDatePicker.Name = "EndDatePicker";
            this.EndDatePicker.Size = new System.Drawing.Size(200, 21);
            this.EndDatePicker.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "最小时点数:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(137, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "最大时点数:";
            // 
            // MaxUpdown
            // 
            this.MaxUpdown.Location = new System.Drawing.Point(214, 65);
            this.MaxUpdown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MaxUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxUpdown.Name = "MaxUpdown";
            this.MaxUpdown.Size = new System.Drawing.Size(62, 21);
            this.MaxUpdown.TabIndex = 9;
            this.MaxUpdown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // MinUpdown
            // 
            this.MinUpdown.Location = new System.Drawing.Point(89, 65);
            this.MinUpdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinUpdown.Name = "MinUpdown";
            this.MinUpdown.Size = new System.Drawing.Size(42, 21);
            this.MinUpdown.TabIndex = 10;
            this.MinUpdown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // WeiboHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 136);
            this.Controls.Add(this.MinUpdown);
            this.Controls.Add(this.MaxUpdown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EndDatePicker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StartDatePicker);
            this.Controls.Add(this.ExportBtn);
            this.Name = "WeiboHistory";
            this.Text = "WeiboHistory";
            this.Load += new System.EventHandler(this.WeiboHistory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinUpdown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExportBtn;
        private System.Windows.Forms.DateTimePicker StartDatePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker EndDatePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown MaxUpdown;
        private System.Windows.Forms.NumericUpDown MinUpdown;
    }
}
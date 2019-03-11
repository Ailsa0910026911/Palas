namespace Crawler.Host
{
    partial class SinaSearch
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.AddQueryBtn = new System.Windows.Forms.Button();
            this.SearchWeiboBtn = new System.Windows.Forms.Button();
            this.AddtionQueryTxt = new System.Windows.Forms.TextBox();
            this.EndDateTxt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.StartDateTxt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.OriginChk = new System.Windows.Forms.CheckBox();
            this.VipChk = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SortDropDown = new System.Windows.Forms.ComboBox();
            this.EndPageNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.StartPageNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.KeywordTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.WeiboSearchKeywordListbox = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.AttrDropdown = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.AgeDropdown = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.GenderDropdown = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.CertDropdown = new System.Windows.Forms.ComboBox();
            this.AddPeopleQueryBtn = new System.Windows.Forms.Button();
            this.SearchPeopleBtn = new System.Windows.Forms.Button();
            this.PeopleAddtionQueryTxt = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.WeiboPeopleEndPageNum = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.WeiboPeopleStartPageNum = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.WeiboPeopleKeywordTxt = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.WeiboPeopleSearchListbox = new System.Windows.Forms.ListBox();
            this.searchAllChk = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EndPageNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPageNum)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WeiboPeopleEndPageNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeiboPeopleStartPageNum)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(602, 361);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.searchAllChk);
            this.tabPage1.Controls.Add(this.AddQueryBtn);
            this.tabPage1.Controls.Add(this.SearchWeiboBtn);
            this.tabPage1.Controls.Add(this.AddtionQueryTxt);
            this.tabPage1.Controls.Add(this.EndDateTxt);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.StartDateTxt);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.OriginChk);
            this.tabPage1.Controls.Add(this.VipChk);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.SortDropDown);
            this.tabPage1.Controls.Add(this.EndPageNum);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.StartPageNum);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.KeywordTxt);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.WeiboSearchKeywordListbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(594, 335);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "搜索微博";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // AddQueryBtn
            // 
            this.AddQueryBtn.Location = new System.Drawing.Point(203, 230);
            this.AddQueryBtn.Name = "AddQueryBtn";
            this.AddQueryBtn.Size = new System.Drawing.Size(144, 23);
            this.AddQueryBtn.TabIndex = 18;
            this.AddQueryBtn.Text = "添加此查询条件";
            this.AddQueryBtn.UseVisualStyleBackColor = true;
            this.AddQueryBtn.Click += new System.EventHandler(this.AddQueryBtn_Click);
            // 
            // SearchWeiboBtn
            // 
            this.SearchWeiboBtn.Location = new System.Drawing.Point(240, 306);
            this.SearchWeiboBtn.Name = "SearchWeiboBtn";
            this.SearchWeiboBtn.Size = new System.Drawing.Size(75, 23);
            this.SearchWeiboBtn.TabIndex = 17;
            this.SearchWeiboBtn.Text = "搜索";
            this.SearchWeiboBtn.UseVisualStyleBackColor = true;
            this.SearchWeiboBtn.Click += new System.EventHandler(this.SearchWeiboBtn_Click);
            // 
            // AddtionQueryTxt
            // 
            this.AddtionQueryTxt.Location = new System.Drawing.Point(114, 201);
            this.AddtionQueryTxt.Name = "AddtionQueryTxt";
            this.AddtionQueryTxt.Size = new System.Drawing.Size(385, 21);
            this.AddtionQueryTxt.TabIndex = 16;
            // 
            // EndDateTxt
            // 
            this.EndDateTxt.Location = new System.Drawing.Point(188, 169);
            this.EndDateTxt.Name = "EndDateTxt";
            this.EndDateTxt.Size = new System.Drawing.Size(100, 21);
            this.EndDateTxt.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(159, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "---";
            // 
            // StartDateTxt
            // 
            this.StartDateTxt.Location = new System.Drawing.Point(53, 169);
            this.StartDateTxt.Name = "StartDateTxt";
            this.StartDateTxt.Size = new System.Drawing.Size(100, 21);
            this.StartDateTxt.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "时间:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "附加查询Url参数:";
            // 
            // OriginChk
            // 
            this.OriginChk.AutoSize = true;
            this.OriginChk.Location = new System.Drawing.Point(193, 142);
            this.OriginChk.Name = "OriginChk";
            this.OriginChk.Size = new System.Drawing.Size(48, 16);
            this.OriginChk.TabIndex = 10;
            this.OriginChk.Text = "原创";
            this.OriginChk.UseVisualStyleBackColor = true;
            // 
            // VipChk
            // 
            this.VipChk.AutoSize = true;
            this.VipChk.Location = new System.Drawing.Point(139, 142);
            this.VipChk.Name = "VipChk";
            this.VipChk.Size = new System.Drawing.Size(48, 16);
            this.VipChk.TabIndex = 9;
            this.VipChk.Text = "认证";
            this.VipChk.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "排序:";
            // 
            // SortDropDown
            // 
            this.SortDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SortDropDown.FormattingEnabled = true;
            this.SortDropDown.Items.AddRange(new object[] {
            "默认",
            "实时",
            "热门"});
            this.SortDropDown.Location = new System.Drawing.Point(53, 140);
            this.SortDropDown.Name = "SortDropDown";
            this.SortDropDown.Size = new System.Drawing.Size(80, 20);
            this.SortDropDown.TabIndex = 7;
            // 
            // EndPageNum
            // 
            this.EndPageNum.Location = new System.Drawing.Point(130, 112);
            this.EndPageNum.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.EndPageNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.EndPageNum.Name = "EndPageNum";
            this.EndPageNum.Size = new System.Drawing.Size(44, 21);
            this.EndPageNum.TabIndex = 6;
            this.EndPageNum.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(101, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "---";
            // 
            // StartPageNum
            // 
            this.StartPageNum.Location = new System.Drawing.Point(53, 112);
            this.StartPageNum.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.StartPageNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.StartPageNum.Name = "StartPageNum";
            this.StartPageNum.Size = new System.Drawing.Size(42, 21);
            this.StartPageNum.TabIndex = 4;
            this.StartPageNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "页码:";
            // 
            // KeywordTxt
            // 
            this.KeywordTxt.Location = new System.Drawing.Point(53, 85);
            this.KeywordTxt.Name = "KeywordTxt";
            this.KeywordTxt.Size = new System.Drawing.Size(535, 21);
            this.KeywordTxt.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "关键词:";
            // 
            // WeiboSearchKeywordListbox
            // 
            this.WeiboSearchKeywordListbox.FormattingEnabled = true;
            this.WeiboSearchKeywordListbox.ItemHeight = 12;
            this.WeiboSearchKeywordListbox.Location = new System.Drawing.Point(6, 6);
            this.WeiboSearchKeywordListbox.Name = "WeiboSearchKeywordListbox";
            this.WeiboSearchKeywordListbox.Size = new System.Drawing.Size(582, 76);
            this.WeiboSearchKeywordListbox.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.AttrDropdown);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.AgeDropdown);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.GenderDropdown);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.CertDropdown);
            this.tabPage2.Controls.Add(this.AddPeopleQueryBtn);
            this.tabPage2.Controls.Add(this.SearchPeopleBtn);
            this.tabPage2.Controls.Add(this.PeopleAddtionQueryTxt);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.WeiboPeopleEndPageNum);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.WeiboPeopleStartPageNum);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.WeiboPeopleKeywordTxt);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.WeiboPeopleSearchListbox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(594, 335);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "搜索人物";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 176);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 12);
            this.label15.TabIndex = 45;
            this.label15.Text = "属性筛选:";
            // 
            // AttrDropdown
            // 
            this.AttrDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AttrDropdown.FormattingEnabled = true;
            this.AttrDropdown.Items.AddRange(new object[] {
            "全部",
            "昵称",
            "标签",
            "学校",
            "公司"});
            this.AttrDropdown.Location = new System.Drawing.Point(72, 173);
            this.AttrDropdown.Name = "AttrDropdown";
            this.AttrDropdown.Size = new System.Drawing.Size(64, 20);
            this.AttrDropdown.TabIndex = 44;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(249, 146);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 43;
            this.label11.Text = "年龄:";
            // 
            // AgeDropdown
            // 
            this.AgeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AgeDropdown.FormattingEnabled = true;
            this.AgeDropdown.Items.AddRange(new object[] {
            "不限",
            "18岁以下",
            "19-22岁",
            "23-29岁",
            "30-39岁",
            "40岁以上"});
            this.AgeDropdown.Location = new System.Drawing.Point(290, 143);
            this.AgeDropdown.Name = "AgeDropdown";
            this.AgeDropdown.Size = new System.Drawing.Size(88, 20);
            this.AgeDropdown.TabIndex = 42;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(147, 146);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 12);
            this.label9.TabIndex = 41;
            this.label9.Text = "性别:";
            // 
            // GenderDropdown
            // 
            this.GenderDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GenderDropdown.FormattingEnabled = true;
            this.GenderDropdown.Items.AddRange(new object[] {
            "不限",
            "男",
            "女"});
            this.GenderDropdown.Location = new System.Drawing.Point(188, 143);
            this.GenderDropdown.Name = "GenderDropdown";
            this.GenderDropdown.Size = new System.Drawing.Size(55, 20);
            this.GenderDropdown.TabIndex = 40;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 39;
            this.label8.Text = "认证:";
            // 
            // CertDropdown
            // 
            this.CertDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CertDropdown.FormattingEnabled = true;
            this.CertDropdown.Items.AddRange(new object[] {
            "不限",
            "普通用户",
            "认证用户"});
            this.CertDropdown.Location = new System.Drawing.Point(48, 143);
            this.CertDropdown.Name = "CertDropdown";
            this.CertDropdown.Size = new System.Drawing.Size(88, 20);
            this.CertDropdown.TabIndex = 38;
            // 
            // AddPeopleQueryBtn
            // 
            this.AddPeopleQueryBtn.Location = new System.Drawing.Point(203, 230);
            this.AddPeopleQueryBtn.Name = "AddPeopleQueryBtn";
            this.AddPeopleQueryBtn.Size = new System.Drawing.Size(144, 23);
            this.AddPeopleQueryBtn.TabIndex = 37;
            this.AddPeopleQueryBtn.Text = "添加此查询条件";
            this.AddPeopleQueryBtn.UseVisualStyleBackColor = true;
            this.AddPeopleQueryBtn.Click += new System.EventHandler(this.AddPeopleQueryBtn_Click);
            // 
            // SearchPeopleBtn
            // 
            this.SearchPeopleBtn.Location = new System.Drawing.Point(240, 306);
            this.SearchPeopleBtn.Name = "SearchPeopleBtn";
            this.SearchPeopleBtn.Size = new System.Drawing.Size(75, 23);
            this.SearchPeopleBtn.TabIndex = 36;
            this.SearchPeopleBtn.Text = "搜索";
            this.SearchPeopleBtn.UseVisualStyleBackColor = true;
            this.SearchPeopleBtn.Click += new System.EventHandler(this.SearchPeopleBtn_Click);
            // 
            // PeopleAddtionQueryTxt
            // 
            this.PeopleAddtionQueryTxt.Location = new System.Drawing.Point(114, 201);
            this.PeopleAddtionQueryTxt.Name = "PeopleAddtionQueryTxt";
            this.PeopleAddtionQueryTxt.Size = new System.Drawing.Size(385, 21);
            this.PeopleAddtionQueryTxt.TabIndex = 35;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 204);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 12);
            this.label10.TabIndex = 30;
            this.label10.Text = "附加查询Url参数:";
            // 
            // WeiboPeopleEndPageNum
            // 
            this.WeiboPeopleEndPageNum.Location = new System.Drawing.Point(130, 112);
            this.WeiboPeopleEndPageNum.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.WeiboPeopleEndPageNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WeiboPeopleEndPageNum.Name = "WeiboPeopleEndPageNum";
            this.WeiboPeopleEndPageNum.Size = new System.Drawing.Size(44, 21);
            this.WeiboPeopleEndPageNum.TabIndex = 25;
            this.WeiboPeopleEndPageNum.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(101, 117);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 24;
            this.label12.Text = "---";
            // 
            // WeiboPeopleStartPageNum
            // 
            this.WeiboPeopleStartPageNum.Location = new System.Drawing.Point(53, 112);
            this.WeiboPeopleStartPageNum.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.WeiboPeopleStartPageNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WeiboPeopleStartPageNum.Name = "WeiboPeopleStartPageNum";
            this.WeiboPeopleStartPageNum.Size = new System.Drawing.Size(42, 21);
            this.WeiboPeopleStartPageNum.TabIndex = 23;
            this.WeiboPeopleStartPageNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 117);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 12);
            this.label13.TabIndex = 22;
            this.label13.Text = "页码:";
            // 
            // WeiboPeopleKeywordTxt
            // 
            this.WeiboPeopleKeywordTxt.Location = new System.Drawing.Point(53, 85);
            this.WeiboPeopleKeywordTxt.Name = "WeiboPeopleKeywordTxt";
            this.WeiboPeopleKeywordTxt.Size = new System.Drawing.Size(535, 21);
            this.WeiboPeopleKeywordTxt.TabIndex = 21;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 88);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 20;
            this.label14.Text = "关键词:";
            // 
            // WeiboPeopleSearchListbox
            // 
            this.WeiboPeopleSearchListbox.FormattingEnabled = true;
            this.WeiboPeopleSearchListbox.ItemHeight = 12;
            this.WeiboPeopleSearchListbox.Location = new System.Drawing.Point(6, 6);
            this.WeiboPeopleSearchListbox.Name = "WeiboPeopleSearchListbox";
            this.WeiboPeopleSearchListbox.Size = new System.Drawing.Size(582, 76);
            this.WeiboPeopleSearchListbox.TabIndex = 19;
            // 
            // checkBox1
            // 
            this.searchAllChk.AutoSize = true;
            this.searchAllChk.Location = new System.Drawing.Point(189, 115);
            this.searchAllChk.Name = "checkBox1";
            this.searchAllChk.Size = new System.Drawing.Size(72, 16);
            this.searchAllChk.TabIndex = 19;
            this.searchAllChk.Text = "抓取所有";
            this.searchAllChk.UseVisualStyleBackColor = true;
            // 
            // SinaSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 381);
            this.Controls.Add(this.tabControl1);
            this.Name = "SinaSearch";
            this.Text = "新浪搜索";
            this.Load += new System.EventHandler(this.SinaSearch_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EndPageNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPageNum)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WeiboPeopleEndPageNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeiboPeopleStartPageNum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown EndPageNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown StartPageNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox KeywordTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox WeiboSearchKeywordListbox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox SortDropDown;
        private System.Windows.Forms.CheckBox VipChk;
        private System.Windows.Forms.CheckBox OriginChk;
        private System.Windows.Forms.TextBox EndDateTxt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox StartDateTxt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SearchWeiboBtn;
        private System.Windows.Forms.TextBox AddtionQueryTxt;
        private System.Windows.Forms.Button AddQueryBtn;
        private System.Windows.Forms.Button AddPeopleQueryBtn;
        private System.Windows.Forms.Button SearchPeopleBtn;
        private System.Windows.Forms.TextBox PeopleAddtionQueryTxt;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown WeiboPeopleEndPageNum;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown WeiboPeopleStartPageNum;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox WeiboPeopleKeywordTxt;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListBox WeiboPeopleSearchListbox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox AttrDropdown;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox AgeDropdown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox GenderDropdown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox CertDropdown;
        private System.Windows.Forms.CheckBox searchAllChk;
    }
}
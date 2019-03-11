using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Common
{
    public class TabCandidateItems
    {
        #region  参选人信息

        [Description("编号")]
        public int Id { get; set; }
        [Description("姓名")]
        public string Name { get; set; }
        [Description("性别")]
        public char Sex { get; set; }
        [Description("年龄")]
        public int Age { get; set; }
        [Description("民族")]
        public string Nation { get; set; }
        [Description("学校")]
        public string School { get; set; }
        [Description("推荐奖项")]
        public string Award { get; set; }
        [Description("身份")]
        public string Identity { get; set; }
        [Description("学历及专业")]
        public string Education { get; set; }
        [Description("政治面貌")]
        public string Politics { get; set; }
        [Description("照片")]
        public string Img { get; set; }
        [Description("票数")]
        public int Votes { get; set; }
        [Description("名次")]
        public int Rank { get; set; }
        [Description("视频")]
        public string Player { get; set; }
        [Description("自我评价")]
        public string Evaluation { get; set; }
        [Description("荣誉及奖项")]
        public string Awards { get; set; }
        [Description("事迹简介")]
        public string Story { get; set; }
        [Description("推荐理由")]
        public string Reason { get; set; }

        #endregion

    }
}

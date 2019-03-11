using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Common
{
    public class TabVoteItems
    {
        #region 投票信息

        [Description("投票编号")]
        public int Id { get; set; }
        [Description("IP")]
        public string Ip { get; set; }
        [Description("Session")]
        public string session { get; set; }
        [Description("浏览器属性")]
        public string user_agent { get; set; }
        [Description("投票时间")]
        public DateTime Votetime { get; set; }
        [Description("候选人Id")]
        public int TabCanId { get; set; }

        #endregion
    }
}

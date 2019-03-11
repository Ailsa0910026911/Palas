using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vote.Common;

namespace VoteWeb
{
    public partial class Info : System.Web.UI.Page
    {
        MySqlQuery query = new MySqlQuery();
        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["ID"]))
                id = Convert.ToInt32(Request["ID"]);


            Repeater1.DataSource = query.GetTabCandidateWhere(id);
            Repeater1.DataBind();
        }

        public static string getRank(object id)
        {
            return MySqlQuery.dic[Convert.ToInt32(id)].ToString();
        }
        
    }
}
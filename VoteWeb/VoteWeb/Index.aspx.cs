using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vote.Common;

namespace VoteWeb
{
    public partial class Index : System.Web.UI.Page
    {
        MySqlQuery query = new MySqlQuery();
        protected void Page_Load(object sender, EventArgs e)
        {
            Repeater2.DataSource = query.GetAward();
            Repeater2.DataBind();
        }

        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rep = e.Item.FindControl("Repeater1") as Repeater;//到里层的repeater对象
                DataRowView rowv = (DataRowView)e.Item.DataItem;//到分类Repeater关联的数据项 
                string typeid = rowv["Award"].ToString(); //获取填充子类的Award
                rep.DataSource = query.GetTabCandidate(typeid);
                rep.DataBind();
            }

        }
    }
}
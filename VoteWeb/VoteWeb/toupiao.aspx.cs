using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vote.Common;

namespace VoteWeb
{
    public partial class toupiao : System.Web.UI.Page
    {
        MySqlQuery query = new MySqlQuery();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) >= Convert.ToDateTime("2015-11-6 17:00:00"))
            {
                Label1.Text = "投票已经结束";
            }
            else
            {

                if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                {
                    string userid = Request.QueryString["Id"];
                    string userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
                    string user_agent = MySqlQuery.getMd5Hash(Request.UserAgent);
                    string session = Guid.NewGuid().ToString(); //Session.SessionID;
                    DateTime time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    if (!ValidateVote.VaildateVote(userid, userIp, session, Request.UserAgent))
                        Label1.Text = "您的IP已经投过票了，或违反了其他投票规则";
                    else
                    {

                        TabVoteItems vote = new TabVoteItems();
                        vote.user_agent = user_agent;
                        vote.session = session;
                        vote.Ip = userIp;
                        vote.TabCanId = Convert.ToInt32(userid);
                        vote.Votetime = time;

                        if (query.Addvote(vote) > 0)
                        {
                            Label1.Text = "投票成功，计数和排名将在5分钟内更新";

                           
                            query.Getcount(Convert.ToInt32(Request.QueryString["Id"]));

                        }
                        else
                            Label1.Text = "您的IP已经投过票了，或违反了其他投票规则";
                    }
                }
            }
        }
    }
}
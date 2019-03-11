<%@ Page Title="" Language="C#" MasterPageFile="~/vote.Master" AutoEventWireup="true" CodeBehind="Info.aspx.cs" Inherits="VoteWeb.Info" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="Content">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <input type="hidden" id="hiddenID" value="<%# Eval("Id") %>" />
                <div class="Content-left">
                    <img src="/images/img/<%# Eval("Img")%>" onerror="this.src='/images/renwu1.png'" style="width: 100%" />
                    <div style="margin-bottom: 10px; padding-bottom: 10px; height: 20px">
                        <div style="float: left; width: 50%; font-size: 15px; font-weight: 700">第<span style="color: #ff6a00;"><%# getRank(Eval("Id")) %></span>名</div>
                        <div style="float: left; width: 50%; font-size: 15px; font-weight: 700"><span style="color: #ff6a00;"><%# Eval("Votes") %></span>票</div>
                    </div>
                    <a href="/toupiao/<%# Eval("Id") %>.html">
                        <img src="/images/toupiao.png" style="width: 80%" /></a>
                </div>
                <div class="Content-right">
                    <div class="title">
                        <div style="font-size: 15px; border-left: 0; width: 40%"><%# Eval("Name") %></div>
                        <div>
                            <%# Eval("Sex") %>
                        </div>
                        <div>
                            <%# Eval("Age") %>岁
                        </div>
                        <div style="position: relative;">
                            NO.<img src="/images/yuan1.png" />
                            <span class="Ranking2"><a><%# getRank(Eval("Id")) %></a></span>
                        </div>
                    </div>
                    <div style="clear: left"></div>
                    <div class="Comment">
                        <p>工作单位：<span><%# Eval("School") %></span></p>
                        <p>自我评价：<span><%# Eval("Evaluation") %></span></p>
                        <p>曾获荣誉及奖项：<span><%# Eval("Awards") %></span></p>
                        <p>事迹简介：<span><%# Eval("Story") %></span></p>
                        <p>推荐理由：<span><%# Eval("Reason") %></span></p>
                    </div>
                    <div class="Video">
                        <%# Eval("Player") %>
                    </div>
                </div>
                <div style="clear: both;"></div>
            </ItemTemplate>
        </asp:Repeater>
        <%--<div class="Content-right">
            <div class="title">
                <div style="font-size: 16px; border-left: 0;">胡歌</div>
                <div style="padding-top: 0;">
                    <p>男</p>
                    <p>23岁</p>
                </div>
                <div style="position: relative;">
                    NO.<img src="images/yuan1.png" />
                    <span class="Ranking2"><a>2</a></span>
                </div>
                <div>第<span>3</span>名</div>
                <div><span>468</span>票</div>
            </div>
            <div style="clear: left"></div>
            <div class="Comment">
                <p>工作单位：<span>上海市杨浦区第11中学</span></p>
                <p>自我评价：<span>上海市杨浦区第11中学</span></p>
                <p>曾获荣誉及奖项：<span>上海市杨浦区第11中学</span></p>
                <p>推荐理由：<span>上海市杨浦区第11中学</span></p>
            </div>
            <div class="Brief">
                <p>胡歌，1982年9月20日出生上海，著名演员、歌手、制片人。</p>
                <p>1996年，14岁的胡歌便成为上海教育电视台的小主持人，2001年高分考入上海戏剧院表演系。2005年因在电视剧《仙剑奇侠传》中成功塑造了好爽深情的“李逍遥”一举成名，并演唱插曲《六月的雨》《逍遥叹》而蜚声歌坛。</p>
            </div>
            <div class="Video">
            </div>
        </div>--%>
    </div>
</asp:Content>

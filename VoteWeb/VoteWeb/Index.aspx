<%@ Page Title="" Language="C#" MasterPageFile="~/vote.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="VoteWeb.Index" %>

<%@ OutputCache Duration="300" VaryByParam="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>2015上海市最佳阳光体育活力园丁奖、最佳阳光体育达人奖评选及颁奖活动</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <div class="Content">
        <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
            <ItemTemplate>
                <div style="padding-left:20px;">
                    <legend><img src="/images/huangguan.png"/><span class="Award"><%#Eval("Award") %>候选人：</span></legend>
                </div>
                <ul class="infoList">
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <li>
                                <div>
                                    <a href="/info/<%# Eval("Id") %>.html">
                                        <img src="images/img/<%# Eval("Img")%>" onerror="this.src='images/renwu1.png'" style="width: 100%;" /></a>
                                </div>
                                <div class="info">
                                    <div title="<%# Eval("Name")%>" style="width:7em; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;"><%# Eval("Name")%></div>
                                    <div style="float: left">
                                        <div style="position: relative; font-size: 10px;">
                                            NO.<img src="images/yuan1.png" style="width: 23px; height: 17px;" />
                                            <span class="Ranking"><a><%# Eval("Rank") %></a></span>
                                        </div>
                                    </div>
                                    <div class="yincang" style="float: right; margin: -10px 0 0 12px;">
                                        <img src="images/pic2.png" />
                                    </div>
                                    <div class="yincang" style="float: right;">
                                        <div style="font-size: 10px;"><span><%# Eval("votes") %></span>票</div>
                                    </div>
                                    <div style="clear: both;"></div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <div style="clear: left;"></div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>

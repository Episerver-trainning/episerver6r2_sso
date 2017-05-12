<%@ Page Language="C#" AutoEventWireup="false" EnableViewState="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="List.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Pages.List" %>

<%@ Register Src="~/Templates/AlloyTech/Blog/Units/ItemSummary.ascx" TagPrefix="Blog"
    TagName="ItemSummary" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/SecondaryList.ascx" TagPrefix="Blog"
    TagName="SecondaryList" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech.Blog" %>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyArea">
        <div id="MainBody">
            <h1>
                <%= GetHeading() %></h1>
            <EPiServer:PageList runat="server" DataSource="<%# Pages %>">
                <ItemTemplate>
                <hr />
                    <Blog:ItemSummary runat="server" SelectedPage="<%# Container.CurrentPage %>"
                        ShowImage="false" />
                </ItemTemplate>
            </EPiServer:PageList>
        </div>
    </div>
    <div id="SecondaryBodyArea">
        <div id="SecondaryBody">
            <Blog:SecondaryList runat="server" BlogStart='<%# BlogStart %>' ShowDetails='<%# !(bool)(CurrentPage[BlogUtility.IsTeamLevelPropertyName] ?? false) %>' />
        </div>
    </div>
</asp:Content>

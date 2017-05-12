<%@ Page Language="C#" AutoEventWireup="false" EnableViewState="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" CodeBehind="TeamStart.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Pages.TeamStart" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech.Blog" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/ItemSummary.ascx" TagPrefix="Blog" TagName="ItemSummary" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/SecondaryList.ascx" TagPrefix="Blog" TagName="SecondaryList" %>

<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
	<div id="MainBodyArea">
	    <div id="MainBody">
	        <EPiServer:Property runat="server" PropertyName="MainBody" />
            <EPiServer:PageList runat="server" DataSource="<%# BlogItems %>" MaxCount="<%# MaxCount %>">
                <ItemTemplate>
                    <hr />
                    <Blog:ItemSummary runat="server" SelectedPage="<%# Container.CurrentPage %>" SummaryTextLength="<%# SummaryTextLength %>" />
                </ItemTemplate>
            </EPiServer:PageList>
        </div>
    </div>
    
    <div id="SecondaryBodyArea">
		<div id="SecondaryBody">
            <Blog:SecondaryList runat="server" BlogStart="<%# BlogStart %>" ShowDetails="false" />
		</div>
	</div>
</asp:Content>
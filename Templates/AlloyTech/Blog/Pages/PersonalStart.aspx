<%@ Page Language="C#" AutoEventWireup="false" EnableViewState="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" CodeBehind="PersonalStart.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Pages.PersonalStart" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/ItemSummary.ascx" TagPrefix="Blog" TagName="ItemSummary" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/SecondaryList.ascx" TagPrefix="Blog" TagName="SecondaryList" %>

<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
	<div id="MainBodyArea">
	    <div id="MainBody">
	       <h1>
                <EPiServer:Property runat="server" PropertyName="PageName" /></h1>
             
            <EPiServer:PageList runat="server" DataSource="<%# BlogItems %>" MaxCount="<%# MaxCount %>">
                <ItemTemplate>
                  <hr />
                    <Blog:ItemSummary runat="server" SelectedPage="<%# Container.CurrentPage %>" SummaryTextLength="<%# SummaryTextLength %>" ShowImage="false" ShowWriter="false" />
                  
                </ItemTemplate>
            </EPiServer:PageList>
        </div>
    </div>
    
    <div id="SecondaryBodyArea">
		<div id="SecondaryBody">
            <Blog:SecondaryList runat="server" BlogStart="<%# BlogStart %>" />
		</div>
	</div>
</asp:Content>
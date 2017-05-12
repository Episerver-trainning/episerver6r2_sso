<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="False" CodeBehind="NewsItem.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.NewsItem" Title="Untitled Page" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>    

<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
	<div id="SecondaryBody">
		<dl>
			<dt><EPiServer:Translate Text="/news/writername" runat="server" /></dt>
			<dd><EPiServer:Property PropertyName="Author" runat="server" /></dd>
			<dt><EPiServer:Translate Text="/news/publishdate" runat="server" /></dt>
			<dd><%=CurrentPage.GetFormattedPublishDate() %></dd>
		</dl>
	</div>
</asp:Content>
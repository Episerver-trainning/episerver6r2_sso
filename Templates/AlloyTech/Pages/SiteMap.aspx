<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="false" Codebehind="SiteMap.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.SiteMap" %>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">

	<div id="MainBody">
	    <AlloyTech:MainBody runat="server" />

	    <EPiServer:PageTree runat="server" ExpandAll="true" id="SiteMapTree" PageLink="<%# IndexRoot %>">
			<HeaderTemplate>
				<div id="SiteMap">
			</HeaderTemplate>
			<IndentTemplate>
				<ul>
			</IndentTemplate>
			<ItemHeaderTemplate>
				<li>
			</ItemHeaderTemplate>
			<ItemTemplate>
				<EPiServer:Property PropertyName="PageLink" runat="server" />
			</ItemTemplate>
			<ItemFooterTemplate>
				</li>
			</ItemFooterTemplate>
			<UnindentTemplate>
				</ul>
			</UnindentTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</EPiServer:PageTree>
    </div>
    
</asp:Content>
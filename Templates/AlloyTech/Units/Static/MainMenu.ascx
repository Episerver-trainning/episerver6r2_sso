<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="False" CodeBehind="MainMenu.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Static.MainMenu" %>
<div id="MainMenu">
<EPiServer:MenuList runat="server" id="Menu">
	<HeaderTemplate>
		<ul >
	</HeaderTemplate>
	<ItemTemplate>
			<li><EPiServer:Property PropertyName="PageLink" runat="server" /></li>
	</ItemTemplate>
	<SelectedTemplate>
			<li class="selected"><EPiServer:Property runat="server" PropertyName="PageLink" /></li>
	</SelectedTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</EPiServer:MenuList>
</div>
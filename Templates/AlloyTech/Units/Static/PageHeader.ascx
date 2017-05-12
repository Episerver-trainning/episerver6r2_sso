<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="False" CodeBehind="PageHeader.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Static.PageHeader" %>
<%@ Import namespace="EPiServer" %>

<div id="Header">
	<div id="Logotype">
		<asp:HyperLink accesskey="1" ID="Logotype" runat="server" />
	</div>
	<div id="Functions">
        <ul>
            <li class="iconLink loginLink">
                <asp:LoginView ID="LoginView" runat="server">
                    <AnonymousTemplate>
                        <a href="<%= GetLoginUrl() %>"><asp:Literal runat="server" Text="<%$ Resources: EPiServer, login.login %>" /></a>
                    </AnonymousTemplate>
                    <LoggedInTemplate>
                        <asp:LoginStatus runat="server" ToolTip="<%$ Resources: EPiServer, login.logout %>" LogoutText="<%$ Resources: EPiServer, login.logout %>" />
                    </LoggedInTemplate>
                </asp:LoginView>
            </li>
            <li runat="server" visible="false" class="iconLink rssLink">
                <EPiServer:Property ID="Rss" PropertyName="PageLink" runat="server" />
            </li>
	        <li runat="server" visible="false" class="iconLink sitemapLink">
	             <EPiServer:Property ID="SiteMap" PropertyName="PageLink" runat="server" />
	        </li>
	        <li runat="server" visible="false" class="iconLink languageLink">
		        <asp:HyperLink ID="Language" runat="server" Visible="false"></asp:HyperLink>
		        <asp:Label ID="LanguageListLabel" runat="server" AssociatedControlID="LanguageList" CssClass="hidden" Visible="false" text="Other languages" />
		        <asp:DropDownList runat="server" ID="LanguageList"  Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ChangeLanguage">
		            <asp:ListItem Text="Other languages" Value="noLangSelected" />
		        </asp:DropDownList>
		        <noscript>
		            <asp:Button runat="server" ID="LanguageButton" OnClick="ChangeLanguage" Text="OK" Visible="false" />
		        </noscript>
	        </li>
        </ul>
    </div>
</div>
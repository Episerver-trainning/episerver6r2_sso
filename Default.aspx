<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Default.aspx.cs" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    Inherits="EPiServer.Templates.Default" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>
<%@ Register TagPrefix="AlloyTech" TagName="Flash" Src="~/Templates/AlloyTech/Units/Placeable/Flash.ascx" %>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <div id="StageArea">
        <div id="ImageArea">
            <div id="FlashContent">
                <AlloyTech:Flash runat="server" ID="Flash" Width="980" Height="340" />
            </div>
        
        </div>
        <div id="MainContentArea">
            <div class="column">
                <EPiServer:Property PropertyName="MainBody" EnableViewState="false" runat="server" />
            </div>
            <div class="column">
                <EPiServer:Property runat="server" EnableViewState="false" PropertyName="SecondaryBody" />
            </div>
            <div class="column">
                <EPiServer:Property runat="server" EnableViewState="false" PropertyName="ThirdBody" />
            </div>
            <div id="Clients">
                <EPiServer:Property ID="ClientsArea" runat="server" EnableViewState="false" PropertyName="ClientsArea" />
            </div>
            <div class="linksArea">
                <EPiServer:Property ID="MainPageListHeading" CustomTagName="h2" runat="server" EnableViewState="false"
                    PropertyName="MainLinksHeading" />
                <EPiServer:PageList ID="MainPageList" MaxCount="5" PageLinkProperty="MainLinksRoot"
                    runat="server">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <EPiServer:Property PropertyName="PageLink" runat="server" />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </EPiServer:PageList>
            </div>
            <div class="linksArea">
                <EPiServer:Property ID="SecondaryPageListHeading" CustomTagName="h2" runat="server" EnableViewState="false"
                    PropertyName="SecondaryLinksHeading" />
                <EPiServer:PageList ID="SecondaryPageList" MaxCount="5" PageLinkProperty="SecondaryLinksRoot"
                    runat="server">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <EPiServer:Property PropertyName="PageLink" runat="server" />
                            <asp:Label runat="server" CssClass="linksItemDate" Visible="<%# IsDateDisplayedInSecondaryLinks() %>">
			                        <%# Container.CurrentPage.GetFormattedPublishDate() %>
                            </asp:Label>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </EPiServer:PageList>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Reseller.aspx.cs" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    Inherits="EPiServer.Templates.AlloyTech.Pages.Reseller" %>

<asp:Content ID="MainRegionContent" ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <EPiServer:Property CustomTagName="h1" PropertyName="PageName" runat="server" />
      <div id="Reseller">
            <h3>
                <EPiServer:Property PropertyName="ResellerDescription" runat="server"  />
            </h3>
          
                <span class="introduction">
                    <EPiServer:Translate Text="/templates/reseller/address" runat="server" />
                </span>
               
                <EPiServer:Property PropertyName="Address1" runat="server" />
               
                <EPiServer:Property PropertyName="Address2" runat="server" />
                
                <EPiServer:Property PropertyName="Address3" runat="server" />
              
                <EPiServer:Property PropertyName="Country" runat="server" />
          
                <span class="introduction">
                    <EPiServer:Translate Text="/templates/reseller/url" runat="server" />
                </span>
                
                <EPiServer:Property ID="Url" PropertyName="Url" runat="server" />
          
                <span class="introduction">
                    <EPiServer:Translate Text="/templates/reseller/contacts" runat="server" />
                </span>
                <EPiServer:Property PropertyName="ContactName" runat="server" DisplayMissingMessage="false" />
                <a href="mailto:<%=CurrentPage["Email"] %>">
                    <%=CurrentPage["Email"] %></a>
              
                <EPiServer:Property PropertyName="Phone" runat="server" />
           
       </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
</asp:Content>

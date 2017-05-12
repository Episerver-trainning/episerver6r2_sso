<%@ Control Language="C#" AutoEventWireup="False" CodeBehind="PageFooter.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Static.PageFooter" EnableViewState="false" %>
<div id="PageFooter">
    <ul>
        <li>
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="EPiWebSitesHeading" />
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="EPiWebSites" />
        </li>
        <li>
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="RelatedSitesHeading" />
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="RelatedSites" />
        </li>
        <li>
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="OtherWebSitesHeading" />
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="OtherWebSites" />
        </li>
        <li>
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="AboutTheSiteHeading" />
            <EPiServer:Property runat="server" PageLink="<%# StartPageReference %>" DisplayMissingMessage="false" PropertyName="AboutTheSite" />
        </li>
    </ul>
    <div id="Copyright">
        © Copyright <%= DateTime.Now.Year %> <a href="http://www.episerver.com/">EPiServer AB</a>
    </div>
</div>

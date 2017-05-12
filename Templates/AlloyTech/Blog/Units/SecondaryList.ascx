<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SecondaryList.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Units.SecondaryList" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/DateTree.ascx" TagPrefix="Blog" TagName="DateTree" %>
<%@ Register Src="~/Templates/AlloyTech/Blog/Units/TagCloud.ascx" TagPrefix="Blog" TagName="TagCloud" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech.Blog" %>

<div runat="server" visible="<%# ShowDetails %>">
   <h2><EPiServer:Translate ID="Translate1" runat="server" Text="/blog/secondarylist/aboutme" /></h2>
    <img runat="server" id="BloggerImage" class="bloggerImage" src="" alt="" visible="false" />
 
    <EPiServer:Property runat="server" PropertyName="Details" PageLink="<%# BlogStart.PageLink %>" />
    <hr />
</div>
   <h2><EPiServer:Translate runat="server" Text="/blog/secondarylist/archive" /></h2>

<Blog:DateTree runat="server" TreeStart='<%# (EPiServer.Core.PageReference)BlogStart[BlogUtility.DateContainerPropertyName] %>' />
<hr />
   <h2><EPiServer:Translate runat="server" Text="/blog/secondarylist/tagcloud" /></h2>

<Blog:TagCloud runat="server" TagStartPage='<%# (EPiServer.Core.PageReference)BlogStart[BlogUtility.TagContainerPropertyName] %>' />
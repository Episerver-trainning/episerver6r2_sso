<%@ Page language="c#" AutoEventWireup="false" Inherits="EPiServer.Templates.AlloyTech.Pages.PersonalSettings" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" Codebehind="PersonalSettings.aspx.cs" %>
<%@ Register TagPrefix="AlloyTech" TagName="PersonalSettings" Src="~/Templates/AlloyTech/Units/Placeable/PersonalSettings.ascx"%>
<%@ Register TagPrefix="public" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>

<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
<div id="MainBodyArea">
        <div id="MainBody">
            <public:MainBody runat="server" />
	        <AlloyTech:PersonalSettings runat="server" />
	    </div>
    </div>
</asp:Content>
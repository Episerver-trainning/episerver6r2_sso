<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="False" CodeBehind="XForm.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.XFormPage" %>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody"  Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="XForm"		Src="~/Templates/AlloyTech/Units/Placeable/XForm.ascx" %>

<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
	<div id="MainBody">
	
	    <AlloyTech:MainBody runat="server" />
	    
        <AlloyTech:XForm
            runat="server" 
            XFormProperty="XForm"
            HeadingProperty="XFormHeading"
            ShowStatistics="false" />
	    
    </div>
</asp:Content>
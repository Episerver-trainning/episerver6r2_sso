<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DefaultView.ascx.cs"
    Inherits="EPiServer.Templates.Advanced.FileManager.DefaultView" %>
<%@ Register TagPrefix="FileManager" Namespace="EPiServer.Templates.Advanced.FileManager.Core.WebControls" 
    Assembly="EPiServer.Templates.AlloyTech"  %>

<div class="filemanagercontrol">
    <FileManager:Region ID="Heading" runat="server" />
    <div class="body">
        <div class="rightpanel">
            <FileManager:Region ID="MainPanel" runat="server" />
        </div>
    </div>
</div>

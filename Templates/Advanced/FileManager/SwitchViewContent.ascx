<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SwitchViewContent.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.SwitchViewContent" %>
<div class="toolbarbuttonarea">

    <div class="toolbarbuttonsectionleft">
    <asp:LinkButton runat="server" ID="AddFile" CssClass="addfile" Text="<%$ Resources: EPiServer, filemanager.switchview.addfile %>" CommandName="LoadView" CommandArgument="AddFile" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="AddFolder" CssClass="addfolder" Text="<%$ Resources: EPiServer, filemanager.switchview.addfolder %>" CommandName="LoadView" CommandArgument="AddFolder" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Rename" CssClass="rename" Text="<%$ Resources: EPiServer, filemanager.switchview.rename %>" CommandName="LoadView" CommandArgument="RenameItem" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Details" CssClass="details" Text="<%$ Resources: EPiServer, filemanager.switchview.details %>" CommandName="LoadView" CommandArgument="ViewFileSummary" OnCommand="Button_Command" CausesValidation="false" />
    </div>
    
    <asp:LinkButton runat="server" ID="Cut" CssClass="cut" Text="<%$ Resources: EPiServer, filemanager.switchview.cut %>" CommandName="CutSelection" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Copy" CssClass="copy" Text="<%$ Resources: EPiServer, filemanager.switchview.copy %>" CommandName="CopySelection" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Paste" CssClass="paste" Text="<%$ Resources: EPiServer, filemanager.switchview.paste %>" CommandName="PasteItems" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Delete" CssClass="delete" Text="<%$ Resources: EPiServer, filemanager.switchview.delete %>" CommandName="LoadView" CommandArgument="DeleteItem" OnCommand="Button_Command" CausesValidation="false" />

    <div class="toolbarbuttonsectionright">
    <asp:LinkButton runat="server" ID="Checkout" CssClass="checkout" Text="<%$ Resources: EPiServer, filemanager.switchview.checkout %>" CommandName="CheckOutSelection" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="UndoCheckout" CssClass="undocheckout" Text="<%$ Resources: EPiServer, filemanager.switchview.undocheckout %>" CommandName="UndoCheckOutSelection" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="CheckIn" CssClass="checkin" Text="<%$ Resources: EPiServer, filemanager.switchview.checkin %>" CommandName="LoadView" CommandArgument="CheckInFile" OnCommand="Button_Command" CausesValidation="false" />
    <asp:LinkButton runat="server" ID="Version" CssClass="version" Text="<%$ Resources: EPiServer, filemanager.switchview.version %>" CommandName="LoadView" CommandArgument="VersionList" OnCommand="Button_Command" CausesValidation="false" />
    </div>
</div>
<h4 class="toolbarmessage"><asp:Literal runat="server" ID="DisabledFunctionsMessage" Visible="false" Text="<%$ Resources: EPiServer, filemanager.disabledfunctions %>"/></h4>

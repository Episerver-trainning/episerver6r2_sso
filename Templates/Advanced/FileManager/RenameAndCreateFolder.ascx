<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RenameAndCreateFolder.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.RenameAndCreateFolder" %>
<fieldset>
    <legend><asp:Literal runat="server" ID="FolderLegend" /></legend>
    <p>
        <asp:Label runat="server" AssociatedControlID="ItemName" ID="ItemNameLabel" />
        <asp:TextBox ID="ItemName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="ItemName" runat="server" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.requiredname %>" />
        <asp:RegularExpressionValidator ID="NameValidation" runat="server" ControlToValidate="ItemName" ValidationExpression="[^/?<>\\:*|&quot;#]*" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.illigalchar %>"/>
        <asp:CustomValidator runat="server" ControlToValidate="ItemName" OnServerValidate="ValidateUniqueName" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.folderexists %>" />
        <asp:CustomValidator runat="server" ControlToValidate="ItemName" OnServerValidate="ValidateNameLength" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.nametolong %>" />
    </p>
    <p>
        <asp:Button ID="SaveButton" OnClick="SaveItem_Click" CssClass="button" Text="<%$ Resources: EPiServer, filemanager.button.rename %>" runat="server" Enabled="false" />
        <asp:Button ID="CancelButton" OnClick="Cancel_Click" CssClass="button" CausesValidation="false" Text="<%$ Resources: EPiServer, button.cancel %>" runat="server" />
    </p>
</fieldset>

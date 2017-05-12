<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditUnifiedSummary.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.EditUnifiedSummary" %>

<fieldset>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.subject %>" AssociatedControlID="Subject" />
        <asp:TextBox ID="Subject" runat="server" />
    </div>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.title %>" AssociatedControlID="Title" />
        <asp:TextBox ID="Title" runat="server" />
    </div>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.author %>"  AssociatedControlID="Author" />
        <asp:TextBox ID="Author" runat="server" />
    </div>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.category %>" AssociatedControlID="Category" />
        <asp:TextBox ID="Category" runat="server" />
    </div>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.keywords %>" AssociatedControlID="Keywords" />
        <asp:TextBox ID="Keywords" runat="server" />
    </div>
    <div>
        <asp:Label runat="server" Text="<%$ Resources: EPiServer, workroom.filemanager.filesummary.comments%>" AssociatedControlID="Comments" />
        <asp:TextBox ID="Comments" runat="server" />
    </div>
    <div>
        <asp:Button ID="SaveButton" runat="server" Text="<%$ Resources: EPiServer, button.save %>" ToolTip="<%$ Resources: EPiServer, button.save %>" CssClass="button" OnClick="SaveButton_Click" />
        <asp:Button ID="CancelButton" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" ToolTip="<%$ Resources: EPiServer, button.cancel %>" CssClass="button" OnClick="CancelButton_Click" CausesValidation="false" />
    </div>
</fieldset>


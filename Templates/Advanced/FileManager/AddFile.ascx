<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddFile.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.AddFile" %>

<asp:Panel runat="server" DefaultButton="UploadButton">

    <fieldset>
        <legend><asp:Literal runat="server" Text="<%$ Resources: EPiServer, filemanager.switchview.addfile %>" /></legend>
        <p runat="server" id="FileUploadPanel">
            <asp:Label runat="server" Text="<%$ Resources: EPiServer, filemanager.choosefile %>" AssociatedControlID="FileUpload" />
            <asp:FileUpload ID="FileUpload" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="FileUpload" runat="server" Text="*" ErrorMessage="<%$ Resources: EPiServer, filemanager.validation.requiredfile %>" />
            <br />
            <asp:Label ID="labelExceptionMessage" runat="server" Visible="false" CssClass="error" />
        </p>
        <asp:Panel runat="server" ID="CommentsPanel">
            <p>
                <asp:Label runat="server" Text="<%$ Resources: EPiServer, filemanager.filecomment %>" AssociatedControlID="CheckInComment" />
                <asp:TextBox runat="server" ID="CheckInComment"/>
            </p>
        </asp:Panel>
        <p>
            <asp:Button runat="server" CssClass="button" ID="UploadButton" Text="<%$ Resources: EPiServer, filemanager.button.upload %>" OnClick="UploadFile_Click"/>
            <asp:Button runat="server" CssClass="button" ID="ReplaceFileButton" Text="<%$ Resources: EPiServer, filemanager.button.replacefile %>" OnClick="ReplaceFile_Click" Visible="false"/>
            <asp:Button runat="server" CssClass="button" ID="CancelButton" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="CancelButton_Click" CausesValidation="false" />
        </p>
    </fieldset>
</asp:Panel>



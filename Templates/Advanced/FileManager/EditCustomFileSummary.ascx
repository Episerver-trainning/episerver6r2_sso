<%@ Control EnableViewState="False" Language="c#" AutoEventWireup="False" Codebehind="EditCustomFileSummary.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.EditCustomFileSummary" %>
<fieldset class="editfilesummary">
    <legend><asp:Literal runat="server" Text="<%$ Resources: EPiServer, filemanager.editfilesummaryheading %>" /></legend>
    
    <XForms:XFormControl id="XFormCtrl" runat="server"/>
    <p>
        <asp:Button ID="SaveButton" CssClass="buttonExt" runat="server" Text="<%$ Resources: EPiServer, button.save %>" ToolTip="<%$ Resources: EPiServer, button.save %>" SkinID="Save" OnClick="SaveButton_Click" />
        <span class="WRbuttonsDiv">&nbsp;</span>
        <asp:Button ID="CancelButton" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" ToolTip="<%$ Resources: EPiServer, button.cancel %>" SkinID="Cancel" OnClick="CancelButton_Click" />
    </p>
</fieldset>
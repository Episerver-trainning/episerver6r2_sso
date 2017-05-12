<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChangeVisiblePages.ascx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Units.ChangeVisiblePages" %>
<fieldset>
    <legend><asp:Literal Text="<%$ Resources: EPiServer, workroom.settings.availablefunctions %>" runat="server" /></legend>
    <asp:CheckBoxList ID="VisibilityList" runat="server" RepeatLayout="Flow" />
</fieldset>
<asp:Button OnClick="SaveVisibility_Click" 
    Text="<%$ Resources: EPiServer, button.save %>" CssClass="button" runat="server" ID="SaveButton"/>

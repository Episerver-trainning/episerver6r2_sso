<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Captcha.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.Captcha" %>

<fieldset class="<%= CssClass %>">
    <legend>
        <%= LegendText %></legend>
    <asp:Label ID="ClientTextLabel" AssociatedControlID="ClientText" runat="server" Text="<%$ Resources: EPiServer, captcha.label %>" />
    <asp:Image runat="server" ID="ServerImage" AlternateText="<%$ Resources: EPiServer, captcha.imagealternativetext %>" />
    <asp:CustomValidator runat="server" ID="ClientTextValidator" ControlToValidate="ClientText"
        OnServerValidate="ValidateClientText" SetFocusOnError="true" ValidateEmptyText="true"
        EnableClientScript="true" ClientValidationFunction="ValidateEmptyClientText"
        Text="*" ErrorMessage="<%$ Resources: EPiServer, captcha.errormessage %>" />
    <asp:TextBox ID="ClientText" runat="server" />
</fieldset>

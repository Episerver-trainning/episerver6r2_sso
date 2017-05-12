<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CreatePageBox.ascx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Units.CreatePageBox" %>

<asp:Panel ID="CreatePageBoxPanel" CssClass="hidden" runat="server" >
    <div class="<%=CssClass%>">
        <asp:Label ID="BoxLabel" runat="server" Text="" AssociatedControlID="PageNameTextBox" /><br />
        <asp:TextBox runat="server" ID="PageNameTextBox" MaxLength="255" />
        <asp:RequiredFieldValidator ID="PageNameRequiredFieldValidator" runat="server" ControlToValidate="PageNameTextBox" EnableClientScript="true"
            text="*" ErrorMessage="<%=RequiredFieldErrorMessage%>"></asp:RequiredFieldValidator>
        <asp:Button runat="server" CssClass="buttonExt" ID="AddNewPageButton" Text="<%$ Resources: EPiServer, button.save %>"
                OnClick="AddNewPageButton_Click"  OnClientClick="preventDoubleClick(this)"  />
        <asp:Button CssClass="button" ID="cancelButton" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>" />    
    </div>
</asp:Panel>


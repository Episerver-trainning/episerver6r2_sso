<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PersonalSettings.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.PersonalSettings" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>

<asp:Panel Runat="server" ID="SaveFailed" Visible="false">
	<h3><episerver:translate Text="/register/error/couldnotsave" runat="server" /></h3>
	<asp:Label ID="ErrorMessage" Runat="server" />
</asp:Panel>
<asp:Panel Runat="server" ID="SaveSucceeded" Visible="false">
	<h3><episerver:translate Text="/admin/edituser/usersaved" runat="server" /></h3>
	<asp:Label ID="SavedMessage" Runat="server" />
</asp:Panel>
<asp:Panel Runat="server" id="CreateEditUser" CssClass="mySettingsForm" DefaultButton="ApplyButton">
	<div runat="server" id="FirstNameRow">
        <asp:Label AssociatedControlID="FirstName" runat="server">
            <episerver:Translate Runat="server" text="/admin/secedit/firstname" />
        </asp:Label>
		<asp:TextBox ID="FirstName" Runat="server" />
		<asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
	</div>
	<div runat="server" id="LastNameRow">
	    <asp:Label AssociatedControlID="LastName" runat="server">
	        <episerver:Translate Runat="server" text="/admin/secedit/lastname" />
	    </asp:Label>
	    <asp:TextBox ID="LastName" Runat="server" />
	    <asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
    </div>
	<div runat="server" id="EmailRow">
        <asp:Label AssociatedControlID="Email" runat="server">
	        <episerver:Translate Runat="server" text="/admin/secedit/editemail" />
	    </asp:Label>
	    <asp:TextBox ID="Email" Runat="server" />
	    <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
	    <asp:RegularExpressionValidator runat="server" ControlToValidate="Email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="*" ></asp:RegularExpressionValidator>
	</div>
	<div runat="server" id="CompanyRow">
        <asp:Label AssociatedControlID="Company" runat="server">
	        <episerver:Translate Runat="server" text="/admin/secedit/editcompany" />
	    </asp:Label>
	    <asp:TextBox ID="Company" Runat="server" />
	</div>
	<div runat="server" id="TitleRow">
	    <asp:Label AssociatedControlID="Title" runat="server">
	        <episerver:Translate Runat="server" text="/admin/secedit/title" />
	    </asp:Label>
		<asp:TextBox ID="Title" Runat="server" />
	</div>
	<div runat="server" id="CountryRow">
	    <asp:Label AssociatedControlID="Country" runat="server">
	        <episerver:Translate Runat="server" text="/admin/secedit/editcountry" />
	    </asp:Label>
		<asp:TextBox ID="Country" Runat="server" />
	</div>
	<div class="buttons">
	<asp:Button id="ApplyButton" Runat="server" translate="/button/save" OnClick="ApplyButton_Click" CssClass="buttonExt"/>
	</div>
</asp:Panel>
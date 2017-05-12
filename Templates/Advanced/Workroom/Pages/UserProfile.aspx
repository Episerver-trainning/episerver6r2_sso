<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="UserProfile.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.UserProfile" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ID="ContentMain" ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR">
        <asp:Panel runat="server" ID="PanelMain" DefaultButton="ButtonSave">
            <asp:Panel runat="server" ID="PanelInfo" CssClass="displayPanelGroup">
                <asp:Panel runat="server" ID="PanelUserNameInfo" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelUserNameInfo" Text="<%$ Resources: EPiServer, workroom.userprofile.username %>" />
                    </div>
                    <div class="displayPanelShow">
                        <asp:Label runat="server" ID="LabelUserNameValue" />
                        <br />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelEmailInfo" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelEmailInfo" Text="<%$ Resources: EPiServer, workroom.userprofile.email %>" />
                    </div>
                    <div class="displayPanelShow">
                        <asp:Label runat="server" ID="LabelEmailValue" />
                        <br />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelRoleInfo" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelRoleInfo" Text="<%$ Resources: EPiServer, workroom.userprofile.role %>" />
                    </div>
                    <div class="displayPanelShow">
                        <asp:Label runat="server" ID="LabelRoleValue" />
                        <br />
                    </div>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="PanelProfile" CssClass="displayPanelGroup">
                <asp:Panel runat="server" ID="PanelFirstName" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelFirstName" AssociatedControlID="TextBoxFirstName"
                            Text="<%$ Resources: EPiServer, workroom.userprofile.firstname %>" />
                    </div>
                    <div class="displayPanelValue">
                        <asp:TextBox runat="server" ID="TextBoxFirstName" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorFirstName" ControlToValidate="TextBoxFirstName"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.firstnamerequired %>" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelLastName" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelLastName" AssociatedControlID="TextBoxLastName"
                            Text="<%$ Resources: EPiServer, workroom.userprofile.lastname %>" />
                    </div>
                    <div class="displayPanelValue">
                        <asp:TextBox runat="server" ID="TextBoxLastName" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorLastName" ControlToValidate="TextBoxLastName"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.lastnamerequired %>" />
                    </div>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="PanelMembership" CssClass="displayPanelGroup">
                <asp:Panel runat="server" ID="PanelUserName" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelUserName" AssociatedControlID="TextBoxUserName"
                            Text="<%$ Resources: EPiServer, workroom.userprofile.username %>" />
                    </div>
                    <div class="displayPanelValue">
                        <asp:TextBox runat="server" ID="TextBoxUserName" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorUserName" ControlToValidate="TextBoxUserName"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.usernamerequired %>" />
                        <asp:CustomValidator runat="server" ID="CustomValidatorUserName" ControlToValidate="TextBoxUserName"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.usernameexists %>"
                            OnServerValidate="CustomValidatorUserName_Validate" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelPassword" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelPassword" AssociatedControlID="TextBoxPassword"
                            Text="<%$ Resources: EPiServer, workroom.userprofile.password %>" />
                    </div>
                    <div class="displayPanelValue">
                        <asp:TextBox runat="server" ID="TextBoxPassword" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorPassword" ControlToValidate="TextBoxPassword"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.passwordrequired %>" />
                        <asp:CustomValidator runat="server" ID="CustomValidatorPassword" ControlToValidate="TextBoxPassword"
                            Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.passwordweak %>"
                            OnServerValidate="CustomValidatorPassword_Validate" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="PanelConfirmPassword" CssClass="displayPanel">
                    <div class="displayPanelLabel">
                        <asp:Label runat="server" ID="LabelConfirmPassword" AssociatedControlID="TextBoxConfirmPassword"
                            Text="<%$ Resources: EPiServer, workroom.userprofile.confirmpassword %>" />
                    </div>
                    <div class="displayPanelValue">
                        <asp:TextBox runat="server" ID="TextBoxConfirmPassword" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorPasswordConfirm"
                            ControlToValidate="TextBoxConfirmPassword" Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.passwordrequired %>" />
                        <asp:CompareValidator runat="server" ID="CompareValidatorPasswordConfirm" ControlToValidate="TextBoxConfirmPassword"
                            Display="Dynamic" ControlToCompare="TextBoxPassword" Operator="Equal" ErrorMessage="<%$ Resources: EPiServer, workroom.userprofile.error.passworddiffers %>" />
                    </div>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="PanelSave" CssClass="buttons">
                <asp:Button CssClass="buttonExt" runat="server" ID="ButtonSave" Text="<%$ Resources: EPiServer, button.save %>"
                    OnClick="ButtonSave_Click" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="PanelLogout" Visible="false">
            <asp:Label ID="LabelLogout" runat="server" Text="<%$ Resources: EPiServer, workroom.userprofile.error.logout %>" />
        </asp:Panel>
        <asp:Panel runat="server" ID="PanelUnavailable" Visible="false">
            <asp:Label ID="LabelUnavailable" runat="server" Text="<%$ Resources: EPiServer, workroom.userprofile.error.unavailable %>" />
        </asp:Panel>
        <asp:Panel runat="server" ID="PanelProvider" Visible="false">
            <asp:Label ID="LabelMembership" runat="server" Text="<%$ Resources: EPiServer, workroom.userprofile.error.membership %>" />
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SecondaryBodyRegion" />

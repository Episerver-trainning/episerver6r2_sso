<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="False" CodeBehind="Register.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.Register"%>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody"  Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
        <asp:CreateUserWizard Email="" ID="RegistrationWizard" CssClass="registerArea" CreateUserButtonStyle-CssClass="button" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server" OnPreRender="CreateUserWizardStep_PreRender">
                    <ContentTemplate>
                        <fieldset>
                            <asp:Label ID="UserNameLabel" Text="<%$ Resources: EPiServer, usersettings.username %>" AssociatedControlID="UserName" runat="server" />
                            <asp:TextBox runat="server" ID="UserName" />
                            <asp:Label ID="PasswordLabel" Text="<%$ Resources: EPiServer, register.password %>" AssociatedControlID="Password" runat="server" />
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                            <asp:Label ID="ConfirmPasswordLabel" Text="<%$ Resources: EPiServer, register.repeatpassword %>" AssociatedControlID="ConfirmPassword" runat="server" />
                            <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
                            <asp:Label ID="EmailLabel" Text="<%$ Resources: EPiServer, usersettings.email %>" AssociatedControlID="Email" runat="server" /> 
                            <asp:TextBox runat="server" ID="Email" />
                            <asp:RequiredFieldValidator ControlToValidate="UserName"
                                                        ErrorMessage="<%$ Resources: EPiServer, login.error.username %>"
                                                        ValidationGroup="RegistrationWizard"
                                                        Display="None"
                                                        EnableClientScript="false"
                                                        runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="Password"
                                                        ErrorMessage="<%$ Resources: EPiServer, register.error.password %>"
                                                        ValidationGroup="RegistrationWizard"
                                                        Display="None"
                                                        EnableClientScript="false"
                                                        runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword"
                                                        ErrorMessage="<%$ Resources: EPiServer, register.error.confirmpassword %>"
                                                        ValidationGroup="RegistrationWizard"
                                                        Display="None"
                                                        EnableClientScript="false"
                                                        runat="server" />
                            <asp:CompareValidator ControlToCompare="Password"
                                                  ControlToValidate="ConfirmPassword" 
                                                  ErrorMessage="<%$ Resources: EPiServer, register.error.passwordrepeat %>"
                                                  ValidationGroup="RegistrationWizard"
                                                  Display="None"
                                                  EnableClientScript="false"
                                                  runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="Email"
                                                        ErrorMessage="<%$ Resources: EPiServer, register.error.email %>"
                                                        ValidationGroup="RegistrationWizard"
                                                        Display="None"
                                                        EnableClientScript="false"
                                                        runat="server" />
                            <asp:RegularExpressionValidator ControlToValidate="Email"
                                                            ErrorMessage="<%$ Resources: EPiServer, register.error.incorrectemail %>"
                                                            ValidationExpression="^[A-Za-z0-9._%+-]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$"
                                                            ValidationGroup="RegistrationWizard"
                                                            Display="None"
                                                            EnableClientScript="false"
                                                            runat="server" />
                            <div id="MessageArea">
                                <asp:ValidationSummary ValidationGroup="RegistrationWizard" DisplayMode="List" CssClass="error" runat="server" />
                                <asp:Label ID="ErrorMessage" CssClass="error" runat="server" EnableViewState="False" />
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                    <ContentTemplate>
                        <div id="confirmationMessage">
                            <EPiServer:Property PropertyName="ConfirmationMessage" runat="server" />
                        </div>
                    </ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>                    
    <asp:Label runat="server" CssClass="error" ID="ProviderDoNotSupportCreateUser" Visible="false" />
    </div>
    
</asp:Content>
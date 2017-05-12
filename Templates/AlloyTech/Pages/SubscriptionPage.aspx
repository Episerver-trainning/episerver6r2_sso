<%@ Page Language="C#" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master" AutoEventWireup="False" Codebehind="SubscriptionPage.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.SubscriptionPage" %>
<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <AlloyTech:MainBody runat="server" />
        <asp:Panel ID="SubscriptionArea" Visible="false" CssClass="subscriptionArea" runat="server">
            <fieldset>
                <asp:ValidationSummary runat="server" ValidationGroup="SubscriptionWizard" />
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, subscription.email %>" CssClass="topLabel" AssociatedControlID="Email" runat="server" />
                    <asp:TextBox ID="Email" runat="server" />
                    <asp:RequiredFieldValidator ID="EmailRequiredValidator" ControlToValidate="Email"
                                                Text="*" ErrorMessage="<%$ Resources: EPiServer, subscription.error.email %>"
                                                ValidationGroup="SubscriptionWizard"
                                                Display="Dynamic" runat="server" CssClass="validator" />
                    <asp:RegularExpressionValidator ID="EmailFormatValidator" ControlToValidate="Email"
                                                    Text="*" ErrorMessage="<%$ Resources: EPiServer, subscription.error.incorrectemail %>"
                                                    ValidationExpression="^[A-Za-z0-9._%+-]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$"
                                                    ValidationGroup="SubscriptionWizard"
                                                    Display="Dynamic" runat="server" CssClass="validator" />
                </div>
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, subscription.interval %>" CssClass="topLabel" AssociatedControlID="Interval" runat="server" />
                    <asp:DropDownList ID="Interval" runat="Server">
                        <asp:ListItem Value="0" Text="<%$ Resources: EPiServer, subscription.fastaspossible %>" />
                        <asp:ListItem Value="1" Text="<%$ Resources: EPiServer, subscription.daily %>" />
                        <asp:ListItem Value="7" Text="<%$ Resources: EPiServer, subscription.weekly %>" />
                        <asp:ListItem Value="30" Text="<%$ Resources: EPiServer, subscription.monthly %>" />
                    </asp:DropDownList>
                </div>
                <div class="subscriptionListArea">
                    <EPiServer:Translate Text="/subscription/subscription" runat="server" />
                    <EPiServer:SubscriptionList ID="SubscriptionList" runat="server" language="<%#CurrentPage.LanguageID%>" />
                </div>
            </fieldset>
            <div>
                <asp:Button OnClick="Subscribe_Click" ValidationGroup="SubscriptionWizard" CssClass="button" Text="<%$ Resources: EPiServer, subscription.subscribe %>" runat="server" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
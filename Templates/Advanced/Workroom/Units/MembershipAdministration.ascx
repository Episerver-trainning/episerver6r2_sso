<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MembershipAdministration.ascx.cs"
    Inherits="EPiServer.Templates.Advanced.Workroom.Units.MembershipAdministration" %>
<%@ Register TagPrefix="EPiServer" Assembly="EPiServer.Templates.AlloyTech" Namespace="EPiServer.Templates.Advanced.Workroom.Core" %>
<div class="WRMembershipAdmin">
    <div class="buttonToolbar" id="ToolBarButtons" runat="server">
        <asp:LinkButton ID="EditMembers" Text="<%$ Resources: EPiServer, workroom.membershipmanager.button.editmembers %>"
            OnClick="EditMembers_Click" Visible="true" CssClass="linkButton editButton" runat="server" />
        <asp:LinkButton ID="AddMembers" Text="<%$ Resources: EPiServer, workroom.membershipmanager.button.addmembers %>"
            OnClick="AddMembers_Click" Visible="true" CssClass="linkButton createButton"
            runat="server" />
    </div>
    <h1>
        <asp:Literal ID="MemberListHeading" runat="server" />
    </h1>
    <asp:GridView runat="server" ID="UserList" DataSourceID="MemberObjectDataSource"
        CssClass="members" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
        DataKeyNames="Name" BorderWidth="0" GridLines="None">
        <RowStyle CssClass="uneven" />
        <AlternatingRowStyle CssClass="even" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="WRMembershipRemove" />
                <HeaderStyle CssClass="deleteColumnHeader" />
                <ItemTemplate>
                    <asp:PlaceHolder OnDataBinding="DeleteUserSelection_DataBinding" runat="server">
                        <asp:CheckBox ID="DeleteUserSelection" EnableViewState="true" runat="server" />
                    </asp:PlaceHolder>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workroom.membershipmanager.username %>">
                <ItemTemplate>
                    <asp:Literal ID="UserName" runat="server" Text="<%# Server.HtmlEncode(((MembershipEntry)Container.DataItem).Name) %>" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workroom.membershipmanager.status %>">
                <ItemTemplate>
                    <asp:PlaceHolder OnDataBinding="MembershipLevel_DataBinding" runat="server">
                        <asp:DropDownList ID="MembershipDropdown" OnSelectedIndexChanged="MembershipDropDown_SelectedIndexChanged"
                            runat="server" Visible="false" />
                        <asp:Literal ID="MembershipLiteral" runat="server" Visible="false" />
                    </asp:PlaceHolder>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources: EPiServer, workroom.membershipmanager.email %>">
                <ItemStyle CssClass="WRMembershipEmail" />
                <ItemTemplate>
                    <asp:PlaceHolder OnDataBinding="Email_DataBinding" runat="server">
                        <asp:HyperLink ID="UserEmail" runat="server" ToolTip="<%# Server.HtmlEncode(((MembershipEntry)Container.DataItem).Email) %>"
                            Text="<%# Server.HtmlEncode(((MembershipEntry)Container.DataItem).Email) %>" CssClass="WRMembershipEmailAddress" />
                    </asp:PlaceHolder>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerSettings Mode="NumericFirstLast" />
        <EmptyDataTemplate>
            <asp:Literal Text="<%$ Resources: EPiServer, workroom.membershipmanager.emptysearchresult %>"
                runat="server" />
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:Panel runat="server" ID="SendNotificationsConfirmation" CssClass="sendNotificationsConfirmation"
        Visible="false">
        <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.sendnotifications %>"
            runat="server" />
        <asp:RadioButton runat="server" ID="SendNotificationsButton" Checked="true" GroupName="SendNotificationsConfirmationButtons"
            Text="<%$ Resources: EPiServer, workroom.membershipmanager.sendnotificationsyes %>" />
        /
        <asp:RadioButton runat="server" ID="DoNotSendNotificationsButton" Checked="false"
            GroupName="SendNotificationsConfirmationButtons" Text="<%$ Resources: EPiServer, workroom.membershipmanager.sendnotificationsno %>" />
    </asp:Panel>
    <asp:Label runat="server" ID="NotificationErrorPanel" Visible="false" CssClass="sendInvitationError"
        EnableViewState="false" />
    <div id="DeleteButton" class="buttons" visible="false" runat="server">
        <asp:Button ID="DeleteUserButton" runat="server" CssClass="button" OnClick="DeleteUser_Click"
            Text="<%$ Resources: EPiServer, workroom.membershipmanager.button.remove %>" />
    </div>
    <div id="PersistButtons" class="buttons" visible="false" runat="server">
        <asp:Button ID="SaveMembership" Text="<%$ Resources: EPiServer, button.save %>" OnClick="SaveMembership_Click"
            CssClass="buttonExt" runat="server" />
        <asp:Button ID="CancelEdit" Text="<%$ Resources: EPiServer, button.cancel %>" OnClick="CancelEdit_Click"
            CssClass="button" runat="server" />
    </div>
    <div runat="server" id="DeleteUserDialog" visible="false">
        <h2 class="caution">
            <asp:Literal Text="<%$ Resources: EPiServer, workroom.membershipmanager.removeheading %>"
                runat="server" />
        </h2>
        <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.confirmremovemember %>"
            runat="server" />
        <div id="ConfirmDeleteButtons" class="buttons" runat="server" visible="false">
            <asp:Button ID="DeleteUserConfirmButton" Text="<%$ Resources: EPiServer, workroom.membershipmanager.button.remove %>"
                OnClick="DeleteUserConfirm_Click" CssClass="buttonExt" runat="server" />
            <asp:Button ID="DeleteUserCancelButton" Text="<%$ Resources: EPiServer, button.cancel %>"
                OnClick="DeleteUserCancel_Click" CssClass="button" runat="server" />
        </div>
    </div>
    <div runat="server" id="SearchDialog" visible="false" class="searchDialog">
        <fieldset>
            <legend>
                <asp:Literal Text="<%$ Resources: EPiServer, workroom.membershipmanager.searchheading %>"
                    runat="server" />
            </legend>
            <div>
                <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.username %>"
                    AssociatedControlID="UserName" runat="server" />
                <asp:TextBox ID="UserName" runat="server" />
            </div>
            <div>
                <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.email %>" AssociatedControlID="Email"
                    runat="server" />
                <asp:TextBox ID="Email" runat="server" />
            </div>
            <div class="buttons">
                <asp:Button ID="SearchUsers" Text="<%$ Resources: EPiServer, workroom.membershipmanager.searchtext %>"
                    OnClick="SearchUsers_Click" CssClass="buttonExt" runat="server" />
            </div>
        </fieldset>
        <fieldset>
            <legend>
                <asp:Literal Text="<%$ Resources: EPiServer, workroom.membershipmanager.inviteheading %>"
                    runat="server" />
            </legend>
            <asp:Label runat="server" ID="InviteMessage" Text="<%$ Resources: EPiServer, workroom.membershipmanager.invitemembermessage %>" CssClass="message" />
            <div>
                <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.username %>"
                    AssociatedControlID="InviteUserName" runat="server" />
                <asp:TextBox ID="InviteUserName" runat="server" />
            </div>
            <div>
                <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.email %>" AssociatedControlID="Email"
                    runat="server" />
                <asp:TextBox ID="InviteEmail" runat="server" />
            </div>
            <div>
                <asp:Label Text="<%$ Resources: EPiServer, workroom.membershipmanager.invitemembership %>"
                    AssociatedControlID="InviteMembershipDropDown" runat="server" />
                <asp:DropDownList ID="InviteMembershipDropDown" runat="server" />
                <asp:Panel runat="server" ID="SendOkPanel" Visible="false" CssClass="sendInvitationError">
                    <%= Translate("/workroom/membershipmanager/sendinvitationok") %>
                </asp:Panel>
                <asp:Panel runat="server" ID="SendErrorPanel" Visible="false" CssClass="sendInvitationError">
                    <%= Translate("/workroom/membershipmanager/sendinvitationerror") %>&nbsp;<asp:Label
                        runat="server" ID="SendInvitationErrorMessage"></asp:Label>
                </asp:Panel>
            </div>
            <div class="buttons">
                <asp:Button ID="InviteUserButton" Text="<%$ Resources: EPiServer, workroom.membershipmanager.button.sendinvitation %>"
                    OnClick="SendInvitation_Click" CssClass="button" runat="server" />
            </div>
        </fieldset>
    </div>
</div>
<asp:ObjectDataSource runat="server" ID="MemberObjectDataSource" EnablePaging="true"
    OnSelecting="MemberObjectDataSource_Selecting" TypeName="EPiServer.Templates.Advanced.Workroom.Core.MembershipSearch"
    SelectMethod="FindUsers" SelectCountMethod="GetRowCount">
    <SelectParameters>
        <asp:Parameter Direction="Input" DefaultValue="true" Name="onlyExistingMembers" Type="Boolean" />
        <asp:ControlParameter Direction="Input" DefaultValue="" ControlID="UserName" PropertyName="Text"
            Name="userName" />
        <asp:ControlParameter Direction="Input" DefaultValue="" ControlID="Email" PropertyName="Text"
            Name="email" />
    </SelectParameters>
</asp:ObjectDataSource>

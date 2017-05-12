<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="CalendarEvent.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.CalendarEvent" %>

<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<%@ Register TagPrefix="units" TagName="DateTimePicker" Src="~/Templates/AlloyTech/Units/Placeable/DateTimePicker.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR">
        <asp:Panel ID="DisplayPanel" runat="server">
            <div class="buttonToolbar">
                <asp:LinkButton ID="EditButton" CssClass="linkButton editButton" runat="server" Text="<%$ Resources: EPiServer, button.edit %>"
                    OnClick="Edit_Click" Visible="false" />
                <asp:LinkButton ID="DeleteButton" CssClass="linkButton deleteButton" runat="server" Text="<%$ Resources: EPiServer, button.delete %>"
                    OnClick="Delete_Click" Visible="false" />
            </div>
            <EPiServer:Property PropertyName="PageName" CustomTagName="h1" runat="server" />
            <span class="datetime"><%= GetDisplayDateString() %></span>
            <EPiServer:Property PropertyName="MainBody" runat="server" />
        </asp:Panel>
        <asp:Panel ID="EditPanel" runat="server" Visible="false">
            <h1 runat="server" id="EditPanelCaption">
                <asp:Literal Text="<%$ Resources: EPiServer, workroom.calendar.editheading %>" runat="server" /></h1>
            <div class="createNews">
                <asp:ValidationSummary CssClass="validation" runat="server" />
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, workroom.newslistpage.headinglabel %>"
                        AssociatedControlID="EditPageName" runat="server" /><br />
                    <asp:TextBox ID="EditPageName" runat="server" MaxLength="255" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="EditPageName" EnableClientScript="true"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.newslistpage.titleempty %>" />
                </div>
                <div>
                    <asp:Label ID="Label1" Text="<%$ Resources: EPiServer, workroom.calendar.starttimelabel %>"
                        AssociatedControlID="StartDatePicker" runat="server" /><br />
                    <units:DateTimePicker ID="StartDatePicker" runat="server"></units:DateTimePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="StartDatePicker"
                        runat="server" EnableClientScript="true" Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.calendar.starttimeempty %>" />
                </div>
                <div>
                    <asp:Label ID="Label2" Text="<%$ Resources: EPiServer, workroom.calendar.stoptimelabel %>"
                        AssociatedControlID="StopDatePicker" runat="server" /><br />
                    <units:DateTimePicker ID="StopDatePicker" runat="server"></units:DateTimePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="StopDatePicker"
                        runat="server" EnableClientScript="true" Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.calendar.stoptimeempty %>" />
                </div>
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, workroom.newslistpage.bodylabel %>" AssociatedControlID="EditMainBody"
                        runat="server" /><br />
                    <AlloyTech:RichTextEditor ID="EditMainBody" runat="server" />
                </div>
                <div class="buttons">
                    <asp:Button ID="EditButtonSave" CssClass="buttonExt" runat="server" Text="<%$ Resources: EPiServer, button.save %>"
                        OnClick="Save_Click" />
                    <asp:Button ID="EditButtonCancel" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>"
                        OnClick="Cancel_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="SecondaryBodyRegion" runat="server">
    <div id="SecondaryBody">
        <div class="CalendarGrid">
            <asp:Calendar ID="CalendarView" PrevMonthText="-" NextMonthText="+" DayNameFormat="FirstTwoLetters"
                runat="server" SelectionMode="DayWeekMonth" CellPadding="0" BorderWidth="0">
                <NextPrevStyle CssClass="nextPrev" ForeColor="#666666" />
                <TitleStyle CssClass="title" BackColor="Transparent" />
                <DayHeaderStyle CssClass="dayHeader" HorizontalAlign="NotSet" />
                <SelectorStyle CssClass="selector" HorizontalAlign="NotSet" ForeColor="#398ac9" />
                <DayStyle ForeColor="#666666" />
                <SelectedDayStyle CssClass="selected" ForeColor="#ffffff" BackColor="#398ac9" />
                <OtherMonthDayStyle CssClass="otherMonth" ForeColor="#aaaaaa" />
            </asp:Calendar>
        </div>
    </div>
</asp:Content>

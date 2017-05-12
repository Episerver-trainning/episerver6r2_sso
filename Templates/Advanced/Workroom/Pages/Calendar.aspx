<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Calendar.aspx.cs" Inherits="EPiServer.Templates.Advanced.Workroom.Pages.Calendar" %>
<%@ Register TagPrefix="WorkRoom" Namespace="EPiServer.Templates.Advanced.Workroom.Units" Assembly="EPiServer.Templates.AlloyTech" %>

<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<%@ Register TagPrefix="AlloyTech" TagName="DateTimePicker" Src="~/Templates/AlloyTech/Units/Placeable/DateTimePicker.ascx" %>
<%@ Register TagPrefix="Workroom" TagName="CreatePageBox" Src="../Units/CreatePageBox.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadRegion">
    <link type="text/css" rel="stylesheet" href="<%= Page.ResolveUrl("~/Templates/Advanced/Workroom/Styles/WRStyles.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody" class="WR">
        <asp:Panel runat="server" ID="PanelHeading">
            <asp:Panel runat="server" ID="EditCalendarSectionButtons" CssClass="buttonToolbar"
                Visible="false">
                <a class="linkButton createButton" id="AddNewsSectionButton" href="<%= GetPage(CurrentPage.ParentLink).LinkURL.ToString()+"&addmode=true" %>">
                    <%=Translate("/workroom/calendarlist/calendaraddnew")%></a> <a class="linkButton editButton"
                        id="EditCalendarSectionNameButton" href="<%=EditCalendarBox.GetActivateJavaScript(PanelHeading.ClientID)%>">
                        <%=Translate("/workroom/calendar/buttoneditname")%></a>
                <asp:LinkButton ID="DeleteCalendarSectionLinkButton" CssClass="linkButton deleteButton"
                    runat="server" Text="<%$ Resources: EPiServer, workroom.calendar.buttondeletecalendar %>"
                    OnClick="DeleteCalendarSection_Click" />
            </asp:Panel>
            <EPiServer:Property ID="PageNameProperty" CustomTagName="h1" PropertyName="PageName"
                runat="server" />
        </asp:Panel>
        <Workroom:CreatePageBox runat="server" ID="EditCalendarBox" Visible="false" />
        <div class="buttonToolbar">
            <asp:LinkButton ID="AddButton" CssClass="linkButton createButton" runat="server"
                Text="<%$ Resources: EPiServer, workroom.calendar.buttonaddevent %>" OnClick="AddButton_Click"
                Visible="false" />
        </div>
        <%-- EVENT LIST --%>
        <asp:Panel ID="DisplayPanel" runat="server">
            <EPiServer:Property PropertyName="MainBody" runat="server" />
            <p class="selectedPeriod">
                <asp:Literal runat="server" Text="<%$ Resources: EPiServer, workroom.calendar.selectedperiod %>" />
                <strong>
                    <%= GetSelectedPeriod() %>
                </strong>
            </p>
            <Workroom:EventList ID="CalendarList" PageLinkProperty="PageLink" NumberOfDaysToRender='<%# GetNumberOfDayToRender() %>'
                runat="server">
                <eventtemplate>
                    <div>	                               
                        <EPiServer:Property Runat="server" CustomTagName="h3" PropertyName="PageLink" />
                        <span class="datetime"><%#GetFormattedTimePeriod(Container.StartDate, Container.StopDate)%></span>
                    </div>
                </eventtemplate>
            </Workroom:EventList>
        </asp:Panel>
        <%-- CREATE/EDIT FORM --%>
        <asp:Panel ID="EditPanel" runat="server" Visible="false">
            <h2>
                <asp:Literal Text="<%$ Resources: EPiServer, workroom.calendar.createheading %>"
                    runat="server" />
            </h2>
            <div class="createNews">
                <asp:ValidationSummary CssClass="validation" runat="server" />
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, workroom.newslistpage.headinglabel %>"
                        AssociatedControlID="EditPageName" runat="server" /><br />
                    <asp:TextBox ID="EditPageName" runat="server" MaxLength="255" />
                    <asp:RequiredFieldValidator ControlToValidate="EditPageName" runat="server" EnableClientScript="true"
                        Text="*" Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.calendar.nameempty %>" />
                </div>
                <div>
                    <asp:Label Text="<%$ Resources: EPiServer, workroom.calendar.starttimelabel %>" AssociatedControlID="StartDatePicker"
                        runat="server" /><br />
                    <AlloyTech:DateTimePicker ID="StartDatePicker" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="StartDatePicker" runat="server" EnableClientScript="true"
                        Text="*" Display="Dynamic" ErrorMessage="<%$ Resources: EPiServer, workroom.calendar.starttimeempty %>" />
                    <br />
                    <asp:Label Text="<%$ Resources: EPiServer, workroom.calendar.stoptimelabel %>" AssociatedControlID="StopDatePicker"
                        runat="server" /><br />
                    <AlloyTech:DateTimePicker ID="StopDatePicker" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="StopDatePicker" runat="server" EnableClientScript="true"
                        Text="*" ErrorMessage="<%$ Resources: EPiServer, workroom.calendar.stoptimeempty %>" />
                </div>
                <div>
                    <asp:Label ID="Label1" Text="<%$ Resources: EPiServer, workroom.newslistpage.bodylabel %>"
                        AssociatedControlID="EditMainBody" runat="server" /><br />
                    <AlloyTech:RichTextEditor ID="EditMainBody" runat="server" />
                </div>
                <div class="buttons">
                    <asp:Button ID="SaveButton" CssClass="buttonExt" runat="server" Text="<%$ Resources: EPiServer, button.save %>"
                        OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, button.cancel %>"
                        OnClick="CancelButton_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <script type="text/javascript">
            //<![CDATA[
            $(document).ready(function() {
                $('#<%=StartDatePicker.Input.ClientID %>').change(function() {
                    var startInputElement = document.getElementById('<%= StartDatePicker.Input.ClientID %>');
                    var stopInputElement = document.getElementById('<%= StopDatePicker.Input.ClientID %>');

                    if (stopInputElement.value != '')
                        return;

                    stopInputElement.value = startInputElement.value;
                });
            });
            //]]>
        </script>

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
    <%--
        MonthlyItems is a hidden calendar list used to calculate what days to highlight in CalenderView.
    --%>
    <EPiServer:Calendar runat="server" ID="MonthlyItems" PageLinkProperty="PageLink">
        <EventTemplate>
        </EventTemplate>
    </EPiServer:Calendar>
</asp:Content>

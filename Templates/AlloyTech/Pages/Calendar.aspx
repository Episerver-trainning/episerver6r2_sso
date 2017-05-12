<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Calendar.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Pages.Calendar" %>

<%@ Register TagPrefix="AlloyTech" TagName="MainBody" Src="~/Templates/AlloyTech/Units/Placeable/MainBody.ascx" %>
<asp:Content ContentPlaceHolderID="MainBodyRegion" runat="server">
    <div id="MainBody">
        <asp:Panel ID="DisplayPeriod" runat="server">
            <AlloyTech:MainBody runat="server" />
            <div class="CalendarList">
                <p class="selectedPeriod">
                    <asp:Literal runat="server" Text="<%$ Resources: EPiServer, calendar.selectedperiod %>" />
                    <strong>
                        <%= GetSelectedPeriod() %></strong>
                </p>
                <EPiServer:Calendar ID="CalendarList" PageLinkProperty="CalendarItemContainer" NumberOfDaysToRender='<%# GetNumberOfDayToRender() %>'
                    runat="server">
                    <DayPrefixTemplate>
                        <h2>
                            <%#Container.Date.ToString(ListDateFormat)%>
                        </h2>
                    </DayPrefixTemplate>
                    <DaySuffixTemplate>
                        <asp:PlaceHolder runat="server" Visible="<%# !CalendarList.ActiveSelectedDates.Contains(Container.Date.Date) %>">
                            <div class="event empty">
                                <asp:Literal runat="server" Text="<%$ Resources: EPiServer, calendar.noeventonday %>" />
                            </div>
                        </asp:PlaceHolder>
                    </DaySuffixTemplate>
                    <EventTemplate>
                        <div class="event">
                            <asp:Label Text="<%# GetTextAndUpdateLabelProperties(Container) %>" ID="EventDate"
                                runat="server" CssClass="eventDate" />
                            <h3>
                                <asp:LinkButton OnCommand="LinkButton_Command" CommandArgument='<%# Container.CurrentPage.PageLink.ID %>'
                                    runat="server" CausesValidation="false"><EPiServer:Property PropertyName="PageName" runat="server" /></asp:LinkButton>
                            </h3>
                        </div>
                    </EventTemplate>
                </EPiServer:Calendar>
            </div>
        </asp:Panel>
        <%-- Event details display --%>
        <asp:Panel ID="DisplayEvent" Visible="false" runat="server">
            <EPiServer:Property ID="EventPageName" CustomTagName="h1" PropertyName="PageName" runat="server" />
            <asp:Label ID="EventDate" runat="server" CssClass="datetime" />
            <EPiServer:Property ID="EventMainBody" PropertyName="MainBody" runat="server" />
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
        <%-- MonthlyItems is a hidden calendar list used to calculate what days to highlight in CalenderView. --%>
        <EPiServer:Calendar runat="server" ID="MonthlyItems" PageLinkProperty="CalendarItemContainer">
            <EventTemplate>
            </EventTemplate>
        </EPiServer:Calendar>
    </div>
</asp:Content>

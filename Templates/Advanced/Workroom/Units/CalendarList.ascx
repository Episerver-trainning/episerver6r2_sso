<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CalendarList.ascx.cs"
    Inherits="EPiServer.Templates.Advanced.Workroom.Units.CalendarList" %>
<asp:Panel runat="server" ID="ContainerView" Visible="true">
    <div class="WRCalendarList WRList">
        <h2>
            <asp:Literal runat="server" Text="<%$ Resources: EPiServer, workroom.eventlist.heading %>" />
        </h2>
        <asp:Panel runat="server" ID="listPanel" Visible="false">
            <EPiServer:PageList DataSource='<%# UpcomingEvents %>' runat="server" ID="CalendarEventList">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <EPiServer:Property runat="server" CustomTagName="h3" PropertyName="PageLink" />
                        <span class="datetime">
                            <%# GetDateString(Container.CurrentPage) %></span> </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </EPiServer:PageList>
        </asp:Panel>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="ListView" Visible="false">
    <EPiServer:PageList DataSource='<%# UpcomingEvents %>' runat="server" ID="PageList">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li><span class="datetime">
                <%# GetDateString(Container.CurrentPage) %></span>
                <EPiServer:Property runat="server" PropertyName="PageLink" />
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </EPiServer:PageList>
</asp:Panel>

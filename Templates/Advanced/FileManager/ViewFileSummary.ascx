<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewFileSummary.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.ViewFileSummary" %>
<fieldset>
    <legend><%= Legend %></legend>
    <asp:Table ID="PropertyTable" runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <EPiServer:Translate Text="/filemanager/browse/size" runat="server" />:
            </asp:TableCell>
            <asp:TableCell><%=SizeString %></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <EPiServer:Translate Text="/filemanager/browse/created" runat="server" />:
            </asp:TableCell>
            <asp:TableCell><%=CreatedString %></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <EPiServer:Translate Text="/filemanager/browse/modified" runat="server" />:
            </asp:TableCell>
            <asp:TableCell><%=ModifiedString %></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                <EPiServer:Translate Text="/filemanager/viewfile/linkingpages" runat="server" />
                :</asp:TableCell>
            <asp:TableCell>
                <asp:Repeater ID="linkedPagesList" DataSource="<%# LinkingPages %>" runat="server">
	                <HeaderTemplate><ul class="horizontal"></HeaderTemplate>
	                <ItemTemplate>
		                <li>
		                    <%# GetLinkingPageString(Container.DataItem) %>
		                </li>
	                </ItemTemplate>
	                <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <p>
        <asp:Button CssClass="buttonExt" runat="server" OnClick="BackButton_Click" Text="<%$ Resources: EPiServer, filemanager.button.back %>" ToolTip="<%$ Resources: EPiServer, filemanager.button.back %>"  />
        <span class="WRbuttonsDiv">&nbsp;</span>
        <asp:Button ID="EditButton" CssClass="buttonExt" runat="server" OnClick="EditSummaryButton_Click" Text="<%$ Resources: EPiServer, button.edit %>" ToolTip="<%$ Resources: EPiServer, button.edit %>"  />
        
    </p>
</fieldset>

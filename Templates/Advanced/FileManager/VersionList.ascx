<%@ Control Language="C#" AutoEventWireup="False" CodeBehind="VersionList.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.VersionList" %>
<%@ Register TagPrefix="EPiServer" Assembly="EPiServer.Templates.AlloyTech" Namespace="EPiServer.Templates.Advanced.Workroom.Core" %>
<fieldset>
    <legend><%= Legend %></legend>
    <asp:Repeater runat="server" ID="VersionRepeater" OnItemCommand="RepeaterItemCommand">
        <HeaderTemplate>
            <table class="fileList" >
            <tr>
                <th></th>
                <th><div class="<%=GetSortingCssClass("version")%>">
                    <asp:linkbutton runat="server" Text="<%$ Resources: EPiServer, filemanager.versionlistheading.version %>" 
                        CommandArgument="version" OnCommand="LinkButton_Command" />
                    </div>
                    </th>
                <th><div class="<%=GetSortingCssClass("comments")%>">
                    <asp:linkbutton  runat="server" Text="<%$ Resources: EPiServer, filemanager.versionlistheading.comments %>" 
                        CommandArgument="comments" OnCommand="LinkButton_Command" />
                    </div>
                    </th>
                <th><div class="<%=GetSortingCssClass("size")%>">
                    <asp:linkbutton  runat="server" Text="<%$ Resources: EPiServer, filemanager.versionlistheading.size %>" 
                        CommandArgument="size" OnCommand="LinkButton_Command" />
                    </div>
                    </th>
                <th><div class="<%=GetSortingCssClass("createdby")%>">
                    <asp:linkbutton runat="server" Text="<%$ Resources: EPiServer, filemanager.versionlistheading.createdby %>" 
                        CommandArgument="createdby" OnCommand="LinkButton_Command" />
                </div></th>
                <th><div class="<%=GetSortingCssClass("created")%>">
                    <asp:linkbutton  runat="server" Text="<%$ Resources: EPiServer, filemanager.versionlistheading.created %>" 
                        CommandArgument="created" OnCommand="LinkButton_Command" />
                    </div>
                    </th>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr <%# SetSelectedStyle((( VersionListItem )Container.DataItem).Id.ToString()) %>>
                <td><EPiServer:EPiRadioButton runat="server" AutoPostBack="true"  
                        CommandName="SelectVersion" CommandArgument='<%# (( VersionListItem )Container.DataItem).Id %>' 
                        Checked="<%# (( VersionListItem )Container.DataItem).Id.ToString().Equals(FileManager.SelectedFileVersionId) %>" /> 
                </td>
                <td>
                    <asp:HyperLink runat="server" Target="_blank" Text='<%# (( VersionListItem )Container.DataItem).VersionNumber %>' NavigateUrl='<%# ((VersionListItem)Container.DataItem ).VirtualPath+"/"+FileManager.SingleSelectedFile.Name %>' ></asp:HyperLink>
                </td>
                <td ><%# Server.HtmlEncode(( ( VersionListItem ) Container.DataItem ).Comments) %> </td>
                <td style="text-align:right"><%# (( VersionListItem )Container.DataItem).Size %></td>
                <td style="text-align:right"><%# ( ( VersionListItem ) Container.DataItem ).CreatedBy%></td>
                <td><%# ( ( VersionListItem ) Container.DataItem ).Created %></td>
            </tr>
        </ItemTemplate> 
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <p>
        <asp:Button ID="ButtonRestore" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, filemanager.button.restore %>" OnClick="RestoreItems_Click"/>
        <asp:Button ID="ButtonDelete" CssClass="button" runat="server" Text="<%$ Resources: EPiServer, filemanager.button.deleteversion %>" OnClick="DeleteItems_Click"/>
        <asp:Button ID="Button1" CssClass="button" runat="server" OnClick="BackButton_Click" Text="<%$ Resources: EPiServer, filemanager.button.back %>"/>
    </p>
</fieldset>
<%@ Control Language="C#" AutoEventWireup="False" CodeBehind="FileList.ascx.cs" Inherits="EPiServer.Templates.Advanced.FileManager.FileList" %>
<%@ Register TagPrefix="EPiServer" Assembly="EPiServer.Templates.AlloyTech" Namespace="EPiServer.Templates.Advanced.Workroom.Core" %>

<asp:Repeater runat="server" ID="FileRepeater" OnItemCommand="RepeaterItemCommand" >
    <HeaderTemplate>
        <table class="fileList" >
        <tr>
            <th></th>
            <th><div class="<%=GetSortingCssClass("name")%>"><asp:linkbutton ID="sortButtonName" runat="server" 
                Text="<%$ Resources: EPiServer, filemanager.filelistheading.name %>" CommandArgument="name" OnCommand="LinkButton_Command" />
                </div></th>
            <th><div class="<%=GetSortingCssClass("size")%>"><asp:linkbutton ID="sortButtonSize" runat="server" 
                Text="<%$ Resources: EPiServer, filemanager.filelistheading.size %>" CommandArgument="size" OnCommand="LinkButton_Command" />
                </div></th>
            <th><div class="<%=GetSortingCssClass("checkedOut")%>"><asp:linkbutton ID="sortButtonCheckedOut" runat="server" 
                Text="<%$ Resources: EPiServer, filemanager.filelistheading.checkedout %>" CommandArgument="checkedOut" OnCommand="LinkButton_Command" />
                </div></th>
            <th><div class="<%=GetSortingCssClass("lastChanged")%>"><asp:linkbutton ID="sortButtonLastChanged" runat="server" 
                Text="<%$ Resources: EPiServer, filemanager.filelistheading.lastchanged %>" CommandArgument="lastChanged" OnCommand="LinkButton_Command" />
                </div></th>
        </tr>
        <tr runat="server" Visible='<%# FileManager.CurrentVirtualDirectory.VirtualPath!=FileManager.RootVirtualPath %>'>
            <td></td>
            <td><asp:LinkButton runat="server" Text="<%$ Resources: EPiServer, filemanager.parentfolder %>" OnCommand="ParentFolder" CommandName="SelectFolder" CssClass="document folderup" ></asp:LinkButton></td>
            <td></td>
            <td></td>
            <td></td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr <%# FileManager.SelectedItems.Contains((string)Eval("VirtualPath")) ? "class='selected'" : "" %>>
            <td><EPiServer:EPiCheckBox runat="server" AutoPostBack="true" CommandName="SelectFile" CommandArgument='<%# Eval("VirtualPath") %>' Checked='<%# FileManager.SelectedItems.Contains((string)Eval("VirtualPath")) %>' />  </td>
            
            <td>
                <asp:HyperLink runat="server" Target="_blank" NavigateUrl='<%# Eval("VirtualPath") %>' Text='<%# Eval("Name") %>' Visible='<%# !(bool)Eval("isDirectory") %>' CssClass="<%# GetFileExtensionCss(Container.DataItem) %>" ></asp:HyperLink>
                <asp:LinkButton runat="server" Visible='<%# (bool)Eval("isDirectory") %>' CssClass="<%# GetFileExtensionCss(Container.DataItem) %>" Text='<%# Eval("Name") %>' OnCommand="SelectFolder" CommandName="SelectFolder" CommandArgument='<%# Eval("VirtualPath") %>' />                     
            </td>
            
            <td style="text-align:right"><%# GetFileSizeString((System.Web.Hosting.VirtualFileBase)Container.DataItem)%> </td>
            <td style="text-align:left"><%# GetCheckedOutString((System.Web.Hosting.VirtualFileBase)Container.DataItem)%></td>
            <td><%# GetChangedDateString(Container.DataItem) %></td>
        </tr>
    </ItemTemplate> 
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
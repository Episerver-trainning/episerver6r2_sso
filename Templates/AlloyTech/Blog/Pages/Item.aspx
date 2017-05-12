<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Templates/AlloyTech/MasterPages/MasterPage.master"
    CodeBehind="Item.aspx.cs" Inherits="EPiServer.Templates.AlloyTech.Blog.Pages.Item" %>
<%@ Import Namespace="EPiServer.Templates.AlloyTech" %>    

<%@ Register Src="~/Templates/AlloyTech/Blog/Units/SecondaryList.ascx" TagPrefix="Blog"
    TagName="SecondaryList" %>
<%@ Register Src="~/Templates/AlloyTech/Units/Placeable/captcha.ascx" TagPrefix="AlloyTech"
    TagName="Captcha" %>
<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<asp:Content ContentPlaceHolderID="MainContentRegion" runat="server">
    <div id="MainBodyArea">
        <div id="MainBody">
          <EPiServer:Property runat="server" CustomTagName="h1" PropertyName="PageName" />
            <div class="blogItem">
              
                <span class="bloginfo">
                    <%= CurrentPage.GetFormattedPublishDate() %></span>
                <span class="bloginfo" runat="server" visible="<%# ShowPostedBy %>">
                    <EPiServer:Translate runat="server" Text="/blog/item/postedby" />
                    <a href="<%= BlogStart.LinkURL %>">
                        <EPiServer:Property runat="server" PropertyName="Writer" CssClass="name" PageLink="<%# BlogStart.PageLink %>" />
                    </a>
                </span>
                <EPiServer:Property runat="server" PropertyName="MainBody" />
                
                <div class="tags">
                    <span class="bloginfo">
                    <EPiServer:Translate runat="server" Text="/blog/item/tags" />
                     </span>
                    <EPiServer:PageList runat="server" DataSource="<%# Tags %>">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li>
                                <EPiServer:Property PropertyName="PageLink" runat="server" />
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </EPiServer:PageList>
                </div>
            </div>
            <hr />
            <h3 id="Comments">
              <EPiServer:Translate runat="server" Text="/blog/item/commentheading" />
            </h3>
            <EPiServer:PageList ID="CommentList" SortOrder="CreatedAscending" runat="server"
                PageLinkProperty="PageLink">
                <HeaderTemplate>
                    <ol class="comments">
                </HeaderTemplate>
                <ItemTemplate>
                    <li id="comment<%# Container.CurrentPage.PageLink.ID.ToString() %>" class="comment">
                        <asp:Literal ID="CommentBody" Text="<%# GetFixedMainBody(Container.CurrentPage)%>"
                            runat="server" />
                        <div class="footer">
                            <span class="commentinfo">
                                <%# Container.CurrentPage.GetFormattedPublishDateWithTime()%>
                                   <EPiServer:Translate runat="server" Text="/blog/item/postedby" />
                              
                                <EPiServer:Property runat="server" PropertyName="Writer" />
                            </span>
                            <a href="#comment<%# Container.CurrentPage.PageLink.ID.ToString() %>">
                               <EPiServer:Translate runat="server" Text="/blog/item/permanentlink" />
                               
                            </a>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ol>
                </FooterTemplate>
            </EPiServer:PageList>
            
            <fieldset runat="server" id="CommentArea" class="comment">
                <legend>
                <EPiServer:Translate runat="server" Text="/blog/item/commentlegend" />
                  </legend>
                <asp:ValidationSummary runat="server" />
                <asp:Label AssociatedControlID="CommentName" runat="server" Text="<%$ Resources: EPiServer, blog.item.name%>" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="CommentName"
                    Text="*" ErrorMessage="<%$ Resources: EPiServer, blog.item.errormessages.requiredname%>"
                    EnableClientScript="false" Display="Dynamic" />
                <asp:TextBox runat="server" CssClass="textinput" ID="CommentName" TextMode="SingleLine"
                    MaxLength="255" />
                <asp:Label AssociatedControlID="CommentText" runat="server" Text="<%$ Resources: EPiServer, blog.item.comment%>" />
                <asp:RequiredFieldValidator ID="CommentTextRequiredFieldValidator" runat="server"
                    ControlToValidate="CommentText" Text="*" ErrorMessage="<%$ Resources: EPiServer, blog.item.errormessages.requiredcomment%>"
                    EnableClientScript="false" Display="Dynamic" />
                <AlloyTech:RichTextEditor ID="CommentText" runat="server" />
                <AlloyTech:Captcha runat="server" LegendText="<%$ Resources: EPiServer, captcha.heading%>"
                    ID="Captcha" />
                <p class="WRbuttonsDiv">
                </p>
                <asp:Button ID="PostCommentButton" CssClass="buttonExt" runat="server" OnClick="PostComment_Click"
                    Text="<%$ Resources: EPiServer, blog.item.send%>" />
            </fieldset>
        </div>
    </div>
    <div id="SecondaryBodyArea">
        <div id="SecondaryBody">
            <Blog:SecondaryList runat="server" BlogStart="<%# BlogStart %>" ShowDetails="<%# ShowDetails %>" />
        </div>
    </div>
</asp:Content>

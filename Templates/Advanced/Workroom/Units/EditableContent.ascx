<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditableContent.ascx.cs"
    Inherits="EPiServer.Templates.Advanced.Workroom.Units.EditableContent" %>
<%@ Register TagPrefix="AlloyTech" TagName="RichTextEditor" Src="~/Templates/AlloyTech/Units/Placeable/TinyMCETextEditor.ascx" %>
<asp:MultiView runat="server" ID="MultiviewMain" ActiveViewIndex="0">
    <asp:View runat="server" ID="ViewShow">
        <div class="buttonToolbar">
            <asp:LinkButton CssClass="linkButton editButton" ID="LinkButtonEdit" OnClick="LinkButtonEdit_Click"
                runat="server" Text="<%# EditButtonTitle %>" CausesValidation="false" />
        </div>
        <EPiServer:Property runat="server" PropertyName="PageName" CustomTagName="h1" />
        <EPiServer:Property runat="server" PropertyName="<%# PropertyName %>" />
    </asp:View>
    <asp:View runat="server" ID="ViewEdit">
        <h1>
            <%= Translate("/workroom/edit") %>
            <EPiServer:Property runat="server" PropertyName="PageName" />
        </h1>
        <div class="editContentForm">
            <div class="richTextEditor">
                <AlloyTech:RichTextEditor ID="TextEditor" runat="server" Advanced="true" />
            </div>
            <div class="buttons">
                <asp:Button ID="ButtonSave" OnClick="ButtonSave_Click" runat="server" Text="<%# SaveButtonTitle %>"
                    CssClass="buttonExt" />
                <asp:Button ID="ButtonCancel" OnClick="ButtonCancel_Click" runat="server" Text="<%# CancelButtonTitle %>"
                    CssClass="button" CausesValidation="false" />
            </div>
        </div>
    </asp:View>
</asp:MultiView>

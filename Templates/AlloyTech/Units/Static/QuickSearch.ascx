<%@ Control EnableViewState="true" Language="C#" AutoEventWireup="False" CodeBehind="QuickSearch.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Static.QuickSearch" %>

<asp:Panel runat="server" CssClass="QuickSearchArea" DefaultButton="SearchButton">
    <div class="quickSearchFieldContainer">
        <asp:Label ID="Label1" runat="server" CssClass="hidden" AssociatedControlID="SearchText" Text="<%$ Resources: EPiServer, search.searchstring %>" />
        <asp:TextBox ID="SearchText" onfocus="this.value='';" runat="server" CssClass="quickSearchField" Text="<%$ Resources: EPiServer, search.searchstring %>" />
    </div>
    <asp:ImageButton ID="SearchButton" runat="server" ImageUrl="~/Templates/AlloyTech/Styles/Default/Images/searchbutton.png" ToolTip="<%$ Resources: EPiServer, navigation.search %>" AlternateText="<%$ Resources: EPiServer, navigation.search %>" CausesValidation="false" CssClass="quickSearchButton" OnClick="Search_Click" />
</asp:Panel>
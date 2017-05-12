<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DateTimePicker.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.DateTimePicker" %>

<asp:TextBox runat="server" ID="DateTextBox" ToolTip="<%$ Resources: EPiServer, workroom.calendar.dateformat %>"></asp:TextBox>
<asp:DropDownList runat="server" ID="SelectHours"></asp:DropDownList>
<asp:DropDownList runat="server" ID="SelectMinutes"></asp:DropDownList>

<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/jquery/jquery-ui-1.8.7.custom.min.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/jquery/jquery.ui.datepicker.custom.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/jquery/ui.datepicker-sv.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/DateTimePicker.js") %>"></script>

<script type="text/javascript">
    //<![CDATA[

    $(document).ready(function() {
        setDateTimePickerOptions('#<%=DateTextBox.ClientID %>', '<%= Language %>');
    });
    //]]>
</script>
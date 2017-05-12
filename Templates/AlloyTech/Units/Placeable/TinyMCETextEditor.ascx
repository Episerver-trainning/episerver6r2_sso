<%@ Control Language='C#' AutoEventWireup='true' CodeBehind='TinyMCETextEditor.ascx.cs' Inherits='EPiServer.Templates.AlloyTech.Units.Placeable.TinyMCETextEditor' %>
<%@ Import Namespace="EPiServer.Globalization" %>

<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/tiny_mce/jquery.tinymce.js") %>"></script>

<script type='text/javascript'>
    $(document).ready(function() {
        $('#<%= TextBoxContent.ClientID %>').tinymce({
            // Location of TinyMCE script
            script_url: '<%= Page.ResolveUrl("~/Templates/AlloyTech/scripts/tiny_mce/tiny_mce.js") %>',

            // General options
            theme: 'advanced',
            skin: 'o2k7',
            skin_variant: 'silver',
            plugins: 'safari,table',
            language: '<%= ContentLanguage.PreferredCulture.IetfLanguageTag %>',

            // Theme options
            theme_advanced_buttons1: '<%= ToolBar %>',
            theme_advanced_buttons2: '',
            theme_advanced_buttons3: '',
            theme_advanced_buttons4: '',
            theme_advanced_toolbar_location: 'top',
            theme_advanced_toolbar_align: 'left',
            theme_advanced_path : false,
            theme_advanced_resizing: true,
            theme_advanced_resize_horizontal: false,

            width: '<%= Width %>',
            height: '<%= Height %>',

            // Example content CSS (should be your site CSS)
            content_css: '<%= Page.ResolveUrl(ContentCss) %>'
        });
    });
</script>

<asp:TextBox ID='TextBoxContent' runat='server' TextMode='MultiLine' />

<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Flash.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.Units.Placeable.Flash" %>

<div id='flashcontent'>
    <img src='<%= FallbackImageUrl %>' alt='<%= FallbackImageAlt %>' />
</div>

<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Templates/AlloyTech/scripts/swfobject.js") %>"></script>

<script type='text/javascript'>
    function addFlash() {
        if (!eval('<%= FlashAvailable.ToString().ToLowerInvariant() %>')) {
            var url = '<%= FallbackImageUrl %>';
            if (!url) {
                $("#flashcontent").remove();
            }
        
            return;
        }

        var link = '<%= Page.ResolveUrl("~/Templates/AlloyTech/scripts/slideshow.swf") %>';
        var width = '<%= Width %>';
        var height = '<%= Height %>';

        var variables = { xml: '<%= ConfigXml %>' };
        var params = { wmode: 'transparent' };
        var attributes = {};

        swfobject.embedSWF(link, 'flashcontent', width, height, '10', false, variables, params, attributes);
    }

    $(document).ready(addFlash);
</script>


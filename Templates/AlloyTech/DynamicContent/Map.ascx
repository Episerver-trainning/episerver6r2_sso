<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Map.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.DynamicContent.Map" %>
<%@ Import Namespace="System.Globalization" %>

<asp:Panel ID="StaticMapPanel" runat="server" Visible="false" >
    <script type="text/javascript">
        //fix right column appearance if content is wider than container
        $(document).ready(function() {
            $('#SecondaryBody > div').css('overflow', 'hidden').css('width', '173px');
        });
    </script>
    <asp:HyperLink ID="GoogleMapLink" runat="server" Target="_blank">
        <asp:Image ID="StaticMapImage" runat="server" />
    </asp:HyperLink>
</asp:Panel>
<asp:Panel ID="InteractiveMapPanel" runat="server" Visible="true">
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>
    <script type="text/javascript">
        // Initialize the map
        $(document).ready(function() {
            var mapContent = new EPi.AlloyTech.MapContent("<%= MapPanel.ClientID%>", true);
            if (!mapContent || !mapContent.isInitialized()) {
                return;
            }
            mapContent.setType("<%= Value.MapType %>");
            mapContent.selectByCoordinates("<%= Value.Latitude.ToString(CultureInfo.InvariantCulture) %>", "<%= Value.Longitude.ToString(CultureInfo.InvariantCulture) %>", "<%= Value.Address %>");
            mapContent.showPoint("<%= Value.Latitude.ToString(CultureInfo.InvariantCulture) %>", "<%= Value.Longitude.ToString(CultureInfo.InvariantCulture) %>", Number("<%= Value.Zoom %>"));
        });
    </script>
    <asp:Panel ID="MapPanel" runat="server" CssClass="MapContainer" />
</asp:Panel>
<asp:Panel ID="MissingMapDataPanel" runat="server" Visible="false">
    <asp:Literal runat="server" Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.missingmapdata %>" ></asp:Literal>
</asp:Panel>

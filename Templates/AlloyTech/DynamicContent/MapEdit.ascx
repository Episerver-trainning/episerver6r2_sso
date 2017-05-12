<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MapEdit.ascx.cs" Inherits="EPiServer.Templates.AlloyTech.DynamicContent.MapEdit" %>

<script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>

<script type="text/javascript">

    var searchAddressTextBox = "#<%= AddressTextBox.ClientID %>";
    var searchResultsPanel = "#<%= SearchResultsPanel.ClientID %>";
    var searchResultsList = "#<%= SearchResultsList.ClientID %>";
    var mapPanelID = "<%= MapPanel.ClientID %>";
    var heightTextBox = "#<%= HeightTextBox.ClientID %>";
    var widthTextBox = "#<%= WidthTextBox.ClientID %>";
    var zoomField = "#<%= ZoomField.ClientID %>";
    var latitudeField = "#<%= LatitudeField.ClientID %>";
    var longitudeField = "#<%= LongitudeField.ClientID %>";
    var mapTypeField = "#<%= MapTypeField.ClientID %>";
    var staticMapCheckBox = "#<%= DisplayAsStaticMapCheckBox.ClientID %>";
    var altTextBox = "#<%= AltTextBox.ClientID %>";
    var altTextPanel = "#<%= AltTextPanel.ClientID %>";
    var mapWrapperPanel = "#<%= MapWrapperPanel.ClientID %>";

    var minHeightWidthValue = 100;
    var maxHeightWidthValue = 2000;

    var wrapperSizeDiff = 2;

    var emptySearchResultsMessage = '<%= TranslateForScript("/templates/dynamiccontent/map/emptysearchresults") %>';
    var unableToInitializeMessage = '<%= TranslateForScript("/templates/dynamiccontent/map/unabletoinitializemessage") %>';


    var optionMarkerIconPath = "<%= Page.ResolveClientUrl("~/Templates/AlloyTech/Styles/Default/Images/gmaps-marker-orange.png") %>";
    var mapContent = null;

    function initialize() {
        $(searchResultsPanel).css("display", "none");
        mapContent = new EPi.AlloyTech.MapContent(mapPanelID, false);
        if (!mapContent || !mapContent.isInitialized()) {
            $("#" + mapPanelID).text(unableToInitializeMessage);
            return;
        }
        mapContent.optionMarkerIconPath = optionMarkerIconPath;
        mapContent.setType($(mapTypeField).val());
        var zoomLevel = Number($(zoomField).val());
        if (isNaN(zoomLevel)) {
            zoomLevel = null;
        }
        var latitude = Number($(latitudeField).val());
        var longitude = Number($(longitudeField).val());
        var address = $(searchAddressTextBox).val();
        if (!isNaN(latitude) && !isNaN(longitude) && !(latitude == 0 && longitude == 0)) {
            mapContent.selectByCoordinates(latitude, longitude, address);
            mapContent.showPoint(latitude, longitude, zoomLevel);
        }

        mapContent.selectionChangedHandler = SelectionChanged;
        mapContent.mapTypeChangedHandler = MapTypeChanged;
        mapContent.zoomChangedHandler = ZoomChaged;

        var mapHeight = Number($(heightTextBox).val());
        var mapWidth = Number($(widthTextBox).val());

        mapHeight = (mapHeight >= minHeightWidthValue && mapHeight <= maxHeightWidthValue) ? mapHeight : 350;
        mapWidth = (mapWidth >= minHeightWidthValue && mapWidth <= maxHeightWidthValue) ? mapWidth : 350;

        $("#" + mapPanelID).height(mapHeight);
        $("#" + mapPanelID).width(mapWidth);
        $(mapWrapperPanel).height(mapHeight + wrapperSizeDiff);
        $(mapWrapperPanel).width(mapWidth + wrapperSizeDiff);
        $(mapWrapperPanel).resizable({
            alsoResize: "#" + mapPanelID,
            minHeight: minHeightWidthValue + wrapperSizeDiff,
            minWidth: minHeightWidthValue + wrapperSizeDiff,
            maxHeight: maxHeightWidthValue + wrapperSizeDiff,
            maxWidth: maxHeightWidthValue + wrapperSizeDiff
        });
        $(mapWrapperPanel).resize(MapSizeChanged);
        $(mapWrapperPanel).resize();


        $(heightTextBox).change(MapSizeChangedInTexbox);
        $(widthTextBox).change(MapSizeChangedInTexbox);

        $(staticMapCheckBox).click(StaticMapOptionChanged);
        StaticMapOptionChanged();
    }

    function ResetMap() {
        $(searchResultsList).html("");
        $(searchResultsPanel).css("display", "none");
        mapContent.reset();
        var latitude = Number($(latitudeField).val());
        var longitude = Number($(longitudeField).val());
        if (!isNaN(latitude) && !isNaN(longitude) && !(latitude == 0 && longitude == 0)) {
            mapContent.getAddressSelected(GetAddressHandler);
        }
        else {
            $(latitudeField).val("0");
            $(longitudeField).val("0");
            $(searchAddressTextBox).val("");
        }
    }

    function SearchAddress() {
        var addressToSearch = $(searchAddressTextBox).val();
        $(searchResultsPanel).css("display", "");
        mapContent.searchAddress(addressToSearch, function(response) {
            if (response == null) {
                var message = $("<span>").text(emptySearchResultsMessage + ": " + addressToSearch);
                $(searchResultsList).html("").append(message);
            }
            else {
                var resultsList = $("<ul>");
                $(searchResultsList).html("").append(resultsList);
                for (var i = 0; i < response.length && i < 10; i++) {
                    var address = response[i].address;
                    var showCallback = response[i].showCallback;
                    var marker = response[i].marker;
                    var result = $("<li>").css("margin-bottom", "3px").appendTo(resultsList);
                    $("<a>").attr("href", "#").css("border-bottom", "1px dotted").appendTo(result).bind('click', { callback: showCallback }, function(event) { event.data.callback.call(); return false; }).text(address);
                }
                $(searchResultsList).find("a:first").click();

            }
        });
    }

    function SelectionChanged(latitude, longitude) {
        if ($(latitudeField).val() == latitude && $(longitudeField).val() == longitude) {
            return;
        }
        $(latitudeField).val(latitude);
        $(longitudeField).val(longitude);

        $(searchResultsPanel).css("display", "none");
        $(searchResultsList).html("");
        mapContent.getAddressSelected(GetAddressHandler);
    }

    function MapTypeChanged(mapTypeValue) {
        $(mapTypeField).val(mapTypeValue);
    }

    function ZoomChaged(newZoomLevel) {
        $(zoomField).val(newZoomLevel);
    }

    function MapSizeChanged() {
        $(widthTextBox).val($("#" + mapPanelID).width());
        $(heightTextBox).val($("#" + mapPanelID).height());
    }

    function MapSizeChangedInTexbox() {
        var height = Number($(heightTextBox).val());
        var width = Number($(widthTextBox).val());
        if (height != $("#" + mapPanelID).height() || width != $("#" + mapPanelID).width()) {
            $(mapWrapperPanel).resizable("disable");
            $(mapWrapperPanel).width(width + wrapperSizeDiff);
            $(mapWrapperPanel).height(height + wrapperSizeDiff);
            $("#" + mapPanelID).height(height);
            $("#" + mapPanelID).width(width);
            $(mapWrapperPanel).resizable("enable");
        }
    }

    function GetAddressHandler(address) {
        $(searchAddressTextBox).val(address);
    }

    function StaticMapOptionChanged() {
        if ($(staticMapCheckBox + ":checked").val()) {
            $(altTextPanel).css("display", "");
        }
        else {
            $(altTextBox).val("");
            $(altTextPanel).css("display", "none");
        }
    }

    function ValidateClientCoordinates(source, args) {
        var latitude = Number($(latitudeField).val());
        var longitude = Number($(longitudeField).val());
        if (!isNaN(latitude) && !isNaN(longitude) && !(latitude == 0 && longitude == 0)) {
            args.IsValid = true;
        }
        else {
            args.IsValid = false;
        }        
    }    

    $(initialize);

</script>

<asp:Panel ID="Data" runat="server">
    <asp:Panel runat="server" CssClass="epirowcontainer">
        <asp:Label CssClass="episize100 epiindent" runat="server" AssociatedControlID="AddressTextBox"
            Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.addresslabel %>" />
        <asp:TextBox ID="AddressTextBox" runat="server" />
        <asp:Button Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.searchbuttonlabel %>"
            runat="server" ID="SearchButton" OnClientClick="SearchAddress(); return false;" />
    </asp:Panel>
    <asp:Panel runat="server" CssClass="epirowcontainer">
        <asp:Label CssClass="episize100 epiindent" runat="server" AssociatedControlID="HeightTextBox"
            Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.heightlabel %>" />
        <asp:TextBox ID="HeightTextBox" runat="server" Text="350" Width="30" />
        <asp:RequiredFieldValidator ID="HeightValidator" ControlToValidate="HeightTextBox"
            EnableClientScript="true" Text="*" runat="server" />
        <asp:RangeValidator ID="HeightRangeValidator" ControlToValidate="HeightTextBox" Type="Integer"
            EnableClientScript="true" Text="*" ErrorMessage="<%$ Resources: EPiServer, templates.dynamiccontent.map.heighterrormessage %>" 
            MinimumValue="100" MaximumValue="2000" runat="server" />
        &nbsp;&nbsp;&nbsp;
        <asp:Label CssClass="episize100 epiindent" runat="server" AssociatedControlID="WidthTextBox"
            Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.widthlabel %>" />
        <asp:TextBox ID="WidthTextBox" runat="server" Text="350" Width="30" />
        <asp:RequiredFieldValidator ID="WidthValidator" ControlToValidate="WidthTextBox"
            EnableClientScript="true" Text="*" runat="server" />
        <asp:RangeValidator ID="WidthRangeValidator" ControlToValidate="WidthTextBox" Type="Integer"
            EnableClientScript="true" Text="*" ErrorMessage="<%$ Resources: EPiServer, templates.dynamiccontent.map.widtherrormessage %>"
            MinimumValue="100" MaximumValue="2000" runat="server" />
    </asp:Panel>
    <asp:Panel runat="server" CssClass="epirowcontainer">
        <span class="episize100 epiindent"></span>
        <asp:CheckBox ID="DisplayAsStaticMapCheckBox" runat="server" Checked="false" Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.displaystaticimagelabel %>" />
    </asp:Panel>
    <asp:Panel ID="AltTextPanel" runat="server" CssClass="epirowcontainer">
        <asp:Label ID="Label1" CssClass="episize100 epiindent" runat="server" AssociatedControlID="AltTextBox"
            Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.altlabel %>" />
        <asp:TextBox ID="AltTextBox" runat="server" />
    </asp:Panel>
    <asp:HiddenField ID="ZoomField" Value="13" runat="server" />
    <asp:HiddenField ID="LatitudeField" runat="server" />
    <asp:HiddenField ID="LongitudeField" runat="server" />
    <asp:HiddenField ID="MapTypeField" Value="roadmap" runat="server" />
</asp:Panel>
<asp:Panel runat="server" ID="SearchResultsPanel">
    <fieldset id="SearchPanel" runat="server">
        <legend style="padding-bottom: 1px;">
            <asp:Literal Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.searchresultslabel %>"
                runat="server" />
            - <a id="HideSearchResultsButton" runat="server" href="#" onclick="ResetMap(); return false;"
                style="border-bottom: 1px dotted; background-color: #ffffdd;">
                <asp:Literal Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.hidesearchresults %>"
                    runat="server" /></a> </legend>
        <asp:Panel runat="server" ID="SearchResultsList" CssClass="epirowcontainer">
        </asp:Panel>
    </fieldset>
</asp:Panel>
<asp:CustomValidator ID="LatLngValidator" runat="server" OnServerValidate="ValidateCoordinates" 
        ClientValidationFunction="ValidateClientCoordinates" 
        Text="<%$ Resources: EPiServer, templates.dynamiccontent.map.selectiontextmessage %>" 
        ErrorMessage="<%$ Resources: EPiServer, templates.dynamiccontent.map.selectionerrormessage %>"/>
<asp:Panel ID="MapWrapperPanel" runat="server" BorderColor="Gray" BorderStyle="Dashed" BorderWidth="2">
    <asp:Panel ID="MapPanel" runat="server">
    </asp:Panel>
</asp:Panel>
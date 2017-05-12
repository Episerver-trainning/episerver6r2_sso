if (typeof window.EPi === "undefined") {
    window.EPi = {};
}

if (typeof EPi.AlloyTech === "undefined") {
    EPi.AlloyTech = {};
}

EPi.AlloyTech.MapContent = function (elementID, readonly) {
    /// <summary>Class encapsulating a google maps map</summary>
    /// <param name="elementID" type="String">Id of the element encapsulating the map</param>
    /// <param name="readonly" type="Boolean">Whether the user should map should be </param>
    var mapElementID = elementID;
    var defaultZoomLevel = 13;
    var defaultLatitude = 59.3354425246738;
    var defaultLongitude = 18.0674189329147;
    var defaultType = "roadmap";
    var readonlyMode = readonly;
    var selectedMarker = null;
    var initialized = false;

    var infoWindow = null;

    var optionMarkers = new Array();

    var self = this;

    var map = null;

    this.optionMarkerIconPath = "http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-dot.png";
    this.selectionChangedHandler = null;
    this.mapTypeChangedHandler = null;
    this.zoomChangedHandler = null;


    try {

        var mapOptions = {
            zoom: defaultZoomLevel,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            center: new google.maps.LatLng(defaultLatitude, defaultLongitude),
            disableDoubleClickZoom: !readonlyMode,
            mapTypeControlOptions: {
                position: google.maps.ControlPosition.TOP_RIGHT,
                style: google.maps.MapTypeControlStyle.DEFAULT,
                mapTypeIds: [google.maps.MapTypeId.ROADMAP, google.maps.MapTypeId.SATELLITE, google.maps.MapTypeId.HYBRID]
            }
        };

        map = new google.maps.Map(document.getElementById(mapElementID), mapOptions);

        if (!readonlyMode) {
            var dblclickCallback = function (event) { self.selectPoint(event.latLng); };
            var zoomChangedCallback = function (oldZoom, newZoom) { self.onZoomChanged(newZoom); };
            var mapTypeChangedCallback = function () { self.onMapTypeChanged(); };

            google.maps.event.addListener(map, "dblclick", dblclickCallback);
            google.maps.event.addListener(map, "zoom_changed", zoomChangedCallback);
            google.maps.event.addListener(map, "maptypeid_changed", mapTypeChangedCallback);
        }

        initialized = true;

    }
    catch (e) {
        initialized = false;
    }

    this.isInitialized = function () {
        /// <summary>Returns a bool indicating if this map has been initialized.</summary>
        /// <returns type="Boolean" />
        return initialized;
    }

    this.setType = function (mapType) {
        /// <summary>Set the map type of map to show in the wrapped google maps object</summary>
        /// <param name="mapType" type="google.maps.mapTypeId">Identifier for the type of map to use</param>
        map.setMapTypeId(mapType);
    }

    this.getType = function () {
        /// <summary>Returns the map type on the wrapped google maps object</summary>
        /// <returns type="google.maps.MapTypeId" />
        return map.getMapTypeId();
    }

    this.showPoint = function (latitude, longitude, zoomLevel) {
        /// <summary>Returns the map type on the wrapped google maps object</summary>
        /// <param name="latitude" type="Double">Latitude for the point to show</param>
        /// <param name="longitude" type="Double">Longitude for the point to show</param>
        /// <param name="zoomLevel" type="Integer">Zoom level of the map</param>
        map.setCenter(new google.maps.LatLng(latitude, longitude));
        if (zoomLevel) {
            map.setZoom(zoomLevel);
        }
    }

    this.onZoomChanged = function () {
        /// <summary>Called when the zoom level of the map changes</summary>
        if (this.zoomChangedHandler != null) {
            var zoom = map.getZoom();
            this.zoomChangedHandler(zoom);
        }
    }

    this.onMapTypeChanged = function () {
        /// <summary>Called when the map type changes</summary>
        if (this.mapTypeChangedHandler != null) {
            var type = this.getType();
            this.mapTypeChangedHandler(type);
        }
    }

    this.onSelectionChanged = function (latlng) {
        /// <summary>Called when the current selection changes</summary>
        /// <param name="latLng" type="google.maps.LatLng">The combined latitude and longitude of the point</param>
        if (this.selectionChangedHandler != null) {
            var latitude = latlng.lat();
            var longitude = latlng.lng();
            this.selectionChangedHandler(latitude, longitude);
        }
    }

    this.selectPoint = function (latlng, description) {
        /// <summary>Select a point on the map</summary>
        /// <param name="latLng" type="google.maps.LatLng">The combined latitude and longitude of the point</param>
        this.clear(true);
        this.createSelectedMarker(latlng.lat(), latlng.lng(), description);
        this.onSelectionChanged(latlng);
    }

    this.selectByCoordinates = function (latitude, longitude, description) {
        /// <summary>Select a point on the map by its coordinates</summary>
        /// <param name="latitude" type="Double">Latitude of the new selection</param>
        /// <param name="longitude" type="Double">Longitude of the new selection</param>
        /// <param name="description" type="String">Description of the selection</param>
        this.selectPoint(new google.maps.LatLng(latitude, longitude), description);
    }

    this.createSelectedMarker = function (latitude, longitude, description) {
        /// <summary>Create a new map marker and show it on the map</summary>
        /// <param name="latitude" type="Double">Latitude of the marker</param>
        /// <param name="longitude" type="Double">Longitude of the marker</param>
        /// <param name="description" type="String">Set as title for the map marker if supplied</param>
        var markerOptions = {
            draggable: !readonlyMode,
            position: new google.maps.LatLng(latitude, longitude),
            map: map
        };
        selectedMarker = new google.maps.Marker(markerOptions);
        if (description) {
            selectedMarker.setTitle(description);
        }
        if (!readonlyMode) {
            var callback = function () { self.selectPoint(selectedMarker.getPosition()); };
            google.maps.event.addListener(selectedMarker, "dragend", callback);
        }
    }

    this.createOptionMarker = function (latitude, longitude) {
        /// <summary>Create a google maps map marker on a specific map location</summary>
        /// <param name="latitude" type="Double">Latitude of the marker</param>
        /// <param name="longitude" type="Double">Longitude of the marker</param>
        /// <returns type="google.maps.Marker" />
        var position = new google.maps.LatLng(latitude, longitude);
        var markerOptions = {
            position: position,
            map: map,
            icon: this.optionMarkerIconPath
        };
        var marker = new google.maps.Marker(markerOptions);
        google.maps.event.addListener(marker, "dblclick", function (event) {
            self.selectPoint(position);
        });
        optionMarkers.push(marker);
        return marker;
    }

    this.showOptionMarker = function (marker, description) {
        /// <summary>Show a supplied marker on the map</summary>
        /// <param name="marker" type="google.maps.Marker">The Google maps marker to show</param>
        /// <param name="description" type="String">Balloon tip description for the map marker</param>
        var point = marker.getPosition();
        marker.setTitle(description);
        if (infoWindow) {
            infoWindow.close();
        }
        else {
            infoWindow = new google.maps.InfoWindow();
        }
        infoWindow.setContent(description);
        infoWindow.open(map, marker);
        self.showPoint(point.lat(), point.lng());
    }

    this.getAddressByCoordinates = function (latitude, longitude, handler) {
        /// <summary>Get the street address corresponding to the supplied coordinates</summary>
        /// <param name="latitude" type="Double">Latitude of the point to look up</param>
        /// <param name="longitude" type="Double">Longitude of the point to look up</param>
        /// <param name="handler" type="Function">Callback executed when the address lookup returns</param>
        var geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(latitude, longitude);
        var request = {
            latLng: latlng
        };
        geocoder.geocode({ latLng: latlng },
            function (response, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    handler(response[0].formatted_address);
                }
                else {
                    handler(null);
                }
            }
        );
    }

    this.getAddressSelected = function (handler) {
        /// <summary>Gets the address for the currently selected map position</summary>
        /// <param name="handler" type="Function">Callback function executed when the address lookup is complete</param>
        if (selectedMarker == null) {
            handler(null);
        }
        var latlng = selectedMarker.getPosition();
        this.getAddressByCoordinates(latlng.lat(), latlng.lng(), handler);
    }

    this.searchAddress = function (address, handler) {
        /// <summary>Uses the google maps API to search for a supplied address</summary>
        /// <param name="address" type="String">The address to search for</param>
        /// <param name="handler" type="Function">A function called when the asynchronous search completes</param>
        console.log("Search address");
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ address: address },
            function (response, status) {
                self.clear();
                if (status != google.maps.GeocoderStatus.OK) {
                    handler(null);
                }
                else {
                    var results = new Array();
                    for (var i = 0; i < response.length; i++) {
                        var point = response[i].geometry.location;
                        var marker = self.createOptionMarker(point.lat(), point.lng());
                        var address = response[i].formatted_address;
                        // Create a closure encapsulating position information for showing the matching position 
                        var showMarkerCallback = function (marker, address) {
                            return function () { self.showOptionMarker(marker, address); }
                        } (marker, address);
                        var result = { address: address, showCallback: showMarkerCallback };
                        results[i] = result;
                    }
                    handler(results);
                }
            }
        );

    }
    this.reset = function () {
        /// <summary>Resets the map to the initial state</summary>
        this.clear();
        if (selectedMarker) {
            map.setCenter(selectedMarker.getPosition());
        }
    }

    this.clear = function (removeSelectedMarker) {
        /// <summary>Clear the all markers from the map and hide any open info window</summary>
        /// <param name="removeSelectedMarker" type="Boolean">A value indicating whether the current slection should be removed as well</param>
        for (var i = 0; i < optionMarkers.length; i++) {
            optionMarkers[i].setMap(null);
        }
        optionMarkers = new Array();
        if (removeSelectedMarker && selectedMarker) {
            selectedMarker.setMap(null);
            selectedMarker = null;
        }
        if (infoWindow) {
            infoWindow.close();
        }
    }

}
#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace EPiServer.Templates.AlloyTech.DynamicContent
{
    /// <summary>
    /// View mode control for map dynamic content
    /// </summary>
    public partial class Map : UserControlBase
    {
        /// <summary>
        /// Map content information data. Set by <see cref="MapDynamicContent"/> when this user control is instantiated.
        /// </summary>
        public MapContentValue Value
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Value != null)
            {
                StaticMapPanel.Visible = Value.DisplayAsStaticMap;
                InteractiveMapPanel.Visible = !Value.DisplayAsStaticMap;
                if (Value.DisplayAsStaticMap)
                {
                    GoogleMapLink.NavigateUrl = GetGoogleMapUrl(Value, CurrentPage.LanguageBranch);
                    StaticMapImage.AlternateText = Value.AltText;
                    StaticMapImage.ImageUrl = GetStaticMapImageUrl(Value, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptInclude("MapContent.js", Page.ResolveClientUrl("~/Templates/AlloyTech/Scripts/MapContent.js"));
                    MapPanel.Width = Value.Width;
                    MapPanel.Height = Value.Height;
                }
            }
            else 
            {
                // No map data available. Show an error message instead
                StaticMapPanel.Visible = InteractiveMapPanel.Visible = false;
                MissingMapDataPanel.Visible = true;
            }
        }

        /// <summary>
        /// Gets the url for a static google maps image based on the supplied map information.
        /// </summary>
        /// <param name="mapContent">Map content data.</param>
        /// <param name="languageCode">The language in which to view the map.</param>
        private static string GetStaticMapImageUrl(MapContentValue mapContent, string languageCode)
        {
            UrlBuilder url = new UrlBuilder("http://maps.google.com/maps/api/staticmap");
            url.QueryCollection["center"] =
                url.QueryCollection["markers"] = mapContent.Latitude + "," + mapContent.Longitude;

            url.QueryCollection["zoom"] = mapContent.Zoom.ToString();
            url.QueryCollection["size"] = mapContent.Width + "x" + mapContent.Height;
            url.QueryCollection["sensor"] = "false";
            url.QueryCollection["maptype"] = mapContent.MapType;
            url.QueryCollection["language"] = languageCode;
            url.QueryCollection["key"] = MapDynamicContentConfiguration.GoogleMapsKey;

            return (string)url;
        }

        /// <summary>
        /// Gets the URL for viewing the google map described by the supplied data.
        /// </summary>
        /// <param name="mapContent">Map content data.</param>
        /// <param name="languageCode">The language in which to view the map.</param>
        private static string GetGoogleMapUrl(MapContentValue mapContent, string languageCode)
        {
            UrlBuilder url = new UrlBuilder("http://maps.google.com/maps");
            
            url.QueryCollection["f"] = "d";
            url.QueryCollection["source"] = "s_q";
            url.QueryCollection["hl"] =  languageCode;
            url.QueryCollection["hnear"] = HttpUtility.UrlEncode(mapContent.Address);
            url.QueryCollection["sll"] = mapContent.Latitude + "," + mapContent.Longitude;
            url.QueryCollection["ie"] = "UTF8";
            url.QueryCollection["cd"] = "1";
            url.QueryCollection["split"] = "0";
            url.QueryCollection["z"] = mapContent.Zoom.ToString();

            return (string)url;
        }
    }
}
#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Templates.AlloyTech.DynamicContent
{
    /// <summary>
    /// Model data container for Google maps dynamic content implementation
    /// </summary>
    [Serializable]
    public class MapContentValue
    {
        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the alt text used when showing a static map image.
        /// </summary>
        public string AltText
        {
            get;
            set;
        }

        /// <summary>
        /// Height of the map view port on the page in pixels.
        /// </summary>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Width of the map view port on the page in pixels.
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Initial zoom level of the map.
        /// </summary>
        public int Zoom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the latitude of selected point.
        /// </summary>
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the longitude of selected point.
        /// </summary>
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the map.
        /// </summary>
        public string MapType
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the map should be displayed as a static image or as an interactive map.
        /// </summary>
        /// <value><c>true</c> if the map should be displayed as a static image; otherwise, <c>false</c>.</value>
        public bool DisplayAsStaticMap
        {
            get;
            set;
        }
    }
}

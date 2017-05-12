#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.Configuration;
using EPiServer.Configuration;

namespace EPiServer.Templates.AlloyTech.DynamicContent
{
    /// <summary>
    /// Configuration settings for google maps dynamic content implementation
    /// </summary>
    public static class MapDynamicContentConfiguration
    {
        /// <summary>
        /// Gets the google maps key from web.config.
        /// </summary>
        public static string GoogleMapsKey
        {
            get
            {
                string siteId = Settings.Instance.Parent.SiteId;
                return WebConfigurationManager.AppSettings[String.Format("GoogleMapsKey_{0}", siteId)];
            }
        }

    }
}

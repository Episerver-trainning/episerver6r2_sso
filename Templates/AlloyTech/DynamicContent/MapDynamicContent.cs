#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using EPiServer.Core;
using EPiServer.DynamicContent;
using EPiServer.PlugIn;
using log4net;

namespace EPiServer.Templates.AlloyTech.DynamicContent
{
    /// <summary>
    /// A dynamic content implementation showing a map from google maps on a page.
    /// </summary>
    [GuiPlugIn(Url = "~/Templates/AlloyTech/DynamicContent/MapEdit.ascx", Area = PlugInArea.DynamicContent)]
    public class MapDynamicContent : IDynamicContent
    {
        private static ILog _log = LogManager.GetLogger(typeof(MapDynamicContent));

        #region IDynamicContent Members

        /// <summary>
        /// Return the dynamic content as a string.
        /// This method will only be called by EPiServer if the <see cref="RendersWithControl"/> property returns false
        /// </summary>
        /// <param name="hostPage">A reference to the EPiServer page hosting the dynamic content. This can be null as Render can be called when there is no page in context</param>
        /// <returns>Always <code>null</code> since we're rendering with a control</returns>
        public string Render(PageBase hostPage)
        {
            return null;
        }

        /// <summary>
        /// Return the control for rendering the map content on the page.
        /// This method will only be called by EPiServer if the <see cref="RendersWithControl"/> property returns true
        /// </summary>
        /// <param name="hostPage">A reference to the EPiServer page hosting the dynamic content.</param>
        /// <returns>An instance of the <see cref="Map"/> user control</returns>
        public Control GetControl(PageBase hostPage)
        {
            if (hostPage == null)
            {
                return null;
            }
            // Load the control and set the persisted map information
            Map mapControl = (Map)hostPage.LoadControl("~/Templates/AlloyTech/DynamicContent/Map.ascx");
            mapControl.Value = Value;
            return mapControl;
        }

        /// <summary>
        /// This property is used by the EPiServer default Dynamic Content editor to display the properties
        /// that require input for your Dynamic Content object.
        /// Since we're implementing a custom editor deriving from <see cref="EPiServer.DynamicContent.DynamicContentEditControl"/>
        /// we can return <c>null</c>.
        /// </summary>
        /// <value>This Always returns <c>null</c></value>
        public PropertyDataCollection Properties
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a value indicating whether content is renders with a control.
        /// </summary>
        /// <value><c>true</c> if rendered with a control; otherwise, <c>false</c>.</value>
        public bool RendersWithControl
        {
            get { return true; }
        }

        /// <summary>
        /// Get and sets the state string containing map settings.
        /// The class should use this property value to serialize and deserialize its internal state.
        /// This can be null or an empty string if your class does not have any properties that affect it's output.
        /// </summary>
        public string State
        {
            get
            {
                if (Value == null)
                {
                    return null;
                }
                // If we have map settings in the Value property we serialize an persist them using the state property
                // JSON yields a rather nice string representation of an object
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return Convert.ToBase64String(Encoding.Unicode.GetBytes(serializer.Serialize(Value)));
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    Value = null;
                    return;
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                try
                {
                    // Try to de-serialize the stored state when we get it from the dynamic content framework
                    Value = serializer.Deserialize<MapContentValue>(Encoding.Unicode.GetString(Convert.FromBase64String(value)));
                }
                catch (Exception e)
                { 
                    _log.Error("Failed deserializing dynamic content state", e);
                    Value = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a <see cref="MapContentValue"/> object containing the settings for the map shown.
        /// </summary>
        public MapContentValue Value
        {
            get;
            set;
        }

    }
}

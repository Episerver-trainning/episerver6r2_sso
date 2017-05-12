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
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.DynamicContent;
using System.Globalization;

namespace EPiServer.Templates.AlloyTech.DynamicContent
{
    /// <summary>
    /// Edit mode control for map dynamic content
    /// </summary>
    public partial class MapEdit : DynamicContentEditControl
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.ClientScript.RegisterClientScriptInclude("MapContent.js", Page.ResolveClientUrl("~/Templates/AlloyTech/Scripts/MapContent.js"));
            Page.ClientScript.RegisterClientScriptInclude("jquery-ui-1.8.7.custom.min.js", Page.ResolveClientUrl("~/Templates/AlloyTech/Scripts/jquery/jquery-ui-1.8.7.custom.min.js"));
            MapDynamicContent content = Content as MapDynamicContent;
            if (content.Value != null)
            {
                AddressTextBox.Text = content.Value.Address;
                AltTextBox.Text = content.Value.AltText;
                HeightTextBox.Text = content.Value.Height.ToString();
                WidthTextBox.Text = content.Value.Width.ToString();
                LatitudeField.Value = content.Value.Latitude.ToString(CultureInfo.InvariantCulture);
                LongitudeField.Value = content.Value.Longitude.ToString(CultureInfo.InvariantCulture);
                ZoomField.Value = content.Value.Zoom.ToString();
                MapTypeField.Value = content.Value.MapType;
                DisplayAsStaticMapCheckBox.Checked = content.Value.DisplayAsStaticMap;                
            }
        }

        /// <summary>
        /// Prepares dynamic content data for save.
        /// </summary>
        public override void PrepareForSave()
        {
            MapContentValue data = new MapContentValue
            {
                Address = AddressTextBox.Text.Trim(),
                AltText = AltTextBox.Text.Trim()
            };
            int boxValue;
            double latlngValue;
            data.Height = Int32.TryParse(HeightTextBox.Text.Trim(), out boxValue) ? boxValue : 300;
            data.Width = Int32.TryParse(WidthTextBox.Text.Trim(), out boxValue) ? boxValue : 300;
            data.Latitude = double.TryParse(LatitudeField.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out latlngValue) ? latlngValue : 0;
            data.Longitude = double.TryParse(LongitudeField.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out latlngValue) ? latlngValue : 0;
            data.Zoom = Int32.TryParse(ZoomField.Value.Trim(), out boxValue) ? boxValue : 13;
            data.MapType = MapTypeField.Value;
            data.DisplayAsStaticMap = DisplayAsStaticMapCheckBox.Checked;
            MapDynamicContent content = Content as MapDynamicContent;
            content.Value = data;
        }

        /// <summary>
        /// Validates the selected coordinates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidateCoordinates(object source, ServerValidateEventArgs args)
        { 
            if (String.IsNullOrEmpty(LatitudeField.Value) || String.IsNullOrEmpty(LongitudeField.Value))
            {
                args.IsValid = false;
                return;
            }
            double latlngValue;
            if (!double.TryParse(LatitudeField.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out latlngValue) ||
                latlngValue == 0)
            {
                args.IsValid = false;
                return;
            }
            if (!double.TryParse(LongitudeField.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out latlngValue) ||
                latlngValue == 0)
            {
                args.IsValid = false;
                return;
            }
            args.IsValid = true;
        }
    }
}
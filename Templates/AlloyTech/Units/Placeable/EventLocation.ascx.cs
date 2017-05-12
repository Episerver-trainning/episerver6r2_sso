#region Copyright
// Copyright © 1996-2011 EPiServer AB.  All rights reserved.
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

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    public partial class EventLocation : UserControlBase
    {

        /// <summary>
        /// Makes the speaker and or location are visible if speaker or location properties are null
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            SpeakerPlaceHolder.Visible = CurrentPage["Speaker"] != null;
            LocationPlaceHolder.Visible = CurrentPage["Location"] != null;

            base.OnLoad(e);
        }

    }
}
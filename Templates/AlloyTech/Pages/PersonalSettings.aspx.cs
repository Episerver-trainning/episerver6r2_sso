#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Architecture limitations
Only uses the standard properties from EPiServerProfile. It is possible to specify your own in Web.config
but this template will not make use of those.
*/
#endregion

using System;
using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.AlloyTech.Pages
{
	public partial class PersonalSettings : TemplatePage
	{
        /// <summary>
        /// Handles on load event
        /// </summary>
        /// <param name="e">Event parameters</param>
        protected override void OnLoad(EventArgs e)
        {
            WorkroomPageBase.InitializeWorkroomPage(this);
            base.OnLoad(e);
        }
    }
}
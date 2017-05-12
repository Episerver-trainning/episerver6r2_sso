/*
Copyright (c) 2010 EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using EPiServer.Core;
using EPiServer.Web.WebControls;
using System.Web.UI.WebControls;
using System;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// A common footer for the website where links common for the whole site are listed. 
    /// </summary>
    public partial class PageFooter : UserControlBase
    {
        /// <summary>
        /// Load page event
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
        }

        protected PageReference StartPageReference
        {
            get { return CurrentPage["StartPage"] as PageReference; }
        }
    }
}

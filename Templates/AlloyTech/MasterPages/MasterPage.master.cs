#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.UI.HtmlControls;
using EPiServer.ClientScript;
using EPiServer.Core;

namespace EPiServer.Templates.AlloyTech.MasterPages
{
    /// <summary>
    /// The masterpage defines the common look and feel and a standard behavior of the website. 
    /// </summary>
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        public string LanguageString { get; private set; }
       
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);            
            if (Submenu != null && MainMenu != null)
            {
                Submenu.MenuList = MainMenu.MenuList;
            }

            this.LanguageString = ((PageBase)Page).CurrentPage.LanguageBranch;
        }

    }
}

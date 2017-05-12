#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Linq;
using EPiServer;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// Lists the pages in the submenu. The root page for the submenu is 
    /// the current page in the main menu.
    /// </summary>
    public partial class Submenu : UserControlBase
    {
        private MenuList _menuList;

        /// <summary>
        /// Gets or sets the data source for this control
        /// </summary>
        public MenuList MenuList
        {
            get { return _menuList; }
            set { _menuList = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (MenuList == null || MenuList.OpenTopPage == null)
            {
                Menu.Visible = false;
                return;
            }
            
            Menu.PageLink = MenuList.OpenTopPage;
            Menu.DataBind();

            // hide the whole menu if only the root page is displayed
            Menu.Visible = Menu.Any(p => p.PageLink.ID != Menu.PageLink.ID);
        }
    }
}

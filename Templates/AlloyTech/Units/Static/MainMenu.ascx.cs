/*
Copyright (c) EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// Lists the pages visible in the main (top) menu.
    /// </summary>
    public partial class MainMenu : UserControlBase
    {
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            Menu.PageLink = GetMainMenuContainer() ?? PageReference.StartPage;
            Menu.PageLoader.GetChildrenCallback = new HierarchicalPageLoader.GetChildrenMethod(LoadChildren);
            Menu.DataBind();
        }

        /// <summary>
        /// Creates the collection for the main menu, adding the startpage
        /// </summary>
        private PageDataCollection LoadChildren(PageReference pageLink)
        {
            PageDataCollection pages = DataFactory.Instance.GetChildren(pageLink);
            pages.Insert(0, DataFactory.Instance.GetPage(pageLink));
            return pages;
        }

        /// <summary>
        /// Gets the root page for the main menu.
        /// </summary>
        private PageReference GetMainMenuContainer()
        {
            PageBase page = Page as PageBase;
            if (page != null)
            {
                PageReference mainMenuProp;
                // in the case if the pagedata is readonly mode then dynamic property can not be reached.
                if (page.CurrentPage != null && page.CurrentPage.Property.TryGetValue<PageReference>("MainMenuContainer", out mainMenuProp))
                {
                    return mainMenuProp;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the MenuList for this control
        /// </summary>
        public MenuList MenuList
        {
            get { return Menu; }
            set { Menu = value; }
        }
    }
}
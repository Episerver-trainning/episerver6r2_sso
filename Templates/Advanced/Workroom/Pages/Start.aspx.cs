#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:
- Performance limitations
Since the file list control has to retrieve all files added to the the work room to be able to
sort them properly there might be performance issues with many files.

*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    public partial class Start : WorkroomPageBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                FileList.Visible = IsFileManagerVisibleInMenu();
                NewsListUnit.Visible = !PageReference.IsNullOrEmpty((PageReference)CurrentPage["NewsRoot"]) &&
                                        DataFactory.Instance.GetPage((PageReference)CurrentPage["NewsRoot"]).VisibleInMenu;
                CalendarList.Visible = !PageReference.IsNullOrEmpty((PageReference)CurrentPage["CalendarContainer"]) &&
                                        DataFactory.Instance.GetPage((PageReference)CurrentPage["CalendarContainer"]).VisibleInMenu;
                FileList.DataBind();
            }
            
            if (NewsListUnit.Visible)
            {
                NewsListUnit.DataSource = GetNewsListPages();
                NewsListUnit.DataBind();
            }
            
            if (CalendarList.Visible)
            {
                CalendarList.DataBind();
            }
        }

        private PageDataCollection GetNewsListPages()
        {
            IList<PageReference> newsItemReferences = DataFactory.Instance.GetDescendents((PageReference)CurrentPage["NewsRoot"]);
            PageDataCollection result = DataFactory.Instance.GetPages(newsItemReferences, LanguageSelector.AutoDetect(true));
            new Filters.FilterCompareTo("PageTypeName", NewsItemPageTypeName).Filter(result);
            new Filters.FilterPropertySort("PageStartPublish", Filters.FilterSortDirection.Ascending).Filter(result);
            return result;
        }

        /// <summary>
        /// Determines whether there is any child to the current page that is a file manager with visible in menu set to true.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if there is a file manager with VisibleInMenu = true; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFileManagerVisibleInMenu()
        {
            return GetChildren(CurrentPageLink).Any(page => String.Equals(page.PageTypeName, FileManagerPageTypeName) && page.VisibleInMenu);
        }
    }
}

#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Usability limitations
On page edit is disabled.

- Multi language limitations
Due to the nature of a blog, the template is not designed and tested with multi languge considerations in mind.

*/
#endregion

using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.Web;


namespace EPiServer.Templates.AlloyTech.Blog.Pages
{
    /// <summary>
    /// Template used for date and tag listings
    /// </summary>
    public partial class List : BlogPageBase
    {
        private PageDataCollection _pages;
        private PropertyCriteriaCollection _criterias = new PropertyCriteriaCollection();
        private PageReference _searchStart;
        private TimeSpan _searchInterval;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        /// <summary>
        /// Sets the search criterias to find the blog items used for the listing. 
        /// Uses the properties IsTagListing, IsDateListing and IsTeamLevel to determine what criterias to set up.
        /// </summary>
        private void SetSearchCriterias()
        {
            if (CurrentPage[BlogUtility.IsTagListingPropertyName] != null)
            {
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageTypeName", PropertyDataType.PageType, BlogUtility.ItemPageTypeName, true));
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageCategory", PropertyDataType.Category, CurrentPage.PageName, true));

                _searchStart = (PageReference)CurrentPage[BlogUtility.StartPropertyName];
            }
            else if (CurrentPage[BlogUtility.IsDateListingPropertyName] != null && CurrentPage[BlogUtility.IsTeamLevelPropertyName] == null)
            {
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageTypeName", PropertyDataType.PageType, BlogUtility.ItemPageTypeName, true));

                _searchStart = CurrentPageLink;
            }
            else if (CurrentPage[BlogUtility.IsDateListingPropertyName] != null && CurrentPage[BlogUtility.IsTeamLevelPropertyName] != null)
            {
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageTypeName", PropertyDataType.PageType, BlogUtility.ItemPageTypeName, true));
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.GreaterThan, "PageStartPublish", PropertyDataType.Date, CurrentPage.StartPublish.ToString(), true));
                _criterias.Add(BlogUtility.CreateCriteria(CompareCondition.LessThan, "PageStartPublish", PropertyDataType.Date, CurrentPage.StartPublish.Add(SearchInterval).ToString(), true));

                _searchStart = (PageReference)CurrentPage[BlogUtility.StartPropertyName];
            }
            else
            {
                throw new EPiServerException(Translate("/blog/listpage/errormessage"));
            }
        }

        /// <summary>
        /// The blog items used for the listing. Filtered by the criterias set up in SetSearchCriterias.
        /// </summary>
        /// <value>A PageDataCollection containing the blog items used for the listing</value>
        protected PageDataCollection Pages
        {
            get 
            {
                
                if (_pages == null)
                {
                    SetSearchCriterias();
                    _pages = DataFactory.Instance.FindPagesWithCriteria(_searchStart, _criterias);
                    FilterForVisitor.Filter(_pages);
                }
                return _pages; 
            }
        }

        private TimeSpan SearchInterval
        {
            get
            {
                if (IsYearListing())
                {
                    _searchInterval = CurrentPage.StartPublish.AddYears(1).Subtract(CurrentPage.StartPublish);
                }
                else
                {
                    _searchInterval = CurrentPage.StartPublish.AddMonths(1).Subtract(CurrentPage.StartPublish);
                }

                return _searchInterval;
            }
        }

        /// <summary>
        /// The source for RSS/Atom feeds, which in this case is the blog items listed on the page.
        /// </summary>
        /// <value>See the Pages property</value>
        protected override PageDataCollection FeedSource
        {
            get
            {
                return Pages;
            }
        }

        /// <summary>
        /// Gets the heading for the listing page.
        /// </summary>
        /// <value>A string containing the heading.</value>
        protected string GetHeading()
        {
            if (CurrentPage["IsTagListing"] != null)
            {
                return String.Format(Translate("/blog/listpage/tagheading"), "<em>" + CurrentPage.PageName + "</em>");
            }
            else if (CurrentPage["IsDateListing"] != null)
            {
                if (IsYearListing())
                {
                    return String.Format(Translate("/blog/listpage/dateheading"), CurrentPage.StartPublish.ToString("yyyy"));
                }
                else
                {
                    return String.Format(Translate("/blog/listpage/dateheading"), CurrentPage.StartPublish.ToString("MMMM yyyy"));
                }
            }
            else
            {
                throw new EPiServerException(Translate("/blog/listpage/errormessage"));
            }
                 
        }

        private bool IsYearListing()
        {
            int year;
            return int.TryParse(CurrentPage.PageName, out year) && CurrentPage.PageName.Length == 4;
        }
    }
}

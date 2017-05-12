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

using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;

namespace EPiServer.Templates.AlloyTech.Blog.Pages
{
    /// <summary>
    /// Template used for the team start page type
    /// </summary>
    public partial class TeamStart : BlogPageBase
    {
        private PageDataCollection _blogItems;
        private TimeSpan _historyLength = new TimeSpan(30, 0, 0, 0);//30 days

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
            base.OnPreRender(e);
        }

        /// <summary>
        /// The source for RSS/Atom feeds, which in this case is the blog items listed on the page.
        /// </summary>
        /// <value>See the BlogItems property</value>
        protected override PageDataCollection FeedSource
        {
            get
            {
                return BlogItems;
            }
        }

        /// <summary>
        /// All the blog items below this page.
        /// </summary>
        /// <value>A PageDataCollection containing all the blog items below this page.</value>
        protected PageDataCollection BlogItems
        {
            get
            {
                if (_blogItems == null)
                {
                    PropertyCriteriaCollection criterias = new PropertyCriteriaCollection();
                    criterias.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageTypeName", PropertyDataType.PageType, BlogUtility.ItemPageTypeName, true));
                    criterias.Add(BlogUtility.CreateCriteria(CompareCondition.GreaterThan, "PageStartPublish", PropertyDataType.Date, DateTime.Now.Subtract(HistoryLength).ToString(), true));
                    _blogItems = DataFactory.Instance.FindPagesWithCriteria(CurrentPage.PageLink, criterias);

                    FilterForVisitor.Filter(_blogItems);
                    new FilterSort(FilterSortOrder.PublishedDescending).Filter(_blogItems);
                }
                return _blogItems;
            }
        }

        /// <summary>
        /// The length to truncate the blog item texts to.
        /// Negative values will be ignored, and default will be used instead.
        /// </summary>
        /// <remarks>Default value is 250</remarks>
        protected int SummaryTextLength
        {
            get 
            {
                if (CurrentPage[BlogUtility.SummaryTextLengthPropertyName] != null && (int)CurrentPage[BlogUtility.SummaryTextLengthPropertyName] >= 0)
                {
                    return (int)CurrentPage[BlogUtility.SummaryTextLengthPropertyName];
                }
                return 250; 
            }
        }

        /// <summary>
        /// Defines how many blog items that will be listed on the page.
        /// </summary>
        /// <remarks>Default value is 5</remarks>
        protected int MaxCount
        {
            get
            {
                if (CurrentPage[BlogUtility.MaxCountPropertyName] != null)
                {
                    return (int)CurrentPage[BlogUtility.MaxCountPropertyName];
                }
                return 5;
            }
        }

        /// <summary>
        /// The TimeSpan that is used to set the age of the items that should be listed. 
        /// Can be set with a property called "HistoryLength" on the team start page in EPiServer.
        /// </summary>
        /// <remarks>There is a max length on the time span of 10 000 days, even if "HistoryLength" is longer.</remarks>
        /// <value>The time span to list.</value>
        protected TimeSpan HistoryLength
        {
            get 
            {
                if (CurrentPage[BlogUtility.HistoryLengthPropertyName] != null)
                {
                    int days = (int)CurrentPage[BlogUtility.HistoryLengthPropertyName];

                    if (days > 10000)
                    {
                        _historyLength = new TimeSpan(10000, 0, 0, 0);
                    }
                    else
                    {
                        _historyLength = new TimeSpan(days, 0, 0, 0);
                    }
                }
                return _historyLength; 
            }
        }
    }
}

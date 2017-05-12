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
    /// Template used the personal start page type.
    /// </summary>
    public partial class PersonalStart : BlogPageBase
    {
        private PageDataCollection _blogItems;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (CurrentPage[BlogUtility.StartPropertyName] == null || ((PageReference)CurrentPage[BlogUtility.StartPropertyName]).ID!= CurrentPageLink.ID)
            {
                DynamicProperty blogStart = DynamicProperty.Load(CurrentPageLink, BlogUtility.StartPropertyName);
                blogStart.PropertyValue.Value = CurrentPageLink;
                blogStart.Save();
            }
        }

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
        /// The length to truncate the blog item texts to.
        /// Negative values will be ignored, and default will be used instead.
        /// </summary>
        /// <remarks>Default value is 250.</remarks>
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
        /// <remarks>Default value is 5.</remarks>
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
                    _blogItems = DataFactory.Instance.FindPagesWithCriteria(BlogStart.PageLink, criterias);

                    FilterForVisitor.Filter(_blogItems);
                    new FilterSort(FilterSortOrder.PublishedDescending).Filter(_blogItems);
                }

                return _blogItems;
            }
        }
    }
}

#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;

using EPiServer.Core;

namespace EPiServer.Templates.AlloyTech.Blog.Units
{
    /// <summary>
    /// A simple implementation of a tag cloud.
    /// </summary>
    public partial class TagCloud : UserControlBase
    {
        private PageReference _tagStartPage;

        /// <summary>
        /// Calculates the size of the rendered tag.
        /// </summary>
        /// <param name="page">The tag page.</param>
        /// <returns></returns>
        protected static string RenderTag(PageData page)
        {
            object value = page[BlogUtility.TagCloudValuePropertyName];
            int size = value is int ? (int)value : 0;
            size = Math.Max(size, 1);
            size = Math.Min(size, 6);
            return String.Format("<h{0}><a rel=\"tag\" href=\"{1}\">{2}</a></h{0}>", size, page.LinkURL, page.PageName);
        }

        /// <summary>
        /// Gets or sets the tag start page container.
        /// </summary>
        /// <value>A reference to the tag container.</value>
        public PageReference TagStartPage
        {
            get 
            {
                return _tagStartPage;
            }
            set 
            {
                _tagStartPage = value;
            }
        }
    }
}
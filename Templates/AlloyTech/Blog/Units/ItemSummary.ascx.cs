#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.DataAbstraction;
using EPiServer.Filters;

namespace EPiServer.Templates.AlloyTech.Blog.Units
{
    /// <summary>
    /// A brief display of a blog item.
    /// </summary>
    public partial class ItemSummary : UserControlBase
    {
        private PageDataCollection _tags;
        private PageData _blogStart;
        private PageData _selectedPage;
        private string _summaryText;
        private int _summaryTextLength = 250;
        private bool _showImage = true;
        private bool _showWriter = true;

        /// <summary>
        /// Checks if an image for the blogger exists, and if so, displays it.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!String.IsNullOrEmpty((string)BlogStart["Image"]) && ShowImage)
            {
                BloggerLink.Visible = true;
                BloggerLink.HRef = BlogStart.LinkURL;
                BloggerImage.Src = BlogStart["Image"].ToString();
                BloggerImage.Alt = BlogStart["Writer"].ToString();
             
            }
        }

        /// <summary>
        /// Strips any html tags from the MainBody property and reduces the lenght to 250 chars.
        /// </summary>
        /// <value>A string with the preview text.</value>
        protected string SummaryText
        {
            get 
            {
                if (SummaryTextLength == 0)
                {
                    return String.Empty;
                }

                if (String.IsNullOrEmpty(_summaryText))
                {
                    _summaryText = TextIndexer.StripHtml((String)SelectedPage["MainBody"], SummaryTextLength);
                }
                return _summaryText; 
            }
        }

        /// <summary>
        /// The number of characters to truncate the preview text to.
        /// </summary>
        public int SummaryTextLength
        {
            get { return _summaryTextLength; }
            set { _summaryTextLength = value; }
        }

        /// <summary>
        /// Defines wether to show the bloggers image.
        /// </summary>
        public bool ShowImage
        {
            get { return _showImage; }
            set { _showImage = value; }
        }

        /// <summary>
        /// Defines wether to show the writers name.
        /// </summary>
        public bool ShowWriter
        {
            get { return _showWriter; }
            set { _showWriter = value; }
        }

        /// <summary>
        /// The blog item to render
        /// </summary>
        public PageData SelectedPage
        {
            get 
            {
                if (_selectedPage == null)
                {
                    return CurrentPage;
                }
                return _selectedPage;
            }
            set { _selectedPage = value; }
        }

        /// <summary>
        /// The tags for the blog item
        /// </summary>
        protected PageDataCollection Tags
        {
            get
            {
                if (_tags == null)
                {
                    
                    _tags = new PageDataCollection();
                    CategoryList cl = (CategoryList)SelectedPage["PageCategory"];

                    if (cl == null)
                    {
                        return _tags;
                    }

                    Category categoryTag;
                    PageDataCollection pdc = DataFactory.Instance.GetChildren((PageReference)BlogStart[BlogUtility.TagContainerPropertyName]);
                    FilterForVisitor.Filter(_tags);

                    foreach (int c in cl)
                    {
                        categoryTag = Category.Find(c);

                        foreach (PageData pd in pdc.Where(page => String.Equals(page.PageName, categoryTag.Name)))
                        {
                            _tags.Add(pd);
                        }
                    }
                }
                return _tags;
            }
        }

        /// <summary>
        /// Gets the blog start page.
        /// </summary>
        public PageData BlogStart
        {
            get 
            {
                if (_blogStart == null) 
                {
                    if( SelectedPage[ BlogUtility.StartPropertyName ] != null)
                    {
                        _blogStart = DataFactory.Instance.GetPage(( PageReference ) SelectedPage[ BlogUtility.StartPropertyName ]);
                    }else
                    {
                        _blogStart = DataFactory.Instance.GetPage(SelectedPage.ParentLink);
                    }
                }
                return _blogStart;
            }  
        }
    }
}
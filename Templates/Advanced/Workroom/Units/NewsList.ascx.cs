#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.Templates.Advanced.Workroom.Pages;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    public partial class NewsList : UserControlBase
    {
        /// <summary>
        /// Occurs when add button is clicked.
        /// </summary>
        public event EventHandler AddButtonClicked;

        private bool _listItemEven = true;
        private bool? _showPostBackLinks;
        private PageReference _selectedPageId;

        /// <summary>
        /// Sets the heading on the page.
        /// Loads the appropriate data to the MainIntroProp and MainBodyProp.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (PageReference.IsNullOrEmpty(PageLink) && PageReference.IsNullOrEmpty((PageReference)CurrentPage[PageLinkProperty]) 
                && Page as NewsListPage == null && DataSource == null)
            {
                this.Visible = false;
            }
            
            if (!String.IsNullOrEmpty(Heading))
            {
                NewsListHeading.InnerText = Heading;
            }
            else
            {
                NewsListHeading.Visible = false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (NewsListing.DataCount==0 && !IsPostBack)
            {
                NewsListContainer.Visible = false;
            }
            if (NewsListing.DataCount>0)
            {
                NewsListContainer.Visible = true;
            }
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the heading before the page list.
        /// </summary>
        public string Heading
        {
            get { return (string)(ViewState["Heading"] ?? String.Empty); }
            set { ViewState["Heading"] = value; }
        }

        /// <summary>
        /// Gets or sets the page property pointing out the parent page for news listing.
        /// </summary>
        /// <value>The name of the page property pointing out the parent page</value>
        public string PageLinkProperty
        {
            get { return NewsListing.PageLinkProperty; }
            set { NewsListing.PageLinkProperty = value; }
        }

        /// <summary>
        /// Gets or sets the page link for the parent for news listing.
        /// </summary>
        /// <value>PageReference PageLink</value>
        public PageReference PageLink
        {
            get { return NewsListing.PageLink; }
            set { NewsListing.PageLink = value; }
        }

        public PageDataCollection DataSource
        {
            get { return NewsListing.DataSource as PageDataCollection; }
            set { NewsListing.DataSource = value; }
        }
       
        /// <summary>
        /// Gets or sets the maximum number of pages to show in listing.
        /// </summary>
        /// <value>Maximum number of pages to show</value>
        public int MaxCount
        {
            get { return NewsListing.MaxCount; }
            set { NewsListing.MaxCount = value; }
        }

        /// <summary>
        /// Gets or sets the length of the preview text.
        /// </summary>
        /// <value>PageReference PageLink. Default is 30</value>
        public int PreviewTextLength
        {
            get { return (int)(ViewState["PreviewTextLength"] ?? 30); }
            set { ViewState["PreviewTextLength"] = value; }
        }


        /// <summary>
        /// Enables paging of the list.
        /// Gets or sets the number of pages that goes into one paging item
        /// </summary>
        /// <value>Number of pages that goes into one paging item</value>
        public int PagesPerPagingItem
        {
            get 
            {
                return NewsListing.PagesPerPagingItem; 
            }
            set 
            {
                NewsListing.Paging = true;    
                NewsListing.PagesPerPagingItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what type of links that should be used for the listing. Postback links, or hyper links.
        /// Postback links should only be used when the control is on pages of the type NewsListPage.
        /// </summary>
        /// <value><c>true</c> if PostBack links should be used; otherwise, <c>false</c>.</value>
        public bool ShowPostBackLinks
        {
            get 
            {
                if (_showPostBackLinks == null)
                {
                    _showPostBackLinks = Page is NewsListPage;
                }
                return _showPostBackLinks.Value; 
            }
            set { _showPostBackLinks = value; }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether list items are in even (2,4,6...) or odd list positions.
        /// </summary>
        /// <value><c>true</c> if item is in even position otherwise, <c>false</c>.</value>
        protected bool ListItemEven
        {
            get 
            {
                return _listItemEven = !_listItemEven;
            }
        }

        /// <summary>
        /// Handles the update of MainIntro and MainBody when an item in the news list is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void LinkButton_Command(Object sender, CommandEventArgs e)
        {
            NewsListPage newsListPage = Page as NewsListPage;
            if (newsListPage != null)
            {
                PageReference pageRef = new PageReference((string)e.CommandArgument);
                SelectedPageId = pageRef;
                if (newsListPage.CurrentDisplayMode != DisplayMode.View)
                {
                    newsListPage.CurrentDisplayMode = DisplayMode.View;
                }
                newsListPage.UpdateDisplayTexts(pageRef);
            }
        }

        /// <summary>
        /// Displays the create form on the NewsListPage that is hosting this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddButton_Click(object sender, EventArgs e)
        {
            OnAddButtonClicked(new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnAddButtonClicked(EventArgs e)
        {
            if (AddButtonClicked != null)
            {
                AddButtonClicked(this, e);
            }
        }

        /// <summary>
        /// Enables/disables the visibility of the add button. 
        /// </summary>
        public bool AddButtonVisible
        {
            get 
            {
                return AddButton.Visible;
            }
            set 
            {
                AddButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the currently selected news item.
        /// </summary>
        /// <value>The selected page ID.</value>
        public PageReference SelectedPageId
        {
            get 
            {
                if (ViewState["SelectedPageId"] == null && _selectedPageId == null)
                {
                    ViewState["SelectedPageId"] = _selectedPageId = GetSelectedPageId();
                }
                else
                {
                    _selectedPageId = ViewState["SelectedPageId"] as PageReference;
                }
                return _selectedPageId;
            }
            set { ViewState["SelectedPageId"] = _selectedPageId = value; }
        }

        private PageReference GetSelectedPageId()
        {
            PageReference pageRef;

            //If there is a query string attribute called selectednewspage, use that.
            if (PageReference.TryParse(Request.QueryString["selectednewspage"], out pageRef))
            {
                return pageRef;
            }
            
            //Use the first item in the list as a default value.
            foreach (PageData page in NewsListing)
            {
                return page.PageLink;
            }

            return CurrentPage.PageLink;
        }
    }
}

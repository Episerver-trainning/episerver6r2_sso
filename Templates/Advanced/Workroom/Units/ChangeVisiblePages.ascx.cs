#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;


namespace EPiServer.Templates.Advanced.Workroom.Units
{
    /// <summary>
    /// User control for toggling visible in menu on a pages children.
    /// If the page that this control is placed on is found in the collection it's excluded from the toggle list.
    /// </summary>
    public partial class ChangeVisiblePages : EPiServer.UserControlBase
    {
        private bool _requiresDataBinding;
        private PageReference _pageLink;
        private EventHandler _visibilityChanged;

        #region Public Properties

        /// <summary>
        /// Occurs when the visibility of pages in the collection are changed.
        /// </summary>
        [Category("Action")]
        public event EventHandler VisibilityChanged
        {
            add { _visibilityChanged += value; }
            remove { _visibilityChanged -= value; }
        }



        /// <summary>
        /// The page to read the child collection from.
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Localizable(true)]
        public PageReference PageLink
        {
            get { return (PageReference)(ViewState["PageLink"] ?? PageReference.EmptyReference); }
            set 
            {
                _pageLink = null;
                _requiresDataBinding = true;
                ViewState["PageLink"] = value; 
            }
        }

        /// <summary>
        /// The name of a property on the current page to get the pagelink from.
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string PageLinkProperty
        {
            get { return (string)(ViewState["PageLinkProperty"] ?? String.Empty); }
            set 
            {
                _pageLink = null;
                _requiresDataBinding = true;
                ViewState["PageLinkProperty"] = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control is enabled.
        /// </summary>
        [Category("Appearance"), DefaultValue(true), Localizable(true)]
        public bool Enabled
        {
            get
            {
                return VisibilityList.Enabled || SaveButton.Enabled;
            } 
            set
            {
                VisibilityList.Enabled = value;
                SaveButton.Enabled = value;
            }
        }

        #endregion

        #region Data Binding

        /// <summary>
        /// Binds the collection pointed out by <see cref="PageLink"/> or <see cref="PageLinkProperty"/> to the display control.
        /// </summary>
        protected override void DataBindChildren()
        {
            base.DataBindChildren();
            if (_requiresDataBinding)
            {
                DataBindVisibilityList();
                _requiresDataBinding = false;
            }
        }

        /// <summary>
        /// Populates the CheckBoxList from the page collection.
        /// </summary>
        private void DataBindVisibilityList()
        {
            PageReference pageLink = GetListPageLink();
            if (!PageReference.IsNullOrEmpty(pageLink))
            {
                VisibilityList.Items.Clear();

                PageDataCollection pages = GetChildren(pageLink);
                EPiServer.Filters.FilterForVisitor.Filter(pages);

                foreach (PageData page in pages.Where(p => !p.PageLink.CompareToIgnoreWorkID(CurrentPage.PageLink)))
                {
                    ListItem item = new ListItem(page.PageName, page.PageLink.ToString());
                    item.Selected = page.VisibleInMenu;
                    VisibilityList.Items.Add(item);
                }
            }
        }

        #endregion

        #region Save event handling

        /// <summary>
        /// Handles the Click event of the SaveVisibility button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveVisibility_Click(object sender, EventArgs e)
        {
            bool itemsModified = false;

            PageDataCollection pages = GetChildren(PageLink);
            foreach (ListItem item in VisibilityList.Items)
            {
                PageData page = FindPage(pages, PageReference.Parse(item.Value));
                if (page != null && page.VisibleInMenu != item.Selected)
                {
                    page = page.CreateWritableClone();
                    page.VisibleInMenu = item.Selected;
                    DataFactory.Instance.Save(page, SaveAction.Publish | SaveAction.ForceCurrentVersion);
                    itemsModified = true;
                }
            }
            if (itemsModified)
            {
                _requiresDataBinding = true;
                OnVisibilityChanged(EventArgs.Empty);
            }
            DataBind();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Raises the <see cref="E:VisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnVisibilityChanged(EventArgs e)
        {
            if (_visibilityChanged != null)
            {
                _visibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Gets a page reference pointing out the root page for the page collection to toggle visibility for.
        /// It looks at <see cref="PageLink"/> property and <see cref="PageLinkProperty"/> in named order to find a <see cref="PageReference"/>.
        /// </summary>
        /// <returns>A <see cref="PageReference"/>; or <c>null</c> if none has been set.</returns>
        private PageReference GetListPageLink()
        {
            if (_pageLink == null)
            {
                if (!PageReference.IsNullOrEmpty(PageLink))
                {
                    _pageLink = PageLink;
                }
                else if (!String.IsNullOrEmpty(PageLinkProperty))
                {
                    _pageLink = CurrentPage[PageLinkProperty] as PageReference;
                }
            }
            return _pageLink;
        }

        /// <summary>
        /// Finds a page in the supplied <c>pages</c> collection.
        /// </summary>
        /// <param name="pages">The collection of pages to search in.</param>
        /// <param name="pageLink">A <see cref="PageReference"/> to find.</param>
        /// <returns>The <see cref="PageData"/> for the page if one could be found; otherwise <c>null</c>.</returns>
        private static PageData FindPage(PageDataCollection pages, PageReference pageLink)
        {
            if (PageReference.IsNullOrEmpty(pageLink))
            {
                return null;
            }
            int index = pages.Find(pageLink);
            if (index != -1)
            {
                return pages[index];
            }
            return null;
        }

        #endregion

    }
}
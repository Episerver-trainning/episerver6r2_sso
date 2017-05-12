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
using System.Web.UI.WebControls;
using EPiServer.Core;
using System.Web.UI;
using System.ComponentModel;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// Breadcrumbs web control
    /// </summary>
    [DefaultEvent("Load")]
    [DefaultProperty("Separator")]
    [DefaultBindingProperty("Separator")]
    public class Breadcrumbs : CompositeControl
    {
        /// <summary>
        /// Gets or sets the maximum number of levels.
        /// </summary>
        /// <value>The maximum number of levels.</value>
        [Category("Breadcrumbs")]
        [Description("Maximum number of levels in breadcrumps")]
        public int MaxLevelsNumber
        {
            get;
            set;
        }

        /// <summary>
        /// String to display insead of pages, when number of pages is bigger than max levels number.
        /// </summary>
        protected const string NotIncludedPagesString = "...";

        string _separator = " / ";

        /// <summary>
        /// Gets or sets the string separator for breadcrumb links.
        /// </summary>
        /// <value>The separator string.</value>
        [Category("Breadcrumbs")]
        [Description("Separator string for breadcrumbs hyperlinks")]
        public string Separator
        {
            get
            {
                return _separator;
            }
            set
            {
                _separator = value;
            }
        }

        /// <summary>
        /// Gets or sets the reference of the lowest page in breadcrumbs hierarchy. Current page is used by default.
        /// </summary>
        /// <value>The lowest page.</value>
        [Category("Breadcrumbs")]
        [Description("Lowest page for breadcrumbs")]
        public PageReference LowestPage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reference of the top page in breadcrumbs hierarchy. Start/Root site page is used by default.
        /// </summary>
        /// <value>The top page.</value>
        [Category("Breadcrumbs")]
        [Description("Top page for breadcrumbs")]
        public PageReference TopPage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom name of the top page to display in breadcrumbs.
        /// </summary>
        /// <value>The custom name of the top page.</value>
        [Category("Breadcrumbs")]
        [Description("The name of top page in breadcrumbs to display. By default it is the name of page or \"Start\"")]
        public string TopPageName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom tool tip for breadcrumbs top page.
        /// </summary>
        /// <value>The custom tool tip for breadcrumbs top page.</value>
        [Category("Breadcrumbs")]
        [Description("Tool tip of top page in breadcrumbs to display.")]
        public string TopPageToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS class for breadcrumbs links.
        /// </summary>
        /// <value>The link CSS class.</value>
        [Category("Styling")]
        [Description("Breadcrumbs hyperlink CSS class")]
        public string LinkCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Collection for pages hierarchy from lowest to top/start page.
        /// </summary>
        private PageDataCollection _pagesList = new PageDataCollection();

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            SetupParameters();
            CreatePagesHierarchyList();
            base.OnLoad(e);
        }

        /// <summary>
        /// Creates the pages hierarchy list.
        /// </summary>
        protected virtual void CreatePagesHierarchyList()
        {
            PageData page = GetPage(LowestPage);
            while (page != null)
            {
                _pagesList.Insert(0, page);
                page = GetParentPageData(page);
            }        
        }

        /// <summary>
        /// Gets the parent page of the specified page.
        /// </summary>
        /// <param name="pageData">The page data.</param>
        /// <returns></returns>
        private PageData GetParentPageData(PageData pageData)
        {
            if (pageData == null || pageData.PageLink == PageReference.StartPage || pageData.ParentLink == PageReference.RootPage ||
                pageData.PageLink == PageReference.RootPage || pageData.PageLink == TopPage)
            {
                return null;
            }
            pageData = GetPage(pageData.ParentLink);
            if (pageData != null && pageData.VisibleInMenu)
            {
                return pageData;
            }
            return GetParentPageData(pageData);
        }

        /// <summary>
        /// Gets the page by page reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        protected PageData GetPage(PageReference reference)
        {
            if (PageReference.IsNullOrEmpty(reference))
            {
                return null;
            }

            PageBase pageBase = (Page as PageBase);
            if (pageBase == null)
            {
                return null;
            }
            return pageBase.GetPage(reference);            
        }

        /// <summary>
        /// Setups the controls parameters.
        /// </summary>
        protected virtual void SetupParameters()
        {
            if (LowestPage == null)
            {
                PageBase pageBase = Page as PageBase;
                if (pageBase != null)
                {
                    LowestPage = pageBase.CurrentPage.PageLink;
                }
            }
            if (MaxLevelsNumber == 0)
            {
                MaxLevelsNumber = Int32.MaxValue;
            }
            MaxLevelsNumber = MaxLevelsNumber > 2 ? MaxLevelsNumber : 2;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            CreateBreadcrumbs();
            ClearChildControlState();
        }

        /// <summary>
        /// Creates the breadcrumbs link list.
        /// </summary>
        protected virtual void CreateBreadcrumbs()
        {
            string pageName = String.Empty;
            string pageToolTip = String.Empty;
            string pageUrl = String.Empty;

            // always show top page in breadcrums
            PageData page = _pagesList[0];
            pageName = !String.IsNullOrEmpty(TopPageName) ? TopPageName : page.Property["PageName"].ToWebString();
            pageToolTip = !String.IsNullOrEmpty(TopPageToolTip) ? TopPageToolTip : pageName;
            pageUrl = Context.Server.UrlPathEncode(page.LinkURL);

            HyperLink pageLink = new HyperLink();
            pageLink.Text = pageName;
            pageLink.ToolTip = pageToolTip;
            pageLink.NavigateUrl = pageUrl;
            if (!String.IsNullOrEmpty(LinkCssClass))
            {
                pageLink.CssClass = LinkCssClass;
            }
            Controls.Add(pageLink);
            
            int i = 1;
            // limit the number of links in breadcrumbs by MaxLevelsNumber parameter            
            if (_pagesList.Count > MaxLevelsNumber)
            {
                // show maximum number of lowest pages in hierarchy
                i = _pagesList.Count - MaxLevelsNumber + 1;
                Literal separator = new Literal();
                separator.Text = Separator + NotIncludedPagesString;
                Controls.Add(separator);
            }

            ILanguageSelector languageSelector = LanguageSelector.AutoDetect();

            for (; i < _pagesList.Count; i++)
            {
                page = _pagesList[i];
                Literal separator = new Literal();
                separator.Text = Separator;
                Controls.Add(separator);

                pageName = page.Property["PageName"].ToWebString();
                pageToolTip = pageName;

                if (!page.IsVisibleOnSite || !PageExistsInLanguageContext(page, languageSelector))
                {
                    //Page does not have a template or does not exist for the current language, add as plain text instead of a hyperlink.
                    Controls.Add(new Literal() { Text = page.PageName });
                    continue;
                }

                pageUrl = Context.Server.UrlPathEncode(page.LinkURL);

                pageLink = new HyperLink();
        
                pageLink.Text = pageName;
                pageLink.ToolTip = pageToolTip;
                pageLink.NavigateUrl = pageUrl;
                if (!String.IsNullOrEmpty(LinkCssClass))
                {
                    pageLink.CssClass = LinkCssClass;
                }

                Controls.Add(pageLink);                
            }           
        }

        private bool PageExistsInLanguageContext(PageData page, ILanguageSelector languageSelector)
        {
            //Checks if page exist in current language context or if a fallback exist
            LanguageSelectorContext args = new LanguageSelectorContext(page);
            languageSelector.SelectPageLanguage(args);
            return !String.IsNullOrEmpty(args.SelectedLanguage);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> value that corresponds to this Web server control. This property is used primarily by control developers.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> enumeration values.
        /// </returns>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

    }
}

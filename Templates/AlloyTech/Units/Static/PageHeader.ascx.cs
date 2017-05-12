#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Web.WebControls;
using EPiServer.Filters;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// A common top area for the whole website, where the logotype is usually presented.
    /// </summary>
    public partial class PageHeader : UserControlBase
    {
        private PageData _startPageData;

        /// <summary>
        /// Gets the start page data object
        /// </summary>
        protected PageData StartPageData
        {
            get
            {
                if (_startPageData == null)
                {
                    PageReference pageReference = CurrentPage["StartPage"] as PageReference;
                    _startPageData = pageReference != null ? DataFactory.Instance.GetPage(pageReference) : DataFactory.Instance.GetPage(PageReference.StartPage);
                }

                return _startPageData;
            }
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                SetPageReference(Rss, StartPageData["RssContainer"] as PageReference);
                SetPageReference(SiteMap, StartPageData["SitemapPage"] as PageReference);

                if (Configuration.Settings.Instance.UIShowGlobalizationUserInterface)
                {
                    SetLanguage();
                }
            }

            Logotype.NavigateUrl = StartPageData.LinkURL;
            Logotype.ImageUrl = StartPageData["Logotype"] as string ?? string.Empty;
            Logotype.ToolTip = Translate("/navigation/startpagelinktitle");
            Logotype.Text = Translate("/navigation/startpagelinktitle");
            
        }

        /// <summary>
        /// Sets a page reference on a property control
        /// </summary>
        /// <param name="propertyControl">The property control</param>
        /// <param name="pageReference">The page reference</param>
        private static void SetPageReference(Property propertyControl, PageReference pageReference)
        {
            if (pageReference != null)
            {
                propertyControl.Parent.Visible = true;
                propertyControl.PageLink = pageReference;
            }
        }

        /// <summary>
        /// Initializes the language link.
        /// Checks the number of available and enabled languages. If more than two, 
        /// populates a dropdown-menu with the available and enabled languages.
        /// Otherwise sets the link to the not currently active language.
        /// </summary>
        private void SetLanguage()
        {
            PageDataCollection languageBranches = DataFactory.Instance.GetLanguageBranches(CurrentPage.PageLink);
            //Filter so pages with Replacement language is filtered away.
            new FilterReplacementLanguage().Filter(languageBranches);

            if (languageBranches.Count > 2)
            {
                LanguageList.Visible = LanguageListLabel.Visible = LanguageButton.Visible = LanguageList.Parent.Visible = true;
                foreach (PageData languageBranch in languageBranches.Where(p => p.LanguageID != CurrentPage.LanguageID && LanguageBranch.Load(p.LanguageID).Enabled))
                {
                    LanguageList.Items.Add(new System.Web.UI.WebControls.ListItem(new CultureInfo(languageBranch.LanguageID).NativeName, languageBranch.LanguageID));
                }
            }
            else
            {
                foreach (PageData languageBranch in languageBranches.Where(p => p.LanguageID != CurrentPage.LanguageID && LanguageBranch.Load(p.LanguageID).Enabled))
                {
                    Language.Visible = Language.Parent.Visible = true;
                    Language.NavigateUrl = EPiServer.UriSupport.AddLanguageSelection(languageBranch.LinkURL, languageBranch.LanguageID);
                    Language.Text = Translate(new CultureInfo(languageBranch.LanguageID).NativeName);
                    break;
                }
            }
        }

        /// <summary>
        /// Redirects to the selected language.
        /// </summary>
        public void ChangeLanguage(object sender, EventArgs e)
        {
            if (LanguageList.SelectedValue != "noLangSelected")
            {
                Response.Redirect(EPiServer.UriSupport.AddLanguageSelection(CurrentPage.LinkURL, LanguageList.SelectedValue));
            }
        }

        /// <summary>
        /// Gets the link to the login page
        /// </summary>
        protected string GetLoginUrl()
        {
            PageReference loginPageRef = CurrentPage["LoginPage"] as PageReference;
            if (loginPageRef != null)
            {
                return Server.HtmlEncode(DataFactory.Instance.GetPage(loginPageRef).LinkURL);
            }
            LoginView.Visible = false;
            return string.Empty;
        }
    }
}
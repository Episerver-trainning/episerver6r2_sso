/*
Copyright (c) EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using System;
using System.Web.UI;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.AlloyTech.Pages
{
	/// <summary>
	/// The search page presents the search result from both the quick search user control
	/// and the search function on this page. The search can be restricted to a page branch by 
	/// setting the "SearchRoot" property.
	/// </summary>
    public partial class SearchPage : TemplatePage
    {
        /// <summary>
        /// When page is loaded on a get request (IsPostBack == false), the querystring 
        /// variable "quicksearchquery" is used to prepopulate the search textbox. 
        /// This value will automatically be used by the SearchDataSource on its initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            Form.DefaultButton = SearchButton.UniqueID;

            //Trying to find the QuickSearch user control. If found make it invisible.
            Control control = Form.FindControl("QuickSearch");
            if (control != null)
            {
                control.Visible = false;
            }

            if (!IsPostBack)
            {
                string query = Request.QueryString["quicksearchquery"];
                if (!String.IsNullOrEmpty(query))
                {
                    SearchText.Text = query;
                    SearchResult.Visible = true;
                }
            }

            SearchDataSource.SearchLocations = "~/Global/,~/Documents/,~/PageFiles/";
            SearchDataSource.Selected += new DataSourceStatusEventHandler(HandleErrors);
            SearchDataSource.Selected += new DataSourceStatusEventHandler(HandleEmptyResult);
        }

        /// <summary>
        /// Search button handler. When Search button is clicked we need to rebind the repeater for the search to be executed.
        /// </summary>
        /// <param name="sender">The instance that fired the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SearchClick(object sender, EventArgs e)
        {
            SearchResult.Visible = true;
            NoSearchResult.Visible = false;
            SearchResult.DataBind();
        }

        /// <summary>
        /// This method looks for exceptions in the DataSourceStatusEventArgs parameter and invalidates a validator with the corresponding exception message.
        /// </summary>
        /// <param name="sender">The sender (the SearchDataSource).</param>
        /// <param name="e">The <see cref="EPiServer.Web.WebControls.DataSourceStatusEventArgs"/> instance containing the event data.</param>
        protected void HandleErrors(object sender, EPiServer.Web.WebControls.DataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                CustomValidator1.ErrorMessage = e.Exception.Message;
                CustomValidator1.Text = e.Exception.Message;
                CustomValidator1.IsValid = false;
                e.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// This method checks if the search resulted in any hits. If not it hides the searchResult and displays a "no results" message. 
        /// </summary>
        /// <param name="sender">The sender (the SearchDataSource).</param>
        /// <param name="e">The <see cref="EPiServer.Web.WebControls.DataSourceStatusEventArgs"/> instance containing the event data.</param>
        protected void HandleEmptyResult(object sender, DataSourceStatusEventArgs e)
        {
            if (e.AffectedRows < 1)
            {
                SearchResult.Visible = false;
                NoSearchResult.Visible = true;
            }
        }

        /// <summary>
        /// Gets the link URL with language parameter.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The url to the page.</returns>
        protected static string GetLinkUrlWithLanguage(PageData page)
        {
            if (page == null)
            {
                return String.Empty;
            }

            return UriSupport.AddLanguageSelection(page.LinkURL, page.LanguageBranch);
        }

    }
}

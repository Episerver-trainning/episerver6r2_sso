#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Web;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Templates.AlloyTech;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// In what mode the news list will be displayed
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Standard view
        /// </summary>
        View,
        /// <summary>
        /// Adding a news item
        /// </summary>
        Add,
        /// <summary>
        /// Editing an existing new 
        /// </summary>
        Edit
    };

    /// <summary>
    /// Shows the current selected news item.
    /// List all news items.
    /// </summary>
    public partial class NewsListPage : WorkroomPageBase
    {
        private PageData _selectedNewsItem;
        private DisplayMode _currentDisplayMode = DisplayMode.View;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                NewsSectionNameTextBox.Text = CurrentPage.PageName;
            }
        }

        /// <summary>
        /// Hides the edit and delete buttons if the current user hasn't got at least Modify membership.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
            {
                DeleteButton.OnClientClick = string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete"));
                DeleteNewsSectionLinkButton.OnClientClick = string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete"));
                NewsListUnit.DataBind();
            }

            UpdateDisplayTexts(NewsListUnit.SelectedPageId);
            SetDisplayMode();

        }

        /// <summary>
        /// Updates the PageLink for MainBody and MainIntro.
        /// </summary>
        /// <param name="selectedNewsPageRef">A PageReference to the page which we want to list data from.</param>
        public void UpdateDisplayTexts(PageReference selectedNewsPageRef)
        {
            PageNameProp.PageLink = MainBodyProp.PageLink = selectedNewsPageRef;
            PageNameProp.DataBind();
            MainBodyProp.DataBind();
        }

        /// <summary>
        /// Toggles the visiblility of the create and display panels and buttons.
        /// </summary>
        private void SetDisplayMode()
        {
            switch (CurrentDisplayMode)
            {
                case DisplayMode.Add:
                    ViewPanel.Visible = false;
                    EditPanel.Visible = true;
                    NewsListUnit.AddButtonVisible = false;
                    EditPanelCaption.InnerHtml = Translate("/workroom/newslistpage/createheading");
                    EditPageName.Text = String.Empty;
                    EditMainBody.Text = String.Empty;
                    EditNewsSectionButtons.Visible = false;
                    if (GetChildren(CurrentPageLink).Count==0)
                    {
                        NewsListUnit.Visible = false;
                    }
                    break;

                case DisplayMode.Edit:
                    ViewPanel.Visible = false;
                    EditPanel.Visible = true;
                    NewsListUnit.AddButtonVisible = false;
                    EditPanelCaption.InnerHtml = Translate("/workroom/newslistpage/editheading");
                    EditNewsSectionButtons.Visible = false;
                    break;

                default:
                    NewsListUnit.Visible = true;
                    ViewPanel.Visible = true;
                    EditPanel.Visible = false;

                    //No news items available
                    if (CurrentPage.PageLink.CompareToIgnoreWorkID(NewsListUnit.SelectedPageId))
                    {
                        DeleteButton.Visible = false;
                        EditButton.Visible = false;
                    }
                    else
                    {
                        DeleteButton.Visible = true;
                        EditButton.Visible = true;
                    }
                    //Check rights
                    if (Membership.QueryDistinctMembershipLevel(GetSelectedNewsItem(NewsListUnit.SelectedPageId), MembershipLevels.Modify))
                    {
                        NewsListUnit.AddButtonVisible = true;
                    }
                    else
                    {
                        DeleteButton.Visible = false;
                        EditButton.Visible = false;
                        NewsListUnit.AddButtonVisible = false;
                    }

                    if (Membership.QueryDistinctMembershipLevel(CurrentPage, MembershipLevels.Administer))
                    {
                        EditNewsSectionButtons.Visible = true;
                        EditNewsSectionNamePanel.Visible = true;
                    }

                    break;
            }
        }

        #region Button click event handlers

        /// <summary>
        /// Handles the Click event of the CreateNewsItem button.
        /// Creates a page of the type "[AlloyTech] News item" and places it below the current page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                PageData page;
                switch (CurrentDisplayMode)
                {
                    case DisplayMode.Add:
                        page = DataFactory.Instance.GetDefaultPageData(CurrentPageLink, NewsItemPageTypeName);
                        page.StartPublish = DateTime.Now.AddMinutes(-1);
                        break;
                    case DisplayMode.Edit:
                        page = GetSelectedNewsItem(NewsListUnit.SelectedPageId).CreateWritableClone();
                        break;
                    default:
                        Page.Validators.Add(new StaticValidator("Unsupported display mode"));
                        return;
                }
                try
                {
                    page.PageName = EditPageName.Text.ToSafeString();
                    page["MainBody"] = EditMainBody.Text.ToSafeString();

                    NewsListUnit.SelectedPageId = DataFactory.Instance.Save(page, SaveAction.Publish);
                }
                catch (Exception ex)
                {
                    Page.Validators.Add(new StaticValidator(ex.Message));
                    return;
                }

                CurrentDisplayMode = DisplayMode.View;
                NewsListUnit.DataBind();
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteNewsItem button.
        /// Deletes the page for the selected news item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Click(object sender, EventArgs e)
        {
            DataFactory.Instance.Delete(MainBodyProp.PageLink, true);

            NewsListUnit.SelectedPageId = null;
            //Redirect to self to reload everything after a delete.
            Response.Redirect(CurrentPage.LinkURL, true);
        }

        /// <summary>
        /// Handles the Click event of the Add button of the news-list.
        /// View the add/edit form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Add_Click(object sender, EventArgs e)
        {
            CurrentDisplayMode = DisplayMode.Add;
        }

        /// <summary>
        /// Handles the Click event of the EditNewsItem button.
        /// Loads the proper values to the editable boxes, and toggles visiblility between the edit panel and the display panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Edit_Click(object sender, EventArgs e)
        {
            EditPageName.Text = (string)PageNameProp.InnerProperty.Value;
            EditMainBody.Text = (string)MainBodyProp.InnerProperty.Value;
            CurrentDisplayMode = DisplayMode.Edit;
        }

        /// <summary>
        /// Handles the Click event of the CancelEdit Button.
        /// Toggles visiblility between the edit panel and the display panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            CurrentDisplayMode = DisplayMode.View;
        }

        /// <summary>
        /// Handles the Click event of the DeleteNewsSection button.
        /// Deletes current news section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteNewsSection_Click(object sender, EventArgs e)
        {
            string redirectUrl = DataFactory.Instance.GetPage(CurrentPage.ParentLink).LinkURL;
            DataFactory.Instance.Delete(CurrentPage.PageLink, true);
            Response.Redirect(redirectUrl);
        }

        /// <summary>
        /// Handles the Click event of the SaveNewsSectionName button.
        /// Saves new name for the current news section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveNewsSectionNameButton_Click(object sender, EventArgs e)
        {
            PageData page = CurrentPage.CreateWritableClone();
            page["PageName"] = NewsSectionNameTextBox.Text.ToSafeString();
            DataFactory.Instance.Save(page, SaveAction.Publish);
            Response.Redirect(page.LinkURL);
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Gets the selected news item.
        /// </summary>
        private PageData GetSelectedNewsItem(PageReference selectedPageRef)
        {
            if (_selectedNewsItem == null)
            {
                _selectedNewsItem = DataFactory.Instance.GetPage(selectedPageRef);
            }
            return _selectedNewsItem;
        }

        /// <summary>
        /// Set what parts of the page to show.
        /// </summary>
        public DisplayMode CurrentDisplayMode
        {
            get
            {
                if (ViewState["CurrentDisplayMode"] != null)
                {
                    _currentDisplayMode = (DisplayMode)ViewState["CurrentDisplayMode"];
                }
                return _currentDisplayMode;
            }
            set { ViewState["CurrentDisplayMode"] = _currentDisplayMode = value; }
        }

        #endregion
    }
}

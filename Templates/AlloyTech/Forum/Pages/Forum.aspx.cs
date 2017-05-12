#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:
 
 * Performance limitation
  - The forum templates have been tested with a setup of ~45 000 pages. They were set up in the following structure:
    1 root forum with 5 sub forums. Each sub forum has 1000 threads with 5 replies, and 2 threads with 1000 replies.
 
    That setup was tested on a development environment and showed very good performance. 
    Larger sites has not been tested and if such are excpected further test are needed.
 
 * Usability limitations
  - The moderation functionality won't work without JavaScript.
  
 * Multi language limitations
  - Due to the nature of a forum, the templates have not been designed and tested with multi-lingual considerations in mind.

*/
#endregion

using System;
using System.Linq;
using System.Text;
using System.Web;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Shell.Web;
using EPiServer.Templates.AlloyTech.Forum.Core;
using EPiServer.Templates.AlloyTech.Units.Static;
using EPiServer.Web;

namespace EPiServer.Templates.AlloyTech.Forum.Pages
{
    /// <summary>
    /// A forum page that lists threads and sub fourms.
    /// </summary>
    public partial class Forum : ForumPageBase
    {
        /// <summary>
        /// A maximal URL lenght for the correct link value
        /// </summary>
        private const int MAX_URL_LENGHT = 255;

        /// <summary>
        /// Different views that this page can be viewed in.
        /// </summary>
        enum ViewMode
        {
            /// <summary>
            /// The regular view that lists pages.
            /// </summary>
            ListView,
            /// <summary>
            /// The view to add a new sub forum.
            /// </summary>
            AddForumView,
            /// <summary>
            /// The view to add a new sub thread.
            /// </summary>
            AddThreadView,
            /// <summary>
            /// The view for when the page is not setup correctly.
            /// </summary>
            ErrorView
        }

        bool _evenForum = true;
        bool _evenThread = true;

     
      
        /// <summary>
        /// Enables moderator functions if the user is a moderator.
        /// Enable the possibility to add a thread if the user is a poster.
        /// Adds sticky threads to the top of the thread list.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsModerator && !IsStartPage)
            {
                DeleteForumButton.Visible = true;
                DeleteForumButton.OnClientClick = string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete"));
            }

            ForumList.DataBind();

            // Add sticky threads.
            PageReference stickyThreadContainer = (PageReference)CurrentPage["StickyThreadContainer"];
            if (!PageReference.IsNullOrEmpty(stickyThreadContainer))
            {
                ThreadList.DataSource = DataFactory.Instance.GetChildren(stickyThreadContainer);
            }
            else if (CurrentPage.Status == VersionStatus.Published)
            {
                // If there is no container pages, and we are viewing the published version, show the error panel.
                SwitchView(ViewMode.ErrorView);
                return;
            }

            if (!IsPostBack)
            {
                SwitchView(ViewMode.ListView);
            }
        }

        /// <summary>
        /// returns if this page is start page of forum
        /// </summary>
        private bool IsStartPage
        {
            get
            {
                return !DataFactory.Instance.GetPage(CurrentPage.ParentLink).PageTypeName.Equals(Manager.ForumPageTypeName);
            }
        }

        /// <summary>
        /// Switches what is displayed in the forum page.
        /// <remarks>
        /// The different options are either the regular listing view, the view to add a new thread and the view to add a new sub forum.
        /// </remarks>
        /// </summary>
        /// <param name="mode">The view you want to switch to.</param>
        private void SwitchView(ViewMode mode)
        {
            switch (mode)
            {
                case ViewMode.ListView:
                    ListThreadPanel.Visible = (bool)(CurrentPage["AllowThreads"] ?? false) || ThreadList.OfType<PageData>().Count() > 0;
                    AddThreadButton.Visible = ((bool)(CurrentPage["AllowThreads"] ?? false)) && IsPoster;
                    ListForumPanel.Visible = (bool)(CurrentPage["AllowForums"] ?? false) || ForumList.OfType<PageData>().Count() > 0;
                    AddForumButton.Visible = ((bool)(CurrentPage["AllowForums"] ?? false)) && IsModerator;
                    InformationPanel.Visible = !String.IsNullOrEmpty((string)CurrentPage["MainBody"]);
                    ErrorPanel.Visible = NewThreadPanel.Visible = NewForumPanel.Visible = false;
                    ButtonPanel.Visible = true;
                    break;
                case ViewMode.AddThreadView:
                    TitleTextBox.MaxLength = MAX_URL_LENGHT - Request.Path.Length;
                    ListThreadPanel.Visible = ListForumPanel.Visible = NewForumPanel.Visible = InformationPanel.Visible = false;
                    NewThreadPanel.Visible = true;
                    ButtonPanel.Visible = false;
                    break;
                case ViewMode.AddForumView:
                    ForumTitleTextBox.MaxLength = 255 - Request.Path.Length;
                    ListThreadPanel.Visible = ListForumPanel.Visible = NewThreadPanel.Visible = InformationPanel.Visible = false;
                    NewForumPanel.Visible = true;
                    ButtonPanel.Visible = false;
                    break;
                case ViewMode.ErrorView:
                    ListThreadPanel.Visible = ListForumPanel.Visible = NewThreadPanel.Visible = InformationPanel.Visible = false;
                    ButtonPanel.Visible = false;
                    ErrorPanel.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Gets a display string containg the changed date and a link to the last updated thread in a forum.
        /// </summary>
        /// <param name="forum">The forum page.</param>
        /// <returns>A string containing the date the thread was changed and a link to the thread.</returns>
        protected string GetLastUpdatedThreadString(PageData forum)
        {
            StringBuilder returnValue = new StringBuilder();

            PageDataCollection threads = Manager.GetLatestUpdatedThreads(forum.PageLink, 1);

            if (threads.Count > 0)
            {
                PageData thread = threads[0];
                returnValue.Append(GetDateString(thread));
                returnValue.Append(" - ");
                returnValue.Append(CreatePageLink(thread, 25, null));
            }
            else
            {
                returnValue.Append(GetDateString(forum));
            }

            return returnValue.ToString();
        }

        /// <summary>
        /// Creates a link for a thread page. Also has a max length for how big part of the PageName will be used in the link.
        /// </summary>
        /// <param name="page">The thread page.</param>
        /// <param name="previewLength">Length of the page name.</param>
        /// <param name="cssClass">Any CSS classes that should be added to the link.</param>
        /// <returns>A string containing the link.</returns>
        protected string CreatePageLink(PageData page, int previewLength, string additionalCssClasses)
        {
            string cssClass = String.Empty;
            PageReference reference = (PageReference)ForumStartPage["StickyThreadContainer"];
            if (!PageReference.IsNullOrEmpty(reference) && page.ParentLink.CompareToIgnoreWorkID(reference))
            {
                cssClass = "sticky";
            }
            if (page["IsLocked"] != null)
            {
                cssClass += "locked";
            }
            if (!String.IsNullOrEmpty(additionalCssClasses))
            {
                cssClass += " " + additionalCssClasses;
            }

            string pageName = page.PageName;
            if (previewLength > 3)
            {
                pageName = pageName.Ellipsis(previewLength);
            }

            return String.Format("<a class=\"{0}\" href=\"{1}\">{2}</a>", cssClass, page.LinkURL, Server.HtmlEncode(pageName));
        }

        /// <summary>
        /// Converts the bool to a string value used as a CSS class.
        /// </summary>
        /// <param name="even">The bool to evaluate.</param>
        /// <returns>A string containing either "even" or "uneven".</returns>
        protected static string GetStyle(bool even)
        {
                return even ? "even" : "uneven";
        }

        /// <summary>
        /// Gets the a string containing the date the thread was latest changed.
        /// </summary>
        /// <param name="thread">A PageData for the thread.</param>
        /// <returns>A string containing the date the thread was latest changed.</returns>
        protected string GetDateString(PageData thread)
        {
            TimeSpan diff = DateTime.Now.Date - thread.Changed.Date;
            string returnValue;

            switch (diff.Days)
            {
                case 0:
                    returnValue = String.Format("{0} {1}", Translate("/forum/today"), thread.Changed.ToFormattedTime());
                    break;
                case 1:
                    returnValue = String.Format("{0} {1}", Translate("/forum/yesterday"), thread.Changed.ToFormattedTime());
                    break;
                default:
                    returnValue = thread.Changed.ToFormattedDateAndTime();
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// Generates an absolute link to a thread with an anchor to the latest reply in that thread. 
        /// </summary>
        /// <remarks>
        /// Used by the RSS/Atom feed.
        /// </remarks>
        /// <param name="page">A PageData to create an absolute link from.</param>
        /// <returns>An absolute link to page.</returns>
        protected override string CreateExternalLink(PageData page)
        {
            UrlBuilder url = new UrlBuilder(UriSupport.Combine(Settings.Instance.SiteUrl.ToString(), page.LinkURL));

            url.Fragment = "Reply" + Manager.GetLatestReply(page.PageLink).PageName;

            if (UrlRewriteProvider.IsFurlEnabled)
            {
                Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.UTF8Encoding.UTF8);
                return url.ToString();
            }
            else
            {
                return url.ToString();
            }
        }

        /// <summary>
        /// It will return the 10 latest updated threads in this and any subforum.
        /// </summary>
        /// <value>The PageDataCollection used for the RSS/Atom feeds.</value>
        protected override PageDataCollection FeedSource
        {
            get
            {
                return Manager.GetLatestUpdatedThreads(CurrentPageLink, FeedCount);
            }
        }

        /// <summary>
        /// Gets all the forums that are children to this forum.
        /// </summary>
        /// <remarks>
        /// Only returns forums that has PageVisibleInMenu set to true.
        /// </remarks>
        /// <value>A PageDataCollection containg sub forums.</value>
        protected PageDataCollection ForumDataSource
        {
            get
            {
                PageDataCollection forums = DataFactory.Instance.GetChildren(CurrentPageLink);
                forums = FilterForVisitor.Filter(forums);
                new FilterCompareTo("PageVisibleInMenu", "true").Filter(forums);
                return forums;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an even thread in the thread list.
        /// </summary>
        /// <value><c>true</c> if even thread in list; otherwise, <c>false</c>.</value>
        public bool EvenThread
        {
            get 
            {
                _evenThread = !_evenThread;
                return _evenThread; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an even forum in the forum list.
        /// </summary>
        /// <value><c>true</c> if even forum in list; otherwise, <c>false</c>.</value>
        public bool EvenForum
        {
            get
            {
                _evenForum = !_evenForum;
                return _evenForum;
            }
        }

        #region Button event handlers

        /// <summary>
        /// Creates a new thread. Switches back to regular view mode.
        /// </summary>
        protected void ThreadSubmit_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }

            if (string.IsNullOrEmpty(HttpUtility.HtmlDecode(Manager.RemoveAllTags(ContentTextBox.Text)).Trim()))
            {
                RequiredFieldValidatorContentTextBox.IsValid = false;
                return;
            }

            Manager.CreateThread(CurrentPage, TitleTextBox.Text, ContentTextBox.Text);
            _evenThread = true;
            SwitchView(ViewMode.ListView);
        }

        /// <summary>
        /// Creates a new sub forum. Switches back to regular view mode.
        /// </summary>
        protected void ForumSubmit_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }
            Manager.CreateForum(CurrentPageLink, ForumTitleTextBox.Text, AllowThreadsCheckBox.Checked, AllowForumsCheckBox.Checked, IconDropDown.SelectedValue);
            _evenForum = true;
            ForumList.DataBind();
            Submenu submenu = Page.FindControl<Submenu>();
            if (submenu != null)
            {
                submenu.DataBind();
            }
            SwitchView(ViewMode.ListView);
        }

        /// <summary>
        /// Switches back to regular view mode.
        /// </summary>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            SwitchView(ViewMode.ListView);
        }

        /// <summary>
        /// Deletes this forum and redirects to the parent page.
        /// </summary>
        protected void Delete_Click(object sender, EventArgs e)
        {
            Manager.DeleteForum(CurrentPageLink);

            Response.Redirect(DataFactory.Instance.GetPage(CurrentPage.ParentLink).LinkURL, true);
        }

        /// <summary>
        /// Clears the text boxes for the add thread view. Switches over to the add thread view.
        /// </summary>
        protected void AddThread_Click(object sender, EventArgs e)
        {
            TitleTextBox.Text = String.Empty;
            ContentTextBox.Text = String.Empty;
            SwitchView(ViewMode.AddThreadView);
        }

        /// <summary>
        /// Clears the text boxes for the add forum view. Switches over to the add thread view.
        /// </summary>
        protected void AddForum_Click(object sender, EventArgs e)
        {
            ForumTitleTextBox.Text = String.Empty;
            SwitchView(ViewMode.AddForumView);
        }

        /// <summary>
        /// Sets up the thread containers necessary for a forumpage
        /// </summary>
        protected void SetupStructure_Click(object sender, EventArgs e)
        {
            Manager.SetupForumStructure(CurrentPageLink);
            Response.Redirect(CurrentPage.LinkURL, true);
        }

        #endregion
        
    }
}

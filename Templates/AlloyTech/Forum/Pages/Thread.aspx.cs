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
 
 * Usability limitations
  - Due to bug #8821, on-page edit does not work.

*/
#endregion

using System;
using System.Web.UI.WebControls;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.AlloyTech.Forum.Core;
using EPiServer.Web;

namespace EPiServer.Templates.AlloyTech.Forum.Pages
{
    /// <summary>
    /// Renders a thread in a forum.
    /// </summary>
    public partial class Thread : ForumPageBase
    {
        private bool _isFirstReply = true;

        /// <summary>
        /// Shows the appropriate panels based on the logged on users.
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsModerator)
            {
                ModeratorPanel.Visible = true;

                DeleteButton.OnClientClick = ConfirmScript;
            }

            if (IsPoster)
            {
                ReplyPanel.Visible = true;
            }

            if (CurrentPage["IsLocked"] != null)
            {
                HyperlinkAddReply.Visible = false;
                ReplyPanel.Visible = false;
                LockButton.Text = Translate("/forum/thread/unlock");
            }
            else
            {
                LockButton.Text = Translate("/forum/thread/lock");
            }

            if (IsSticky)
            {
                StickyButton.Text = Translate("/forum/thread/unsticky");
            }
            else
            {
                StickyButton.Text = Translate("/forum/thread/sticky");
            }
        }

        /// <summary>
        /// Gets a confirmation script.
        /// </summary>
        /// <returns>A string containing a javascript that shows a confirm message.</returns>
        protected string ConfirmScript
        {
            get
            {
                return string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete"));
            }
        }

        /// <summary>
        /// Create a string with an absolute link to the page. Checks if furl is enabled and rewrites the link if appropriate.
        /// Adds an anchor to the latest reply.
        /// </summary>
        /// <param name="page">A PageData to create an absolute link from.</param>
        /// <returns>An absolute link to page.</returns>
        protected override string CreateExternalLink(PageData page)
        {
            UrlBuilder url = new UrlBuilder(UriSupport.Combine(Settings.Instance.SiteUrl.ToString(), GetPage(page.ParentLink).LinkURL));

            url.Fragment = "Reply" + page.PageName;

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
        /// The collection used for creating items for the feed.
        /// </summary>
        /// <value>The latest replies to this page.</value>
        protected override PageDataCollection FeedSource
        {
            get
            {
                return  GetChildren(CurrentPageLink);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this thread is a sticky thread.
        /// </summary>
        /// <value><c>true</c> if this thead is sticky; otherwise, <c>false</c>.</value>
        public bool IsSticky
        {
            get
            {
                if (ForumStartPage == null)
                {
                    return false;
                }

                PageReference reference = (PageReference)ForumStartPage["StickyThreadContainer"];

                if (PageReference.IsNullOrEmpty(reference))
                {
                    return false;
                }

                return CurrentPage.ParentLink.CompareToIgnoreWorkID(reference);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this reply is first reply in the thread.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this reply is first reply in the thread; otherwise, <c>false</c>.
        /// </value>
        protected bool IsFirstReply
        {
            get
            {
                if (_isFirstReply)
                {
                    _isFirstReply = false;
                    return true;
                }
                return false;
            }
        }

        #region Button event handlers

        /// <summary>
        /// Creates a reply page below the current page.
        /// </summary>
        protected void Reply_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }
            if (System.Web.HttpUtility.HtmlDecode(Manager.RemoveAllTags(Body.Text)).Trim() == String.Empty)
            {
                ReplyBodyRequiredFieldValidator.IsValid = false;
                return;
            }

            if (Manager.CreateReply(CurrentPageLink, ForumStartPage, Body.Text))
            {
                Response.Redirect(CurrentPage.LinkURL);
            }
            Body.Text = String.Empty;
        }

        /// <summary>
        /// Deletes this thread page. Redirects to the forum start page.
        /// </summary>
        protected void Delete_Click(object sender, EventArgs e)
        {
            string forumStartPageUrl = ForumStartPage.LinkURL;
            Manager.DeleteThread(CurrentPageLink);
            Response.Redirect(forumStartPageUrl, true);
        }

        /// <summary>
        /// Toggels the value of the property IsLocked on the thread page.
        /// </summary>
        protected void Lock_Click(object sender, EventArgs e)
        {
            PageData page = CurrentPage.CreateWritableClone();
            page["IsLocked"] = page["IsLocked"] != null ? false : true;
            DataFactory.Instance.Save(page, SaveAction.Publish);
            Response.Redirect(CurrentPage.LinkURL, true);
        }

        /// <summary>
        /// If the thread is sticky it moves the page back to the active thread container.
        /// Otherwise, it moves the thread page to the sticky thread container.
        /// </summary>
        protected void Sticky_Click(object sender, EventArgs e)
        {
            string temp = IsSticky ? "ActiveThreadContainer" : "StickyThreadContainer";
            PageReference targetPageRef = (PageReference)ForumStartPage[temp];

            if (PageReference.IsNullOrEmpty(targetPageRef))
            {
                return;
            }

            DataFactory.Instance.Move(CurrentPageLink, targetPageRef);
            // Since we moved the page we are currently loading, we need to redirect to reload it from it's new place.
            Response.Redirect(CurrentPage.LinkURL, true);
        }

        /// <summary>
        /// Deletes a reply page. Reloads the data for the list of replies.
        /// </summary>
        protected void DeleteReply_Command(Object sender, CommandEventArgs e)
        {
            Manager.DeleteReply(PageReference.Parse((string)e.CommandArgument), CurrentPageLink);

            PostList.DataBind();
        }

        #endregion
    }
}

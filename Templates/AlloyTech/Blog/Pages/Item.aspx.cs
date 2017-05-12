#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Usability limitations
On page edit is disabled.
Added comment only shows up in list after reload.

- Multi language limitations
Due to the nature of a blog, the template is not designed and tested with multi languge considerations in mind.

*/
#endregion

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using EPiServer.Security;
using EPiServer.Shell.Web;
using EPiServer.Templates.AlloyTech.Forum.Core;

namespace EPiServer.Templates.AlloyTech.Blog.Pages
{
    /// <summary>
    /// Template used for blog item page type.
    /// </summary>
    public partial class Item : BlogPageBase
    {
        private PageDataCollection _tags;

        /// <summary>
        /// Checks if comments and/or captcha should be displayed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            bool allowAnonymousComments = BlogStart[BlogUtility.DisableAnonymousCommentsPropertyName] == null;
            bool isLoggedIn = Context.User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                CommentArea.Visible = true;
                Captcha.Visible = false;
                CommentName.Text = EPiServerProfile.Current.DisplayName;
            }
            else if (allowAnonymousComments)
            {
                CommentArea.Visible = true;
                Captcha.Visible = true;
            }
            else
            {
                CommentArea.Visible = false;
                Captcha.Visible = false;
            }

            DataBind();
        }


        /// <summary>
        /// Gets the fixed main body.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected static string GetFixedMainBody(PageData page)
        {
            string mainBody = page["MainBody"] as string;
            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            Regex pRegex = new Regex("<p(\\s{1,}.*)*>.*", options);
            Regex imgRegex = new Regex("<img\\s{1,}", options);
            MatchCollection matches = pRegex.Matches(mainBody);
            if ((matches.Count > 0 && imgRegex.IsMatch(matches[0].Value)) || mainBody.StartsWith("<ul>", StringComparison.OrdinalIgnoreCase)
                || mainBody.StartsWith("<ol>", StringComparison.OrdinalIgnoreCase))
            {
                mainBody = "<p class=\"emptyParagraph\">&nbsp;</p>" + mainBody;
            }
            return mainBody;
        }

        /// <summary>
        /// The source for RSS/Atom feeds, which in this case is the comment pages.
        /// </summary>
        /// <value>All children to this blog item.</value>
        protected override PageDataCollection FeedSource
        {
            get
            {
                return DataFactory.Instance.GetChildren(CurrentPage.PageLink);
            }
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
                    CategoryList cl = CurrentPage["PageCategory"] as CategoryList;
                    PageReference reference = BlogStart[BlogUtility.TagContainerPropertyName] as PageReference;

                    if ((cl == null) || PageReference.IsNullOrEmpty(reference))
                    {
                        return _tags;
                    }

                    Category categoryTag;

                    PageDataCollection pdc = DataFactory.Instance.GetChildren(reference);
                    

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
        /// Creates a new comment page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PostComment_Click(object sender, EventArgs e)
        {
            // Make sure that the page, and particurlarly the captcha, is valid. 
            if (!IsValid)
            {
                return;
            }

            if (HttpUtility.HtmlDecode(Manager.RemoveAllTags(CommentText.Text)).Trim() == String.Empty)
            {
                CommentTextRequiredFieldValidator.IsValid = false;
                return;
            }

            PageData newComment = DataFactory.Instance.GetDefaultPageData(CurrentPage.PageLink, BlogUtility.CommentPageTypeName);
                
            newComment.PageName = DateTime.Now.ToString();
            newComment["MainBody"] = CommentText.Text.ToSafeString();
            newComment["Writer"] = CommentName.Text.ToSafeString().Ellipsis(255);
            newComment.VisibleInMenu = false;

            // To get the new comment into the list. If we don't do this the
            // StartPublish value will be greater than the one used for
            // the publishing-filter.
            newComment.StartPublish = DateTime.Now.AddMinutes(-1);

            DataFactory.Instance.Save(newComment, DataAccess.SaveAction.Publish, AccessLevel.NoAccess);

            CommentText.Text = String.Empty;

            Captcha.Refresh();
        }

        /// <summary>
        /// Gets a value indicating whether to show the bloggers details. 
        /// </summary>
        protected bool ShowDetails
        {
            get
            {
                return BlogStart["Details"] != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show who has posted the item. 
        /// </summary>
        protected bool ShowPostedBy
        {
            get
            {
                return BlogStart["Writer"] != null;
            }
        }
    }
}

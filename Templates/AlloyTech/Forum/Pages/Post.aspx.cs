#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using EPiServer.Templates.AlloyTech.Forum.Core;

namespace EPiServer.Templates.AlloyTech.Forum.Pages
{
    /// <summary>
    /// Page templete used by a forum post(reply).
    /// Doesn't show any data, only redirects to the thread(parent) page with an anchor to the correct post.
    /// </summary>
    public partial class Post : ForumPageBase
    {
        /// <summary>
        /// Redirects to the thread(parent) page with an anchor to the correct post.
        /// </summary>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            UrlBuilder url = new UrlBuilder(DataFactory.Instance.GetPage(CurrentPage.ParentLink).LinkURL);

            url.Fragment = "Reply" + CurrentPage.PageName;

            Response.Redirect(url.ToString());
        }
    }
}

#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.UI.HtmlControls;
using System.Xml;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.Security;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Web;

namespace EPiServer.Templates.AlloyTech.Forum.Core
{
    /// <summary>
    /// Base class for pages with forum functionality
    /// </summary>
    public class ForumPageBase : TemplatePage
    {
        private PageReference _forumStartReference;
        private PageData _forumStartPage;
        private int _feedCount;

        /// <summary>
        /// Checks the query string for a feed parameter. If the feed prameter equals "RSS" we generate an RSS 2.0 feed instead of the normal display.
        /// If the feed equals "Atom" we generate a Atom 1.0 feed instead of the normal display.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnPreInit(EventArgs e)
        {
            if (Request["feed"] != null && Request["feed"].Equals("RSS", StringComparison.OrdinalIgnoreCase))
            {
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "text/xml";
                WriteRss();
                Response.End();
            }
            else if (Request["feed"] != null && Request["feed"].Equals("Atom", StringComparison.OrdinalIgnoreCase))
            {
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "text/xml";
                WriteAtom();
                Response.End();
            }
        }

        /// <summary>
        /// Adds links for the RSS and Atom feed to the head section to make the avaliable for subscription.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            HtmlLink link = new HtmlLink();
            string url = CreateExternalLinkInternal(CurrentPage);
            string separator = "?";

            if (url.Contains("?"))
            {
                separator = "&";
            }

            link.Href = String.Format("{0}{1}feed=RSS", url, separator);
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("type", "application/rss+xml");
            link.Attributes.Add("title", String.Format("RSS {0} ", CurrentPage.PageName));
            Header.Controls.Add(link);

            link = new HtmlLink();
            link.Href = String.Format("{0}{1}feed=Atom", url, separator);
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("type", "application/atom+xml");
            link.Attributes.Add("title", String.Format("Atom {0} ", CurrentPage.PageName));
            Header.Controls.Add(link);

            WorkroomPageBase.InitializeWorkroomPage(this);
            base.OnLoad(e);
        }

        /// <summary>
        /// Writes the RSS feed.
        /// </summary>
        protected virtual void WriteRss()
        {
            XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
            string url;
            PageData page;

            writer.WriteStartDocument();
            writer.WriteStartElement("rss");
            writer.WriteAttributeString("version", "2.0");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteStartElement("channel");
            writer.WriteElementString("language", CurrentPage.LanguageID);
            writer.WriteElementString("title", CurrentPage.PageName);
            writer.WriteElementString("link", Settings.Instance.SiteUrl.ToString());
            writer.WriteElementString("description", CurrentPage["Description"] as string);
            writer.WriteElementString("ttl", "60");
            writer.WriteElementString("generator", "EPiServer CMS 6 R2");


            for (int i = 0; i < FeedSource.Count && i < FeedCount; i++)
            {
                page = FeedSource[i];
                url = CreateExternalLink(page);
                writer.WriteStartElement("item");
                writer.WriteElementString("title", page.PageName);
                writer.WriteElementString("link", url);
                writer.WriteElementString("description", TextIndexer.StripHtml((String)page["MainBody"], 250));
                writer.WriteElementString("guid", url);
                writer.WriteElementString("author", page.CreatedBy);
                writer.WriteElementString("pubDate", page.Changed.ToString("r"));

                foreach (int category in page.Category)
                {
                    writer.WriteElementString("category", page.Category.GetCategoryName(category));
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        /// <summary>
        /// Writes the atom feed.
        /// </summary>
        protected virtual void WriteAtom()
        {
            XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
            string url;
            PageData page;

            writer.WriteStartDocument();
            writer.WriteStartElement("feed");
            writer.WriteAttributeString("xmlns", "http://www.w3.org/2005/Atom");

            writer.WriteStartElement("title");
            writer.WriteAttributeString("type", "text");
            writer.WriteValue(CurrentPage.PageName);
            writer.WriteEndElement();

            writer.WriteStartElement("link");
            writer.WriteAttributeString("href", Settings.Instance.SiteUrl.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("updated", DateTime.Now.ToUniversalTime().ToString("o"));
            writer.WriteElementString("id", CreateExternalLink(CurrentPage));

            writer.WriteStartElement("generator");
            writer.WriteAttributeString("uri", Settings.Instance.SiteUrl.ToString());
            writer.WriteAttributeString("version", "1.0");
            writer.WriteValue("EPiServer CMS 6 R2");
            writer.WriteEndElement();

            for (int i = 0; i < FeedSource.Count && i < FeedCount; i++)
            {
                page = FeedSource[i];
                url = CreateExternalLink(page);
                writer.WriteStartElement("entry");
                writer.WriteElementString("title", page.PageName);

                writer.WriteStartElement("link");
                writer.WriteAttributeString("href", url);
                writer.WriteEndElement();

                writer.WriteElementString("id", url);
                writer.WriteElementString("updated", page.Changed.ToUniversalTime().ToString("o"));
                writer.WriteStartElement("author");
                writer.WriteElementString("name", page.CreatedBy);
                writer.WriteEndElement();

                writer.WriteElementString("summary", TextIndexer.StripHtml((String)page["MainBody"], 250));

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        /// <summary>
        /// Create a string with an absolute link to the page. Checks if furl is enabled and rewrites the link if appropriate.
        /// </summary>
        /// <param name="page">A PageData to create an absolute link from.</param>
        /// <returns>An absolute link to page</returns>
        protected virtual string CreateExternalLink(PageData page)
        {
            return CreateExternalLinkInternal(page);
        }

        private static string CreateExternalLinkInternal(PageData page)
        {
            UrlBuilder url = new UrlBuilder(UriSupport.Combine(Settings.Instance.SiteUrl.ToString(), page.LinkURL));
            if (UrlRewriteProvider.IsFurlEnabled)
            {
                EPiServer.Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.Encoding.UTF8);
            }
            return url.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether the current prinipal is moderator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the prinipal is moderator; otherwise, <c>false</c>.
        /// </value>
        protected bool IsModerator
        {
            get
            {
                if (ForumStartPage != null)
                {
                    return ForumStartPage.ACL.QueryDistinctAccess(AccessLevel.Administer);
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current prinipal is poster.
        /// </summary>
        /// <value><c>true</c> if the prinipal is poster; otherwise, <c>false</c>.</value>
        protected bool IsPoster
        {
            get
            {
                if (ForumStartPage != null)
                {
                    return ForumStartPage.ACL.QueryDistinctAccess(AccessLevel.Read | AccessLevel.Create | AccessLevel.Publish);
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the feed count property.
        /// </summary>
        /// <value>The amount of items listed in the feed. Can be overridden by a page/dynamic property.</value>
        protected virtual int FeedCount
        {
            get
            {
                if (CurrentPage["FeedCount"] != null)
                {
                    _feedCount = (int)CurrentPage["FeedCount"];

                }
                if (_feedCount == 0)
                {
                    _feedCount = Int32.MaxValue;
                }
                return _feedCount;
            }
        }

        /// <summary>
        /// The collection used for creating items for the feed. In this the base version, it is an empty collection. 
        /// Pages that inherit from this should override this property to define a relevant source for the feeds.
        /// </summary>
        /// <value>An empty PageDataCollection.</value>
        protected virtual PageDataCollection FeedSource
        {
            get
            {
                return new PageDataCollection();
            }
        }

        /// <summary>
        /// Gets the forum start page.
        /// </summary>
        /// <value>The forum start page.</value>
        public PageData ForumStartPage
        {
            get
            {
                if (_forumStartPage == null && !PageReference.IsNullOrEmpty(ForumStartReference))
                {
                    _forumStartPage = DataFactory.Instance.GetPage(ForumStartReference);
                }
                return _forumStartPage;
            }
        }

        /// <summary>
        /// Gets the forum start page reference.
        /// </summary>
        /// <value>The forum start page reference.</value>
        public PageReference ForumStartReference
        {
            get 
            {
                if (_forumStartReference == null)
                {
                    _forumStartReference = (PageReference)CurrentPage["ForumStart"];

                    if (String.Equals(CurrentPage.PageTypeName, Manager.ForumPageTypeName, StringComparison.OrdinalIgnoreCase) && PageReference.IsNullOrEmpty(_forumStartReference))
                    {
                        _forumStartReference = CurrentPageLink;
                    }
                }
                return _forumStartReference; 
            }
        }

    }
}

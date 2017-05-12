#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.DynamicContent;

namespace EPiServer.Templates.AlloyTech
{
    /// <summary>
    /// Extension methods for the page data class used by the public templates package.
    /// </summary>
    public static class PageDataExtensions
    {
        private const string MainBody = "MainBody";
        private const string MainIntro = "MainIntro";

        /// <summary>
        /// Gets the navigate URL to the news item
        /// </summary>
        /// <param name="newsItemPageData">News item PageData.</param>
        /// <returns>
        /// A link to the newslist page page with the id of the selected page as a query string parameter.
        /// </returns>
        public static string GetNewsItemNavigateUrl(this PageData newsItemPageData)
        {
            UrlBuilder url = new UrlBuilder(DataFactory.Instance.GetPage(newsItemPageData.ParentLink).LinkURL);
            url.QueryCollection["selectednewspage"] = newsItemPageData.PageLink.ToString();
            
            return (string)url;
        }

        /// <summary>
        /// The preview text is primarily created from the MainIntro property if it exists,
        /// otherwise parts of the MainBody property are being used.
        /// If neither a MainIntro nor a MainBody property exists, the preview will not be shown.
        /// The length of the preview text is defined in <code>PreviewTextLength</code>
        /// </summary>
        /// <param name="page">The page to extract the preview text from.</param>
        /// <param name="previewTextLength">Length of the preview text.</param>
        /// <returns>
        /// Returns the preview text for the specified PageData
        /// </returns>
        public static string GetPreviewText(this PageData page, int previewTextLength)
        {
            if (previewTextLength <= 0)
            {
                return string.Empty;
            }

            if (page.Property[MainIntro] != null && page.Property[MainIntro].ToString().Length > 0)
            {
                return StripPreviewText(page.Property[MainIntro].ToWebString(), previewTextLength);
            }
            
            string previewText = String.Empty;

            if (page.Property[MainBody] != null)
            {
                previewText = page.Property[MainBody].ToWebString();
            }
             
            if (String.IsNullOrEmpty(previewText))
            {
                return string.Empty;
            }

            //If the MainBody contains DynamicContents, replace those with an empty string
            StringBuilder regexPattern = new StringBuilder(@"<span[\s\W\w]*?classid=""");
            regexPattern.Append(DynamicContentFactory.Instance.DynamicContentId.ToString());
            regexPattern.Append(@"""[\s\W\w]*?</span>");
            previewText = Regex.Replace(previewText, regexPattern.ToString(), string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return TextIndexer.StripHtml(previewText, previewTextLength);
        }
        /// <summary>
        /// Gets the Formatted published date based on a language definition
        /// </summary>
        /// <param name="page">The page to get the date from</param>
        /// <returns></returns>
        public static string GetFormattedPublishDate(this PageData page)
        {
            return page.StartPublish.ToFormattedDate();
        }

        /// <summary>
        /// Gets the Formatted published date based on a language definition
        /// </summary>
        /// <param name="page">The page to get the date from</param>
        /// <returns></returns>
        public static string GetFormattedPublishDateWithTime(this PageData page)
        {
            return page.StartPublish.ToFormattedDateAndTime();
        }

        /// <summary>
        /// Strips a text to a given length without splitting the last word.
        /// </summary>
        /// <param name="previewText">The string to shorten</param>
        /// <param name="maxLength">The max lenth of the returned string</param>
        /// <returns>A shortened version of the given string</returns>
        private static string StripPreviewText(string previewText, int maxLength)
        {
            if (previewText.Length <= maxLength)
            {
                return previewText;
            }
            previewText = previewText.Substring(0, maxLength);
            // The maximum number of characters to cut from the end of the string.
            int maxCharCut = (previewText.Length > 15 ? 15 : previewText.Length - 1);
            int previousWord = previewText.LastIndexOfAny(new char[] { ' ', '.', ',', '!', '?' }, previewText.Length - 1, maxCharCut);
            if (previousWord <= 0)
            {
                previewText = previewText.Substring(0, previousWord);
            }
            return previewText + " ...";
        }
    }
}

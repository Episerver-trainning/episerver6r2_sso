#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.PlugIn;
using EPiServer.Templates.AlloyTech.Blog.PropertyControls;


namespace EPiServer.Templates.AlloyTech.Blog
{
    /// <summary>
    /// The property type for blog tags.
    /// </summary>
    [PageDefinitionTypePlugIn]
    public class PropertyBlogTags : PropertyString
    {
        /// <summary>
        /// Creates a new instance of PropertyBlogTagsControl.
        /// </summary>
        /// <returns>
        /// An PropertyBlogTagsControl that is used to display a user interface for the property.
        /// </returns>
        public override IPropertyControl CreatePropertyControl()
        {
            return new PropertyBlogTagsControl();
        }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>The string.</value>
        protected override string String
        {
            get
            {
                return base.String;
            }
            set
            {
                base.String = NormalizeString(value);
            }
        }

        /// <summary>
        /// Alphabetical sort of comma separated string. Removes duplicate entries.
        /// </summary>
        /// <param name="value">Comma separated string</param>
        /// <returns>A comma separated string.</returns>
        private static string NormalizeString(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] tags = value.Split(',');

            List<string> tagList = new List<string>(tags.Length);
            string tempTag;

            foreach (string tag in tags)
            {
                tempTag = tag.Trim();
                if (!tagList.Contains(tempTag, StringComparer.OrdinalIgnoreCase) && !String.IsNullOrEmpty(tempTag))
                {
                    if (tempTag.Length > 50)
                    {
                        // Since the tags in the commaseparated string is saved as categories we throw error if the tag exceeds 50 characters.
                        // which is maximum length of a category name.
                        throw new EPiServerException(String.Format(LanguageManager.Instance.Translate("/blog/propertyblogtags/errormessage/categorytoolong"), 50));
                    }
                    tagList.Add(tempTag);
                }
            }

            tagList.TrimExcess();
            tagList.Sort(StringComparer.OrdinalIgnoreCase);

            return String.Join(", ",  tagList.ToArray());
        }
    }
}
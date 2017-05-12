using System;
using System.Text.RegularExpressions;
using EPiServer.Configuration;
using EPiServer.Framework;
using EPiServer.HtmlParsing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace EPiServer.Templates.AlloyTech
{
    /// <summary>
    /// Extension methods fot the string class used by the Alloy Tech templates.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes non allowed tags from the string. Non allowed tags are defined in the <see cref="DefaultFilterRules"/> class.
        /// In this scenario we add the ability to allow the element img to have width and height
        /// </summary>
        /// <param name="input">A string that may contain html-tags.</param>
        /// <returns>
        /// The input washed from non allowed html-tags.
        /// </returns>
        public static string ToSafeString(this string input)
        {
            using (var inputStream = new StringReader(input))
            using (var outputStrean = new StringWriter())
            {
                DefaultFilterRules defaultRules = new DefaultFilterRules(elem => GetAttributesForElement(elem), new DefaultFilterRules());
                HtmlFilter filter = new HtmlFilter(defaultRules);
                filter.FilterHtml(inputStream, outputStrean);
                return outputStrean.ToString();
            }
        }

        /// <summary>
        /// This method is used to extend DefaultFilterRules with our own implementation allowing width and height to be used on the image element
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns></returns>
        public static IEnumerable<AttributeToken> GetAttributesForElement(ElementToken element)
        {
            if (element == ElementToken.Img)
            {
                return new AttributeToken[] { AttributeToken.Width, AttributeToken.Height };
            }
            return Enumerable.Empty<AttributeToken>();
        }
    }
}

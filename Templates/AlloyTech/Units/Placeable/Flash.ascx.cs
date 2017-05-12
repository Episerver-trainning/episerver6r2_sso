#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// A slide settings for a Flash control
    /// </summary>
    [ParseChildren(true, "Text")]
    public class FlashSlide
    {
        /// <summary>
        /// Create Slide
        /// </summary>
        public FlashSlide() { }

        /// <summary>
        /// A text for the slide
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// An URL of an image for the slide
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Time to display the slide
        /// </summary>
        public double DisplayTime { get; set; }

        /// <summary>
        /// Creates an XML element from the slide
        /// </summary>
        /// <param name="document">An XML document</param>
        /// <returns>An XML element which contains a settings for the slide</returns>
        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement element = document.CreateElement("item");
            element.SetAttribute("url", ImageUrl);
            element.SetAttribute("title", Text);

            if (DisplayTime > 0)
            {
                element.SetAttribute("displayTime", DisplayTime.ToString());
            }

            return element;
        }
    }

    /// <summary>
    /// A control to display a flash movie
    /// </summary>
    [ParseChildren(true, "Slides")]
    public partial class Flash : UserControl, INamingContainer
    {
        /// <summary>
        /// A list of flash slides
        /// </summary>
        public IList<FlashSlide> Slides { get; private set; }

        /// <summary>
        /// Create control and set up default values
        /// </summary>
        public Flash()
        {
            Slides = new List<FlashSlide>();
            DisplayTime = 6;
            TransitionTime = 2;
            Width = 860;
            Height = 341;
            FallbackImageUrl = string.Empty;
            FallbackImageAlt = string.Empty;
            ConfigXml = string.Empty;
            FlashAvailable = false;
        }

        /// <summary>
        /// Time to display a slide
        /// </summary>
        public double DisplayTime { get; set; }

        /// <summary>
        /// Time for switching slides
        /// </summary>
        public double TransitionTime { get; set; }

        /// <summary>
        /// Width of a control
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Height of a control
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// A fallback image url for the control
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// A fallback image alt text for the control
        /// </summary>
        public string FallbackImageAlt { get; set; }

        /// <summary>
        /// Config XML string
        /// </summary>
        protected string ConfigXml { get; private set; }

        /// <summary>
        /// If flash is shown
        /// </summary>
        protected bool FlashAvailable { get; private set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("slideshow");

            if (DisplayTime > 0)
            {
                root.SetAttribute("displayTime", DisplayTime.ToString());
            }

            if (TransitionTime > 0)
            {
                root.SetAttribute("transitionTime", TransitionTime.ToString());
            }

            var valid = Slides.Where(slide => !string.IsNullOrEmpty(slide.ImageUrl));
            var elements = valid.Select(slide => slide.ToXmlElement(document)).ToList();
            elements.ForEach(element => root.AppendChild(element));

            document.AppendChild(root);

            ConfigXml = HttpUtility.UrlEncode(document.OuterXml);
            FlashAvailable = elements.Count > 0;
        }
    }
}
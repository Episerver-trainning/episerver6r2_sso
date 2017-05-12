#region Copyright
// Copyright © 1996-2011 EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web;
using EPiServer.Core;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    public partial class PageList : UserControlBase
    {
        private const string HeadingProperty = "Heading";
        private string _listHeading;
        private int _previewTextLength = -1;

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                EPiPageList.PageLinkProperty = PageLinkProperty;
                if (MaxCountProperty != null && CurrentPage[MaxCountProperty] != null)
                {
                    EPiPageList.MaxCount = (int)CurrentPage[MaxCountProperty];
                }
                EPiPageList.DataBind();

                // Hide the whole control if the list is empty
                this.Visible = EPiPageList.DataCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets the name of the page property pointing out the parent page of this page list
        /// </summary>
        /// <returns>The name of the page property pointing out the parent page.</returns>
        public string PageLinkProperty { get; set; }

        public string ListHeadingProperty { get; set; }

        /// <summary>
        /// Gets or sets the name of the page property that should be used to generate the date
        /// </summary>
        /// <returns>The name of the page property used for generating the date</returns>
        /// <remarks>
        /// Several built-in page properties could be used in this case, for instance PageSaved, 
        /// PageChanged, PageStartPublish, PageCreated.
        /// </remarks>
        public string DateProperty { get; set; }

        public string DisplayDateProperty { get; set; }
        /// <summary>
        /// Gets or sets the name of the page property indicating the amount of items in the PageList
        /// </summary>
        /// <returns>The name of the page property which points out the amount of items in the PageList</returns>
        public string MaxCountProperty { get; set; }

        public string PreviewTextLengthProperty { get; set; }

        public string DisplayDocumentLinkProperty { get; set; }

        public string DocumentLinkProperty { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the top hr separator.
        /// </summary>
        /// <returns>A boolen indicating if the top horisontal ruler is visible or not</returns>
        public bool ShowTopRuler { get; set; }

        /// <summary>
        /// Gets or sets the heading above the page list
        /// </summary>
        /// <returns>The heading of the page list</returns>
        /// <remarks>
        /// If this property hasn't been set, then either the Heading property or PageName 
        /// property of the target page will be used instead.
        /// </remarks>
        public string ListHeading
        {
            get
            {
                // if Heading has been set then return it as is.
                if (_listHeading != null)
                {
                    return _listHeading;
                }

                // Get heading from local property
                if (!string.IsNullOrEmpty(ListHeadingProperty) && CurrentPage[ListHeadingProperty] != null)
                {
                    return _listHeading = CurrentPage[ListHeadingProperty] as string;
                }

                // Get heading from target listing page
                PageReference listReference = null;

                if (!string.IsNullOrEmpty(PageLinkProperty))
                {
                    listReference = CurrentPage[PageLinkProperty] as PageReference;
                    if (!PageReference.IsNullOrEmpty(listReference))
                    {
                        PageData listRootPage = GetPage(listReference);
                        return _listHeading = listRootPage.PageName;
                    }
                }
                return string.Empty;
            }
            set { _listHeading = value; }
        }

        /// <summary>
        /// Gets or sets the number of characters that should be shown in the preview text (default = 0)
        /// </summary>
        /// <returns>The character count of the preview text</returns>
        /// <remarks>
        /// The preview text is primarily created from the MainIntro property if exists, 
        /// otherwise parts of the MainBody property are being used. If neither a MainIntro nor a 
        /// MainBody property exists, the preview will not be shown.
        /// </remarks>
        public int PreviewTextLength
        {
            get
            {
                if (_previewTextLength < 0 && !string.IsNullOrEmpty(PreviewTextLengthProperty) && CurrentPage[PreviewTextLengthProperty] is int)
                {
                    _previewTextLength = (int)CurrentPage[PreviewTextLengthProperty];
                }
                return _previewTextLength;
            }
            set
            {
                _previewTextLength = value;
            }
        }

        /// <summary>
        /// Gets display date propery value, if property is absent returns false.
        /// </summary>
        protected bool DisplayDate
        {
            get { return (string.IsNullOrEmpty(DisplayDateProperty) || CurrentPage[DisplayDateProperty] != null) && CurrentPage[DateProperty] != null; }
        }

        /// <summary>
        /// Gets a value indicating whether to display the document link.
        /// This will be true if the DisplayDocumentLinkProperty value is empty or set and true
        /// and 
        /// </summary>
        /// <value>
        ///   <c>true</c> if the document link should be displayed; otherwise, <c>false</c>.
        /// </value>
        protected bool DisplayDocumentLink(PageData page)
        {
            return (string.IsNullOrEmpty(DisplayDocumentLinkProperty) || CurrentPage[DisplayDocumentLinkProperty] != null) && page[DocumentLinkProperty] != null;
        }
    }
}
#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has no known limitations.
*/
#endregion

using System;
using System.Web.UI.HtmlControls;

using EPiServer.Core;
using EPiServer.Templates.AlloyTech.Units.Placeable;
using EPiServer.DataAbstraction;
using System.Globalization;

namespace EPiServer.Templates
{
    /// <summary>
    /// The default start page
    /// </summary>
	public partial class Default : TemplatePage
	{
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            SetFlash();

            //change the css class for this page type
            Form.Attributes["class"] = "StartPage";

            if (CurrentPage["MainLinksCount"] != null)
            {
                MainPageList.MaxCount = (int)CurrentPage["MainLinksCount"];
            }

            if (CurrentPage["SecondaryLinksCount"] != null)
            {
                SecondaryPageList.MaxCount = (int)CurrentPage["SecondaryLinksCount"];
            }

            DynamicProperty property = DynamicProperty.Load(CurrentPageLink, "StartPage");
            
            if (property == null)
            {
                return;
            }

            PageReference reference = property.PropertyValue.Value as PageReference;
            if (PageReference.IsNullOrEmpty(reference) || (reference.ID != CurrentPageLink.ID))
            {   
                property.PropertyValue.Value = CurrentPageLink;
                property.Save();
            }
        }

        private void SetFlash()
        {
            for (int index = 1; index <= 5; index++)
            {
                FlashSlide slide = new FlashSlide
                {
                    ImageUrl = CurrentPage["Image" + index] as string,
                    Text = CurrentPage["Text" + index] as string
                };
                Flash.Slides.Add(slide);
            }
            
            Flash.FallbackImageUrl = CurrentPage["FallbackImage"] as string;
            Flash.FallbackImageAlt = CurrentPage.PageName;
        }

        /// <summary>
        /// Determines whether date should be displayed in secondary links.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if date should be displayed in secondary links; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsDateDisplayedInSecondaryLinks()
        {
            return (bool)(CurrentPage["SecondaryLinksDisplayDate"] ?? false);
        }

	}

}

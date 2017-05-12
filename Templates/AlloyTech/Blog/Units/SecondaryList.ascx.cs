#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace EPiServer.Templates.AlloyTech.Blog.Units
{
    /// <summary>
    /// The list displayed in the left column.
    /// </summary>
    public partial class SecondaryList : UserControlBase
    {
        private PageData _blogStart;
        private bool _showDetails = true;

        protected override void OnLoad(EventArgs e)
        {

            if (!String.IsNullOrEmpty((string)BlogStart["Image"]))
            {
                BloggerImage.Visible = true;
                BloggerImage.Src = BlogStart["Image"] as string;
                BloggerImage.Alt = BlogStart["Writer"] as string;
            }
           
            base.OnLoad(e);
        }

        /// <summary>
        /// Gets or sets the blog start page.
        /// </summary>
        public PageData BlogStart
        {
            get 
            {
                if (_blogStart == null && ViewState["BlogStartPageID"] != null)
                {
                    _blogStart = DataFactory.Instance.GetPage((PageReference)ViewState["BlogStartPageID"]);
                }
                else
                {
                    if (CurrentPage[BlogUtility.StartPropertyName] != null)
                    {
                        _blogStart = DataFactory.Instance.GetPage((PageReference)CurrentPage[BlogUtility.StartPropertyName]);
                    }
                    else
                    {
                        _blogStart = CurrentPage;
                    }
                }
                return _blogStart; 
            }
            set 
            {
                ViewState["BlogStartPageID"] = value.PageLink;
                _blogStart = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the bloggers details..
        /// </summary>
        public bool ShowDetails
        {
            get { return _showDetails; }
            set { _showDetails = value; }
        }
    }
}
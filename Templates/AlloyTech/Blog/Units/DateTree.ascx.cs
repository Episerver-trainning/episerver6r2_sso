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
    /// The date archive tree used default in the left column.
    /// </summary>
    public partial class DateTree : UserControlBase
    {
        private PageReference _treeStart;

        /// <summary>
        /// Gets or sets the start page for the tree.
        /// </summary>
        public PageReference TreeStart
        {
            get { return _treeStart; }
            set { _treeStart = value; }
        }
    }
}
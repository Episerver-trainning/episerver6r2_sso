#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;
using System.ComponentModel;

namespace EPiServer.Templates.Advanced.FileManager.Core.WebControls
{
    /// <summary>
    /// The Region control is used to declare a content region for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    [ToolboxData("<{0}:Region runat=server></{0}:Region>"), ParseChildren]
    public class Region : PlaceHolder, INamingContainer, IFileManagerRegion
    {
        private string _contentSource; 
        private FileManagerControl _fileManager;

        #region IFileMangerRegion interface implementation


        /// <summary>
        /// Gets or sets a reference to the hosting file manager instance.
        /// </summary>
        public FileManagerControl FileManager
        {
            get { return _fileManager; }
            set { _fileManager = value; }
        }

        /// <summary>
        /// Gets or sets the location of the web user control to load in this content region.
        /// </summary>
        public string ContentSource
        {
            get { return _contentSource; }
            private set { _contentSource = value; }
        }

        /// <summary>
        /// Called from the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/> to set configuration properties 
        /// and load the configured content control when loading this region.
        /// </summary>
        /// <param name="fileManager">The file manager.</param>
        /// <param name="contentSource">The location of the content to be loaded in this region.</param>
        public void Initialize(FileManagerControl fileManager, string contentSource)
        {
            FileManager = fileManager;
            ContentSource = contentSource;

            EnsureChildControls();
        }

        #endregion

        /// <summary>
        /// Overridden to load the content control configured in the corresponding <see cref="Configuration.RegionElement"/> configuration element.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            if (!String.IsNullOrEmpty(ContentSource))
            {
                Control c = Page.LoadControl(ContentSource);
                Controls.Add(c);
            }
        }
    }
}

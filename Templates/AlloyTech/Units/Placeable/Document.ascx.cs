/*
Copyright (c) EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using System;
using System.Web;
using EPiServer.Core;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Shows a link to a single document, also displaying an icon corresponding to the file type.
    /// </summary>
    public partial class Document : UserControlBase
    {
        private string _filePath;
        private PageData _documentPage;
        private string _documentProperty;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!IsPostBack)
            {
                if (FilePath != null)
                {
                    DocumentLink.Visible = true;
                    DocumentLink.NavigateUrl = FilePath;
                    DocumentLink.CssClass = string.Format("document {0}Extension", VirtualPathUtility.GetExtension(FilePath).Substring(1));
                    DocumentLink.Text = Server.HtmlEncode(VirtualPathUtility.GetFileName(FilePath));
                    DataBind();
                }
                else if (DisplayMissingMessage)
                {
                    // Show the error message label and set the translated error message
                    ErrorMessage.Visible = true;
                    ErrorMessage.Text = Translate("/error/document");
                }
                else
                {
                    this.Visible = false;
                }
            }
        }

        public bool DisplayMissingMessage { get; set; }

        /// <summary>
        /// Gets the encoded path to the document file.
        /// </summary>
        /// <value>The file path, HTTP path-encoded, since it comes from a PropertyUrlReference</value>
        public string FilePath
        {
            get 
            {
                if (_filePath == null)
                {
                    _filePath = DocumentPage[DocumentProperty] as string;
                }
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the page property that holds the document path.
        /// </summary>
        /// <value>The document page property.</value>
        public string DocumentProperty
        {
            get { return _documentProperty ?? "DocumentInternalPath"; }
            set { _documentProperty = value; }
        }

        /// <summary>
        /// Gets or sets the document page.
        /// </summary>
        /// <value>The document page.</value>
        public PageData DocumentPage
        {
            get { return _documentPage ?? CurrentPage; }
            set { _documentPage = value; }
        }
    }
}
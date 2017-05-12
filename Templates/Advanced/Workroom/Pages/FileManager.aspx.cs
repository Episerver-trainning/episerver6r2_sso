#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Usability limitations
 * Depends on javascript.

- Functionality limitations
 * Uploading of more than one file at a time is not supported. 
 * The size of the uploaded file can not exceed the maxRequestLength configured in web.config.

- Validation limitations
 * The asp:Linkbutton control produce invalid html when setting enabled = false.

*/
#endregion

using System;
using System.Web;
using System.Web.UI;
using System.Web.Hosting;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Hosting;
using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// This template shows a simple tree view of the workroom file structure and provides the user with possibility to upload new files.
    /// </summary>
    public partial class FileManager : WorkroomPageBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!IsPostBack)
            {
                FileManagerControl.DataBind();  

                if (FileManagerControl.CurrentVirtualDirectory == null)
                {
                    CreateFolder();
                }                              
            }
        }

        /// <summary>
        /// Creates the page folder of the File Manager page, if it isn't already created.
        /// </summary>        
        private void CreateFolder()
        {
            //We want to allow folder to be created without access checks performed therefore this 
            //construction. Otherwise it would be enough to call CurrentPage.GetPageDirectory(true)
            UnifiedDirectory pageDirectory = CurrentPage.GetPageDirectory(false);
            if (pageDirectory == null)
            {
                UnifiedDirectory rootDirectory = VirtualPathHandler.Instance.GetDirectory(VirtualPathHandler.PageDirectoryRootVirtualPath, true) as UnifiedDirectory;
                rootDirectory.CreateSubdirectory(CurrentPage.Property["PageFolderID"].ToString());
            }
        }
    }
}

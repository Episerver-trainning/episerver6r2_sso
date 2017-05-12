#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Globalization;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Templates.AlloyTech;
using EPiServer.Security;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;
using EPiServer.Web.Hosting;
using EPiServer.Web.WebControls;
using log4net;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Uploads a new file or replaces an existing in the <see cref="FileManagerControl.CurrentVirtualDirectory"/>
    /// </summary>
    public partial class AddFile : ContentUserControlBase
    {
        private static ILog _log = LogManager.GetLogger(typeof(AddFile));
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!IsPostBackView)
            {
                if (!FileManager.CurrentVirtualDirectory.QueryDistinctAccess(AccessLevel.Create | AccessLevel.Edit | AccessLevel.Delete))
                {
                    UploadButton.Enabled = false;
                    Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccess")));
                    return;
                }
                ResetUploadForm();
            }
        }

        /// <summary>
        /// Handles file upload when the UploadFile button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UploadFile_Click(object sender, EventArgs e)
        {
            this.labelExceptionMessage.Visible = false;

            if (string.IsNullOrEmpty(FileUpload.PostedFile.FileName))
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/validation/requiredfile")));
                return;
            }

            // Abort if there is a folder with the same name as the file
            if (GenericHostingEnvironment.VirtualPathProvider.DirectoryExists(VirtualPathUtility.Combine(FileManager.CurrentVirtualDirectory.VirtualPath, FileUpload.FileName)))
            {
                Page.Validators.Add(new StaticValidator(String.Format(CultureInfo.CurrentCulture , Translate("/filemanager/validation/folderwithsamename"),  FileUpload.FileName)));
                return;
            }


            // Check if there already exists a file with the same name 
            VirtualFile existingFile = FileSystemUtility.GetFile(FileManager.CurrentVirtualDirectory, FileUpload.FileName);

            if (existingFile == null)
            {
                // No file found. Let's create a new file.
                try
                {
                    FileSystemUtility.CreateFile(FileManager.CurrentVirtualDirectory, FileUpload.FileName, CheckInComment.Text.ToSafeString(), FileUpload.PostedFile.InputStream);
                    ResetUploadForm();
  
                    FileManager.LoadDefaultView(this);
                }
                catch (InvalidOperationException ex)
                {
                    this.labelExceptionMessage.Text = ex.Message;
                    this.labelExceptionMessage.Visible = true;
                }
            }
            else
            {
                // A file with the same name already exists.
                IVersioningFile versioningFile = existingFile as IVersioningFile;
                if (versioningFile != null && FileSystemUtility.IsCheckedOutBySomeoneElse(versioningFile))
                {
                    Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/checkedout")));
                    return;
                }

                try
                {
                    // File already exists; create a temp file for upload and alert the user.
                    UnifiedFile targetFile = FileSystemUtility.CreateTempFile(FileManager.CurrentVirtualDirectory, FileUpload.PostedFile.InputStream);

                    TemporaryFileName = targetFile.Name;
                    DestinationFileName = FileUpload.FileName;
                }
                catch (Exception ex)
                {
                    Page.Validators.Add(new StaticValidator(ex.Message));
                    return;
                }

                ReplaceFileButton.Visible = true;
                Page.Validators.Add(new StaticValidator(String.Format(Translate("/filemanager/errormessage/replacefile"), DestinationFileName)));

                FileUploadPanel.Visible = false;
                UploadButton.Visible = false;
                CommentsPanel.Visible = false;
              
           

            }
        }

        /// <summary>
        /// Handles the Click event of the ReplaceFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ReplaceFile_Click(object sender, EventArgs e)
        {            
            UnifiedFile temporaryFile = FileSystemUtility.GetFile(FileManager.CurrentVirtualDirectory, TemporaryFileName) as UnifiedFile;
            UnifiedFile destinationFile = FileSystemUtility.GetFile(FileManager.CurrentVirtualDirectory, DestinationFileName) as UnifiedFile;

            if (temporaryFile != null && destinationFile != null)
            {
                try
                {
                    FileSystemUtility.WriteToFile(temporaryFile, destinationFile, CheckInComment.Text.ToSafeString());
                }
                catch (FileIsCheckedOutException)
                {
                    // File is versioned and checked out to someone else
                    Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/checkedout")));
                    return;
                }
                finally 
                {
                    try 
                    {
                        temporaryFile.Delete();
                    }
                    catch (Exception exception) 
                    {
                        _log.Warn("Failed to delete temporary file " + temporaryFile.VirtualPath, exception);
                    }
                }

                ResetUploadForm();
                FileManager.LoadDefaultView(this);
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel button to cancel the current action.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TemporaryFileName))
            {
                // File was uploaded to a temporary location, but the replace was aborted.
                UnifiedFile tempFile = FileSystemUtility.GetFile(FileManager.CurrentVirtualDirectory, TemporaryFileName) as UnifiedFile;
                if (tempFile != null)
                {
                    tempFile.Delete();
                }
                ResetUploadForm();
            }
            else 
            {
                FileManager.LoadDefaultView(this);
            }
        }




        /// <summary>
        /// Resets the upload form to the initial state.
        /// </summary>
        private void ResetUploadForm()
        {
            UploadButton.Visible = UploadButton.Enabled = true;
            FileUploadPanel.Visible = true;
            
            // Show comments panel only for versioning files
            CommentsPanel.Visible = FileManager.CurrentVirtualDirectory.IsVersioningSupported;
            CheckInComment.Text = String.Empty;
            
            ReplaceFileButton.Visible = false;
            DestinationFileName = TemporaryFileName = null;
        }


        /// <summary>
        /// Gets or sets the name of the temporary file used when replacing an existing file.
        /// </summary>
        /// <value>The name of the temporary file.</value>
        private string TemporaryFileName 
        {
            get { return ViewState["TempFileName"] as string; }
            set { ViewState["TempFileName"] = value; }
        }


        /// <summary>
        /// Gets or sets the name of the destination file when replacing an existing file.
        /// </summary>
        /// <value>The name of the temporary file.</value>
        private string DestinationFileName 
        {
            get { return ViewState["DestinationFileName"] as string; }
            set { ViewState["DestinationFileName"] = value; }
        }

    }
}
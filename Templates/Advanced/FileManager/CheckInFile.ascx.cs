#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Security;
using EPiServer.Web.Hosting;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Web controls used for perfom 'Check in' operation for selected files.     
    /// </summary>
    public partial class CheckInFile : ContentUserControlBase
    {
        UnifiedFile[] _selectedFiles;

        #region Properties

        /// <summary>
        /// Gets or sets the index of the current file.
        /// </summary>
        /// <value>The index of the current file.</value>
        private int CurrentFileIndex
        {
            get
            {
                if (this.ViewState["_currentFileIndex"] != null)
                {
                    return (int)this.ViewState["_currentFileIndex"];
                }
                return 0;
            }
            set
            {
                this.ViewState["_currentFileIndex"] = value;
            }
        }

        /// <summary>
        /// Gets the selected files as UnifiedFile array;
        /// </summary>
        /// <value>The selected files.</value>
        private UnifiedFile[] SelectedFiles
        {
            get
            {
                if (_selectedFiles == null)
                {
                    _selectedFiles = GetSelectedFiles();
                }
                return _selectedFiles;
            }
        }

        #endregion


        #region Event Handlers
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
                UpdateForm();
            }
        }

        /// <summary>
        /// Handles the Click event of the CheckInFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CheckInFile_Click(object sender, EventArgs e)
        {
            if (!this.FileUpload.HasFile)
            {
                this.labelExceptionMessage.Text = "*";
                this.labelExceptionMessage.Visible = true;
            }
            else if ((base.Request.Files.Count != 0) && (base.Request.Files[0].ContentLength != 0))
            {
                UnifiedFile selectedFile = this.SelectedFiles[this.CurrentFileIndex];

                try
                {
                    FileSystemUtility.WriteToFile(base.Request.Files[0].InputStream, selectedFile, CheckInComment.Text);
                }
                catch (FileIsCheckedOutException)
                {
                    Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                    FileManager.LoadDefaultView(this);
                    return;
                }                
                catch (Exception exception)
                {
                    Page.Validators.Add(new StaticValidator(exception.Message));
                    FileManager.LoadDefaultView(this);
                    return;
                }

                this.CurrentFileIndex++;
                if (this.CurrentFileIndex < this.SelectedFiles.Length)
                {
                    this.UpdateForm();
                }
                else
                {                    
                    GoBack();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel button to cancel the current action.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        #endregion

        /// <summary>
        /// Resets the upload form to the initial state.
        /// </summary>
        private void UpdateForm()
        {
            labelExceptionMessage.Visible = false;

            UploadButton.Visible = UploadButton.Enabled = true;
            FileUploadPanel.Visible = true;

            // Show comments panel only for versioning files
            CommentsPanel.Visible = FileManager.CurrentVirtualDirectory.IsVersioningSupported;
            CheckInComment.Text = String.Empty;

            UnifiedFile[] selectedFiles = this.SelectedFiles;
            if (selectedFiles.Length == 0)
            {
                this.UploadButton.Enabled = false;
            }
            else
            {
                UnifiedFile file = this.SelectedFiles[this.CurrentFileIndex];
                IVersioningFile versioningFile = file as IVersioningFile;
                
                this.fileCount.Visible = selectedFiles.Length > 1;
                this.fileCount.Text = "(" + string.Format(Translate("/filemanager/checkinselection/filexofy"), this.CurrentFileIndex + 1, selectedFiles.Length) + ")";
                if (versioningFile != null && !FileSystemUtility.IsCheckedOutByCurrentUser(versioningFile))
                {
                    this.fileCount.Visible = true;
                    this.fileCount.ForeColor = Color.Red;
                    this.fileCount.Text = Translate("/filemanager/checkinselection/filenotcheckedout");
                    this.CommentsPanel.Enabled = false;
                    this.CheckInComment.Enabled = false;
                    this.UploadButton.Visible = false;
                }
                this.currentFile.Text = file.Name;
            }
        }

        /// <summary>
        /// Gets the selected files.
        /// </summary>
        /// <returns></returns>
        private UnifiedFile[] GetSelectedFiles()
        {
            return FileManager.SelectedItems.Where(item => item is UnifiedFile).Select(item => item as UnifiedFile).ToArray();
        }


        /// <summary>
        /// Goes back to default file manager view.
        /// </summary>
        private void GoBack()
        {            
            FileManager.LoadDefaultView(this);
            FileManager.ClearFileVersionSelection();
        }
       
    }
}
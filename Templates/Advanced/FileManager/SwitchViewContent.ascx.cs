#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Security;
using EPiServer.Web.Hosting;
using EPiServer.Web.WebControls;
using System.Web;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Displays a button bar for changing the active view.
    /// </summary>
    public partial class SwitchViewContent : ContentUserControlBase
    {
        const string disabledButtonCssClass = "_dis";

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            FileManager.CommandExecuted += new EventHandler<FileManagerCommandEventArgs>(FileManager_CommandExecuted);
            FileManager.SelectedItems.CollectionChanged += new EventHandler<CollectionChangedEventArgs>(SelectedItems_Changed);
            FileManager.PasteBuffer.CollectionChanged += new EventHandler<CollectionChangedEventArgs>(SelectedItems_Changed);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!IsPostBackView)
            {
                EnableButtons();
            }
        }

        /// <summary>
        /// Handles the Changed event of the SelectedItems collection to update button state when file selection changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.Templates.Advanced.FileManager.Core.CollectionChangedEventArgs"/> instance containing the event data.</param>
        private void SelectedItems_Changed(object sender, CollectionChangedEventArgs e)
        {
            EnableButtons();
        }

        /// <summary>
        /// Handles the CommandExecuted event of the FileManager control to update button state when file checkin state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.Templates.Advanced.FileManager.Core.FileManagerCommandEventArgs"/> instance containing the event data.</param>
        private void FileManager_CommandExecuted(object sender, FileManagerCommandEventArgs e)
        {
            if (e.CommandName == "CheckOutSelection" ||
                e.CommandName == "CheckInFile" ||
                e.CommandName == "UndoCheckOutSelection")
            {
                EnableButtons();
            }
        }

        /// <summary>
        /// Handles the Command event of the Button controls to change the current view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void Button_Command(object sender, CommandEventArgs e)
        {
            try
            {
                LinkButton clickedButton = (LinkButton)sender;
                if (clickedButton == AddFile || clickedButton == AddFolder)
                {
                    FileManager.SelectedItems.Clear();
                }
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(e.CommandName, e.CommandArgument));
            }
            catch (HttpCompileException)
            { 
                // Must never catch compile exception. Since we're loading and compiling usercontrols 
                // it's very likely no validation summary exists showing the errors.
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccess")));
            }
            catch (ArgumentException ex)
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
            }
        }

        /// <summary>
        /// Returs a mark if the name of load view control is VersionList.
        /// </summary>
        /// <returns><c>True</c> if the load view contorl has VersionList name, otherwise <c>false</c>.</returns>
        public bool IsVersionView()
        {
            return String.Equals(FileManager.CurrentView, "VersionList", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returs a mark if the name of load view control is CheckInFile.
        /// </summary>
        /// <returns><c>True</c> if the load view contorl has CheckInFile name, otherwise <c>false</c>.</returns>
        public bool IsCheckInView()
        {
            return String.Equals(FileManager.CurrentView, "CheckInFile", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Changes the enabled state of buttons based on current file manager state.
        /// </summary>
        private void EnableButtons()
        {
            Details.Enabled = IsDetailsAllowed();
            Version.Enabled = IsVersionAllowed();

            UnifiedDirectory currentDirectory = FileManager.CurrentVirtualDirectory;
            if (currentDirectory != null && currentDirectory.QueryDistinctAccess(AccessLevel.Create | AccessLevel.Edit | AccessLevel.Delete))
            {
                DisabledFunctionsMessage.Visible = false;
                AddFile.Enabled = IsDefaultView();
                AddFolder.Enabled = IsDefaultView();
                Rename.Enabled = IsRenameAllowed();
                Delete.Enabled = IsChangeAllowed();
                Cut.Enabled = IsChangeAllowed();
                Copy.Enabled = IsDefaultView() && FileManager.SelectedItems.Count > 0;
                Paste.Enabled = IsDefaultView() && FileManager.PasteBuffer.Count > 0;
                Checkout.Enabled = IsCheckOutAllowed();
                UndoCheckout.Enabled = IsUndoCheckOutAllowed();
                CheckIn.Enabled = IsCheckInAllowed();
            }
            else
            {
                DisabledFunctionsMessage.Visible = true;
                AddFile.Enabled = false;
                AddFolder.Enabled = false;
                Rename.Enabled = false;
                Delete.Enabled = false;
                Cut.Enabled = false;
                Copy.Enabled = false;
                Paste.Enabled = false;
                Checkout.Enabled = false;
                UndoCheckout.Enabled = false;
                CheckIn.Enabled = false;
            }

            ApplyDisabledCssClassToDisabledButtons();
        }

        private void ApplyDisabledCssClassToDisabledButtons()
        {
            foreach (LinkButton linkButton in this.Controls.Cast<Control>().Where(c => c is LinkButton).Select(c => c as LinkButton))
            {
                if (!linkButton.Enabled)
                {
                    if (!linkButton.CssClass.Contains(disabledButtonCssClass))
                    {
                        linkButton.CssClass += disabledButtonCssClass;
                    }
                }
                else
                {
                    linkButton.CssClass = linkButton.CssClass.Replace(disabledButtonCssClass, "");
                }
            }
        }

        private bool IsDefaultView()
        {
            return String.Equals(FileManager.CurrentView, FileManager.DefaultView, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsVersionAllowed()
        {
            return IsDefaultView() && FileManager.SelectedItems.Count == 1 && FileManager.SingleSelectedFile is IVersioningFile;
        }

        private bool IsDetailsAllowed()
        {
            return IsDefaultView() && FileManager.SelectedItems.Count == 1 && FileManager.SingleSelectedItem is VirtualFile;
        }

        private bool IsChangeAllowed()
        {
            if (!IsDefaultView() || FileManager.SelectedItems.Count == 0)
            {
                return false;
            }

            return !FileManager.SelectedItems.Any(vf => !FileSystemUtility.CanEdit(vf));   
        }

        private bool IsRenameAllowed()
        {
            if (!IsDefaultView() || FileManager.SingleSelectedItem == null)
            {
                return false;
            }
            return FileSystemUtility.CanEdit(FileManager.SingleSelectedItem);
        }

        private bool IsCheckOutAllowed()
        {
            if (!IsDefaultView() || FileManager.SelectedItems.Count == 0)
            {
                return false;
            }

            return !FileManager.SelectedItems.Any(vf => !FileSystemUtility.CanCheckOut(vf));
        }

        private bool IsUndoCheckOutAllowed()
        {
            if (!IsDefaultView() || FileManager.SelectedItems.Count == 0)
            {
                return false;
            }

            return !FileManager.SelectedItems.Any(file => !FileSystemUtility.CanUndoCheckOut(file));
        }

        private bool IsCheckInAllowed()
        {
            if (!IsDefaultView() || FileManager.SelectedItems.Count == 0)
            {
                return false;
            }

            foreach(var file in FileManager.SelectedItems) {
                IVersioningFile versioningFile = file as IVersioningFile;
                // If any selected file isn't a checked out versioned file with edit permissions check in is disabled
                if (versioningFile == null || !versioningFile.IsCheckedOut || !FileSystemUtility.CanEdit(file))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}

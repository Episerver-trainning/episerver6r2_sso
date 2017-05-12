#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.WebControls;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// User control handling renaming files and folders and creating new folders.
    /// </summary>
    public partial class RenameAndCreateFolder : ContentUserControlBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FileManager.ClearFileVersionSelection();

            if (!IsPostBackView)
            {
                if (IsAddFolderView)
                {
                    FolderLegend.Text = Translate("/filemanager/switchview/addfolder");
                    ItemNameLabel.Text = Translate("/filemanager/foldername");
                    string preferredNewPath = VirtualPathUtility.Combine(FileManager.CurrentVirtualPath, Translate("/filemanager/newfolder"));
                    // Get a name of a folder that doesn't exist.
                    string freePath = FileSystemUtility.FindAvailableVirtualPath(preferredNewPath);
                    ItemName.Text = VirtualPathUtility.GetFileName(freePath);
                    SaveButton.Enabled = true;
                    SaveButton.Text = Translate("/button/create");
                }
                else if (FileManager.SingleSelectedItem != null)
                {
                    if (FileManager.SingleSelectedItem.IsDirectory)
                    {
                        FolderLegend.Text = Translate("/filemanager/renamefolder");
                        ItemNameLabel.Text = Translate("/filemanager/foldername");
                    }
                    else
                    {
                        FolderLegend.Text = Translate("/filemanager/renamefile");
                        ItemNameLabel.Text = Translate("/filemanager/filename");
                    }
                    
                    ItemName.Text = FileManager.SingleSelectedItem.Name;
                    SaveButton.Enabled = true;
                }
            }

        }

        /// <summary>
        /// Gets a value indicating whether the control was included in the add folder view or the rename view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this the requested action is to create a new folder; otherwise, <c>false</c>.
        /// </value>
        protected bool IsAddFolderView
        {
            get { return String.Equals(FileManager.CurrentView, "AddFolder", StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Handles the Click event of the SaveItem control to perform the file folder renaming.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveItem_Click(object sender, EventArgs e)
        { 
            if (Page.IsValid)
            {
                try
                {
                    if (IsAddFolderView)
                    {
                        FileSystemUtility.CreateDirectory(FileManager.CurrentVirtualDirectory, ItemName.Text);
                    }
                    else
                    {
                        FileSystemUtility.RenameItem(FileManager.SingleSelectedItem, ItemName.Text);
                        FileManager.SelectedItems.Clear();
                    }

                    FileManager.LoadDefaultView(this);
                }
                catch (UnauthorizedAccessException)
                {
                    if (IsAddFolderView)
                    {
                        Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccesscreatefolder")));
                    }
                    else
                    {
                        Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccessrename")));
                    }
                }
                catch (FileIsCheckedOutException)
                {
                    if (FileManager.SingleSelectedItem.IsDirectory)
                    {
                        Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/containscheckedout")));
                    }
                    else
                    {
                        Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                    }
                }
                catch (Exception ex)
                {
                    Page.Validators.Add(new StaticValidator(ex.Message));
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel control to revert back to the state prior to the rename view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            FileManager.LoadDefaultView(this);
        }

        /// <summary>
        /// Validates the name uniqueness of the new name to prevent name clashes in a folder.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidateUniqueName(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = false;
            if (!String.IsNullOrEmpty(e.Value))
            {
                string parentVirtualPath = (IsAddFolderView ? FileManager.CurrentVirtualDirectory.VirtualPath : VirtualPathUtility.GetDirectory(FileManager.SingleSelectedItem.VirtualPath));
                
                // Assemble the resulting new virtual path
                string newVirtualPath = VirtualPathUtility.Combine(parentVirtualPath, e.Value);
                
                // Make sure that no file or folder exists with the new name.
                e.IsValid = FileSystemUtility.IsPathAvailable(newVirtualPath);
            }
        }

        /// <summary>
        /// Validates that the lenght of the name doesn't exceed 255 characters.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidateNameLength(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ItemName.Text.Length < 256;
        }
        
    }
}
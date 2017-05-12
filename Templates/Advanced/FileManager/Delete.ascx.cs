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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Hosting;

using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.Hosting;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Region content for deleting items in the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>.
    /// </summary>
    public partial class Delete : ContentUserControlBase
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBackView)
            {
                ListItem listItem = null;

                // Populate lists with items to be deleted
                foreach (VirtualFileBase item in FileManager.SelectedItems)
                {                    
                    if (item.IsDirectory)
                    {
                        if (((VirtualDirectory)item).Children.GetEnumerator().MoveNext())
                        {
                            // Directory is not empty
                            listItem = new ListItem(String.Format("{0} ({1})", item.Name, Translate("/filemanager/foldernotempty")));
                            FolderEmptyWarning.Visible = true;
                        }
                        else
                        {
                            // Directory is empty
                            listItem = new ListItem(item.Name);
                        }
                        listItem.Attributes.Add("class", "document folder");
                        DirectoryList.Items.Add(listItem);
                    }
                    else 
                    {
                        listItem = new ListItem(item.Name);
                        string itemsExtension = ((UnifiedFile)item).Extension;
                        if (!String.IsNullOrEmpty(itemsExtension) && itemsExtension.Length>0)
                        {
                            listItem.Attributes.Add("class", String.Format("document {0}Extension", itemsExtension.Substring(1)));
                        }
                        FileList.Items.Add(listItem);
                    }
                }
                DirectoryConfirmation.Visible = (DirectoryList.Items.Count > 0);
                FileConfirmation.Visible = (FileList.Items.Count > 0);
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteItems button to perform the remove operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteItems_Click(object sender, EventArgs e)
        {
            try
            {
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.DeleteSelection, null));
                FileManager.LoadDefaultView(this);
            }
            catch (UnauthorizedAccessException)
            { 
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccessremove")));
            }
            catch (Exception ex)
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
            }
        }

        /// <summary>
        /// Handles the Click event of the CancelDelete button to load the previous view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelDelete_Click(object sender, EventArgs e)
        {
            FileManager.LoadDefaultView(this);
        }
    }
}
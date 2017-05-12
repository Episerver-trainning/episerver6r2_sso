#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Used to display a simple heading for a file manager view.
    /// The heading shows the name of the current view and cookie crumbs for the current virtual directory.
    /// </summary>
    public partial class HeadingContent : ContentUserControlBase
    {
        private int _linkId;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Listen to file manager events
            FileManager.CommandExecuted += new EventHandler<FileManagerCommandEventArgs>(FileManager_CommandExecuted);
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CurrentFolderBreadcrumbs.Controls.Clear();
            AppendBreadCrumbs(FileManager.CurrentVirtualDirectory, CurrentFolderBreadcrumbs);
        }

        /// <summary>
        /// Handles the CommandExecuted event raised by the FileManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.Templates.Advanced.FileManager.Core.FileManagerCommandEventArgs"/> instance containing the event data.</param>
        void FileManager_CommandExecuted(object sender, FileManagerCommandEventArgs e)
        {
            // If the current folder changes we update the cookie crumbs
            if (e.CommandName == FileManagerCommandName.SelectFolder)
            {
                CreateChildControls();
            }
        }

        /// <summary>
        /// Appends the cookie crumb links to the control upplied as targetControl.
        /// </summary>
        /// <param name="currentDirectory">The current directory.</param>
        /// <param name="targetControl">The target control to append cookie crumb links to.</param>
        private void AppendBreadCrumbs(UnifiedDirectory currentDirectory, Control targetControl)
        {            
            if (currentDirectory == null || FileManager.RootVirtualPath == currentDirectory.VirtualPath)
            {
                // Reset the link enumeration when we reach the topmost directory.
                _linkId = 0;
            }
            else
            {
                // Append cookie crumb for the parent directory before adding for the current directory.
                AppendBreadCrumbs(currentDirectory.Parent, targetControl);
                Literal slash = new Literal();
                slash.Text = " / ";
                targetControl.Controls.Add(slash);
            }
            if (currentDirectory == null)
            {
                return;
            }
            string directoryName = currentDirectory.Name;
            if (currentDirectory.Parent != null && currentDirectory.Parent.IsFirstLevel)
            {
                PageData page = UnifiedDirectory.GetOwnerPageFromVirtualPath(currentDirectory.VirtualPath);
                if (page != null)
                {
                    directoryName = page.PageName;
                }
            }

            LinkButton b = new LinkButton();
            b.ID = "link" + _linkId++;
            b.Text = directoryName;
            b.CausesValidation = false;
            b.CommandArgument = currentDirectory.VirtualPath;
            b.CommandName = FileManagerCommandName.SelectFolder;
            b.Command += new CommandEventHandler(RaiseCommand);
            targetControl.Controls.Add(b);
        }

        /// <summary>
        /// Handles the Command event of the CookieCrumb controls to raise a command event on the file manager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void RaiseCommand(object sender, CommandEventArgs e)
        {
            FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(e.CommandName, e.CommandArgument));
            FileManager.LoadDefaultView(this);
        }

    }
}
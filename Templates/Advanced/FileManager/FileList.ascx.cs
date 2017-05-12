#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Displays a list of files in the <see cref="FileManagerControl.CurrentVirtualDirectory"/>
    /// </summary>
    public partial class FileList : ContentUserControlBase
    {
        private bool _isDataBound;
        private IComparer<VirtualFileBase> _selectedComparer = new NameComparer(SortDirection.Descending);

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void  OnInit(EventArgs e)
        {
         	base.OnInit(e);
            FileManager.CommandExecute += new EventHandler<FileManagerCommandEventArgs>(FileManager_CommandExecute);
        }

        /// <summary>
        /// Sets up a comparer with the previous command. Used to remember sort order when changing folders.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!String.IsNullOrEmpty(PreviousCommand))
            {
                SetupSelectedComparer(PreviousCommand);
            }

            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            
            if (!_isDataBound)
            {
                DataBindFileList();
            }
        }

        /// <summary>
        /// Raises a command on the current <see cref="FileManager"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void SelectFile(object sender, CommandEventArgs e)
        {
            string virtualPath = (string)e.CommandArgument;
            UpdateSelectedFilesList(GetVirtualFileByPath(virtualPath));
            FileManager.ClearFileVersionSelection();
            DataBindFileList();
        }

        private void UpdateSelectedFilesList(VirtualFileBase file)
        {
            int currentIndex = FileManager.SelectedItems.IndexOf(file);
            if (currentIndex >= 0)
            {
                FileManager.SelectedItems.RemoveAt(currentIndex);
            }
            else
            {
                FileManager.SelectedItems.Add(file);
            }
        }

        private static VirtualFileBase GetVirtualFileByPath(string virtualPath)
        {
            VirtualFileBase file = null;
            if (virtualPath.EndsWith("/", StringComparison.Ordinal))
            {
                file = HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath) as VirtualFileBase;
            }
            else
            {
                file = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath) as VirtualFileBase;
            }
            return file;
        }

        /// <summary>
        /// Binds the file list to the file repeater control.
        /// </summary>
        protected void DataBindFileList()
        {
            if (FileManager.CurrentVirtualDirectory != null)
            {
                FileRepeater.DataSource = GetFilesAndFolders();
                FileRepeater.DataBind();
                _isDataBound = true;
            }
        }

        /// <summary>
        /// Handles the CommandExecute event of the FileManager control to update the file list when a change occur.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.Templates.Advanced.FileManager.Core.FileManagerCommandEventArgs"/> instance containing the event data.</param>
        private void FileManager_CommandExecute(object sender, FileManagerCommandEventArgs e)
        {
            if (sender == this)
            {
                return;
            }
            if (e.CommandName == FileManagerCommandName.SelectFolder) 
            {
                // Repopulate the file list if the current folder changed.
                DataBindFileList();
            }
        }

        /// <summary>
        /// Gets the file size as a string in human readable format.
        /// </summary>
        /// <param name="fileItem">The file item.</param>
        /// <returns>The file size, or an empty string if the input is not a <see cref="UnifiedFile"/></returns>
        protected static string GetFileSizeString(VirtualFileBase fileItem)
        {
            UnifiedFile unifiedFile = fileItem as UnifiedFile;
            return unifiedFile != null ? GetFileSizeString(unifiedFile.Length) : String.Empty;
        }

        /// <summary>
        /// Gets the css for a folder or a file extension
        /// </summary>
        /// <param name="fileItem">The file item.</param>
        /// <returns>The folder or a file extension css string or an empty string if the input is not a <see cref="UnifiedFile"/></returns>
        protected static string GetFileExtensionCss(object fileItem)
        {
            UnifiedFile unifiedFile = fileItem as UnifiedFile;
            if (unifiedFile != null)
            {
                string extension = String.Empty;
                if (!String.IsNullOrEmpty(unifiedFile.Extension))
                {
                    extension = unifiedFile.Extension.Substring(1);
                }
                return String.Format("document {0}Extension", extension.ToLowerInvariant());
            }

            VirtualFileBase fileBase = fileItem as VirtualFileBase;
            if (fileBase != null && fileBase.IsDirectory)
            {
                return "document folder";
            }

            return string.Empty;
        }



        /// <summary>
        /// Gets the changed date as a string.
        /// </summary>
        /// <param name="fileItem">The file item.</param>
        /// <returns>The changed date or an empty string if the input is not a <see cref="UnifiedFile"/></returns>
        protected static string GetChangedDateString(object fileItem)
        {
            UnifiedFile unifiedFile = fileItem as UnifiedFile;
            return unifiedFile != null ? unifiedFile.Changed.ToString("g") : String.Empty;
        }

        /// <summary>
        /// Gets the username for the user the file is checked out to.
        /// </summary>
        /// <param name="fileItem">The file item.</param>
        /// <returns>The user name, or an epmty string if the file is not checked out.</returns>
        protected static string GetCheckedOutString(VirtualFileBase fileItem)
        {
            IVersioningFile versioningFile = fileItem as IVersioningFile;
            if (versioningFile != null && versioningFile.IsCheckedOut)
            {
                return versioningFile.CheckedOutBy;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets a sorted collection with files and folders.
        /// </summary>
        /// <returns>A sorted IEnumerable containing VirtualFileBase:s</returns>
        private IEnumerable GetFilesAndFolders()
        {
            var directories = from item in FileManager.CurrentVirtualDirectory.GetDirectories()
                              select item as VirtualFileBase;

            var files = from item in FileManager.CurrentVirtualDirectory.GetFiles()
                        where !item.Name.StartsWith(FileSystemUtility.TempFileName, StringComparison.OrdinalIgnoreCase) && item.Extension != FileSystemUtility.TempFileExtension
                        select item as VirtualFileBase;

            return directories.Concat(files).OrderBy(item => item, SelectedComparer);
        }

        /// <summary>
        /// Determines the sort order when a user clicks on a column heading
        /// </summary>
        protected void LinkButton_Command(Object sender, CommandEventArgs e)
        {
            string commandArgument = (string)e.CommandArgument;

            if (commandArgument.Equals(PreviousCommand))
            {
                Direction = (Direction == SortDirection.Descending) ? SortDirection.Ascending : SortDirection.Descending;
            }
            else
            {
                PreviousCommand = commandArgument;
                Direction = SortDirection.Descending;
            }

            SetupSelectedComparer(commandArgument);
        }

        /// <summary>
        /// Sets up the selected comparer.
        /// </summary>
        /// <param name="commandArgument">The command argument indicating what comparer to set up.</param>
        private void SetupSelectedComparer(string commandArgument)
        {
            SortingColumn = commandArgument;
            switch (commandArgument)
            {
                case "name":
                    SelectedComparer = new NameComparer(Direction);
                    break;
                case "size":
                    SelectedComparer = new SizeComparer(Direction);
                    break;
                case "checkedOut":
                    SelectedComparer = new CheckedOutByComparer(Direction);
                    break;
                case "lastChanged":
                    SelectedComparer = new DateComparer(Direction);
                    break;
            }
        }

        protected string GetSortingCssClass(string columnName)
        {
            string result = "sort";
            if (columnName == SortingColumn)
            {
                result += " " + Direction.ToString();
            }
            return result;
        }
        /// <summary>
        /// Gets or sets the previously chosen command.
        /// </summary>
        /// <value>The previously chosen command.</value>
        private string PreviousCommand
        {
            get { return (string)ViewState["PreviousCommand"]; }
            set { ViewState["PreviousCommand"] = value; }
        }

        /// <summary>
        /// Gets or sets curent sorting column name
        /// </summary>
        protected string SortingColumn
        {
            get { return (string)(ViewState["SortingColumn"] ?? ""); }
            set { ViewState["SortingColumn"] = value; }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        protected SortDirection Direction
        {
            get { return (SortDirection)(ViewState["Direction"] ?? SortDirection.Ascending); }
            set { ViewState["Direction"] = value; }
        }

        /// <summary>
        /// Gets or sets the currently selected comparer.
        /// </summary>
        /// <value>The currently selected comparer.</value>
        protected IComparer<VirtualFileBase> SelectedComparer
        {
            get { return _selectedComparer; }
            set { _selectedComparer = value; }
        }

        #region Comparers

        /// <summary>
        /// Sorts VirtualFileBase:s according to name. Groups files and folders separatly.
        /// </summary>
        class NameComparer : IComparer<VirtualFileBase>
        {
            SortDirection _direction;
            int _compareValue;

            public NameComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VirtualFileBase x, VirtualFileBase y)
            {
                //Check if we are comparing a folder and a file
                if (x.GetType() != y.GetType())
                {
                    _compareValue = (x is VirtualDirectory) ? -1 : 1;
                }
                else
                {
                    _compareValue = String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
                }

                if (_direction == SortDirection.Ascending)
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts VirtualFileBase:s according to size. Groups files and folders separatly.
        /// </summary>
        class SizeComparer : IComparer<VirtualFileBase>
        {
            SortDirection _direction;
            int _compareValue;

            public SizeComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VirtualFileBase x, VirtualFileBase y)
            {
                UnifiedFile xAsFile = x as UnifiedFile;

                //Check if we are comparing a folder and a file
                if (x.GetType() != y.GetType())
                {
                    _compareValue = (x is VirtualDirectory) ? -1 : 1;
                }
                else if (xAsFile != null)
                {
                    _compareValue = xAsFile.Length.CompareTo(((UnifiedFile)y).Length);
                }
                else
                {
                    //We are comparing two folders who doesn't have length
                    _compareValue = 0;
                }
                
                if (_direction == SortDirection.Ascending)
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts VirtualFileBase:s according to the changed date. Groups files and folders separatly.
        /// </summary>
        class DateComparer : IComparer<VirtualFileBase>
        {
            SortDirection _direction;
            int _compareValue;

            public DateComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VirtualFileBase x, VirtualFileBase y)
            {
                UnifiedFile xAsFile = x as UnifiedFile;

                //Check if we are comparing a folder and a file
                if (x.GetType() != y.GetType())
                {
                    _compareValue = (x is VirtualDirectory) ? -1 : 1;
                }
                else if (xAsFile != null)
                {
                    _compareValue = xAsFile.Changed.CompareTo(((UnifiedFile)y).Changed);
                }
                else
                {
                    //We are comparing two folders who doesn't have a last changed property
                    _compareValue = 0;
                }
                
                if (_direction == SortDirection.Ascending)
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts VirtualFileBase:s according to who has the file checked out. Groups files and folders separatly.
        /// </summary>
        class CheckedOutByComparer : IComparer<VirtualFileBase>
        {
            SortDirection _direction;
            int _compareValue;

            public CheckedOutByComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VirtualFileBase x, VirtualFileBase y)
            {
                IVersioningFile xAsFile = x as IVersioningFile;

                //Check if we are comparing a folder and a file
                if (x.GetType() != y.GetType())
                {
                    _compareValue = (x is VirtualDirectory) ? -1 : 1;
                }
                else if (xAsFile != null)
                {
                    _compareValue = String.Compare(xAsFile.CheckedOutBy, ((IVersioningFile)y).CheckedOutBy, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    _compareValue = 0;
                }
             
                if (_direction == SortDirection.Ascending)
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        #endregion


        protected void RepeaterItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectFile")
            {
                SelectFile(source, e);
            }
        }

        protected void SelectFolder(object sender, CommandEventArgs e)
        {
            FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(e.CommandName, e.CommandArgument));
        }

        protected void ParentFolder(object sender, CommandEventArgs e)
        {
            if (FileManager.CurrentVirtualDirectory.VirtualPath == FileManager.RootVirtualPath)
            {
                return;
            }
            FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(e.CommandName, FileManager.CurrentVirtualDirectory.Parent.VirtualPath));
            
        }
    }
}

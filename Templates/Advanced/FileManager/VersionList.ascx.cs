#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;
using EPiServer.Security;
using EPiServer.Web.Hosting;
using EPiServer.XForms.WebControls;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Displays a list of versions for the selected file in the <see cref="FileManagerControl.CurrentVirtualDirectory"/>
    /// </summary>
    public partial class VersionList : ContentUserControlBase
    {
        private bool _isDataBound;
        private IComparer<VersionListItem> _selectedComparer = new VersionNumberComparer(SortDirection.Ascending);

        /// <summary>
        /// Gets or sets the previously chosen command.
        /// </summary>
        /// <value>The previously chosen command.</value>
        private string PreviousCommand
        {
            get { return ( string ) ViewState[ "PreviousCommand" ]; }
            set { ViewState[ "PreviousCommand" ] = value; }
        }

        protected string SortingColumn
        {
            get { return (string)(ViewState["SortingColumn"] ?? ""); }
            set { ViewState["SortingColumn"] = value; }

        }
        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        private SortDirection Direction
        {
            get { return ( SortDirection ) ViewState[ "Direction" ]; }
            set { ViewState[ "Direction" ] = value; }
        }

        private string CurrentVersion
        {
            get { return ( string ) ViewState[ "CurrentVersion" ]; }
            set { ViewState[ "CurrentVersion" ] = value; }
        }

        /// <summary>
        /// Gets the legend text.
        /// </summary>
        /// <value>The legend text.</value>
        public string Legend
        {
            get
            {
                return String.Format("{0} <strong>{1}</strong>", Translate("/filemanager/versionfileheading"), HttpUtility.HtmlEncode(FileManager.SingleSelectedFile.Name));
            }
        }

        /// <summary>
        /// Gets or sets the currently selected comparer.
        /// </summary>
        /// <value>The currently selected comparer.</value>
        private IComparer<VersionListItem> SelectedComparer
        {
            get { return _selectedComparer; }
            set { _selectedComparer = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Sets up a comparer with the previous command. Used to remember sort order when changing folders.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBackView)
            {
                Direction = SortDirection.Ascending;
                PreviousCommand = "version";
            }
         
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

            if (string.IsNullOrEmpty(FileManager.SelectedFileVersionId) || FileManager.SelectedFileVersionId.Equals(CurrentVersion))
            {
                ButtonDelete.Visible = false;
                ButtonRestore.Visible = false;
            }

            if (!_isDataBound)
            {
                DataBindVersionList();
            }
        }

        /// <summary>
        /// Raises a command on the current <see cref="FileManager"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void SelectVersion(object sender, CommandEventArgs e)
        {
            string versionId = (string)e.CommandArgument;
            ButtonDelete.Visible = false;
            ButtonRestore.Visible = false;

            IVersioningFile selectedFile = FileManager.SingleSelectedFile as IVersioningFile;
            if (selectedFile != null)
            {
                FileManager.SelectedFileVersionId = versionId;
                if (FileManager.CurrentVirtualDirectory.QueryDistinctAccess(AccessLevel.Create | AccessLevel.Edit | AccessLevel.Delete))
                {
                    ButtonRestore.Visible = IsRestoreVersionEnabled(selectedFile);
                    ButtonDelete.Visible = IsDeleteVersionEnabled(selectedFile);
                }
            }
            DataBindVersionList();
        }

        private bool IsDeleteVersionEnabled(IVersioningFile file)
        {
            return !String.IsNullOrEmpty(FileManager.SelectedFileVersionId) &&
                FileSystemUtility.CanChangeVersions(file);
        }

        private bool IsRestoreVersionEnabled(IVersioningFile file)
        {
            return !FileManager.SelectedFileVersionId.Equals(CurrentVersion) &&
                FileSystemUtility.CanChangeVersions(file);
        }

        /// <summary>
        /// Binds the file list to the file repeater control.
        /// </summary>
        protected void DataBindVersionList()
        {
            IVersioningFile selectedFile = FileManager.SingleSelectedFile as IVersioningFile;
            if (selectedFile != null)
            {
                VersionRepeater.DataSource = GetVersions(selectedFile.GetVersions());
                VersionRepeater.DataBind();
                _isDataBound = true;
            }
        }

        /// <summary>
        /// Gets a sorted collection with files and folders.
        /// </summary>
        /// <param name="fileVersions">A list of version file </param>
        /// <returns>A sorted IEnumerable containing UnifiedVersion:s</returns>
        private IEnumerable GetVersions(IList<UnifiedVersion> fileVersions)
        {
            List<VersionListItem> sortedVersions = fileVersions.Select(fileVersion =>
                                                        new VersionListItem
                                                        {
                                                            Id = fileVersion.Id,
                                                            VersionNumber = fileVersion.Name,
                                                            Comments = fileVersion.Comments,
                                                            Size = GetFileSizeString(fileVersion.Length),
                                                            CreatedBy = fileVersion.CreatedBy,
                                                            Created = fileVersion.Created.ToString("g"),
                                                            VirtualPath = fileVersion.VirtualPath
                                                        }).ToList();

            sortedVersions.Sort(new VersionNumberComparer(SortDirection.Ascending));
            CurrentVersion = sortedVersions[0].Id.ToString();

            sortedVersions.Sort(SelectedComparer);

            return sortedVersions;
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
                case "version":
                    SelectedComparer = new VersionNumberComparer(Direction);
                    break;
                case "comments":
                    SelectedComparer = new CommentsComparer(Direction);
                    break;
                case "size":
                    SelectedComparer = new SizeComparer(Direction);
                    break;
                case "created":
                    SelectedComparer = new DateComparer(Direction);
                    break;
                case "createdby":
                    SelectedComparer = new CreatedByComparer(Direction);
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

        #region Click handlers (button and headers)

        /// <summary>
        /// Determines the sort order when a user clicks on a column heading
        /// </summary>
        protected void LinkButton_Command(Object sender, CommandEventArgs e)
        {
            string commandArgument = (string)e.CommandArgument;

            if( commandArgument.Equals(PreviousCommand) )
            {
                Direction = ( Direction == SortDirection.Descending ) ? SortDirection.Ascending : SortDirection.Descending;
            }
            else
            {
                PreviousCommand = commandArgument;
                Direction = SortDirection.Descending;
            }

            SetupSelectedComparer(commandArgument);
        }


        /// <summary>
        /// Handles the Click event of the Restore control. Restore selected version of the versioning file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RestoreItems_Click(object sender, EventArgs e)
        {
            try
            {
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.RestoreVersion, null));
                _isDataBound = false;
                DataBindVersionList();
            }
            catch (FileIsCheckedOutException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch (VersionNotFoundException)
            {
                Page.Validators.Add(new StaticValidator(String.Format(Translate("/filemanager/errormessage/versionisnotfound"), FileManager.SelectedFileVersionId)));
                return;
            }
            catch (InvalidVersioningOperationException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch( UnauthorizedAccessException )
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccessremove")));
                return;
            }
            catch( Exception ex )
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the Back control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BackButton_Click(object sender, EventArgs e)
        {
            FileManager.ClearFileVersionSelection();
            FileManager.LoadDefaultView(this);
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
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.DeleteVersion, null));
                _isDataBound = false;
                DataBindVersionList();
            }
            catch (FileIsCheckedOutException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch (VersionNotFoundException)
            {
                Page.Validators.Add(new StaticValidator(String.Format(Translate("/filemanager/errormessage/versionisnotfound"), FileManager.SelectedFileVersionId)));
                return;
            }
            catch (InvalidVersioningOperationException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccessremove")));
                return;
            }
            catch (Exception ex)
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
                return;
            }
        }

        protected void RepeaterItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectVersion")
            {
                SelectVersion(source, e);
            }
        }

        #endregion

        protected string SetSelectedStyle(string itemId)
        {
            return itemId.Equals(FileManager.SelectedFileVersionId) ? "class='selected'" : "";
        }

        #region Inner class for version view

        /// <summary>
        /// A version list item class for Repeater control in <see cref="EPiServer.Templates.Advanced.FileManager.VersionList" />.
        /// </summary>
        protected class VersionListItem
        {
            /// <summary>
            /// Gets or sets a file version number.
            /// </summary>
            public string VersionNumber
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a version Id (Guid).
            /// </summary>
            public object Id
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets comments for the version.
            /// </summary>
            public string Comments
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a version file size in.
            /// </summary>
            public string Size
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a created user login.
            /// </summary>
            public string CreatedBy
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a datetime when the version was created.
            /// </summary>
            public string Created
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a virtual path to the version file.
            /// </summary>
            public string VirtualPath
            {
                get;
                set;
            }

        }


        #endregion

        #region Comparers

        /// <summary>
        /// Sorts UnifiedVersion:s according to version numbers.
        /// </summary>
        class VersionNumberComparer : IComparer<VersionListItem>
        {
            SortDirection _direction;
            int _compareValue;

            public VersionNumberComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VersionListItem x, VersionListItem y)
            {
                double xValue = double.Parse(x.VersionNumber);
                double yValue = double.Parse(y.VersionNumber);

                _compareValue = xValue.CompareTo(yValue);

                if( _direction == SortDirection.Ascending )
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts UnifiedVersion:s according to version comments.
        /// </summary>
        class CommentsComparer : IComparer<VersionListItem>
        {
            SortDirection _direction;
            int _compareValue;

            public CommentsComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VersionListItem x, VersionListItem y)
            {
                _compareValue = String.Compare(x.Comments, y.Comments, StringComparison.Ordinal);

                if( _direction == SortDirection.Ascending )
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts UnifiedVersion:s according to size.
        /// </summary>
        class SizeComparer : IComparer<VersionListItem>
        {
            SortDirection _direction;
            int _compareValue;

            public SizeComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VersionListItem x, VersionListItem y)
            {
                _compareValue = String.Compare(x.Size, y.Size, StringComparison.Ordinal);

                if( _direction == SortDirection.Ascending )
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts UnifiedVersion:s according to the created date. 
        /// </summary>
        class DateComparer : IComparer<VersionListItem>
        {
            SortDirection _direction;
            int _compareValue;

            public DateComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VersionListItem x, VersionListItem y)
            {
                _compareValue = String.Compare(x.Created, y.Created, StringComparison.Ordinal);

                if( _direction == SortDirection.Ascending )
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        /// <summary>
        /// Sorts UnifiedVersion:s according to the user ID which created the version.
        /// </summary>
        class CreatedByComparer : IComparer<VersionListItem>
        {
            SortDirection _direction;
            int _compareValue;

            public CreatedByComparer(SortDirection sortDirection)
            {
                _direction = sortDirection;
            }

            public int Compare(VersionListItem x, VersionListItem y)
            {

                _compareValue = String.Compare(x.CreatedBy, y.CreatedBy, StringComparison.OrdinalIgnoreCase);

                if( _direction == SortDirection.Ascending )
                {
                    _compareValue *= -1;
                }

                return _compareValue;
            }
        }

        #endregion
    }
}
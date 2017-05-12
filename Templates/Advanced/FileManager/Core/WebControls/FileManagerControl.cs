#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.FileManager.Core.WebControls
{

    /// <summary>
    /// Web control rendering a file manager based on a view configuration.
    /// </summary>
    public class FileManagerControl : WebControl, INamingContainer
    {
        private EventHandler<FileManagerCommandEventArgs> _commandExecuting;
        private EventHandler<FileManagerCommandEventArgs> _commandExecute;
        private EventHandler<FileManagerCommandEventArgs> _commandExecuted;

        private UnifiedDirectory _currentVirtualDirectory;
        private bool _isPostBackView = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManagerControl"/> class.
        /// </summary>
        public FileManagerControl() : base(HtmlTextWriterTag.Div) { }

        /// <summary>
        /// Raised before a File Manager command is executed. Use <see cref="FileManagerCommandEventArgs.CancelCommand"/> to abort execution.
        /// </summary>
        public event EventHandler<FileManagerCommandEventArgs> CommandExecuting
        {
            add { _commandExecuting += value; }
            remove { _commandExecuting -= value; }
        }

        /// <summary>
        /// Raised when File Manager command is executed.
        /// </summary>
        public event EventHandler<FileManagerCommandEventArgs> CommandExecute
        {
            add { _commandExecute += value; }
            remove { _commandExecute -= value; }
        }

        /// <summary>
        /// Raised when a file manager command has been executed.
        /// </summary>
        public event EventHandler<FileManagerCommandEventArgs> CommandExecuted
        {
            add { _commandExecuted += value; }
            remove { _commandExecuted -= value; }
        }

        #region File Management Properties

        /// <summary>
        /// Gets or sets the initial root path
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Localizable(false)]
        public string RootVirtualPath
        {
            get { return (string)(ViewState["RootVirtualPath"] ?? String.Empty); }
            set { ViewState["RootVirtualPath"] = value; }
        }

        /// <summary>
        /// Gets or sets the currently browsed virtual path
        /// </summary>
        public string CurrentVirtualPath
        {
            get { return (string)(ViewState["CurrentVirtualPath"] ?? RootVirtualPath); }
            set
            {
                ViewState["CurrentVirtualPath"] = value;
                _currentVirtualDirectory = null;
                SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Gets the currently browsed virtual directory based on <see cref="CurrentVirtualPath"/>.
        /// </summary>
        /// <value>The current virtual directory; or null if it <see cref="CurrentVirtualPath"/> isn't a valid virtual directory.</value>
        public UnifiedDirectory CurrentVirtualDirectory
        {
            get
            {
                if (!String.IsNullOrEmpty(CurrentVirtualPath) && _currentVirtualDirectory == null)
                {
                    _currentVirtualDirectory = HostingEnvironment.VirtualPathProvider.GetDirectory(Uri.UnescapeDataString(CurrentVirtualPath)) as UnifiedDirectory;
                }
                return _currentVirtualDirectory;
            }
        }


        #endregion

        #region Properties in View State

        /// <summary>
        /// Gets or sets the name of the default view.
        /// </summary>
        /// <value>The name of the default view.</value>
        [Category("Appearance"), DefaultValue("Default"), Localizable(false)]
        public string DefaultView
        {
            get { return (string)(ViewState["DefaultView"] ?? "Default"); }
            set { ViewState["DefaultView"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the current view.
        /// </summary>
        /// <value>The name of the current view.</value>
        public string CurrentView
        {
            get { return (string)ViewState["CurrentViewID"] ?? DefaultView; }
            protected set 
            {
                if (!String.Equals(value, (string)ViewState["CurrentViewID"], StringComparison.OrdinalIgnoreCase))
                {
                    PreviousView = CurrentView;
                }
                ViewState["CurrentViewID"] = value; 
            }
        }

        /// <summary>
        /// Gets or sets the name of the previously shown view.
        /// </summary>
        /// <value>The name of the previously shown view.</value>
        public string PreviousView
        {
            get { return (string)ViewState["PreviousViewID"] ?? String.Empty; }
            protected set { ViewState["PreviousViewID"] = value; }
        }

        /// <summary>
        /// Gets the selected files and folders.
        /// </summary>
        /// <value>The selected files and folders.</value>
        public VirtualFileBaseCollection SelectedItems
        {
            get
            {
                if (ViewState["SelectedItems"] == null)
                {
                    ViewState["SelectedItems"] = new VirtualFileBaseCollection();
                }
                return (VirtualFileBaseCollection)ViewState["SelectedItems"];
            }
        }


        /// <summary>
        /// Gets a collection of virtual files or directories to be copied or moved in the next paste operation.
        /// </summary>
        /// <value>A collection containing virtual files and directories.</value>
        public VirtualFileBaseCollection PasteBuffer
        {
            get
            {
                if (ViewState["PasteBuffer"] == null)
                {
                    ViewState["PasteBuffer"] = new VirtualFileBaseCollection();
                }
                return (VirtualFileBaseCollection)ViewState["PasteBuffer"];
            }
        }


        /// <summary>
        /// Determines how the source items will be treated when a paste operation occurs.
        /// </summary>
        public PasteMode PasteMode
        {
            get
            {
                return (ViewState["PasteAction"] != null ? (PasteMode)Enum.Parse(typeof(PasteMode), (string)ViewState["PasteAction"]) : PasteMode.Copy);
            }
            set { ViewState["PasteAction"] = value.ToString(); }
        }


        /// <summary>
        /// Gets or sets the version id of a specific file version if one is selected
        /// </summary>
        public string SelectedFileVersionId
        {
            get { return ViewState[ "SelectedFileVersionID" ] as string; }
            set { ViewState[ "SelectedFileVersionID" ] = value; }
        }

        #endregion

        /// <summary>
        /// Gets the selected file if exactly one file is currently selected.
        /// </summary>
        /// <value>The selected file.</value>
        /// <remarks>If there is no file selected or if several files and/or folders are selected this methods returns null.</remarks>
        public UnifiedFile SingleSelectedFile
        {
            get
            {
                return (SelectedItems.Count == 1 ? SelectedItems[0] as UnifiedFile : null);
            }
        }

        /// <summary>
        /// Gets the selected file or folder if exactly one item is currently selected.
        /// </summary>
        /// <value>The selected item.</value>
        /// <remarks>If no item is selected or if several files and/or folders are selected this methods returns null.</remarks>
        public VirtualFileBase SingleSelectedItem
        {
            get 
            {
                return (SelectedItems.Count == 1 ? SelectedItems[0] : null);
            }
        }

        #region Control Creation

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            // Don't clear state when re-creating the view on post-back.
            LoadView(CurrentView, false);
        }

        /// <summary>
        /// Reads the view configuration for the requested view and populates the control collection.
        /// </summary>
        /// <param name="viewName">Name of the view to load.</param>
        /// <param name="clearState">if set to <c>true</c> the state information for child controls are cleared before re-creation..</param>
        protected virtual void LoadView(string viewName, bool clearState)
        {
            Configuration.FileManagerSection fileManagerConfig = Configuration.FileManagerSection.Section;
            Configuration.ViewElement view = fileManagerConfig.Views.GetView(viewName);

            Controls.Clear();
            if (clearState)
            {
                ClearChildState();
            }

            // The IsPostBack flag of the page is not the same as the post-back of a view.
            // The view NOT a post-back if the state has been cleared (changing to a new view).
            _isPostBackView = Page.IsPostBack && !clearState;

            if (view == null)
            {
                throw new EPiServerException(String.Format(CultureInfo.InvariantCulture, "No view definition found with Name=\"{0}\"", viewName));
            }

            CurrentView = viewName;

            Control userControl = Page.LoadControl(view.FrameworkSource);

            foreach (Configuration.RegionDefinition region in view.Regions.Values)
            {
                IFileManagerRegion regionControl = userControl.FindControl(region.Id) as IFileManagerRegion;
                if (regionControl != null)
                {
                    regionControl.Initialize(this, region.ContentSource);
                }
            }

            Controls.Add(userControl);
        }

        /// <summary>
        /// Gets a value indicating whether the current view is beeing loaded again in response to a post back, 
        /// or if it's beeing loaded because of a view change.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the view is loaded in reponse to a post-back; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsPostBackView
        {
            get { return _isPostBackView; }
        }

        #endregion

        /// <summary>
        /// Clear a selection for file version.
        /// </summary>
        public void ClearFileVersionSelection()
        {
            SelectedFileVersionId = null;
        }

        /// <summary>
        /// Loads the default view.
        /// </summary>
        /// <param name="sender">The sender requesting the view change.</param>
        /// <returns>The <see cref="FileManagerCommandEventArgs"/> object used when loading the view.</returns>
        public virtual FileManagerCommandEventArgs LoadDefaultView(object sender)
        {
            FileManagerCommandEventArgs e = new FileManagerCommandEventArgs(FileManagerCommandName.LoadView, DefaultView);
            RaiseCommand(sender, e);
            return e;
        }


        /// <summary>
        /// Loads the previous view.
        /// </summary>
        /// <param name="sender">The sender requesting the view change.</param>
        /// <returns>The <see cref="FileManagerCommandEventArgs"/> object used when loading the view.</returns>
        public virtual FileManagerCommandEventArgs LoadPreviousView(object sender)
        { 
            FileManagerCommandEventArgs e = new FileManagerCommandEventArgs(FileManagerCommandName.LoadView, PreviousView);
            RaiseCommand(sender, e);
            return e;
        }


        /// <summary>
        /// Raises a file manager command event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An <see cref="EPiServer.Templates.Advanced.FileManager.Core.FileManagerCommandEventArgs"/> object that contains the event data.</param>
        public virtual void RaiseCommand(object sender, FileManagerCommandEventArgs e)
        {
            if (_commandExecuting != null)
            {
                _commandExecuting(sender, e);
            }

            if (e.CancelCommand)
            {
                return;
            }

            ExecuteCommand(sender, e);

            if (_commandExecute != null)
            {
                _commandExecute(sender, e);
            }

            if (_commandExecuted != null)
            {
                _commandExecuted(sender, e);
            }
        }

        /// <summary>
        /// Called by the FileManager control to execute a command.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="EPiServer.Templates.Advanced.FileManager.Core.FileManagerCommandEventArgs"/> instance containing the event data.</param>
        protected virtual void ExecuteCommand(object sender, FileManagerCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case FileManagerCommandName.LoadView:
                    // Loading a new view so the state of the old view should be cleared.
                    LoadView((string)e.CommandArguments, true);
                    break;
                case FileManagerCommandName.SelectFolder:
                    CurrentVirtualPath = (string)e.CommandArguments;
                    ClearFileVersionSelection();
                    break;
                case FileManagerCommandName.CopySelection:
                    PasteBuffer.Clear();
                    SelectedItems.CopyTo(PasteBuffer);
                    PasteMode = PasteMode.Copy;
                    break;
                case FileManagerCommandName.CutSelection:
                    PasteBuffer.Clear();
                    SelectedItems.CopyTo(PasteBuffer);
                    PasteMode = PasteMode.Cut;
                    break;
                case FileManagerCommandName.PasteItems:
                    FileSystemUtility.PasteFiles(PasteBuffer, CurrentVirtualDirectory, PasteMode);
                    // Reset the paste buffer
                    PasteBuffer.Clear();
                    PasteMode = PasteMode.Copy;
                    break;
                case FileManagerCommandName.DeleteSelection:
                    FileSystemUtility.DeleteItems(SelectedItems);
                    SelectedItems.Clear();
                    ClearFileVersionSelection();
                    break;
                case FileManagerCommandName.DeleteVersion:
                    FileSystemUtility.DeleteVersion(SingleSelectedFile as IVersioningFile,  SelectedFileVersionId);
                    ClearFileVersionSelection();
                    break;
                case FileManagerCommandName.RestoreVersion:
                    FileSystemUtility.RestoreVersion(SingleSelectedFile as IVersioningFile, SelectedFileVersionId);
                    ClearFileVersionSelection();
                    break;
                case FileManagerCommandName.CheckoutSelection:
                    FileSystemUtility.CheckoutItems(SelectedItems);
                    ClearFileVersionSelection();
                    break;
                case FileManagerCommandName.UndoCheckoutSelection:
                    FileSystemUtility.UndoCheckoutItems(SelectedItems);
                    ClearFileVersionSelection();
                    break;
            }
        }
    }
}

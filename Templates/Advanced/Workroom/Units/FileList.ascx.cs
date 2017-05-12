#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using System.Web.UI;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    /// <summary>
    /// Presents a list of most recent changed files
    /// </summary>
    [ToolboxData("<{0}:FileList runat=server></{0}:FileList>")]
    public partial class FileList : UserControlBase
    {
        // Used to set rendered file name length
        private const int _allowedFileNameLength = 23;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (String.IsNullOrEmpty(Heading))
            {
                FileListHeading.Visible = false;
            }

            FileListHeading.InnerText = Heading;
           
            if (String.IsNullOrEmpty(RootFolder))
            {
                ErrorMessage.Visible = true;
                return;
            }

            var lastModifiedFiles = FindLastModifiedFiles(RootFolder, MaxCount);
            FileListPanel.Visible = lastModifiedFiles.Count > 0;
            FileListing.DataSource = lastModifiedFiles;
            FileListing.DataBind();
        }

        /// <summary>
        /// Formats the length of the supplied file name.
        /// </summary>
        /// <param name="filename">The filename to format.</param>
        protected static string FormatFileNameLength(string fileName)
        {
            return fileName.Length > _allowedFileNameLength ? String.Format("{0}...", fileName.Substring(0, _allowedFileNameLength)) : fileName;
        }

        /// <summary>
        /// Finds the last modified files.
        /// </summary>
        protected IList<UnifiedFile> FindLastModifiedFiles(string rootFolder, int maxCount)
        {
            var files = new List<UnifiedFile>();
            UnifiedDirectory directory = null;

            try
            {
                directory = HostingEnvironment.VirtualPathProvider.GetDirectory(rootFolder) as UnifiedDirectory;
            }
            catch (UnauthorizedAccessException) {}

            // If directory not found or access not granted for directory 
            // return empty list
            if (directory == null)
            {
                ShowErrorMessage(Translate("/workroom/filelist/errormessage"));
                return files;
            }

            UnifiedSearchQuery query = new UnifiedSearchQuery 
            {
                ModifiedFrom = DateTime.MinValue, 
                ModifiedTo = DateTime.Now, 
                Path = rootFolder,
                FreeTextQuery = String.Empty
            };

            UnifiedSearchHitCollection hits;
            try
            {
                hits = directory.Search(query);
            }
            catch (IOException)
            {
                ShowErrorMessage(Translate("/workroom/filelist/errormissingindex"));
                return files;
            }

            for (int i = 0; i < hits.Count; i++)
            {
                try
                {
                    files.Add((UnifiedFile)HostingEnvironment.VirtualPathProvider.GetFile(hits[i].Path));
                }
                catch (UnauthorizedAccessException)
                {
                    // If user has limited access right we still want to show files with access granted.
                }
            }
            files.Sort(new FileModifiedComparer());

            if (files.Count > maxCount)
            {
                files.RemoveRange(maxCount - 1, files.Count - maxCount);
            }
            return files;
        }

        /// <summary>
        /// Sets an error message to the ErrorMessage literal control shows the control.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        private void ShowErrorMessage(string message)
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text = message;
        }

        #region Layout and Rendering Properties

        /// <summary>
        /// Gets or sets the CSS class for the File List.
        /// </summary>
        public string CssClass
        {
            get
            {
                return (string)(ViewState["CssClass"] ?? String.Empty);
            }
            set
            {
                ViewState["CssClass"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the heading for the File List.
        /// </summary>
        public string Heading
        {
            get
            {
                return (string)(ViewState["Heading"] ?? String.Empty);
            }
            set
            {
                ViewState["Heading"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of files to show.
        /// </summary>
        /// <value>Number of files. Default is 5.</value>
        public int MaxCount
        {
            get
            {
                return (int)(ViewState["MaxCount"] ?? 5);
            }
            set
            {
                ViewState["MaxCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of files to show.
        /// </summary>
        /// <value>Number of files. Default is 5</value>
        public string RootFolder
        {
            get
            {
                return (string)(ViewState["RootFolder"] ?? String.Empty);
            }
            set
            {
                ViewState["RootFolder"] = value;
            }
        }

        #endregion

        protected static string GetFileExtension(UnifiedFile file)
        {
            return String.IsNullOrEmpty(file.Extension) ? String.Empty : file.Extension.Substring(1).ToLowerInvariant();
        }
    }

    /// <summary>
    /// Compares UnifiedFiles for when they are changed 
    /// </summary>
    public class FileModifiedComparer : IComparer<UnifiedFile>
    {
        #region IComparer<UnifiedFile> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(UnifiedFile x, UnifiedFile y)
        {
            return y.Changed.CompareTo(x.Changed);
        }

        #endregion
    }
}
#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// Base class for user controls loaded into regions of the <see cref="FileManager"/>.
    /// </summary>
    public class ContentUserControlBase : EPiServer.UserControlBase
    {
        private IFileManagerRegion _parentRegion;

        /// <summary>
        /// Gets a reference to the file manager FileManager hosting this control.
        /// </summary>
        /// <value>The file manager FileManager; or null if this control isn not loaded by a file manager FileManager.</value>
        public FileManagerControl FileManager
        {
            get
            {
                if (_parentRegion == null)
                {
                    Control c = Parent;
                    while (c != null && !(c is IFileManagerRegion))
                    {
                        c = c.Parent;
                    }

                    _parentRegion = c as IFileManagerRegion;
                }

                return _parentRegion != null ? _parentRegion.FileManager : null;
            }
        }

        /// <summary>
        /// Gets the file size in kilobytes or megabytes.
        /// </summary>
        /// <param name="bytes">The size of the file.</param>
        /// <returns>A string with the size in kilobytes or megabytes.</returns>
        public static string GetFileSizeString(long bytes)
        {
            if (bytes == 0)
            {
                return "0 kB";
            }
            else if (bytes < 1024)
            {
                return "1 kB";
            }
            else if (bytes < (1024 * 1024))
            {
                return Convert.ToInt32(bytes / 1024) + " kB";
            }
            else
            {
                return Convert.ToInt32(bytes / (1024 * 1024)) + " MB";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current view is beeing loaded again in response to a post back, 
        /// or if it's beeing loaded because of a view change.
        /// </summary>
        /// <remarks>
        /// If this control isn't used from a <see cref="FileManagerControl"/> the returned value is the same as <see cref="System.Web.UI.UserControl.IsPostBack"/>.
        /// </remarks>
        /// <value>
        /// 	<c>true</c> if this instance is post back view; otherwise, <c>false</c>.
        /// </value>
        protected bool IsPostBackView
        {
            get { return (FileManager != null ? FileManager.IsPostBackView : IsPostBack); }
        }
    }
}

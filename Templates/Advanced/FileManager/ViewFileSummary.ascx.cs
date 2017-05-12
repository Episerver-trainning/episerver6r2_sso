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
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// A user control that shows detailed information for a file.
    /// </summary>
    public partial class ViewFileSummary : ContentUserControlBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EditButton.Visible = !(FileManager.SingleSelectedFile is NativeFile) &&
                FileSystemUtility.CanEdit(FileManager.SingleSelectedFile);
            linkedPagesList.DataBind();
            SetupMetaData();
        }

        private void SetupMetaData()
        {
            UnifiedFile selectedFile = FileManager.SingleSelectedFile;

            if (selectedFile.Summary != null)
            {
                // Sort the Dictionary entries by key name and render.
                SortedList sorted = new SortedList(selectedFile.Summary.Dictionary);
                foreach (DictionaryEntry entry in sorted)
                {
                    // New table row.
                    TableRow newRow = new TableRow();
                    PropertyTable.Rows.Add(newRow);

                    // Key cell.
                    AddLabledCellToRow(newRow,
                        LanguageManager.Instance.TranslateFallback("/filemanager/editfilesummary/input" + entry.Key.ToString().ToLowerInvariant(), 
                        entry.Key.ToString()) + ":");

                    // Value cell.
                    AddLabledCellToRow(newRow,
                        entry.Value != null ? Server.HtmlEncode(entry.Value.ToString()) : String.Empty);
                }
            }
        }

        private static void AddLabledCellToRow(TableRow row, string cellText)
        {
            Label label = new Label();
            label.Text = cellText;

            TableCell cell = new TableCell();
            cell.VerticalAlign = VerticalAlign.Top;
            cell.Controls.Add(label);

            row.Cells.Add(cell);
        }

        /// <summary>
        /// Handles the Click event of the EditSummaryButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BackButton_Click(object sender, EventArgs e)
        {
            FileManager.LoadDefaultView(this);
        }
        
        /// <summary>
        /// Handles the Click event of the EditSummaryButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EditSummaryButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(FileManager.SingleSelectedFile.Parent.CustomFileSummaryVirtualPath))
            {
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.LoadView, "EditUnifiedSummary"));
            }
            else 
            {
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.LoadView, "EditCustomSummary"));
            }
        }

        #region Accessors
        /// <summary>
        /// Selected file size as string property.
        /// </summary>
        protected string SizeString
        {
            get { return GetFileSizeString(FileManager.SingleSelectedFile.Length); }
        }

        /// <summary>
        /// Selected file creation date string.
        /// </summary>
        protected string CreatedString
        {
            get { return FileManager.SingleSelectedFile.Created.ToLongDateString(); }
        }

        /// <summary>
        /// Selected file modified date string.
        /// </summary>
        protected string ModifiedString
        {
            get { return FileManager.SingleSelectedFile.Changed.ToLongDateString(); }
        }

        /// <summary>
        /// Gets the legend text.
        /// </summary>
        /// <value>The legend text.</value>
        public string Legend
        {
            get 
            {
                return String.Format("{0} <strong>{1}</strong>", Translate("/filemanager/filesummaryheading"), FileManager.SingleSelectedFile.Name);
            }
        }
	
        /// <summary>
        /// A collection of pages using this file.
        /// </summary>
        protected IList<SoftLink> LinkingPages
        {
            get 
            { 
                return SoftLink.Load(FileManager.SingleSelectedFile);
            }
        }

        /// <summary>
        /// Gets a display string for a softlink.
        /// </summary>
        /// <param name="link">The SoftLink.</param>
        /// <returns>A string containing a page name and id.</returns>
        protected static string GetLinkingPageString(object link)
        {

            SoftLink softLink = (SoftLink)link;
            return String.Format("{0} [{1}]", DataFactory.Instance.GetPage(softLink.OwnerPageLink).PageName, softLink.OwnerPageLink.ID);
            
        }

        #endregion
    }
}
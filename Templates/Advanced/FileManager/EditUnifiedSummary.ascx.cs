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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using EPiServer.Templates.Advanced.FileManager.Core;
using EPiServer.Web.Hosting;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.FileManager
{
    /// <summary>
    /// Filemanager user control for modifying an <see cref="IUnifiedSummary"/> for the currently selected file.
    /// </summary>
    public partial class EditUnifiedSummary : ContentUserControlBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBackView && FileManager.SingleSelectedFile != null)
            {
                PopulateFormFields(FileManager.SingleSelectedFile.Summary);
            }
            if (!FileSystemUtility.CanEdit(FileManager.SingleSelectedFile))
            {
                SaveButton.Enabled = false;
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
        }

        /// <summary>
        /// Populates the form with data from the file summary.
        /// </summary>
        /// <param name="summary">A file summary to get form data from.</param>
        public void PopulateFormFields(IUnifiedSummary summary)
        {
            if (summary == null)
            {
                return;
            }
            Author.Text = summary.Author;
            Category.Text = summary.Category;
            Comments.Text = summary.Comments;
            Keywords.Text = summary.Keywords;
            Subject.Text = summary.Subject;
            Title.Text = summary.Title;
        }

        /// <summary>
        /// Loads the post data into a file summary object.
        /// </summary>
        /// <param name="summary">The summary object to populate with posted data.</param>
        public void LoadPostData(IUnifiedSummary summary)
        {
            if (summary == null)
            {
                return;
            }
            summary.Author = Author.Text;
            summary.Category = Category.Text;
            summary.Comments = Comments.Text;
            summary.Keywords = Keywords.Text;
            summary.Subject = Subject.Text;
            summary.Title = Title.Text;
        }

        /// <summary>
        /// Handles the Click event of the Save button to persist changes in the file summary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            LoadPostData(FileManager.SingleSelectedFile.Summary);
            try
            {
                FileSystemUtility.UpdateFileSummary(FileManager.SingleSelectedFile, null);
                FileManager.RaiseCommand(this, new FileManagerCommandEventArgs(FileManagerCommandName.LoadView, FileManager.PreviousView));
            }
            catch (FileIsCheckedOutException)
            {
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/cannotchange")));
                return;
            }
            catch (UnauthorizedAccessException)
            { 
                Page.Validators.Add(new StaticValidator(Translate("/filemanager/errormessage/insufficientaccessmetadata")));
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel button to return to the previous view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            FileManager.RaiseCommand(this, new FileManagerCommandEventArgs("LoadView", "Default"));
        }
    }
}
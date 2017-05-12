#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.ComponentModel;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Templates.AlloyTech;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    /// <summary>
    /// Renders the content of a string property and buttons to edit the property if the user has publish permissions to the current page
    /// </summary>
    public partial class EditableContent : UserControlBase
    {
        /// <summary>
        /// Handles load event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LinkButtonEdit.Visible = Membership.QueryDistinctMembershipLevel(CurrentPage, MembershipLevels.Administer);
            PageBase.DataBind();
        }

        /// <summary>
        /// Gets or sets the name of the property to show edit.
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Bindable(true)]
        public string PropertyName { get; set;}

        /// <summary>
        /// Gets or sets title of the edit button
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Bindable(true)]
        public string EditButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets title of the save button
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Bindable(true)]
        public string SaveButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets title of the cancel button
        /// </summary>
        [Category("Appearance"), DefaultValue(""), Bindable(true)]
        public string CancelButtonTitle { get; set; }

        #region Button Click Event Handlers

        /// <summary>
        /// Stst the content control to editable state and shows save and cancel buttons.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LinkButtonEdit_Click(object sender, EventArgs e)
        {
            TextEditor.Text = CurrentPage[PropertyName] as string;
            MultiviewMain.SetActiveView(ViewEdit);
        }

        /// <summary>
        /// Saves the updated content and publishes the current page. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            CurrentPage = CurrentPage.CreateWritableClone();
            CurrentPage[PropertyName] = TextEditor.Text.ToSafeString();
            DataFactory.Instance.Save(CurrentPage, SaveAction.Publish);
            Response.Redirect(CurrentPage.LinkURL);
        }

        /// <summary>
        /// Cancels the editable state of the property and restores button visibility state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            MultiviewMain.SetActiveView(ViewShow);
        }

        #endregion

    }



}
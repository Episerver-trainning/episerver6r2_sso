#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Personalization;
using EPiServer.Templates.Advanced.Workroom.Core;

using Membership=EPiServer.Templates.Advanced.Workroom.Core.Membership;
using log4net;
using EPiServer.Templates.Advanced.Workroom.Core.Notification;

namespace EPiServer.Templates.Advanced.Workroom.Units
{

    /// <summary>
    /// Implements user search and the possibility to modify the workroom membership level for listed users.
    /// </summary>
    public partial class MembershipAdministration : UserControlBase
    {
        private static ILog _log = LogManager.GetLogger(typeof(MembershipAdministration));

        // A collection of modified members. Will be stored in ViewState when changing pages in the user list.
        private List<MembershipEntry> _modifiedMembers = new List<MembershipEntry>();        
        private PageData _workroomStartPage;
        
        /// <summary>
        /// Enumeration of available modes for this control
        /// </summary>
        private enum ViewMode
        {
            /// <summary>
            /// Only View the members with no editable fields
            /// </summary>
            ViewMembers,
            /// <summary>
            /// Edit the membership level of current members
            /// </summary>
            EditMembers,
            /// <summary>
            /// Search for and add new members
            /// </summary>
            AddMembers,
            /// <summary>
            /// Remove member
            /// </summary>
            DeleteMember
        }

        /// <summary>
        /// Gets or sets the control active mode.
        /// </summary>
        /// <value>The active mode.</value>
        private ViewMode ActiveMode
        {
            get { return (ViewMode)(ViewState["ActiveMode"] ?? ViewMode.ViewMembers); }
            set { ViewState["ActiveMode"] = value; }
        }

        /// <summary>
        /// Gets or sets the selected users to delete.
        /// </summary>
        /// <value>The users to delete.</value>
        protected IList<MembershipEntry> UsersToDelete
        {
            get
            {
                return ViewState["UsersToDelete"] as List<MembershipEntry> ?? new List<MembershipEntry>();
            }
            set
            {
                ViewState["UsersToDelete"] = value;
            }
        }

        /// <summary>
        /// Get the <see cref="EPiServer.Core.PageData"/> for the workroom start page.
        /// </summary>
        private PageData WorkroomStartPage
        {
            get
            {
                if (_workroomStartPage == null && Page is WorkroomPageBase)
                {
                    _workroomStartPage = ((WorkroomPageBase)Page).WorkroomStartPage;
                }
                return _workroomStartPage;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                SetViewMode(ViewMode.ViewMembers);
            }
        }

        /// <summary>
        /// Handles the DataBinding event of the MembershipLevel control in membership column of the the UserList gridview control.
        /// Used to populate the membership level drop down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void MembershipLevel_DataBinding(object sender, EventArgs e)
        {
            Control container = (Control)sender;

            // We're inside a data row which is a data item container.
            IDataItemContainer dataContainer = container.BindingContainer as IDataItemContainer;

            if (dataContainer != null)
            {
                // Get the entry we're binding against.
                MembershipEntry userEntry = (MembershipEntry)dataContainer.DataItem;

                if ((ActiveMode == ViewMode.EditMembers && userEntry.Name != Context.User.Identity.Name) || (ActiveMode == ViewMode.AddMembers && userEntry.Membership == MembershipLevels.None))
                {
                    DropDownList membershipDropdown = (DropDownList)container.FindControl("MembershipDropdown");

                    foreach (MembershipLevels level in Enum.GetValues(typeof(MembershipLevels)))
                    {
                        membershipDropdown.Items.Add(new ListItem(Membership.TranslateMembershipLevel(level), userEntry.Name + ":" + level.ToString()));
                    }

                    // Check if the user has been modified but not saved.
                    MembershipEntry modifiedMembership;
                    int matchIndex = _modifiedMembers.IndexOf(userEntry);
                    if (matchIndex != -1)
                    {
                        modifiedMembership = _modifiedMembers[matchIndex];
                    }
                    else
                    {
                        modifiedMembership = userEntry;
                    }

                    membershipDropdown.SelectedValue = userEntry.Name + ":" + modifiedMembership.Membership.ToString();
                    membershipDropdown.Visible = true;
                }
                else
                {
                    Literal membershipLiteral = (Literal)container.FindControl("MembershipLiteral");
                    membershipLiteral.Text = userEntry.MembershipLocalName;
                    membershipLiteral.Visible = true;
                }
            }
        }

        /// <summary>
        /// Handles the DataBinding event of the DeleteUserSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUserSelection_DataBinding(object sender, EventArgs e)
        {
            Control container = (Control)sender;
            IDataItemContainer dataContainer = container.BindingContainer as IDataItemContainer;
            if (dataContainer != null)
            {
                MembershipEntry member = (MembershipEntry)dataContainer.DataItem;
                CheckBox checkBox = (CheckBox)container.FindControl("DeleteUserSelection");
                checkBox.Checked = UsersToDelete.Contains(member);
                checkBox.Enabled = ActiveMode == ViewMode.EditMembers && member.Name != Context.User.Identity.Name;
            }
        }

        /// <summary>
        /// Handles the DataBinding event of the Email control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Email_DataBinding(object sender, EventArgs e)
        {
            Control container = (Control)sender;
            IDataItemContainer dataContainer = container.BindingContainer as IDataItemContainer;
            if (dataContainer != null)
            {
                MembershipEntry member = (MembershipEntry)dataContainer.DataItem;
                HyperLink link = (HyperLink)container.FindControl("UserEmail");
                link.NavigateUrl = String.Format("mailto:{0}", member.Email);
            }
        }

        /// <summary>
        /// Shows or hides the delete user selection column.
        /// </summary>
        private void SetupDeleteUserSelection()
        {
            UserList.Columns[0].Visible = ActiveMode == ViewMode.EditMembers || ActiveMode == ViewMode.DeleteMember;
        }
        

        #region Control Event Handlers

        /// <summary>
        /// Updates the state of the control (visible buttons etc.) according to the new View Mode.
        /// </summary>
        /// <param name="viewMode">The new view mode.</param>
        private void SetViewMode(ViewMode viewMode)
        {
            bool isAdministrator = Membership.QueryDistinctMembershipLevel(WorkroomStartPage, MembershipLevels.Administer);
            if (!isAdministrator)
            {
                viewMode = ViewMode.ViewMembers;
            }
            
            ActiveMode = viewMode;
            switch (viewMode)
            {
                case ViewMode.ViewMembers:
                    ToolBarButtons.Visible = isAdministrator;
                    PersistButtons.Visible = false;
                    ConfirmDeleteButtons.Visible = false;
                    SearchDialog.Visible = false;
                    ConfirmDeleteButtons.Visible = false;
                    DeleteButton.Visible = false;
                    DeleteUserDialog.Visible = false;
                    MemberListHeading.Text = Translate("/workroom/membershipmanager/viewmembersheading");
                    SendNotificationsConfirmation.Visible = false;
                    break;
                case ViewMode.AddMembers:
                    ToolBarButtons.Visible = false;
                    PersistButtons.Visible = true;
                    DeleteButton.Visible = false;
                    SearchDialog.Visible = true;
                    ConfirmDeleteButtons.Visible = false;
                    DeleteButton.Visible = false;
                    DeleteUserDialog.Visible = false;
                    MemberListHeading.Text = Translate("/workroom/membershipmanager/searchresultheading");
                    FillInviteMembershipDropDown();
                    SendNotificationsConfirmation.Visible = true;
                    SendOkPanel.Visible = false;
                    SendErrorPanel.Visible = false;
                    SendNotificationsConfirmation.Visible = false;
                    break;
                case ViewMode.EditMembers:
                    ToolBarButtons.Visible = false;
                    PersistButtons.Visible = true;
                    SearchDialog.Visible = false;
                    DeleteButton.Visible = true;
                    ConfirmDeleteButtons.Visible = false;
                    DeleteUserDialog.Visible = false;
                    MemberListHeading.Text = Translate("/workroom/membershipmanager/managemembersheading");
                    SendNotificationsConfirmation.Visible = true;
                    break;
                case ViewMode.DeleteMember:
                    ToolBarButtons.Visible = false;
                    PersistButtons.Visible = false;
                    DeleteButton.Visible = false;
                    ConfirmDeleteButtons.Visible = true;
                    DeleteUserDialog.Visible = true;
                    SearchDialog.Visible = false;
                    MemberListHeading.Text = Translate("/workroom/membershipmanager/managemembersheading");
                    SendOkPanel.Visible = false;
                    SendErrorPanel.Visible = false;
                    SendNotificationsConfirmation.Visible = true;
                    break;
            }

            //Clear search criteria
            UserName.Text = string.Empty;
            Email.Text = string.Empty;

            // Always clear the collection of modified members, 
            // this is only for persisting modifications between changes in paging.
            _modifiedMembers.Clear();
            SetupDeleteUserSelection();
            UserList.PageIndex = 0;
            UserList.DataBind();
        }

        /// <summary>
        /// If the Save and Removo buttons should apeear
        /// </summary>
        private bool IsEditable
        {
            get
            {
                var rows = UserList.Rows.OfType<GridViewRow>();
                var dropdowns = rows.Select(row => row.FindControl("MembershipDropdown") as DropDownList);
                return dropdowns.Any(dropdown => (dropdown != null) && dropdown.Visible);
            }
        }

        /// <summary>
        /// Handles the Click event of the EditMembers button.
        /// Shows all existing members with a permission drop down.
        /// </summary>
        protected void EditMembers_Click(object sender, EventArgs e)
        {
            SetViewMode(ViewMode.EditMembers);
            SaveMembership.Visible = IsEditable;
            DeleteUserButton.Visible = SaveMembership.Visible;
            SendNotificationsConfirmation.Visible = SaveMembership.Visible;
        }

        /// <summary>
        /// Handles the Click event of the AddMembers button to enable edit of current members in the workroom.
        /// </summary>
        protected void AddMembers_Click(object sender, EventArgs e)
        {
            SetViewMode(ViewMode.AddMembers);
            SaveMembership.Visible = IsEditable;
            DeleteUserButton.Visible = SaveMembership.Visible;
            SendNotificationsConfirmation.Visible = SaveMembership.Visible;
        }

        /// <summary>
        /// Handles the Click event of the CancelEdit to revert back to view mode of current members.
        /// </summary>
        protected void CancelEdit_Click(object sender, EventArgs e)
        {
            SetViewMode(ViewMode.ViewMembers);
        }

        /// <summary>
        /// Handles the Click event of the SaveMembership control to save the changes in membership.
        /// </summary>
        /// <param name="sender">The Save button.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveMembership_Click(object sender, EventArgs e)
        {
            if (_modifiedMembers.Count > 0 && Membership.QueryDistinctMembershipLevel(WorkroomStartPage, MembershipLevels.Administer))
            {
                PageData page = WorkroomStartPage.CreateWritableClone();
                Membership.ModifyPermission(page, _modifiedMembers);
                DataFactory.Instance.Save(page, SaveAction.Publish);
                if (SendNotificationsButton.Checked)
                {
                    NotifyUsersByEmailAboutChangedPermissions(_modifiedMembers);
                }
            }
            _modifiedMembers.Clear();
            SetViewMode(ViewMode.ViewMembers);
            DataBindChildren();
        }

        /// <summary>
        /// Handles the Click event of the SearchUsers control to databind the user list with search result.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SearchUsers_Click(object sender, EventArgs e)
        {
            // Make sure to clear any membership modifications before showing the new search result.
            _modifiedMembers.Clear();
            UserList.PageIndex = 0;
            UserList.DataBind();

            //Hide 'Save' button if there is empty result.
            SaveMembership.Visible = IsEditable;
            SendNotificationsConfirmation.Visible = SaveMembership.Visible;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the membershipDropdown control.
        /// When a membership level changes this is stored in the private collection _modifiedMembers, which is saved when the save button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void MembershipDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dropDown = (DropDownList)sender;
            string[] userValuePair = dropDown.SelectedValue.Split(':');
            MembershipEntry member = new MembershipEntry(userValuePair[0], (MembershipLevels)Enum.Parse(typeof(MembershipLevels), userValuePair[1]));
            if (_modifiedMembers.Contains(member))
            {
                _modifiedMembers.Remove(member);
            }
            _modifiedMembers.Add(member);
        }

        /// <summary>
        /// Executed when the Membership ObjectDataSource is selecting to decide whether to show 
        /// only existing members or a search result from all available users.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs"/> instance containing the event data.</param>
        protected void MemberObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["onlyExistingMembers"] = (ActiveMode != ViewMode.AddMembers);
        }

        /// <summary>
        /// Handles the Click event of the SendInvitation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SendInvitation_Click(object sender, EventArgs e)
        {
            SendInvitationToUser();
        }

        /// <summary>
        /// Handles the Click event of the DeleteUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUser_Click(object sender, EventArgs e)
        {
            List<MembershipEntry> deletedMembers = new List<MembershipEntry>();
            foreach (GridViewRow row in UserList.Rows)
            {
                CheckBox checkBox = row.FindControl("DeleteUserSelection") as CheckBox;
                if (checkBox != null && checkBox.Checked)
                {
                    MembershipEntry member = new MembershipEntry(UserList.DataKeys[row.RowIndex].Value.ToString(), MembershipLevels.None);
                    deletedMembers.Add(member);
                }
            }
            if (deletedMembers.Count > 0)
            {
                UsersToDelete = deletedMembers;
                SetViewMode(ViewMode.DeleteMember);
            }
            else
            {
                UsersToDelete = null;
                SetViewMode(ViewMode.ViewMembers);
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteUserConfirm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUserConfirm_Click(object sender, EventArgs e)
        {
            IList<MembershipEntry> deletedMembers = UsersToDelete;
            if (deletedMembers.Count > 0 && Membership.QueryDistinctMembershipLevel(WorkroomStartPage, MembershipLevels.Administer))
            {
                PageData page = WorkroomStartPage.CreateWritableClone();
                Membership.ModifyPermission(page, deletedMembers);
                DataFactory.Instance.Save(page, SaveAction.Publish);
                if (SendNotificationsButton.Checked)
                {
                    NotifyUsersByEmailAboutChangedPermissions(deletedMembers);
                }
            }
            UsersToDelete = null;
            SetViewMode(ViewMode.ViewMembers);
            DataBindChildren();
        }

        /// <summary>
        /// Handles the Click event of the DeleteUserCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUserCancel_Click(object sender, EventArgs e)
        {
            UsersToDelete = null;
            SetViewMode(ViewMode.ViewMembers);
        }


        #endregion

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState["ModifiedMembers"] = _modifiedMembers;
            return base.SaveViewState();
        }

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState"/> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object"/> that represents the user control state to be restored.</param>
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _modifiedMembers = ViewState["ModifiedMembers"] as List<MembershipEntry>;
        }

        /// <summary>
        /// Notifies users about changed permissions
        /// </summary>
        private void NotifyUsersByEmailAboutChangedPermissions(IList<MembershipEntry> members)
        {
            if (String.IsNullOrEmpty(EPiServerProfile.Current.Email))
            {
                NotificationErrorPanel.Text = Translate("/workroom/membershipmanager/unabletosendnotification") + " " +
                    Translate("/workroom/membershipmanager/inviteunfiledsenderemail");
                NotificationErrorPanel.Visible = true;
                return;
            }
            NotificationSender notificationSender;
            try
            {
                notificationSender = new MembershipStatusChangeNotificationSender(EPiServerProfile.Current.Email, EPiServerProfile.Current.FirstName,
                                                                                  EPiServerProfile.Current.LastName, WorkroomPageBase.CreateExternalLink(WorkroomStartPage));
            }
            catch (FormatException ex)
            {
                NotificationErrorPanel.Text = Translate("/workroom/membershipmanager/unabletosendnotification") + " " + ex.Message;
                NotificationErrorPanel.Visible = true;
                return;
            }
            
            foreach (MembershipEntry member in members)
            {
                try
                {
                    notificationSender.Send(Page as WorkroomPageBase, member);
                }
                catch (Exception ex)
                {
                    NotificationErrorPanel.Text = Translate("/workroom/membershipmanager/unabletosendnotification") + " " + ex.Message;
                    NotificationErrorPanel.Visible = true;
                    _log.Error(String.Format("Unable to send email notification about changed membership status to workroom member {0}.", member.Name), ex);
                }
            }
        }


        /// <summary>
        /// Fills the invite membership drop down.
        /// </summary>
        private void FillInviteMembershipDropDown()
        {
            if (InviteMembershipDropDown.Items.Count == 0)
            {
                foreach (MembershipLevels level in Enum.GetValues(typeof(MembershipLevels)))
                {
                    InviteMembershipDropDown.Items.Add(new ListItem(Membership.TranslateMembershipLevel(level), level.ToString()));
                }
            }
        }

        /// <summary>
        /// Sends the invitation to user.
        /// </summary>
        private void SendInvitationToUser()
        {
            string invitationPageLink = GetInvitationPageLink();

            SendOkPanel.Visible = false;
            SendErrorPanel.Visible = false;
            if (String.IsNullOrEmpty(EPiServerProfile.Current.Email))
            {
                SendErrorPanel.Visible = true;
                SendInvitationErrorMessage.Text = Translate("/workroom/membershipmanager/inviteunfiledsenderemail");
                return;
            }

            if (InviteMembershipDropDown.SelectedValue.Equals(MembershipLevels.None.ToString()))
            {
                SendErrorPanel.Visible = true;
                SendInvitationErrorMessage.Text = Translate("/workroom/membershipmanager/invitewithoutstatus");
                return;
            }

            NotificationSender invitationSender = new InvitationSender(EPiServerProfile.Current.Email, EPiServerProfile.Current.FirstName,
                EPiServerProfile.Current.LastName, invitationPageLink);
            try
            {
                MembershipLevels memberLevel = (MembershipLevels)Enum.Parse(typeof(MembershipLevels), InviteMembershipDropDown.SelectedValue);
                invitationSender.Send(Page as WorkroomPageBase, InviteUserName.Text, InviteEmail.Text, memberLevel);
                SendOkPanel.Visible = true;
            }
            catch (Exception ex)
            {
                SendErrorPanel.Visible = true;
                SendInvitationErrorMessage.Text = ex.Message;
            }            
        }

        /// <summary>
        /// Gets the invitation page link.
        /// </summary>
        /// <returns></returns>
        private string GetInvitationPageLink()
        {
            PageData userProfilePage = GetChildren(WorkroomStartPage.ParentLink).FirstOrDefault(page => String.Equals(page.PageTypeName, WorkroomPageBase.UserProfilePageTypeName));
            return userProfilePage != null ? WorkroomPageBase.CreateExternalLink(userProfilePage) : String.Empty;
        }
    }
}

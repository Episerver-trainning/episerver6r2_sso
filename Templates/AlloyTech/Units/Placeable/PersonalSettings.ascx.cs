#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.Security;

using EPiServer.Personalization;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    public partial class PersonalSettings : UserControlBase
    {
        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                HideFields();
            }
            InitializeFields();
        }

        /// <summary>
        /// If the user isn't logged on we hide the panel containing the form.
        /// If the user is logged on the data from the user is loaded to the form.
        /// </summary>
        protected virtual void InitializeFields()
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                CreateEditUser.Visible = false;
                return;
            }
            if (!IsPostBack)
            {
                LoadUserData();
            }
        }

        /// <summary>
        /// Checks that the user is logged on. If he is, we save the user. If not we add a message and hides the panel containing the form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void ApplyButton_Click(object sender, EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                ErrorOccurred(Translate("/templates/personalsettings/expiredidentity"));
                CreateEditUser.Visible = false;
                return;
            }
            SaveUser();
        }

        /// <summary>
        /// Saves the current profile.
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveUser()
        {
            if (!Page.IsValid)
            {
                return false;
            }

            try
            {
                if (Email.Text != EPiServerProfile.Current.Email && 
                    Membership.FindUsersByEmail(Email.Text).Count > 0)
                {
                    ErrorOccurred(Translate("/templates/personalsettings/validation/emailexists"));
                    return false;
                }

                SetPostedData();
                EPiServerProfile.Current.Save();
                SaveFailed.Visible = false;
                SaveSucceeded.Visible = true;
                CreateEditUser.Visible = false;
            }
            catch (Exception error)
            {
                ErrorOccurred(error.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Displays any error that occurs and hides the panel containing the form.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        protected void ErrorOccurred(string errorMessage)
        {
            SaveFailed.Visible = true;
            SaveSucceeded.Visible = false;
            ErrorMessage.Text = errorMessage;
        }

        /// <summary>
        /// Loads the user data from the current profile to the form.
        /// </summary>
        private void LoadUserData()
        {
            if (EPiServerProfile.Enabled)
            {
                // Get preset properties
                EPiServerProfile user = EPiServerProfile.Current;
                Company.Text = user.Company;
                FirstName.Text = user.FirstName;
                LastName.Text = user.LastName;
                Title.Text = user.Title;
                Country.Text = user.Country;

                MembershipUser membershipUser = Membership.GetUser(user.UserName);
                if (membershipUser != null && !String.IsNullOrEmpty(membershipUser.Email))
                {
                    Email.Text = membershipUser.Email;
                }
                else
                {
                    Email.Text = user.Email;
                }
            }
        }

        /// <summary>
        /// Adds the data from the form to the current profile.
        /// </summary>
        protected void SetPostedData()
        {
            EPiServerProfile user = EPiServerProfile.Current;

            user.Company = StripAnyTags(Company.Text);
            user.Email = StripAnyTags(Email.Text);
            user.FirstName = StripAnyTags(FirstName.Text);
            user.LastName = StripAnyTags(LastName.Text);
            user.Title = StripAnyTags(Title.Text);
            user.Country = StripAnyTags(Country.Text);

            MembershipUser membershipUser = Membership.GetUser(user.UserName);
            if (EPiServer.Security.ProviderCapabilities.IsSupported(membershipUser.ProviderName, EPiServer.Security.ProviderCapabilities.Action.Update))
            {
                membershipUser.Email = user.Email;
                Membership.UpdateUser(membershipUser);
            }

        }

        /// <summary>
        /// Strips any tags by removing any great then or less then caracters from the string.
        /// </summary>
        /// <param name="s">A string that should be stripped from tags.</param>
        /// <returns>A string without tags</returns>
        protected static string StripAnyTags(string input)
        {
            return input == null ? null : input.Replace("<", "").Replace(">", "");
        }

        /// <summary>
        /// Hides any fields that are set up to be disabled in edit mode.
        /// </summary>
        protected void HideFields()
        {
            CountryRow.Visible = !IsValue("DisableCountry");
            CompanyRow.Visible = !IsValue("DisableCompany");
            EmailRow.Visible = !IsValue("DisableEmail");
            FirstNameRow.Visible = !IsValue("DisableFirstname");
            LastNameRow.Visible = !IsValue("DisableLastname");
            TitleRow.Visible = !IsValue("DisableTitle");

            //Hide 'Save' button if there is nothing to edit
            if (!CountryRow.Visible &&
                !CompanyRow.Visible &&
                !EmailRow.Visible &&
                !FirstNameRow.Visible &&
                !LastNameRow.Visible &&
                !TitleRow.Visible)
            {
                ApplyButton.Visible = false;
            }
        }
    }
}
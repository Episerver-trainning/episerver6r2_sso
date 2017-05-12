#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer.Personalization;
using EPiServer.Security;
using EPiServer.Templates.Advanced.Workroom.Core;
using Membership=System.Web.Security.Membership;
using EPiServer.Templates.Advanced.Workroom.Core.Notification;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// Page for updating user profile
    /// </summary>
    public partial class UserProfile : WorkroomPageBase
    {
        /// <summary>
        /// URL parameter in request
        /// </summary>
        private const string urlParameter = "returnUrl";

        /// <summary>
        /// Invitation ticket for user
        /// </summary>
        private InvitationTicket Ticket { get; set; }

        /// <summary>
        /// User name of user to change
        /// </summary>
        private string UserName { get; set; }

        /// <summary>
        /// Profile to change
        /// </summary>
        private EPiServerProfile Profile { get; set; }

        /// <summary>
        /// Gets whether redirect to the user profile page enabled or not
        /// </summary>
        protected override bool RedirectEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Refirect to the workroom page
        /// </summary>
        private void Redirect()
        {
            var url = Ticket == null ? Request[urlParameter] : DataFactory.Instance.GetPage(Ticket.WorkroomStartPage).LinkURL;
            if (!string.IsNullOrEmpty(url))
            {
                Response.Redirect(HttpUtility.UrlDecode(url));
            }
        }

        /// <summary>
        /// Initialize ticket and user name
        /// </summary>
        private void Initialize()
        {
            var ticketParameter = Request[InvitationSender.TicketParameter];
            Ticket = string.IsNullOrEmpty(ticketParameter) ? null : InvitationTicket.Decrypt(ticketParameter);

            if ((Ticket != null) && !string.IsNullOrEmpty(Ticket.Email))
            {
                var enumerator = Membership.FindUsersByEmail(Ticket.Email).GetEnumerator();
                UserName = enumerator.MoveNext() ? ((MembershipUser)enumerator.Current).UserName : string.Empty;
            }

            Profile = string.IsNullOrEmpty(UserName) ? EPiServerProfile.Current : EPiServerProfile.Get(UserName);
        }
      

        /// <summary>
        /// Handles page load
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event parameters</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();

            //If we try to go to the form without ticket or autentication we need to show error message
            if ((Ticket == null) && !User.Identity.IsAuthenticated)
            {
                PanelMain.Visible = false;
                PanelUnavailable.Visible = true;
                return;
            }

            //If username from ticket differs from current user name we need to show error message
            if (User.Identity.IsAuthenticated && (Ticket != null) && !Ticket.Email.Equals(EPiServerProfile.Current.Email))
            {
                PanelMain.Visible = false;
                PanelLogout.Visible = true;
                return;
            }

            if (User.Identity.IsAuthenticated && !IsPostBack)
            {
                TextBoxFirstName.Text = Profile.FirstName;
                TextBoxLastName.Text = Profile.LastName;
            }

            if (Ticket != null)
            {
                //Show information from ticket to the membership panel
                PanelMembership.Visible = string.IsNullOrEmpty(UserName);
                LabelEmailValue.Text = Ticket.Email;
                LabelRoleValue.Text = Core.Membership.TranslateMembershipLevel(Ticket.MembershipLevel);

                PanelUserNameInfo.Visible = !string.IsNullOrEmpty(UserName);
                LabelUserNameValue.Text = UserName;
            }
            else
            {
                //We are going to use current user, so we don't need membership panel
                PanelMembership.Visible = false;
                PanelInfo.Visible = false;
            }

            //If profile is filled then just redirect
            if (Profile.IsAnonymous || string.IsNullOrEmpty(Profile.FirstName) || string.IsNullOrEmpty(Profile.LastName))
            {
                return;
            }

            SetRights();
            Login();
            Redirect();
        }
        
        /// <summary>
        /// Creates new user
        /// <returns>If user was created</returns>
        /// </summary>
        private bool CreateUser()
        {
            try
            {
                Membership.CreateUser(TextBoxUserName.Text, TextBoxPassword.Text, Ticket.Email);
                UserName = TextBoxUserName.Text;
                Profile = EPiServerProfile.Get(UserName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Set user right on workroom
        /// </summary>
        private void SetRights()
        {
           if (Ticket == null)
           {
               return;
           }

           var data = DataFactory.Instance.GetPage(Ticket.WorkroomStartPage);
           var entry = new MembershipEntry(UserName, Ticket.MembershipLevel);
           Core.Membership.ModifyPermission(data, entry);
        }

        /// <summary>
        /// Updates user profile
        /// </summary>
        private void UpdateProfile()
        {
            if ((Ticket != null) && string.IsNullOrEmpty(Profile.Email))
            {
                Profile.Email = Ticket.Email;
            }

            Profile.FirstName = TextBoxFirstName.Text;
            Profile.LastName = TextBoxLastName.Text;
            Profile.Save();
        }

        /// <summary>
        /// Handles page load
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event parameters</param>
        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }

            //Create new user with membership and profile
            if (string.IsNullOrEmpty(UserName) && !User.Identity.IsAuthenticated && !CreateUser())
            {
                PanelMain.Visible = false;
                PanelProvider.Visible = true;
                return;
            }

            UpdateProfile();
            SetRights();
            Login();
            Redirect();
        }

        /// <summary>
        /// Login user to CMS
        /// </summary>
        private void Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return;
            }

            PrincipalInfo.CurrentPrincipal = PrincipalInfo.CreatePrincipal(UserName);

            if (FormsSettings.IsFormsAuthentication)
            {
                FormsAuthentication.SetAuthCookie(UserName, false);
            }
        }

        /// <summary>
        /// Check string on regular expression rule in membership provider
        /// </summary>
        /// <param name="provider">Membership provider to check</param>
        /// <param name="text">String to check</param>
        /// <returns>If string is valid</returns>
        private static bool CheckRegex(MembershipProvider provider, string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(provider.PasswordStrengthRegularExpression))
                {
                    var regex = new Regex(provider.PasswordStrengthRegularExpression);
                    return regex.IsMatch(text);
                }
            }
            catch (Exception) {}
            return true;
        }

        /// <summary>
        /// Check string on length rule in membership provider
        /// </summary>
        /// <param name="provider">Membership provider to check</param>
        /// <param name="text">String to check</param>
        /// <returns>If string is valid</returns>
        private static bool CheckLength(MembershipProvider provider, string text)
        {
            try
            {
                return text.Length >= provider.MinRequiredPasswordLength;
            }
            catch (Exception) {}
            return true;
        }

        /// <summary>
        /// Check string on minimal nonalphanumerics count rule in membership provider
        /// </summary>
        /// <param name="provider">Membership provider to check</param>
        /// <param name="text">String to check</param>
        /// <returns>If string is valid</returns>
        private static bool CheckNonAlphanumerics(MembershipProvider provider, string text)
        {
            try
            {
                var count = 0;
                foreach (var c in text)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        count++;
                    }
                }
                return count >= provider.MinRequiredNonAlphanumericCharacters;
            }
            catch (Exception) {}
            return true;
        }

        /// <summary>
        /// Handles password validating
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event parameters</param>
        protected void CustomValidatorPassword_Validate(object sender, ServerValidateEventArgs e)
        {
            var provider = Membership.Provider;
            e.IsValid = CheckLength(provider, TextBoxPassword.Text) && CheckNonAlphanumerics(provider, TextBoxPassword.Text) &&
                        CheckRegex(provider, TextBoxPassword.Text);
        }

        /// <summary>
        /// Handles user name vaildating
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event parameters</param>
        protected void CustomValidatorUserName_Validate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = Membership.FindUsersByName(TextBoxUserName.Text).Count == 0;
        }
    }
}

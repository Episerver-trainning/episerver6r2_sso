#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// Invitation sender
    /// </summary>
    public class InvitationSender : NotificationSender
    {
        public InvitationSender(string emailAddressFrom, string firstNameFrom, string lastNameFrom, string invitationPageUrl) :
            base(emailAddressFrom, firstNameFrom, lastNameFrom, invitationPageUrl) { }

        /// <summary>
        /// Ticket parameter in request
        /// </summary>
        public const string TicketParameter = "ticket";

        /// <summary>
        /// Gets the notification URL.
        /// </summary>
        /// <returns></returns>
        protected override string GetNotificationUrl()
        {
            InvitationTicket ticket = new InvitationTicket(WorkroomPage.WorkroomStartPageLink, MemberLevel, EmailTo);

            string urlParametersSeparator = "?";
            if (NotificationPageUrl.Contains(urlParametersSeparator))
            {
                urlParametersSeparator = "&";
            }
            return String.Format("{0}{1}{2}={3}", NotificationPageUrl, urlParametersSeparator, TicketParameter, ticket.Encrypt());
        }

        /// <summary>
        /// Returns e-mail Invitation or Notification creator
        /// </summary>
        /// <returns></returns>
        protected override EmailCreator GetEmailCreator()
        {
            var users = System.Web.Security.Membership.FindUsersByEmail(EmailTo);
            if (users.Count == 0)
            {
                return new NewUserInvitationCreator(WorkroomPage.WorkroomStartPage.ParentLink);
            }
            else
            {
                return new ExistingUserInvitationCreator(WorkroomPage.WorkroomStartPage.ParentLink);
            }
        }
    }
}

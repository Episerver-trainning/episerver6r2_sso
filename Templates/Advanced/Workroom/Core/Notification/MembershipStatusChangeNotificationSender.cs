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
    /// Sender for membership status changes notifications
    /// </summary>
    public class MembershipStatusChangeNotificationSender : NotificationSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipStatusChangeNotificationSender"/> class.
        /// </summary>
        /// <param name="emailAddressFrom">The email address from.</param>
        /// <param name="firstNameFrom">The first name from.</param>
        /// <param name="lastNameFrom">The last name from.</param>
        /// <param name="invitationPageUrl">The invitation page URL.</param>
        public MembershipStatusChangeNotificationSender(string emailAddressFrom, string firstNameFrom, string lastNameFrom, string invitationPageUrl) :
            base(emailAddressFrom, firstNameFrom, lastNameFrom, invitationPageUrl) { }

        /// <summary>
        /// Gets the notification URL.
        /// </summary>
        /// <returns></returns>
        protected override string GetNotificationUrl()
        {
            return NotificationPageUrl;
        }

        /// <summary>
        /// Returns e-mail Invitation or Notification creator
        /// </summary>
        /// <returns></returns>
        protected override EmailCreator GetEmailCreator()
        {
            if (MemberLevel == MembershipLevels.None)
            {
                return new RemoveMemberNotificationCreator(WorkroomPage.WorkroomStartPage.ParentLink);
            }
            else
            {
                return new MembershipStatusChangeNotificationCreator(WorkroomPage.WorkroomStartPage.ParentLink);
            }
        }
    }
}

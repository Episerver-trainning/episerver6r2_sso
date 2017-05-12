#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System.Net.Mail;

namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// Notification sender abstract class
    /// </summary>
    public abstract class NotificationSender
    {
        /// <summary>
        /// E-mail address to send invitation to
        /// </summary>
        public string EmailTo { get; private set; }
        /// <summary>
        /// User name to send invitation to
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; private set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; private set; }
        /// <summary>
        /// Workroom to send invitation to
        /// </summary>
        public WorkroomPageBase WorkroomPage { get; private set; }
        /// <summary>
        /// Membership level of invited user
        /// </summary>
        public MembershipLevels MemberLevel { get; private set; }
        /// <summary>
        /// Sender e-mail address
        /// </summary>
        public MailAddress EmailFrom { get; private set; }
        /// <summary>
        /// Subject of invitation e-mail
        /// </summary>
        public string EmailSubject { get; private set; }
        /// <summary>
        /// Body of invitation e-mail
        /// </summary>
        public string EmailBody { get; private set; }
        /// <summary>
        /// Url to a page for invited user 
        /// </summary>
        public string NotificationPageUrl { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSender"/> class.
        /// </summary>
        /// <param name="emailAddressFrom">The email address from.</param>
        /// <param name="firstNameFrom">The first name from.</param>
        /// <param name="lastNameFrom">The last name from.</param>
        /// <param name="invitationPageUrl">The invitation page URL.</param>
        protected NotificationSender(string emailAddressFrom, string firstNameFrom, string lastNameFrom, string invitationPageUrl)
        {
            EmailFrom = new MailAddress(emailAddressFrom, string.Format("{0} {1}", firstNameFrom, lastNameFrom));
            NotificationPageUrl = invitationPageUrl;
        }
        /// <summary>
        /// Sends the notification e-mail
        /// </summary>
        /// <param name="workroom">Workroom to invite user to</param>
        /// <param name="userName">Name of user to invite</param>
        /// <param name="eMail">Invited user e-mail</param>
        /// <param name="memberLevel">Invited user member level</param>
        public void Send(WorkroomPageBase workroom, string userName, string email, MembershipLevels memberLevel)
        {
            WorkroomPage = workroom;
            UserName = userName;
            EmailTo = email;
            MemberLevel = memberLevel;

            MailMessage message = GetMessage();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }

        /// <summary>
        /// Sends the notification to specified member.
        /// </summary>
        /// <param name="workroom">The workroom.</param>
        /// <param name="member">The member.</param>
        public void Send(WorkroomPageBase workroom, MembershipEntry member)
        {
            WorkroomPage = workroom;
            UserName = member.Name;
            FirstName = member.FirstName;
            LastName = member.LastName;
            EmailTo = member.Email;
            MemberLevel = member.Membership;

            MailMessage message = GetMessage();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);            
        }

        /// <summary>
        /// Gets the notification URL.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetNotificationUrl();

        /// <summary>
        /// Prepares invitation e-mail message
        /// </summary>
        /// <returns></returns>
        protected MailMessage GetMessage()
        {
            EmailCreator emailCreator = GetEmailCreator();

            EmailVariableSet emailCreatorParameters = new EmailVariableSet
                                                      {
                                                          Link = GetNotificationUrl(),
                                                          GuestEmail = EmailTo,
                                                          GuestName = UserName,
                                                          FirstName = FirstName,
                                                          LastName = LastName,
                                                          OriginatorEmail = EmailFrom.Address,
                                                          OriginatorName = EmailFrom.DisplayName,
                                                          WorkroomName = WorkroomPage.WorkroomStartPage.PageName,
                                                          GuestRole = Membership.TranslateMembershipLevel(MemberLevel)
                                                      };

            EmailSubject = emailCreator.GetSubject(emailCreatorParameters);
            EmailBody = emailCreator.GetBody(emailCreatorParameters);

            MailAddress toAddress = new MailAddress(EmailTo);
            MailMessage message = new MailMessage(EmailFrom, toAddress)
            {
                IsBodyHtml = true,
                Subject = EmailSubject,
                Body = EmailBody
            };
            return message;
        }
        /// <summary>
        /// Returns e-mail Invitation or Notification creator
        /// </summary>
        /// <returns></returns>
        protected abstract EmailCreator GetEmailCreator();
    }
}

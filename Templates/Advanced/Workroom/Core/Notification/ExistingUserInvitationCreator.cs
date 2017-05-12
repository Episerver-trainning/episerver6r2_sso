#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using EPiServer.Core;
namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// Existing user invitation creator
    /// </summary>
    public class ExistingUserInvitationCreator : EmailCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExistingUserInvitationCreator"/> class.
        /// </summary>
        /// <param name="workroomBaseRoot"></param>
        public ExistingUserInvitationCreator(PageReference workroomBaseRoot)
            : base(workroomBaseRoot)
        {
        }


        /// <summary>
        /// Gets a template page type name for email creation.
        /// </summary>
        /// <value></value>
        public override string EmailTemplatePageType
        {
            get { return "[AlloyTech] Notification Email"; }
        }

        /// <summary>
        /// Gets a template page name for email creation.
        /// </summary>
        /// <value></value>
        public override string EmailTemplatePageName
        {
            get { return "Existing user invitation"; }
        }
    }
}

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
using EPiServer.Core;

namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// Notification creator for changed membership status
    /// </summary>
    public class MembershipStatusChangeNotificationCreator : EmailCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipStatusChangeNotificationCreator"/> class.
        /// </summary>
        /// <param name="workroomBaseRoot"></param>
        public MembershipStatusChangeNotificationCreator(PageReference workroomBaseRoot)
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
            get { return "Membership status change notification"; }
        }
    }
}

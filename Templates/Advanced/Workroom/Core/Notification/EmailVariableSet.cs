#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
namespace EPiServer.Templates.Advanced.Workroom.Core.Notification
{
    /// <summary>
    /// A class that are used for creating a complete email content. Its property should be initialized with values which replace
    /// special words in email template page properties.
    /// </summary>
    public class EmailVariableSet
    {
        /// <summary>
        /// Gets, sets a guest user name that recievies a letter.
        /// </summary>
        public string GuestName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets, sets a guest user email that recievies a letter.
        /// </summary>
        public string GuestEmail { get; set; }

        /// <summary>
        /// Gets, sets a guest user name that is a owner of the workroom.
        /// </summary>
        public string OriginatorName { get; set; }

        /// <summary>
        /// Gets, sets a guest user email that is a owner of the workroom.
        /// </summary>
        public string OriginatorEmail { get; set; }

        /// <summary>
        /// Gets, sets a link to the workroom invitation page.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets, sets workroom name
        /// </summary>
        public string WorkroomName { get; set; }

        /// <summary>
        /// Gets, sets a role for guest
        /// </summary>
        public string GuestRole { get; set; }
    }
}

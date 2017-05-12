#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using EPiServer.Personalization;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Representation of a workroom membership.
    /// </summary>
    [Serializable]
    public struct MembershipEntry
    {
        private string _name;
        private string _eMail;
        private string _firstName;
        private string _lastName;
        private MembershipLevels _membership;


        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipEntry"/>.
        /// </summary>
        /// <param name="userName">Name of the member.</param>
        /// <param name="membership">The membeship level for the member.</param>
        public MembershipEntry(string userName, MembershipLevels membership)
        {
            _name = userName;
            _membership = membership;
            _eMail = null;
            _firstName = null;
            _lastName = null;
        }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets en email address of the member
        /// </summary>
        /// <remarks>If no email address has been set it's fetched when requested from profile or membership data.</remarks>
        public string Email
        {
            get 
            {
                if (_eMail == null) 
                {
                    _eMail = ResolveEmail(Name);
                }
                return _eMail; 
            }
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName
        {
            get
            {
                if (_firstName == null)
                {
                    _firstName = ResolveFirstName(Name);
                }
                return _firstName;
            }
        }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName
        {
            get
            {
                if (_lastName == null)
                {
                    _lastName = ResolveLastName(Name);
                }
                return _lastName;
            }
        }


        /// <summary>
        /// Gets or sets the membership level for the member.
        /// </summary>
        public MembershipLevels Membership
        {
            get { return _membership; }
            set { _membership = value; }
        }

        /// <summary>
        /// Returns the localized name of the <see cref="Membership"/>.
        /// </summary>
        /// <value>The name of the membership local.</value>
        public string MembershipLocalName
        {
            get { return Core.Membership.TranslateMembershipLevel(Membership); }
        }

        /// <summary>
        /// Returns a string representation of the entry, containing the user name and the membership level.
        /// </summary>
        public override string ToString()
        {
            return Name + ": " + Membership.ToString();
        }

        /// <summary>
        /// Resolves the email.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        private static string ResolveEmail(string userName)
        {
            EPiServerProfile profile = EPiServer.Personalization.EPiServerProfile.Get(userName);
            return (profile != null ? profile.EmailWithMembershipFallback : String.Empty);
        }

        /// <summary>
        /// Resolves the last name.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        private static string ResolveLastName(string userName)
        {
            EPiServerProfile profile = EPiServer.Personalization.EPiServerProfile.Get(userName);
            return (profile != null ? profile.LastName : String.Empty);

        }

        /// <summary>
        /// Resolves the first name.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        private static string ResolveFirstName(string userName)
        {
            EPiServerProfile profile = EPiServer.Personalization.EPiServerProfile.Get(userName);
            return (profile != null ? profile.FirstName : String.Empty);

        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MembershipEntry))
                return false;
            return String.Equals(Name, ((MembershipEntry)obj).Name);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            if (Name == null)
                return 0;
            return Name.GetHashCode();
        }

    }
}

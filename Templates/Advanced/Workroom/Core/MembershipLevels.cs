#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.ComponentModel;
using EPiServer.Security;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Definition of different membership levels in a workroom.
    /// </summary>
    [Flags, Serializable]
    public enum MembershipLevels
    {
        /// <summary>
        /// Not a member
        /// </summary>
        None = 0,
        /// <summary>
        /// member with read access
        /// </summary>
        Read = AccessLevel.Read,
        /// <summary>
        /// Participating member
        /// </summary>
        Modify = Read | AccessLevel.Edit | AccessLevel.Create | AccessLevel.Delete | AccessLevel.Publish,
        /// <summary>
        /// Administrator
        /// </summary>
        Administer = Modify | AccessLevel.Administer
    }
}

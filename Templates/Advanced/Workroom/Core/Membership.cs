#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using EPiServer.Core;
using EPiServer.Security;


namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// A class handling workroom membership.
    /// </summary>
    public static class Membership
    {
        /// <summary>
        /// Updates the a specific users permissions to a workroom. 
        /// To remove a user as member of a workroom supply the <see cref="MembershipLevel.None"/>.
        /// </summary>
        /// <param name="page">The start page for a workroom.</param>
        /// <param name="membership">A <see cref="MembershipEntry"/> containing the user name and new membership level.</param>
        public static void ModifyPermission(PageData page, MembershipEntry membership)
        {
            if (page.ACL.IsReadOnly)
            {
                PageData writeblePage = page.CreateWritableClone();
                ModifyPermission(writeblePage, membership.Name, ConvertAccessLevel(membership.Membership));
                writeblePage.ACL.Save(SecuritySaveType.RecursiveReplace);
            }
            else
            {
                ModifyPermission(page, membership.Name, ConvertAccessLevel(membership.Membership));
                page.ACL.Save(SecuritySaveType.RecursiveReplace);
            }
        }


        /// <summary>
        /// Updates the permissions to a workroom for a collection containing <see cref="MembershipEntry"/>.
        /// To remove a user as member of a workroom supply the <see cref="MembershipLevel.None"/>.
        /// </summary>
        /// <param name="page">The start page for a workroom.</param>
        /// <param name="membershipEntries">A collection containing <see cref="MembershipEntry"/> to modifiy access for.</param>
        public static void ModifyPermission(PageData page, IEnumerable<MembershipEntry> membershipEntries)
        {
            foreach (MembershipEntry entry in membershipEntries)
            {
                ModifyPermission(page, entry.Name, ConvertAccessLevel(entry.Membership)); 
            }
            page.ACL.Save(SecuritySaveType.RecursiveReplace);
        }



        /// <summary>
        /// Modifies the permission for a specific user on the supplied page.
        /// </summary>
        /// <param name="page">The page to modify permission on.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="accessLevel">The new access level.</param>
        private static void ModifyPermission(PageData page, string userName, AccessLevel accessLevel)
        {
            AccessControlEntry ace = new AccessControlEntry(userName, accessLevel, SecurityEntityType.User);

            if (page.ACL.Exists(ace.Name))
            {
                if (ace.Access == AccessLevel.NoAccess)
                {
                    page.ACL.Remove(ace.Name);
                }
                else
                {
                    page.ACL[ace.Name] = ace;
                }
            }
            else
            {
                page.ACL.Add(ace);
            }
        }


        /// <summary>
        /// Returnes a collection containing the members of a workroom and their level of membership.
        /// </summary>
        /// <param name="page">A workroom start page</param>
        /// <returns>A list of the workrrom members.</returns>
        public static IList<MembershipEntry> GetMembers(PageData page)
        {
            IList<MembershipEntry> members = new List<MembershipEntry>();

            foreach (KeyValuePair<string, EPiServer.Security.AccessControlEntry> entity in page.ACL.Where(pair => pair.Value.EntityType == SecurityEntityType.User))
            {
                members.Add(new MembershipEntry(entity.Value.Name, ConvertAccessLevel(entity.Value.Access)));
            }

            return members;
        }

        /// <summary>
        /// Converts an <see cref="EPiServer.Security.AccessLevel"/> to a corresponding <see cref="MembershipLevel"/>.
        /// </summary>
        /// <param name="accessLevel">The access level to convert.</param>
        /// <returns>The corresponding <see cref="MembershipLevel"/>.</returns>
        public static MembershipLevels ConvertAccessLevel(AccessLevel accessLevel)
        {
            MembershipLevels workroomAccess = 0;
            workroomAccess |= TestMembership(accessLevel, MembershipLevels.Read);
            workroomAccess |= TestMembership(accessLevel, MembershipLevels.Modify);
            workroomAccess |= TestMembership(accessLevel, MembershipLevels.Administer);
            return workroomAccess;
        }

        /// <summary>
        /// Converts a <see cref="MembershipLevel"/> to a corresponding <see cref="EPiServer.Security.AccessLevel"/>.
        /// </summary>
        /// <param name="membershipLevel">The membership to convert.</param>
        /// <returns>The corresponding <see cref="EPiServer.Security.AccessLevel"/></returns>
        public static AccessLevel ConvertAccessLevel(MembershipLevels membershipLevel)
        {
            return (AccessLevel)membershipLevel;
        }

        /// <summary>
        /// Gets the <see cref="MembershipLevel"/> a specific user has on the supplied <see cref="EPiServer.Core.PageData"/>.
        /// </summary>
        /// <param name="page">The <see cref="EPiServer.Core.PageData"/> to query permissions on.</param>
        /// <param name="principal">The principal describing the user.</param>
        /// <returns>The MembershipLevel on the supplied page.</returns>
        public static MembershipLevels QueryMembershipLevel(PageData page, IPrincipal principal)
        {
            return ConvertAccessLevel(page.ACL.QueryAccess(principal));
        }

        /// <summary>
        /// Gets the <see cref="MembershipLevel"/> the current user has on the supplied <see cref="EPiServer.Core.PageData"/>.
        /// </summary>
        /// <param name="page">The <see cref="EPiServer.Core.PageData"/> to query permissions on.</param>
        /// <returns>The MembershipLevel on the supplied page.</returns>
        public static MembershipLevels QueryMembershipLevel(PageData page)
        {
            return QueryMembershipLevel(page, HttpContext.Current.User);
        }

        /// <summary>
        /// Checks whether the current user has the supplied <see cref="MembershipLevel"/> on a <see cref="EPiServer.Core.PageData"/>.
        /// </summary>
        /// <param name="page">The <see cref="EPiServer.Core.PageData"/> to query permissions on.</param>
        /// <param name="membershipLevel">The membership level.</param>
        /// <returns><c>true</c> if the user has at least the permission supplied <c>membershipLevel</c> on the <c>page</c>; otherwise <c>false</c>.</returns>
        public static bool QueryDistinctMembershipLevel(PageData page, MembershipLevels membershipLevel)
        {
            return page.ACL.QueryDistinctAccess(ConvertAccessLevel(membershipLevel));
        }

        /// <summary>
        /// Translates the membership level to a localized string.
        /// </summary>
        /// <param name="level">The <see cref="MembershipLevel"/> to get a string description for.</param>
        /// <returns>A string describing the membership level.</returns>
        public static string TranslateMembershipLevel(MembershipLevels level)
        {
            switch(level)
            {
                case MembershipLevels.Modify:
                    return LanguageManager.Instance.Translate("/workroom/membershiplevel/modify");
                case MembershipLevels.Read:
                    return LanguageManager.Instance.Translate("/workroom/membershiplevel/read");
                case MembershipLevels.Administer:
                    return LanguageManager.Instance.Translate("/workroom/membershiplevel/administer");
            }
            return String.Empty;
        }

        #region Private Helper Methods

        private static MembershipLevels TestMembership(AccessLevel accessLevel, MembershipLevels membershipLevel)
        {
            return ((int)accessLevel & (int)membershipLevel) == (int)membershipLevel ? membershipLevel : MembershipLevels.None;
        }
        
        #endregion

    }
}

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
using System.Web.Security;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Helper class to provide an <see cref="System.Web.UI.WebControls.ObjectDataSource"/> with a common search method toward the <see cref="System.Web.Security.Membership"/> API.
    /// </summary>
    public class MembershipSearch
    {
        private int _rowCount;
        private IList<MembershipEntry> _workroomMembers;

        /// <summary>
        /// Get a collection containing the members of this workroom.
        /// </summary>
        private IList<MembershipEntry> WorkroomMembers
        {
            get
            {
                if (_workroomMembers == null)
                {
                    WorkroomPageBase currentPage = HttpContext.Current.CurrentHandler as WorkroomPageBase;
                    if (currentPage != null)
                    {
                        _workroomMembers = Core.Membership.GetMembers(currentPage.GetPage(currentPage.WorkroomStartPageLink));
                    }
                    else 
                    {
                        _workroomMembers = new List<MembershipEntry>();
                    }
                }
                return _workroomMembers;
            }
        }

        /// <summary>
        /// Returns the total number of search hits in the last search performed.
        /// </summary>
        /// <param name="userName">For <see cref="System.Web.UI.WebControls.ObjectDataSource"/> compatibility. Not used.</param>
        /// <param name="email">For <see cref="System.Web.UI.WebControls.ObjectDataSource"/> compatibility. Not used.</param>
        ///  <param name="onlyExistingMembers">For <see cref="System.Web.UI.WebControls.ObjectDataSource"/> compatibility. Not used.</param>
        /// <returns>The number of search hits.</returns>
        public int GetRowCount(string userName, string email, bool onlyExistingMembers)
        {
            return _rowCount;
        }

        /// <summary>
        /// Encapsulates the search methods found in <see cref="System.Web.Security.Membership"/> and searches with the appropriate method based on input arguments.
        /// This method is used when the EnablePaging is <c>true</c> on the ObjectDataSource.
        /// </summary>
        /// <param name="userName">Partial or complete username to match</param>
        /// <param name="email">Partial or complete e-mail to match</param>
        /// <param name="onlyExistingMembers">if set to <c>true</c> only members of the workrrom is returned and the search parameters are not used.</param>
        /// <param name="startRowIndex">The row number of the first hit to return.</param>
        /// <param name="maximumRows">The maximum number of hits to return when showing paged data</param>
        /// <returns>A collection of users matching the search criteria.</returns>
        public IList<MembershipEntry> FindUsers(string userName, string email, bool onlyExistingMembers, int startRowIndex, int maximumRows)
        {
            if (maximumRows <= 0)
                maximumRows = int.MaxValue;

            // Convert the supplied row index to a corresponding page index.
            int startPageIndex = startRowIndex / maximumRows;

            if (onlyExistingMembers)
            {
                return GetWorkroomMemberRange(startRowIndex, maximumRows, out _rowCount);
            }

            MembershipUserCollection membershipUsers = null;
            if (!String.IsNullOrEmpty(userName))
            {
                membershipUsers = System.Web.Security.Membership.FindUsersByName(PrepareSearchString(userName), startPageIndex, maximumRows, out _rowCount);
            }
            else if (!String.IsNullOrEmpty(email))
            {
                membershipUsers = System.Web.Security.Membership.FindUsersByEmail(PrepareSearchString(email), startPageIndex, maximumRows, out _rowCount);
            }
            else
            {
                membershipUsers = System.Web.Security.Membership.GetAllUsers(startPageIndex, maximumRows, out _rowCount);
            }
            return MergeMemberLists(membershipUsers, WorkroomMembers.ToList());
        }

        /// <summary>
        /// Encapsulates the search methods found in <see cref="System.Web.Security.Membership"/> and searches with the appropriate method based on input arguments.
        /// This method is used when the EnablePaging is <c>false</c> on the ObjectDataSource.
        /// </summary>
        /// <param name="userName">Partial or complete username to match</param>
        /// <param name="email">Partial or complete e-mail to match</param>
        /// <param name="onlyExistingMembers">if set to <c>true</c> only members of the workrrom is returned and the search parameters are not used.</param>
        /// <returns>A collection of users matching the search criteria.</returns>
        public IList<MembershipEntry> FindUsers(string userName, string email, bool onlyExistingMembers)
        {
            return FindUsers(userName, email, onlyExistingMembers, 0, int.MaxValue);
        }


        /// <summary>
        /// Merges a <see cref="MembershipUserCollection"/> retreived from a search from the <see cref="System.Web.Security.Membership"/> API.
        /// with a collection of workroom membership entries.
        /// </summary>
        /// <param name="users">The result from a search for users.</param>
        /// <param name="workroomMembers">The workroom members.</param>
        /// <returns>A collection of <see cref="MembershipEntry"/>.</returns>
        private static IList<MembershipEntry> MergeMemberLists(MembershipUserCollection users, List<MembershipEntry> workroomMembers)
        {
            List<MembershipEntry> membershipEntries = new List<MembershipEntry>(users.Count);
            foreach (MembershipUser user in users)
            {
                MembershipEntry match = workroomMembers.Find(delegate(MembershipEntry entry) { return String.Equals(entry.Name, user.UserName, StringComparison.OrdinalIgnoreCase); });
                if (String.IsNullOrEmpty(match.Name))
                {
                    match.Name = user.UserName;
                    match.Membership = MembershipLevels.None;
                }
                membershipEntries.Add(match);
            }
            return membershipEntries;
        }


        /// <summary>
        /// Gets a range of workroom members for paged data display.
        /// </summary>
        /// <param name="startRowIndex">The zero-based index of the first member to return.</param>
        /// <param name="maximumRows">The maximum number of members to return.</param>
        /// <param name="rowCount">The total number of members in the workroom.</param>
        /// <returns>A list of workroom members.</returns>
        private IList<MembershipEntry> GetWorkroomMemberRange(int startRowIndex, int maximumRows, out int rowCount)
        {
            rowCount = WorkroomMembers.Count;

            if (startRowIndex >= 0 && startRowIndex < rowCount && maximumRows > 0)
            {
                maximumRows = Math.Min(maximumRows, rowCount - startRowIndex);
                return WorkroomMembers.ToList().GetRange(startRowIndex, maximumRows);
            }

            return WorkroomMembers;
        }


        /// <summary>
        /// Clean up a search expression by escaping characters used as wildcards in SQL LIKE statements.
        /// Also replaces '?' and '*' with '_' and '%' which is wildcard characters in LIKE statements.
        /// </summary>
        /// <param name="text">The text to "clean up"</param>
        /// <returns>A text suitable for SQL LIKE matching.</returns>
        private static string PrepareSearchString(string text)
        {
            System.Text.RegularExpressions.Regex escaper = new System.Text.RegularExpressions.Regex(@"([\[\]_])", System.Text.RegularExpressions.RegexOptions.Compiled);
            text = escaper.Replace(text, "[$1]");
            text = text.Replace('?', '_').Replace('*', '%');
            if (!text.EndsWith("%", StringComparison.Ordinal))
            {
                text += "%";
            }
            return text;
        }
    }
}

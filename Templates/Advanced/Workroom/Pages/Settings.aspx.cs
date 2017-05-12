#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Web;
using System.Web.UI;
using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// Page template to enable/disable functionality of a workroom
    /// </summary>
    public partial class Settings : WorkroomPageBase
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            bool isEditAllowedToUser = (Membership.QueryMembershipLevel(CurrentPage) == MembershipLevels.Administer);
            AvailableFunctions.Enabled = isEditAllowedToUser;

            if (!IsPostBack)
            {
                AvailableFunctions.DataBind();
            }
        }

        /// <summary>
        /// Handles the VisibilityChanged event of the VisibleInMenuList control to rebind menus if the visibility for pages has changed.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AvailableFunctions_VisibilityChanged(object source, EventArgs e)
        {
            // If page visibility has changed then databind the page to update menus.
            this.DataBind();
        }
    }
}

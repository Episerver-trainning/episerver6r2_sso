/*
Copyright (c) EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using System;

namespace EPiServer.Templates.AlloyTech.Units.Static
{
    /// <summary>
    /// Used to change a user password.
    /// </summary>
    public partial class ChangePassword : UserControlBase
    {

        /// <summary>
        /// Toggles the visibility of the ChangePassword control
        /// </summary>
        protected void ToggleChangePass_Click( object sender, EventArgs e )
        {
            if (changePassArea.Visible)
            {
                changePassArea.Visible = false;
            }
            else
            {
                changePassArea.Visible = true;
            }
        }
    }
}
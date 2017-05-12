/*
Copyright (c) EPiServer AB.  All rights reserved.

This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published in 20 August 2007. 
See http://www.episerver.com/Specific_License_Conditions for details. 
*/

using System;
using System.Web.UI.WebControls;
using EPiServer.Security;
using EPiServer.Web;

namespace EPiServer.Templates.AlloyTech.Pages
{
    /// <summary>
    /// A registration form page used to create users with the default Membership provider.
    /// </summary>
    public partial class Register : TemplatePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Checks if provider supports creating users.
            if (!ProviderCapabilities.IsSupported(ProviderFacade.GetDefaultMembershipProviderName(), ProviderCapabilities.Action.Create))
            {
                RegistrationWizard.Visible = false;
                ProviderDoNotSupportCreateUser.Visible = true;
                ProviderDoNotSupportCreateUser.Text = string.Format("<br />" +Translate("/admin/secedit/providerdonotsupportcreateuser"), ProviderFacade.GetDefaultMembershipProviderName());
            }

        }

        /// <summary>
        /// We want to set set the create user button as the default button,
        /// so we do that in the pre render event of that wizard step.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CreateUserWizardStep_PreRender(object sender, EventArgs e)
        {
            Button createUserButton = Page.FindControl<Button>("StepNextButtonButton");
            if (createUserButton != null)
            {
                Form.DefaultButton = createUserButton.UniqueID;
            }
        }

    }
}
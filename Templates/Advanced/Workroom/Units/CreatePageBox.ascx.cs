#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    public partial class CreatePageBox : UserControlBase
    {   
        private string _requiredFieldErrorMessageKey = "RequiredFieldErrorMessage";
        private string _cssClassKey = "CssClassKey";

        public event CommandEventHandler ButtonSaveClicked;

        #region Properies

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>The label text.</value>
        public string LabelText
        {
            get { return BoxLabel.Text; }
            set { BoxLabel.Text = value; }
        }

        /// <summary>
        /// Gets or sets the text field value.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return PageNameTextBox.Text; }
            set { PageNameTextBox.Text = value; }
        }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        public string CssClass
        {
           get
            {
                if ((ViewState[_cssClassKey]) == null)
                {
                    return string.Empty;
                }
                else { return ViewState[_cssClassKey].ToString(); }
            }
            set { ViewState[_cssClassKey] = value; }
        }
    

        /// <summary>
        /// Gets or sets the required field validator error message.
        /// </summary>
        /// <value>The required field validator error message.</value>
        public string RequiredFieldValidatorErrorMessage
        {
            get
            {                
                if (ViewState[_requiredFieldErrorMessageKey] == null)
                {
                    return Translate("/workroom/newslistpage/titleempty");
                }
                else { return ViewState[_requiredFieldErrorMessageKey].ToString(); }
            }
            set { ViewState[_requiredFieldErrorMessageKey] = value; }
        }

        /// <summary>
        /// Gets or sets the button ok title.
        /// </summary>
        /// <value>The button ok title.</value>
        public string ButtonOkTitle
        {
            get { return AddNewPageButton.Text; }
            set { AddNewPageButton.Text = value; }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RegisterClientScript();

            if (!IsPostBack)
            {
                PageNameRequiredFieldValidator.ValidationGroup = CreatePageBoxPanel.ClientID + "_ValidationGroup";
                AddNewPageButton.ValidationGroup = CreatePageBoxPanel.ClientID + "_ValidationGroup";
            }
        }

        /// <summary>
        /// Handles the Click event of the AddNewPageButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddNewPageButton_Click(object sender, EventArgs e)
        {            
            // Fire the ButtonOkClickedEvent event...
            if (ButtonSaveClicked != null)
            {
                CommandEventArgs saveArgs = new CommandEventArgs(PageNameTextBox.Text, null);
                ButtonSaveClicked(this, saveArgs);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the java script activate function call.
        /// </summary>
        /// <param name="HideCtrlID">The ID of the control that should be hided on activate.</param>
        /// <returns></returns>
        public string GetActivateJavaScript(string hideControlId)
        {
            return string.Format("javascript:{0}_Show('{1}')", this.ClientID, hideControlId);
        }

        /// <summary>
        /// Registers the client java script.
        /// </summary>
        private void RegisterClientScript()
        {            
            string SenderVar = this.ClientID + "_Sender";
            string ValueVar = this.ClientID + "_Value";
            string ShowFunction = this.ClientID + "_Show";
            string HideFunction = this.ClientID + "_Hide";
            string KeyPressFunction = this.ClientID + "_Click";

            StringBuilder script = new StringBuilder();
            script.Append(" var " + SenderVar + " = \"\";");
            script.Append(" var " + ValueVar + " = \"\";");
            script.Append(" function " + ShowFunction + "(sender) {");            
            script.Append(SenderVar + " = sender;");
            script.Append(" $(\"#" +CreatePageBoxPanel.ClientID + "\").show(); ");
            script.Append(" $(\"#\"+sender).hide(); ");
            script.Append(" $(\"#" + PageNameTextBox.ClientID + "\").keydown(" + KeyPressFunction + ");");
            script.Append(" $(\"#" + PageNameTextBox.ClientID + "\").select();");
            script.Append(ValueVar + " = $(\"#" + PageNameTextBox.ClientID + "\").val();");
            
            script.Append(" } ");

            script.Append(" function " + HideFunction + "() {");
            script.Append(" $(\"#" + CreatePageBoxPanel.ClientID + "\").hide(); ");
            script.Append(" $(\"#\"+" + SenderVar + ").show();");
            script.Append(" $(\"#" + PageNameTextBox.ClientID + "\").val(" + ValueVar + ");");
            script.Append(" $(\"#" + PageNameRequiredFieldValidator.ClientID + "\").css(\"visibility\",\"hidden\");");
            script.Append(" } ");

            script.Append(" function " + KeyPressFunction + "(e) {");
            script.Append("  if (e.which == 13) { ");
            script.Append(" $(\"#" + AddNewPageButton.ClientID + "\").click();");
            script.Append(" return false; }");            
            script.Append(" if (e.which == 27) {  ");            
            script.Append(" $(\"#" + cancelButton.ClientID + "\").click();");
            script.Append(" return false; }");
            script.Append(" }");

            cancelButton.OnClientClick = "javascript:" + HideFunction + "(); return false;";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID + "_Script", script.ToString(), true);
        }

        #endregion
    }
}
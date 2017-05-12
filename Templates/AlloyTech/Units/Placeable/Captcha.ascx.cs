#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Verfication of user input using an image CAPTCHA.
    /// Renders a fieldset containing a legend, image and a textbox with a label.
    /// </summary>
    [ToolboxData("<{0}:Captcha runat=server></{0}:Captcha>")]
    public partial class Captcha : System.Web.UI.UserControl
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!Page.IsPostBack)
            {
                InitializeCaptcha();
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateEmptyClientText",
                "function ValidateEmptyClientText(source, args) { args.IsValid = (args.Value!=''); }", true);
        }

        #region Layout Properties

        /// <summary>
        /// Gets or sets the fieldset CSS class.
        /// </summary>
        public string CssClass 
        {
            get { return (string)(ViewState["CssClass"] ?? "captcha"); }
            set { ViewState["CssClass"] = value; }
        }

        #endregion

        #region Text Properties

        /// <summary>
        /// Gets or sets the legend text.
        /// </summary>
        public string LegendText
        {
            get { return (string)(ViewState["LegendText"] ?? String.Empty); }
            set { ViewState["LegendText"] = value; }
        }
        
        #endregion

        #region Captcha Functionality

        /// <summary>
        /// Gets or sets the random text used to verify user.
        /// </summary>
        private string HashedText {
            get { return (string)(ViewState["HashedText"] ?? String.Empty); }
            set { ViewState["HashedText"] = value; }
        }

        /// <summary>
        /// Update Captcha information
        /// </summary>
        public void Refresh()
        {
            InitializeCaptcha();
            this.ClientText.Text = "";
        }

        /// <summary>
        /// Creates a random text and initializes the CAPTCHA image.
        /// </summary>
        private void InitializeCaptcha()
        {
            string captchaText = GenerateCaptchaString();
            HashedText = HashText(captchaText);
            ServerImage.ImageUrl = "CaptchaImageHandler.ashx?text=" + HttpUtility.UrlEncode(CaptchaSecurity.Encrypt(captchaText));
        }

        /// <summary>
        /// Verifies that the user entered correct text.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidateClientText(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = String.Equals(HashedText, HashText(args.Value));
            if (!args.IsValid)
            {
                InitializeCaptcha();
                ClientText.Text = String.Empty;
            }
        }

        /// <summary>
        /// Returns an Base 64 encoded MD5 hash of the supplied text.
        /// </summary>
        private static string HashText(string text)
        {
            System.Security.Cryptography.MD5 m = System.Security.Cryptography.MD5.Create();
            byte[] crypted = m.ComputeHash(Encoding.UTF8.GetBytes(text.ToLowerInvariant()));
            return Convert.ToBase64String(crypted);
        }

        /// <summary>
        /// Generates a random 8 letter string from A-Z.
        /// </summary>
        /// <returns></returns>
        protected static string GenerateCaptchaString()
        {
            Random r = new Random();
            string captchaText = String.Empty;

            for (int i = 0; i < 8; i++)
            {
                captchaText += Convert.ToChar(0x41 + r.Next(26));
            }

            return captchaText;
        }

        #endregion
    }
}
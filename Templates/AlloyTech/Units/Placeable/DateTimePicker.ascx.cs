#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using EPiServer.Globalization;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Code behind class for DateTimePicker control
    /// Note: This control utilize AJAX toolkit control so ScriptManager should be registered on the page to use this control
    /// </summary>
    [ControlValueProperty("Text"), ValidationProperty("Text")]
    public partial class DateTimePicker : UserControlBase
    {        
        #region Properties

        /// <summary>
        /// Gets or sets the DatePiker TextBox text. This property is used as Validation Property
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return DateTextBox.Text; }
            set { DateTextBox.Text = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public DateTime Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
       }

        /// <summary>
        /// Gets the input control.
        /// </summary>
        /// <value>The input.</value>
        public WebControl Input
        {
            get { return DateTextBox; }
        }

        /// <summary>
        /// Gets the current language.
        /// </summary>
        /// <value>The language.</value>
        public static string Language
        {
            get
            {
                return ContentLanguage.PreferredCulture.IetfLanguageTag;
            }
        }

      
        #endregion

        #region EventHandlers

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            SelectHours.Items.Clear();
            for (int i = 0; i <= 23; i++)
            {
                SelectHours.Items.Add(new ListItem(i.ToString() ,i.ToString()));
            }
            
            SelectMinutes.Items.Clear();
            for (int i = 0; i <= 55; i=i+5)
            {
                SelectMinutes.Items.Add(new ListItem(i.ToString("D2"), i.ToString()));
            }

            Page.Header.Controls.Add(CreateCSSLink(Page.ResolveUrl("~/Templates/AlloyTech/scripts/jquery/jquery.ui.datepicker.custom.theme.css")));
        }
       

        #endregion

        #region Helpers
      
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetValue(DateTime value)
        {
            //Initialize DatePicker text box
            switch (this.CurrentPage.LanguageBranch.ToUpperInvariant())
            {
                case "EN":
                    DateTextBox.Text = value.ToString("MM/dd/yyyy");
                    break;
                case "SV":
                    DateTextBox.Text = value.ToString("yyyy/MM/dd");
                    break;
                default:
                    DateTextBox.Text = value.ToString("MM/dd/yyyy");
                    break;
            }            

            //Initialize Hour selector drop down
            string hourValue = value.Hour.ToString();
            SelectHours.ClearSelection();

            ListItem hourSelected = SelectHours.Items.Cast<ListItem>().FirstOrDefault(item => String.Equals(item.Value, hourValue, StringComparison.OrdinalIgnoreCase));
            if (hourSelected != null)
            {
                hourSelected.Selected = true;
            }
            else
            {
                ListItem newHourItem = new ListItem(hourValue, hourValue);
                SelectHours.Items.Add(newHourItem);
                newHourItem.Selected = true;
            }

            //Initialize Minute selector drop down
            string minuteValue = value.Minute.ToString();
            SelectMinutes.ClearSelection();

            ListItem minutesSelected = SelectMinutes.Items.Cast<ListItem>().FirstOrDefault(item => String.Equals(item.Value, minuteValue, StringComparison.OrdinalIgnoreCase));
            if (minutesSelected != null)
            {
                minutesSelected.Selected = true;
            }
            else
            {
                ListItem newMinuteItem = new ListItem(minuteValue, minuteValue);
                SelectMinutes.Items.Add(newMinuteItem);
                newMinuteItem.Selected = true;
            }
        }


        /// <summary>
        /// Gets the current DateTimePicker DateTime value.
        /// </summary>
        /// <returns></returns>
        private DateTime GetValue()
        {
            DateTime dateValue = new DateTime();
            try
            {
                dateValue = DateTime.Parse(DateTextBox.Text);
            }
            catch (FormatException) {}
            DateTime dateTimeValue = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, int.Parse(SelectHours.SelectedValue), int.Parse(SelectMinutes.SelectedValue), 0);           
            return dateTimeValue;
        }

        /// <summary>
        /// Create a HtmlLink element with attributes for a css file
        /// </summary>
        /// <param name="cssFilePath">Path to the css file to be included</param>
        /// <returns>A new HtmlLink element</returns>
        private HtmlLink CreateCSSLink(string cssFilePath)
        {
            HtmlLink linkTag = new HtmlLink();
            linkTag.Attributes.Add("type", "text/css");
            linkTag.Attributes.Add("rel", "stylesheet");
            linkTag.Href = ResolveUrl(cssFilePath);
            return linkTag;
        }

        #endregion
    }
}
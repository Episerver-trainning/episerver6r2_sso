#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
#region Known Limitations
/*
The template has the following known limitations:

- Usability limitations
  CalendarView selection won't work without javascript.
  
- Performance Limitations
  Using two calendar lists may not be optimal performance wise.

- Architecture limitations
  Calendar item pages has to be visible in menu. 
*/
#endregion

using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Templates.AlloyTech;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    public partial class Calendar : WorkroomPageBase
    {
        private const string calendarEventPageTypeName = "[AlloyTech] Workroom Calendar event";        
        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditCalendarBox.ButtonSaveClicked += new CommandEventHandler(EditCalendarBox_ButtonSaveClicked);
        }

        /// <summary>
        /// Handles the ButtonSaveClicked event of the EditCalendarBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        void EditCalendarBox_ButtonSaveClicked(object sender, CommandEventArgs e)
        {
            PageData page = CurrentPage.CreateWritableClone();
            string pageName = EditCalendarBox.Text.ToSafeString();
            page["PageName"] = pageName.Length > 255 ? pageName.Remove(254) : pageName;
            DataFactory.Instance.Save(page, SaveAction.Publish);
            Response.Redirect(page.LinkURL);
        }
        /// <summary>
        /// Handles the Click event of the DeleteCalendarSection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteCalendarSection_Click(object sender, EventArgs e)
        {
            string redirectUrl = DataFactory.Instance.GetPage(CurrentPage.ParentLink).LinkURL;
            DataFactory.Instance.Delete(CurrentPage.PageLink, true);
            Response.Redirect(redirectUrl);
        }
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                EnableViewMode();

                EditCalendarBox.Text = CurrentPage.PageName;
                DeleteCalendarSectionLinkButton.OnClientClick = string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete")); 

                // Default begin date is todays date.
                CalendarList.BeginDate = DateTime.Now;
                CalendarList.DataBind();

                // MonthlyItems is a hidden calendar list used to calculate what days to highlight in CalenderView.
                // We use the first of current month as begin date but substract 6 days since CalendarView can show up to 6 days of previous month.
                MonthlyItems.BeginDate = new DateTime(CalendarView.TodaysDate.Year, CalendarView.TodaysDate.Month, 1).AddDays(-6);
                // NumberOfDaysToRender is calculated to 6 + 31 + 13 = 50.
                // 6 = CalendarView can show up to 6 days of previous month.
                // 31 = Maximum number of days in a month.
                // 13 = Calenda rView can show up to 13 days of next month.
                MonthlyItems.NumberOfDaysToRender = 50;
                MonthlyItems.DataBind();
            }

            CalendarView.SelectionChanged += new EventHandler(CalendarView_SelectionChanged);
            CalendarView.DayRender += new DayRenderEventHandler(CalendarView_DayRender);
            CalendarView.VisibleMonthChanged += new MonthChangedEventHandler(CalendarView_VisibleMonthChanged);
        }
    

        /// <summary>
        /// Gets a display string for the selected period.
        /// </summary>
        /// <returns>A string with start and end date of selected period</returns>
        protected string GetSelectedPeriod()
        {
            EnsureChildControls();

            StringBuilder returnValue = new StringBuilder();

            if (CalendarList.SelectedDates.Count == 1)
            {
                returnValue.Append(CalendarList.SelectedDates[0].ToFormattedDate());
            }
            else if (CalendarList.SelectedDates.Count > 1)
            {
                returnValue.Append(CalendarList.SelectedDates[0].ToFormattedDate());
                returnValue.Append(" &#8722; ");
                returnValue.Append(CalendarList.SelectedDates[CalendarList.SelectedDates.Count - 1].ToFormattedDate());
            }
            else
            {
                DateTime beginDate = CalendarList.BeginDate;
                returnValue.Append(beginDate.ToFormattedDate());
                if (CalendarList.NumberOfDaysToRender > 1)
                {
                    returnValue.Append(" &#8722; ");
                    returnValue.Append(beginDate.AddDays(CalendarList.NumberOfDaysToRender - 1).ToFormattedDate());
                }
            }
            
            return returnValue.ToString();
        }

        /// <summary>
        /// Setups the data for the Calendar list.
        /// </summary>
        protected void SetupData()
        {
            CalendarList.SelectedDates = CalendarView.SelectedDates;
            CalendarList.DataBind();
        }

        protected int GetNumberOfDayToRender()
        {
            int totalDays = (int)(new DateTime(9999, 12, 31) - DateTime.Now).TotalDays + 1;
            if ((int)CurrentPage["nDaysToRender"] > totalDays)
            {
                return totalDays;
            }
            return (int)CurrentPage["nDaysToRender"];
        }

        #region CalendarView Event Handlers
        
        private void CalendarView_SelectionChanged(object sender, EventArgs e)
        {
            SetupData();
        }

        private void CalendarView_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            // Set BeginDate for hidden calendar list.
            // Substract 6 days since calendar view can show up to 6 days of previous month.
            MonthlyItems.BeginDate = e.NewDate.AddDays(-6);
        }

        private void CalendarView_DayRender(object sender, DayRenderEventArgs e)
        {
            TableCell cell = e.Cell;
            cell.HorizontalAlign = HorizontalAlign.NotSet;
            if (e.Day.IsToday)
            {
                cell.CssClass += " today";
            }

            if (MonthlyItems.DateIsActive(e.Day.Date))
            {
                cell.CssClass += " active";
                //cell.ForeColor = ColorTranslator.FromHtml("#ffffff");
            }
        }
        
        #endregion

        #region Button event handlers

        /// <summary>
        /// Creates a new page of the calendar event type and saves it to the database with the values entered by the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            PageData newCalendarEvent = DataFactory.Instance.GetDefaultPageData(CurrentPageLink, calendarEventPageTypeName);
            if (!Page.IsValid)
            {
                EnableEditMode();
                return;
            }
            try
            {
                DateTime startDate = StartDatePicker.Value;
                DateTime stopDate = StopDatePicker.Value;

                if (startDate == DateTime.MinValue)
                {
                    EnableEditMode();
                    Page.Validators.Add(new StaticValidator(Translate("/workroom/calendar/invalidstartdate")));
                    return;
                }

                if (stopDate == DateTime.MinValue)
                {
                    EnableEditMode();
                    Page.Validators.Add(new StaticValidator(Translate("/workroom/calendar/invalidstopdate")));
                    return;
                }

                if (startDate > stopDate)
                {
                    EnableEditMode();
                    Page.Validators.Add(new StaticValidator(Translate("/workroom/calendar/latestarttime")));
                    return;
                }

                newCalendarEvent.PageName = EditPageName.Text.ToSafeString();
                newCalendarEvent["MainBody"] = EditMainBody.Text.ToSafeString();
                newCalendarEvent["EventStartDate"] = startDate;
                newCalendarEvent["EventStopDate"] = stopDate;
                newCalendarEvent.StartPublish = DateTime.Now.AddMinutes(-1);

                DataFactory.Instance.Save(newCalendarEvent, SaveAction.Publish);
                //Rebind calendar listing to ensure that the new item is included.
                SetupData();

                EnableViewMode();
            }
            catch (Exception x)
            {
                EnableEditMode();
                Page.Validators.Add(new StaticValidator(x.Message));
            }
        }

        /// <summary>
        /// Switches from edit mode to view mode.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            EnableViewMode();
        }

        /// <summary>
        /// Switches from view mode to edit mode.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddButton_Click(object sender, EventArgs e)
        {
            EnableEditMode();
        }

        #endregion

        #region Private helper methods

        private void EnableViewMode()
        {
            EditPanel.Visible = false;
            EditPageName.Text = EditMainBody.Text = StartDatePicker.Text = StopDatePicker.Text = String.Empty;

            CalendarView.Enabled = true;
            DisplayPanel.Visible = true;
            
            if (Membership.QueryDistinctMembershipLevel(CurrentPage, MembershipLevels.Administer))
            {
                EditCalendarSectionButtons.Visible = true;
                EditCalendarBox.Visible = true;
            }

            if (Membership.QueryDistinctMembershipLevel(CurrentPage, MembershipLevels.Modify))
            {
                AddButton.Visible = true;
            }
        }

        private void EnableEditMode()
        {
            CalendarView.Enabled = false;
            DisplayPanel.Visible = false;
            AddButton.Visible = false;

            EditCalendarSectionButtons.Visible = false;
            EditPanel.Visible = true;
        }

        #endregion

        #region Public helper methods
        /// <summary>
        /// Gets the HTML-Formatted event time period.
        /// </summary>
        /// <returns></returns>
        public string GetFormattedTimePeriod(DateTime startDate, DateTime endDate)
        {
            StringBuilder timePeriod = new StringBuilder();
            if (startDate.ToShortDateString().Equals(endDate.ToShortDateString()))
            {
                timePeriod.Append("<div class=\"calendarDateRow\">");
                timePeriod.Append("<div class=\"calendarDateValue\">");
                timePeriod.Append(startDate.DayOfWeek.ToString());
                timePeriod.Append(", ");
                timePeriod.Append(startDate.ToFormattedDate());
                timePeriod.Append(" ");
                timePeriod.Append(startDate.ToFormattedTime());
                timePeriod.Append(" - ");
                timePeriod.Append(endDate.ToFormattedTime());
                timePeriod.Append("</div>");
                timePeriod.Append("</div>");
            }
            else
            {
                timePeriod.Append("<div class=\"calendarDateRow\">");
                timePeriod.Append("<div class=\"calendarDateLabel\">" + Translate("/workroom/calendarlist/calendaritemfrom") + "</div> ");
                timePeriod.Append("<div class=\"calendarDateValue\">");
                timePeriod.Append(startDate.DayOfWeek.ToString());
                timePeriod.Append(", ");
                timePeriod.Append(startDate.ToFormattedDate());
                timePeriod.Append(" ");
                timePeriod.Append(startDate.ToFormattedTime());
                timePeriod.Append("</div>");
                timePeriod.Append("</div>");
                timePeriod.Append("<div class=\"calendarDateRow\">");
                timePeriod.Append("<div class=\"calendarDateLabel\">" + Translate("/workroom/calendarlist/calendaritemto") + "</div> ");
                timePeriod.Append("<div class=\"calendarDateValue\">");
                timePeriod.Append(endDate.DayOfWeek.ToString());
                timePeriod.Append(", ");
                timePeriod.Append(endDate.ToFormattedDate());
                timePeriod.Append(" ");
                timePeriod.Append(endDate.ToFormattedTime());
                timePeriod.Append("</div>");
                timePeriod.Append("</div>");
            }
            return timePeriod.ToString();
        }

        #endregion
    }
}


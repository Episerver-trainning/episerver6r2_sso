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
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Web.WebControls;
using System.Drawing;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace EPiServer.Templates.AlloyTech.Pages
{
    public partial class Calendar : TemplatePage
    {
        /// <summary>
        /// Gets the date format sting to use when presenting an event date.
        /// </summary>
        protected string DateFormat
        {
            get { return "d"; } 
        }

        /// <summary>
        /// Gets the time format string to use when presenting an event time.
        /// </summary>
        protected string TimeFormat
        {
            get { return "t"; }
        }

        /// <summary>
        /// Gets the date format string to use in the calendar event list.
        /// </summary>
        protected string ListDateFormat
        {
            get { return "D"; }
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CalendarView.SelectionChanged += new EventHandler(CalendarView_SelectionChanged);
            CalendarView.DayRender += new DayRenderEventHandler(CalendarView_DayRender);
            CalendarView.VisibleMonthChanged += new MonthChangedEventHandler(CalendarView_VisibleMonthChanged);

            if (!IsPostBack)
            {
                // Default begin date is todays date.
                CalendarList.BeginDate = DateTime.Now;
                CalendarList.DataBind();

                // MonthlyItems is a hidden calendar list used to calculate what days to highlight in CalenderView.
                // We use the first of current month as begin date but substract 6 days since CalendarView can show up to 6 days of previous month.
                MonthlyItems.BeginDate = new DateTime(CalendarView.TodaysDate.Year, CalendarView.TodaysDate.Month, 1).AddDays(-6);
                // NumberOfDaysToRender is calculated to 6 + 31 + 13 = 50.
                // 6 = CalendarView can show up to 6 days of previous month.
                // 31 = Maximum number of days in a month.
                // 13 = CalendarView can show up to 13 days of next month.
                MonthlyItems.NumberOfDaysToRender = 50;
                MonthlyItems.DataBind();
            }
        }

        /// <summary>
        /// Setups the data for the Calendar list.
        /// </summary>
        protected void SetupData()
        {
            CalendarList.SelectedDates = CalendarView.SelectedDates;
            CalendarList.DataBind();
        }

        /// <summary>
        /// Gets a correct nember of day to show the calendar
        /// </summary>
        /// <returns></returns>
        protected int GetNumberOfDayToRender()
        {
            int totalDays = (int)(new DateTime(9999, 12, 31) - DateTime.Now).TotalDays + 1;
            if ((int)CurrentPage["nDaysToRender"] > totalDays)
            {
                return totalDays;
            }
            return (int)CurrentPage["nDaysToRender"];
        }


        /// <summary>
        /// Switch to display event information when an item in the calendar list is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void LinkButton_Command(Object sender, CommandEventArgs e)
        {
            PageReference pageRef = new PageReference((string)e.CommandArgument);
            EventMainBody.PageLink = pageRef;
            EventPageName.PageLink = pageRef;

            EventDate.Text = GetEventDateString(GetPage(pageRef));
            EventDate.Visible = EventDate.Text != string.Empty;

            CalendarView.SelectedDates.Clear();

            SetVisiblePanel(false);
        }

        #region CalendarView Event Handlers

        private void CalendarView_SelectionChanged(object sender, EventArgs e)
        {
            SetupData();
            SetVisiblePanel(true);
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

        #region Helper methods

        /// <summary>
        /// Gets a display string for the selected period.
        /// </summary>
        /// <returns>A string with start and end date of selected period</returns>
        protected string GetSelectedPeriod()
        {
            DateTime start = CalendarList.SelectedDates.Count > 0 ? CalendarList.SelectedDates[0] : CalendarList.BeginDate;
            DateTime end = CalendarList.SelectedDates.Count > 0
                ? CalendarList.SelectedDates[CalendarList.SelectedDates.Count - 1]
                : CalendarList.BeginDate.AddDays(CalendarList.NumberOfDaysToRender - 1);

            return start.ToString(DateFormat) + ((end - start).Days < 1 ? string.Empty : (" &#8722; " + end.ToString(DateFormat)));
        }

        /// <summary>
        /// Gets a display string containing the start and stop date of this event.
        /// </summary>
        /// <returns>A string containing the start and stop date of this event.</returns>
        private string GetEventDateString(PageData calendarEvent)
        {
            StringBuilder returnValue = new StringBuilder();

            DateTime startDate = (DateTime)calendarEvent["EventStartDate"];
            DateTime stopDate = (DateTime)calendarEvent["EventStopDate"];

            if (startDate.Date == stopDate.Date)
            {
                returnValue.Append(startDate.ToString(DateFormat));
                returnValue.Append(" ");
                returnValue.Append(FormatEventTimeString(startDate, stopDate));
            }
            else if (((int)(calendarEvent["ValidDays"] ?? 0)) != 0)
            {
                returnValue.Append(FormatEventTimeString(startDate, stopDate));
            }
            else
            {
                bool timeIsSet = (startDate.TimeOfDay != TimeSpan.Zero || stopDate.TimeOfDay != TimeSpan.Zero);

                returnValue.Append(startDate.ToString(DateFormat));
                if (timeIsSet)
                {
                    returnValue.Append(" ");
                    returnValue.Append(startDate.ToString(TimeFormat));
                }
                returnValue.Append(" &#8722; ");
                returnValue.Append(stopDate.ToString(DateFormat));
                if (timeIsSet)
                {
                    returnValue.Append(" ");
                    returnValue.Append(stopDate.ToString(TimeFormat));
                }
            }

            return returnValue.ToString();
        }

        private string FormatEventTimeString(DateTime startDate, DateTime stopDate)
        {
            if (startDate.TimeOfDay != stopDate.TimeOfDay)
            {
                return string.Format("{0} &#8722; {1}", startDate.ToString(TimeFormat), stopDate.ToString(TimeFormat));
            }
            else if (startDate.TimeOfDay != TimeSpan.Zero)
            {
                return startDate.ToString(TimeFormat);
            }
            return string.Empty;
        }


        private void SetVisiblePanel(bool showPeriodPanel)
        {
            DisplayPeriod.Visible = showPeriodPanel;
            DisplayEvent.Visible = !showPeriodPanel;
        }

        /// <summary>
        /// Creates the text used to display a specific day of an event. Returns a string containing the time for the calendar event.
        /// Sets up tooltip when needed and sets the relevant css class for the label EventDate.
        /// </summary>
        /// <param name="calendarEvent">A CalendarEventTemplateContainer for this specific event and day.</param>
        /// <returns>A string containing the time for the calendar event.</returns>
        protected string GetTextAndUpdateLabelProperties(CalendarEventTemplateContainer calendarEvent)
        {
            Label eventDate = calendarEvent.FindControl("EventDate") as Label;
            if (eventDate == null) return string.Empty;

            DateTime startDate = DateTime.Parse(calendarEvent.StartDate + " " + calendarEvent.StartTime);
            DateTime stopDate = DateTime.Parse(calendarEvent.StopDate + " " + calendarEvent.StopTime);

            // Check if recurring
            bool recurring = ((int)(calendarEvent.CurrentPage["ValidDays"] ?? 0)) != 0;

            //Depending on what kind of event it is (single day, multiple day or recurring), we want to display the time differently.

            // The event is a single day event, or a recurring event.
            if (startDate.Date == stopDate.Date || recurring)
            {
                if (recurring)
                {
                    eventDate.CssClass += " recurring";
                }

                if (startDate.TimeOfDay != stopDate.TimeOfDay)
                {
                    return string.Format("{0} - {1}", startDate.ToString(TimeFormat), stopDate.ToString(TimeFormat));
                }

                if (startDate.TimeOfDay != TimeSpan.Zero)
                {
                    return startDate.ToString(TimeFormat);
                }
            }
            else
            {
                // Set tooltip to the whole event date span
                eventDate.ToolTip = string.Format("{0} {1} - {2} {3}", startDate.ToString(DateFormat), startDate.ToString(TimeFormat), stopDate.ToString(DateFormat), stopDate.ToString(TimeFormat));

                // The event is a multi day event, and we're on the first day.
                if (calendarEvent.CurrentDate.Date == startDate.Date)
                {
                    eventDate.CssClass += " multiDate firstDate";
                    return string.Format("{0} -", startDate.ToString(TimeFormat));
                }
                // The event is a multi day event, and we're on the last day.
                else if (calendarEvent.CurrentDate.Date == stopDate.Date)
                {
                    eventDate.CssClass += " multiDate lastDate";
                    return string.Format("- {0}", stopDate.ToString(TimeFormat));
                }
                // The event is a multi day event, and we're neither on the first or the last day.
                else
                {
                    eventDate.CssClass += " multiDate midDate";
                }
            }
            return string.Empty;
        }

        #endregion
    }
}


#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Templates.AlloyTech;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    public partial class CalendarEvent : WorkroomPageBase
    {
       
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                DeleteButton.OnClientClick = string.Format("if(!confirm('{0}'))return false;", Translate("/common/messages/confirmdelete"));
                CalendarView.VisibleDate = (DateTime)CurrentPage["EventStartDate"];
            }

            if (QueryDistinctMembershipLevel(MembershipLevels.Modify))
            {
                DeleteButton.Visible = true;
                EditButton.Visible = true;
            }

            CalendarView.DayRender += new DayRenderEventHandler(CalendarView_DayRender);
        }

        /// <summary>
        /// Gets a display string containing the start and stop date of this event.
        /// </summary>
        /// <returns>A string containing the start and stop date of this event.</returns>
        protected string GetDisplayDateString()
        {
            StringBuilder sb = new StringBuilder();

            DateTime startDate = (DateTime)CurrentPage["EventStartDate"];
            DateTime stopDate = (DateTime)CurrentPage["EventStopDate"];

            if (startDate.Date == stopDate.Date)
            {
                sb.Append(startDate.ToFormattedDate());
                sb.Append(" &#8722; ");
                sb.Append(stopDate.ToFormattedTime());
            }
            else
            {
                sb.Append(startDate.ToFormattedDate());
                sb.Append(" &#8722; ");
                sb.Append(stopDate.ToFormattedDate());
            }
            
            return sb.ToString();
        }

        #region Button event handlers

        /// <summary>
        /// Deletes the current calendar event and redirects to it's parent.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Click(object sender, EventArgs e)
        {
            DataFactory.Instance.Delete(CurrentPageLink, true);

            Response.Redirect(DataFactory.Instance.GetPage(CurrentPage.ParentLink).LinkURL);
        }

        /// <summary>
        /// Hides the DisplayPanel and shows the EditPanel.
        /// Populates the TextBoxes with values from CurrentPage.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Edit_Click(object sender, EventArgs e)
        {
            DisplayPanel.Visible = false;
            EditPanel.Visible = true;

            EditPageName.Text = CurrentPage.PageName;
            EditMainBody.Text = (string)CurrentPage["MainBody"];
            StartDatePicker.Value = ((DateTime)CurrentPage["EventStartDate"]);
            StopDatePicker.Value = ((DateTime)CurrentPage["EventStopDate"]);
        }

        /// <summary>
        /// Updates the CurrentPage with the values entered by the user and saves the page to the database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            try
            {
                PageData curPage = CurrentPage.CreateWritableClone();

                DateTime startDate = StartDatePicker.Value;
                DateTime stopDate = StopDatePicker.Value;


                if (startDate > stopDate)
                {
                    Page.Validators.Add(new StaticValidator(Translate("/workroom/calendar/latestarttime")));
                    return;
                }

                curPage.PageName = EditPageName.Text.ToSafeString();
                curPage["MainBody"] = EditMainBody.Text.ToSafeString();
                if (curPage.LanguageBranch != curPage.MasterLanguageBranch)
                {
                    PageReference curPageRef = DataFactory.Instance.Save(curPage, SaveAction.Publish);

                    curPage = DataFactory.Instance.GetLanguageBranches(curPageRef)
                        .Cast<PageData>().First(page => page.LanguageID == page.MasterLanguageBranch);
                    curPage = curPage.CreateWritableClone();
                }
                curPage["EventStartDate"] = startDate;
                curPage["EventStopDate"] = stopDate;

                DataFactory.Instance.Save(curPage, SaveAction.Publish);
                Response.Redirect(DataFactory.Instance.GetPage(CurrentPageLink).LinkURL);
                
              
            }            
            catch (FormatException ex)
            {
                Page.Validators.Add(new StaticValidator(ex.Message));
            }
            catch (Exception x)
            {
                Page.Validators.Add(new StaticValidator(x.Message));
            }
        }

        /// <summary>
        /// Hides the edit panel, shows the display panel and clears all the input fields.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            DisplayPanel.Visible = true;
            EditPanel.Visible = false;
        }

        #endregion

        #region Private helper methods

        private void CalendarView_DayRender(object sender, DayRenderEventArgs e)
        {
            TableCell cell = e.Cell;
            cell.HorizontalAlign = HorizontalAlign.NotSet;
            if (e.Day.IsToday)
            {
                cell.CssClass += " today";
            }

            if (IsActive(e.Day.Date))
            {
                cell.CssClass += " active";
                //cell.ForeColor = ColorTranslator.FromHtml("#ffffff");
            }
        }

        private bool IsActive(DateTime day)
        {
            DateTime startDate = ((DateTime)CurrentPage["EventStartDate"]);
            startDate = startDate.Subtract(startDate.TimeOfDay);
            DateTime stopDate = (DateTime)CurrentPage["EventStopDate"];

            return day >= startDate && day <= stopDate;
        }

        #endregion
    }
}


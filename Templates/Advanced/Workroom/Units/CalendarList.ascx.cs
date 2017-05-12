#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Templates.AlloyTech;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    /// <summary>
    /// Upcoming list view mode
    /// </summary>
    public enum UpcomingListViewMode
    {
        /// <summary>
        /// List view
        /// </summary>
        ListView,
        /// <summary>
        /// Container view
        /// </summary>
        ContainerView
    }

    /// <summary>
    /// Lists X number of upcoming events.
    /// </summary>
    public partial class CalendarList : UserControlBase, INamingContainer
    {     
        private const string HideIfEmptyKey = "HideIfEmpty";
        private const string ViewModeKey = "ViewModeKey";

        // calendarEventPageTypeName is used to filter upcoming events.
        private const string calendarEventPageTypeName = "[AlloyTech] Workroom Calendar event";
      
        private PageDataCollection _upcomingEvents;
        private int _maxCount = 10;
        private PageReference _calendarContainer;
        private bool _listItemEven = true;


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (PageReference.IsNullOrEmpty(CalendarContainer))
            {
                this.Visible = false;
            }   
            switch(ViewMode)
            {
                case UpcomingListViewMode.ContainerView:
                    {
                        ContainerView.Visible = true;
                        ListView.Visible = false;
                        break;
                    }
                case UpcomingListViewMode.ListView:
                    {
                        ContainerView.Visible = false;
                        ListView.Visible = true;
                        break;
                    }
            }
            DataBind();
            
            if (CalendarEventList.DataCount == 0 && this.HideIfEmpty)
            {
                this.Visible = false;
            }
        }

        /// <summary>
        /// Gets a display string contain the start date and the end date of a calendar event.
        /// </summary>
        /// <param name="page">The PageData of the calendar event.</param>
        /// <returns>A string contain the start date and the end date of the calendar event.</returns>
        protected static string GetDateString(PageData page)
        {
            if (page["EventStartDate"] == null || page["EventStopDate"] == null)
            {
                throw new EPiServerException("All calender event has to have the properties EventStartDate and EventStopDate defined");
            }

            StringBuilder sb = new StringBuilder();

            DateTime startDate = (DateTime)page["EventStartDate"];
            DateTime stopDate = (DateTime)page["EventStopDate"];

            if (startDate.Date == stopDate.Date)
            {
                sb.Append(startDate.ToFormattedDateAndTime());
                sb.Append(" &#8722; ");
                sb.Append(stopDate.ToFormattedTime());
            }
            else
            {
                sb.Append(startDate.ToFormattedDateAndTime());
                sb.Append(" &#8722; ");
                sb.Append(stopDate.ToFormattedDateAndTime());
            }
            return sb.ToString();
        }
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {

            base.DataBind();
            CalendarEventList.DataSource = this.UpcomingEvents;
            PageList.DataSource = this.UpcomingEvents;

            listPanel.Visible = CalendarEventList.DataCount != 0;
            PageList.DataBind();
            CalendarEventList.DataBind();
            PageList.DataBind();
        }


        #region Public properties

        /// <summary>
        /// Gets or sets a list with the upcoming calendar events.
        /// The list is sorted by the start date of the event, and will only contain the X number of closest upcoming events.
        /// </summary>
        /// <remarks>
        /// Can also be set to a custom PageDataCollection when calling the user control.
        /// </remarks>
        /// <value>A PageDataCollection with the upcoming events.</value>
        public PageDataCollection UpcomingEvents
        {
            get 
            {
                if (_upcomingEvents == null)
                {
                    if (PageReference.IsNullOrEmpty(CalendarContainer))
                    {
                        _upcomingEvents = new PageDataCollection();
                    }
                    else
                    {
                        _upcomingEvents = new PageDataCollection();

                        PageDataCollection calendarListsCollection = DataFactory.Instance.GetChildren(CalendarContainer);
                        foreach (PageData calendarList in calendarListsCollection)
                        {
                            if (String.Equals(calendarList.PageTypeName, calendarEventPageTypeName, StringComparison.OrdinalIgnoreCase))
                            {
                                _upcomingEvents.Add(calendarList);
                            }
                            else
                            {
                                foreach (PageData childPage in DataFactory.Instance.GetChildren(calendarList.PageLink).Where(child => 
                                    String.Equals(child.PageTypeName, calendarEventPageTypeName, StringComparison.OrdinalIgnoreCase)))
                                {
                                    _upcomingEvents.Add(childPage);
                                }
                            }
                        }

                        _upcomingEvents = FilterForVisitor.Filter(_upcomingEvents);
                        new FilterCompareTo("PageTypeName", calendarEventPageTypeName).Filter(_upcomingEvents);
                        _upcomingEvents = ClearObsoleteEvents(_upcomingEvents);
                        new FilterPropertySort("EventStartDate", FilterSortDirection.Ascending).Filter(_upcomingEvents);
                        new FilterCount(MaxCount).Filter(_upcomingEvents);
                    }
                }
                return _upcomingEvents; 
            }
        }

        /// <summary>
        /// Gets or sets the number of events to list.
        /// </summary>
        /// <value>The number of events to list.</value>
        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value; }
        }

        /// <summary>
        /// Gets or sets the calendar page reference.
        /// </summary>
        /// <value>The a PageReference to the calendar page.</value>
        public PageReference CalendarContainer
        {
            get { return _calendarContainer; }
            set { _calendarContainer = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether controls is hidden if no items.
        /// </summary>
        /// <value><c>true</c> [hide if empty]; otherwise, <c>false</c>.</value>
        public bool HideIfEmpty
        {
            get
            {
                if (ViewState[HideIfEmptyKey] == null) { return false; }
                else { return bool.Parse(ViewState[HideIfEmptyKey].ToString()); }
            }
            set { ViewState[HideIfEmptyKey] = value; }
        }

        /// <summary>
        /// Gets or sets the view mode.
        /// </summary>
        /// <value>The view mode.</value>
        public UpcomingListViewMode ViewMode
        {
            get
            {
                if (ViewState[ViewModeKey] == null) { return UpcomingListViewMode.ContainerView; }
                else { return (UpcomingListViewMode)Enum.Parse(typeof(UpcomingListViewMode), ViewState[ViewModeKey].ToString(), true); }
            }
            set { ViewState[ViewModeKey] = value; }
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Gets a value indicating whether list items are in even (2,4,6...) or odd list positions.
        /// </summary>
        /// <value><c>true</c> if item is in even position otherwise, <c>false</c>.</value>
        protected bool ListItemEven
        {
            get
            {
                return _listItemEven = !_listItemEven;
            }
        }
        
        
        //Removes any events that has an EventStopDate that is in the past.
        private static PageDataCollection ClearObsoleteEvents(PageDataCollection upcomingEvents)
        {
            return new PageDataCollection(upcomingEvents.Where(p => ((DateTime)p["EventStopDate"]) > DateTime.Now));
        }

        #endregion
    }
}

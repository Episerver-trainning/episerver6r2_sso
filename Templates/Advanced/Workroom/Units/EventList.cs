#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using EPiServer.Core;
using EPiServer.Web.WebControls;
using EPiServer.Filters;

namespace EPiServer.Templates.Advanced.Workroom.Units
{
    /// <summary>
    /// 
    /// </summary>
    [ParseChildren(true), PersistChildren(false)]
    public class EventList : PageControlBase, INamingContainer
    {
        // Fields    
        private ITemplate _itemTemplate;
        private SelectedDatesCollection _selectedDates;

        private const string EVENT_START_DATE_PROPERTY_NAME = "EventStartDate";
        private const string EVENT_STOP_DATE_PROPERTY_NAME = "EventStopDate";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventList"/> class.
        /// </summary>
        public EventList()
        {
            this.NumberOfDaysToRender = 7;         
        }

        #region Protected methods
        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (this.EventTemplate != null)
            {
                this.SetCalendarInterval();
                PageDataCollection pages = base.GetPages();

                int num = 0;
                this.ValidatePages(pages);
                foreach (PageData data in pages)
                {
                    DateTime _eventStartDate = (DateTime)data[EVENT_START_DATE_PROPERTY_NAME];
                    DateTime _eventStopDate = (DateTime)data[EVENT_STOP_DATE_PROPERTY_NAME];
                    if ((_eventStartDate.Date <= this.BeginDate.AddDays(this.NumberOfDaysToRender - 1) && (_eventStopDate.Date >= this.BeginDate.Date)))
                    {
                        this.CreateEventTemplate(data, _eventStartDate, _eventStopDate);
                        num++;
                    }
                    if ((this.MaxCount > 0) && (num >= this.MaxCount))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the event template.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="eventStartDate">The event start date.</param>
        /// <param name="eventStopDate">The event stop date.</param>
        protected virtual void CreateEventTemplate(PageData page, DateTime eventStartDate, DateTime eventStopDate)
        {
            Control container = new EventListEventTemplateContainer(page, eventStartDate, eventStopDate);
            this.EventTemplate.InstantiateIn(container);
            this.Controls.Add(container);
        }

        /// <summary>
        /// Creates the pre sort filters.
        /// </summary>
        protected override void CreatePreSortFilters()
        {
            if (this.PageTypeId != 0)
            {
                FilterCompareTo to = new FilterCompareTo("PageTypeID", this.PageTypeId.ToString());
                base.Filter += new FilterEventHandler(to.Filter);
            }
            if (this.EnableVisibleInMenu)
            {
                FilterCompareTo to2 = new FilterCompareTo("PageVisibleInMenu", "True");
                base.Filter += new FilterEventHandler(to2.Filter);
            }
            FilterRemoveNullValues values = new FilterRemoveNullValues(EVENT_START_DATE_PROPERTY_NAME);
            base.Filter += new FilterEventHandler(values.Filter);
            FilterRemoveNullValues values2 = new FilterRemoveNullValues(EVENT_STOP_DATE_PROPERTY_NAME);
            base.Filter += new FilterEventHandler(values2.Filter);
        }

        /// <summary>
        /// Creates the post sort filters.
        /// </summary>
        protected override void CreatePostSortFilters()
        {
        }


        /// <summary>
        /// Creates the sort filters.
        /// </summary>
        protected override void CreateSortFilters()
        {
            EventSorter sorter = new EventSorter();
            base.Filter += new FilterEventHandler(sorter.Filter);
        }


        /// <summary>
        /// Populates the pages.
        /// </summary>
        /// <param name="pages">The pages.</param>
        protected override void PopulatePages(PageDataCollection pages)
        {
            PageTreeData data = new PageTreeData();
            data.EnableVisibleInMenu = false;
            data.PageLink = this.PageLink;
            data.PageSource = this.PageSource;
            data.ExpandAll = true;
            foreach (PageData page in data)
            {
                pages.Add(page);
            }
            base.ExecFilters(pages);
        }

        /// <summary>
        /// Sets the calendar interval.
        /// </summary>
        protected virtual void SetCalendarInterval()
        {            
            //Selected Dates is nit null than update BeginDate and NumberOfDaysToRender properties
            if ((this._selectedDates != null) && (this._selectedDates.Count > 0))
            {
                DateTime time;
                DateTime timeTemp;
                this.BeginDate = time = this._selectedDates[0];
                timeTemp = this._selectedDates[this._selectedDates.Count - 1];
                TimeSpan span = (TimeSpan)(timeTemp - time);
                this.NumberOfDaysToRender = span.Days + 1;
            }

        }

        #endregion

        #region Helpers
        /// <summary>
        /// Validates the pages.
        /// </summary>
        /// <param name="pages">The pages.</param>
        private void ValidatePages(PageDataCollection pages)
        {
            bool ignoreInvalidPages = this.IgnoreInvalidPages;
            for (int i = pages.Count - 1; i >= 0; i--)
            {
                PageData data = pages[i];
                if ((data.Property[EVENT_START_DATE_PROPERTY_NAME] == null) || (data.Property[EVENT_STOP_DATE_PROPERTY_NAME] == null))
                {
                    if (!ignoreInvalidPages)
                    {
                        throw new EPiServerException("Calendar pages must include properties EventStartDate and EventStopDate");
                    }
                    pages.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the begin date. Begining from this date the control will display events.
        /// </summary>
        /// <value>The begin date.</value>
        public DateTime BeginDate
        {
            get
            {
                if (this.ViewState["BeginDate"] != null)
                {
                    return (DateTime)this.ViewState["BeginDate"];
                }
                return DateTime.Now;
            }
            set
            {
                this.ViewState["BeginDate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable visible in menu].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable visible in menu]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false), Bindable(true), Description("Specifies whether the 'visible in menu' settings should be considered"), Category("Information"), Browsable(true)]
        public bool EnableVisibleInMenu
        {
            get
            {
                return (bool)(this.ViewState["_EnableVisibleInMenu"] ?? false);
            }
            set
            {
                this.ViewState["_EnableVisibleInMenu"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the event template.
        /// </summary>
        /// <value>The event template.</value>
        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(EventListEventTemplateContainer))]
        public ITemplate EventTemplate
        {
            get
            {
                return this._itemTemplate;
            }
            set
            {
                this._itemTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore invalid pages].
        /// </summary>
        /// <value><c>true</c> if [ignore invalid pages]; otherwise, <c>false</c>.</value>
        public bool IgnoreInvalidPages
        {
            get
            {
                return (bool)(this.ViewState["IgnoreInvalidPages"] ?? false);
            }
            set
            {
                this.ViewState["IgnoreInvalidPages"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the max count.
        /// </summary>
        /// <value>The max count.</value>
        public int MaxCount
        {
            get
            {
                if (this.ViewState["MaxCount"] != null)
                {
                    return (int)this.ViewState["MaxCount"];
                }
                return -1;
            }
            set
            {
                this.ViewState["MaxCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of days to render. Specifies the number of days from wich events should be display.
        /// </summary>
        /// <value>The number of days to render.</value>
        public int NumberOfDaysToRender
        {
            get
            {
                if (this.ViewState["NumberOfDaysToRender"] != null)
                {
                    return (int)this.ViewState["NumberOfDaysToRender"];
                }
                return 7;
            }
            set
            {
                this.ViewState["NumberOfDaysToRender"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the page type ID.
        /// </summary>
        /// <value>The page type ID.</value>
        public int PageTypeId
        {
            get
            {
                if (this.ViewState["PageTypeId"] != null)
                {
                    return (int)this.ViewState["PageTypeId"];
                }
                return 0;
            }
            set
            {
                this.ViewState["PageTypeId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected dates.
        /// </summary>
        /// <value>The selected dates.</value>
        public SelectedDatesCollection SelectedDates
        {
            get
            {
                if (this._selectedDates == null)
                {
                    this._selectedDates = new SelectedDatesCollection(new ArrayList());
                }
                return this._selectedDates;
            }
            set
            {
                DateTime[] array = new DateTime[value.Count];
                value.CopyTo(array, 0);
                this._selectedDates = new SelectedDatesCollection(new ArrayList(array));
            }
        }

        #endregion

      
        #region Event sorter class

        /// <summary>
        /// IComparer class using for sorting Event items.
        /// </summary>
        protected class EventSorter : IComparer<PageData>
        {                       
            /// <summary>
            /// Compares the specified x page.
            /// </summary>
            /// <param name="xPage">The x page.</param>
            /// <param name="yPage">The y page.</param>
            /// <returns></returns>
            public int Compare(PageData x, PageData y)
            {
                DateTime time = (DateTime)x.Property[EVENT_START_DATE_PROPERTY_NAME].Value;
                DateTime time2 = (DateTime)y.Property[EVENT_START_DATE_PROPERTY_NAME].Value;
                if (time2.TimeOfDay > time.TimeOfDay)
                {
                    return -1;
                }
                if (time2.TimeOfDay < time.TimeOfDay)
                {
                    return 1;
                }
                return 0;
            }

            /// <summary>
            /// Filters the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="EPiServer.Filters.FilterEventArgs"/> instance containing the event data.</param>
            public void Filter(object sender, FilterEventArgs e)
            {
                e.Pages.Sort(this);
            }
        }
        #endregion
    } 
       

    #region Event Template container class

    /// <summary>
    /// Template Contaiber class for Calendar Event item
    /// </summary>
    public class EventListEventTemplateContainer : PageTemplateContainer, INamingContainer
    {
        
        // Fields          
        private DateTime _startDate;
        private DateTime _stopDate;        
  
        // Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListEventTemplateContainer"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="stopDate">The stop date.</param>
        public EventListEventTemplateContainer(PageData page, DateTime startDate, DateTime stopDate)
            : base(page)
        {
            this._startDate = startDate;
            this._stopDate = stopDate;            
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get
            {
                return this._startDate;
            }
        }

        /// <summary>
        /// Gets the stop date.
        /// </summary>
        /// <value>The stop date.</value>
        public DateTime StopDate
        {
            get
            {
                return this._stopDate;
            }
        }
    }

    #endregion
}

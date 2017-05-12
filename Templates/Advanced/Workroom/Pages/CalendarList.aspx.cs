#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;

using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Core;
using EPiServer.DataAccess;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    public partial class CalendarList : WorkroomPageBase
    {
        private bool IsAddNewMode
        {
            get { return Request.QueryString["addmode"] == "true"; }
        }

        #region Event Handlers
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CreateCalendarBox.ButtonSaveClicked += CreateCalendarBox_ButtonSaveClicked;
        }

        /// <summary>
        /// Handles the ButtonSaveClicked event of the CreateCalendarBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        void CreateCalendarBox_ButtonSaveClicked(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            PageData newPage = DataFactory.Instance.GetDefaultPageData(CurrentPageLink, CalendarPageTypeName);
            newPage.PageName = CreateCalendarBox.Text;
            newPage.StartPublish = DateTime.Now.AddMinutes(-1);
            DataFactory.Instance.Save(newPage, SaveAction.Publish);

            CalendarPageList.DataBind();
            CreateCalendarBox.Text = string.Empty;
            if (IsAddNewMode)
            {
                Response.Redirect(Request.RawUrl.Replace("addmode=true", "addmode=false"));
            }
            InitializePage();
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
                PageDataCollection children = GetChildren(CurrentPageLink);
                if (children.Count == 1)
                {
                    if (!IsAddNewMode)
                    {
                        Response.Redirect(children[0].LinkURL);
                    }
                }              

                if (QueryDistinctMembershipLevel(MembershipLevels.Administer))
                {
                    ToolBarSection.Visible = true;
                    CreateCalendarBox.Visible = true;
                }
            }
            CalendarPageList.DataBind();
        }

        #endregion
    }
}

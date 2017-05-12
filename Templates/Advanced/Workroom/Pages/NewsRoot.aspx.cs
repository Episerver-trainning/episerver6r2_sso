#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Security;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    public partial class NewsRoot : WorkroomPageBase
    {
        private bool IsAddNewMode
        {
            get { return Request.QueryString["addmode"] == "true"; }
        }

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
                NewsLists.DataBind();
                if (QueryDistinctMembershipLevel(MembershipLevels.Administer))
                {
                    NewsSectionToolBar.Visible = true;
                    AddNewNewsSectionPanel.Visible = true;
                }
            }
        }

        protected void AddNewNewsSection_Click(object sender, EventArgs e)
        {

            PageData newPage = DataFactory.Instance.GetDefaultPageData(CurrentPageLink, NewsListPageTypeName);
            
            newPage.PageName = NewNewsSectionNameTextBox.Text;
            newPage["MainBody"] = GetDefaultNewsSectionBody();
            newPage.StartPublish = DateTime.Now.AddMinutes(-1);
            DataFactory.Instance.Save(newPage, SaveAction.Publish);

            NewsLists.DataBind();
            NewNewsSectionNameTextBox.Text = string.Empty;
            InitializePage();
            if (IsAddNewMode)
            {
                Response.Redirect(Request.RawUrl.Replace("addmode=true","addmode=false"));
            }
        }

        private string GetDefaultNewsSectionBody()
        {
            string defaultBody = String.Empty;

            PageReference currentWorkroomTemplate = WorkroomStartPage["WorkroomTemplate"] as PageReference;
            if (currentWorkroomTemplate == null)
            {
                return defaultBody;
            }
            
            PageReference currentWorkroomNewsTemplate = DataFactory.Instance.GetPage(currentWorkroomTemplate)["NewsRoot"] as PageReference;
            if (currentWorkroomNewsTemplate == null)
            {
                return defaultBody;
            }

            PageDataCollection templatePages = DataFactory.Instance.GetChildren(currentWorkroomNewsTemplate);
            if (templatePages.Count == 0)
            {
                return defaultBody;
            }
            
            return templatePages[0]["MainBody"] as string;
        }
    }
}

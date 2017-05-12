#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion

using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.Templates.Advanced.Workroom.Core;
using EPiServer.Templates.AlloyTech;
using EPiServer.Web;
using EPiServer.Web.Hosting;

namespace EPiServer.Templates.Advanced.Workroom.Pages
{
    /// <summary>
    /// List all avaliable work rooms.
    /// Used to create new workrooms.
    /// </summary>
    public partial class WorkroomList : WorkroomPageBase
    {
        private const string _changeString = "document.getElementById('{0}').innerHTML = this.options[this.selectedIndex].getAttribute('description');";
        private const string _mainBodyPropertyName = "MainBody";
        private PageDataCollection _workrooms;

        /// <summary>
        /// Gets whether redirect to the user profile page enabled or not
        /// </summary>
        protected override bool RedirectEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a list of all workrooms. Removes any that has where PageVisibleInMenu is false.
        /// </summary>
        /// <remarks>
        /// We need to manually remove pages with PageVisibleInMenu false since the PageList, which uses this property, doesn't filter that on it's own.
        /// </remarks>
        /// <value>A PageDataCollection containing all workrooms.</value>
        protected PageDataCollection Workrooms
        {
            get
            {
                if (_workrooms == null)
                {
                    _workrooms = DataFactory.Instance.GetChildren(CurrentPageLink);

                    FilterCompareTo filter = new FilterCompareTo("PageTypeName", StartPageTypeName);
                    filter.Condition = CompareCondition.Equal;
                    filter.Filter(_workrooms);
                }
                return _workrooms;
            }
        }

        #region Button click event handlers

        /// <summary>
        /// Sets up a new workroom and redirects to that workroom when it's been set up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CreateNewWorkroom_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                PageData newWorkroom = CopyAndSetUpWorkroomPage(PageReference.Parse(SelectTemplateDropDown.SelectedValue));

                CreateFolder(newWorkroom);

                SetUpAccessRights(newWorkroom);

                DeleteNotPublishedPageVersions(newWorkroom.PageLink);

                Response.Redirect(newWorkroom.LinkURL);
            }
        }

        /// <summary>
        /// Toggles the view between the workroom list panel and the create workroom panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ToggleView_Click(object sender, EventArgs e)
        {
            WorkroomListPanel.Visible = !WorkroomListPanel.Visible;
            NewWorkroomPanel.Visible = !NewWorkroomPanel.Visible;

            if (NewWorkroomPanel.Visible)
            {
                InitializeTemplateDropdown();
            }
        }

        #endregion

        /// <summary>
        /// Creates list item for template
        /// </summary>
        /// <param name="page">Template to use</param>
        /// <returns>List item for the template</returns>
        private static ListItem CreateListItem(PageData page)
        {
            ListItem item = new ListItem(page.PageName, page.PageLink.ID.ToString());
            item.Attributes["description"] = (string)page[_mainBodyPropertyName];
            return item;
        }

        /// <summary>
        /// Initializes drop down with templates
        /// </summary>
        private void InitializeTemplateDropdown()
        {
            SelectTemplateDropDown.Items.Clear();
            PageReference root = WorkroomTemplates;
            var pages = DataFactory.Instance.GetChildren(root);
            var templates = pages.Where(page => page.PageTypeName.Equals(StartPageTypeName));
            var items = templates.Select(template => CreateListItem(template));

            if (items.Count() == 0)
            {
                return;
            }

            items.ToList().ForEach(SelectTemplateDropDown.Items.Add);
            SelectTemplateDropDown.Attributes["onchange"] = string.Format(_changeString, DescriptionLabel.ClientID);
            SelectTemplateDropDown.SelectedIndex = 0;
            DescriptionLabel.Text = (string)templates.First()[_mainBodyPropertyName];
        }

        /// <summary>
        /// Lists all avaliable workroom templates from the workroom template container.
        /// Displayes a description for the selected workroom template.
        /// Disables the create workroom button if the user lacks create rights.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                WorkroomListPanel.Visible = true;
                NewWorkroomPanel.Visible = false;
            }

            if (CurrentPage.ACL.QueryDistinctAccess(AccessLevel.Publish))
            {
                ShowCreatePanelButton.Visible = true;
            }

            WorkroomPageList.DataBind();
        }

        /// <summary>
        /// Copies the selected workroom template to create an active workrooms. 
        /// Sets up the name of the new workroom.
        /// </summary>
        /// <param name="templateRef">A reference to the selected workroom template</param>
        /// <returns>The new workroom startpage.</returns>
        private PageData CopyAndSetUpWorkroomPage(PageReference templateRef)
        {
            //Copy page structure wihout access check of source pages since the user might not have read access to the template pages.
            PageReference newWorkroomRef = DataFactory.Instance.Copy(templateRef, CurrentPageLink, AccessLevel.NoAccess, AccessLevel.Publish, true, false);
            PageData newWorkroom = DataFactory.Instance.GetPage(newWorkroomRef).CreateWritableClone();

            newWorkroom.PageName = NewWorkroomName.Text.ToSafeString();
            newWorkroom[_mainBodyPropertyName] = NewWorkroomDescription.Text.ToSafeString();
            newWorkroom.URLSegment = String.Empty;

            newWorkroom.URLSegment = UrlSegment.CreateUrlSegment(newWorkroom);
            newWorkroom["WorkroomTemplate"] = templateRef;
            newWorkroom.VisibleInMenu = true;
            DataFactory.Instance.Save(newWorkroom, SaveAction.Publish);
            
            return newWorkroom;
        }

        /// <summary>
        /// Creates the page folder of the workroom start page, if it isn't already created.
        /// </summary>
        /// <param name="workroomPage">The workroom start page.</param>
        private static void CreateFolder(PageData workroomPage)
        {
            //We want to allow folder to be created without access checks performed therefore this 
            //construction. Otherwise it would be enough to call CurrentPage.GetPageDirectory(true)
            UnifiedDirectory pageDirectory = workroomPage.GetPageDirectory(false);
            if (pageDirectory == null)
            {
                UnifiedDirectory rootDirectory = VirtualPathHandler.Instance.GetDirectory(VirtualPathHandler.PageDirectoryRootVirtualPath, true) as UnifiedDirectory;
                rootDirectory.CreateSubdirectory(workroomPage.Property["PageFolderID"].ToString());
            }
        }

        /// <summary>
        /// Clears any access rights on the workroom and adds the current user with full access.
        /// </summary>
        /// <param name="workroomPage">The workroom start page.</param>
        private static void SetUpAccessRights(PageData workroomPage)
        {
            workroomPage.ACL.Clear();
            workroomPage.ACL.Add(new AccessControlEntry(HttpContext.Current.User.Identity.Name, AccessLevel.FullAccess, SecurityEntityType.User));
            workroomPage.ACL.Save(SecuritySaveType.RecursiveReplace);
        }

        /// <summary>
        /// Validates that there isn't another workroom with the same name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidateName(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !DataFactory.Instance.GetChildren(CurrentPageLink).Any(page => String.Equals(args.Value, page.PageName, StringComparison.OrdinalIgnoreCase));
        }
    }
}

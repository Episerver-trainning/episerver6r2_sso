#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Linq;
using System.Web;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using EPiServer.Templates.AlloyTech.Units.Static;
using EPiServer.Web.Hosting;
using EPiServer.Web.PageExtensions;
using EPiServer.Web.WebControls;

namespace EPiServer.Templates.Advanced.Workroom.Core
{
    /// <summary>
    /// Base class for pages with workroom functionality
    /// </summary>
    public class WorkroomPageBase : TemplatePage
    {
        private PageReference _workroomStartPageLink;
        private PageData _workroomStartPage;

        private string _fileFolderRoot;
        private bool? _editableContent;

        private const string _subMenuControlID = "MainRegion$SubMenu$Menu";
        private const string _workroomStartProperty = "WorkroomStart";
        private const string _breadcrumbsID = "MainRegion$BreadCrumbsRegion$Breadcrumbs";
        protected const string WorkroomListPageTypeName = "[AlloyTech] Workroom list";
        protected const string StartPageTypeName = "[AlloyTech] Workroom Start";
        protected const string NewsListPageTypeName = "[AlloyTech] Workroom news list";
        protected const string NewsItemPageTypeName = "[AlloyTech] News item";
        protected const string FileManagerPageTypeName = "[AlloyTech] Workroom File Manager";
        internal const string UserProfilePageTypeName = "[AlloyTech] User profile";
        protected const string CalendarPageTypeName = "[AlloyTech] Workroom Calendar";

        /// <summary>
        /// URL parameter in request for UserProfile
        /// </summary>
        private const string returnUrlUserProfile = "returnUrl";

        public const string TemplateContainer = "TemplateContainer";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkroomPageBase"/> class.
        /// </summary>
        protected WorkroomPageBase()
        {
            // Since the workroom has it's own UI to edit items from view mode, we don't want on page edit to be enabled.
            ContextMenu.Options &= ~ContextMenuOption.OnPageEdit;
        }

        /// <summary>
        /// Gets a value indicating if the ViewState has been restored.
        /// </summary>
        /// <value><c>true</c> if view state is loaded; otherwise, <c>false</c>.</value>
        public bool ViewStateLoaded { get; private set; }

        /// <summary>
        /// Restores view-state information from a previous page request that was saved by the <see cref="M:System.Web.UI.Control.SaveViewState"/> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object"/> that represents the control state to be restored.</param>
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            ViewStateLoaded = true;
        }

        /// <summary>
        /// Gets whether redirect to the user profile page enabled or not
        /// </summary>
        protected virtual bool RedirectEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializePage();
        }
        
        /// <summary>
        /// Redirects to UserProfile
        /// </summary>
        private static void RedirectToUserProfilePage(PageBase page)
        {
            EPiServerProfile user = EPiServerProfile.Current;
            if (user.IsAnonymous || !(string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName)))
            {
                return;
            }

            PageReference start = GetWorkroomStartPageLink(page);
            PageReference parent = DataFactory.Instance.GetPage(start).ParentLink;
            PageDataCollection children = DataFactory.Instance.GetChildren(parent);
            PageData data = children.FirstOrDefault(child => child.PageTypeName.Equals(UserProfilePageTypeName));
            if (data == null)
            {
                return;
            }
            string symbol = data.LinkURL.Contains("?") ? "&" : "?";
            string url = HttpUtility.UrlEncode(page.Request.RawUrl);
            string callUrl = string.Format("{0}{1}{2}={3}", data.LinkURL, symbol, returnUrlUserProfile, url);
            page.Response.Redirect(callUrl);
        }

        /// <summary>
        /// Gets or sets the work room file folder root.
        /// </summary>
        /// <value>Path to root folder for files.</value>
        public string FileFolderRoot
        {
            get
            {
                if (String.IsNullOrEmpty(_fileFolderRoot))
                {
                    _fileFolderRoot = VirtualPathUtility.AppendTrailingSlash(VirtualPathHandler.PageDirectoryRootVirtualPath);
                    _fileFolderRoot = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.Combine(_fileFolderRoot, WorkroomStartPage["PageFolderID"].ToString()));
                }
                return _fileFolderRoot;
            }
        }


        /// <summary>
        /// Gets or sets the writeable state of the CurrentPage property.
        /// If the to true the <see cref="EPiServer.Core.PageData"/> returned from <see cref="CurrentPage"/> will be writable.
        /// </summary>
        /// <remarks>
        /// The value is stored in ViewState and persisted between requests.
        /// Setting the value to false does <c>not</c> make the <see cref="EPiServer.Core.PageData"/> returned from <see cref="CurrentPage"/> read-only. 
        /// </remarks>
        public bool EditableContent
        {
            get
            {
                if (!_editableContent.HasValue && ViewStateLoaded)
                {
                    _editableContent = (bool)(ViewState["EditableContent"] ?? false);
                }
                return _editableContent.GetValueOrDefault(false);
            }
            set 
            { 
                ViewState["EditableContent"] = _editableContent = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="EPiServer.Core.PageData"/> for the current page.
        /// If <see cref="EditableContent"/> is <c>true</c> the <see cref="EPiServer.Core.PageData"/> returned will be writable.
        /// </summary>
        public override PageData CurrentPage
        {
            get
            {
                if (EditableContent && base.CurrentPage.IsReadOnly)
                {
                    base.CurrentPage = base.CurrentPage.CreateWritableClone();
                }
                return base.CurrentPage;
            }
            set
            {
                base.CurrentPage = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="EPiServer.Core.PageReference"/> for the start page of this workroom.
        /// </summary>
        /// <remarks>An exception is thrown if the WorkroomStart property is not defined.</remarks>
        public virtual PageReference WorkroomStartPageLink
        {
            get 
            {
                if (PageReference.IsNullOrEmpty(_workroomStartPageLink))
                {
                    _workroomStartPageLink = GetWorkroomStartPageLink(this);
                    if (PageReference.IsNullOrEmpty(_workroomStartPageLink))
                    {
                        throw new EPiServerException("The property \"WorkroomStart\" must be defined for workroom pages");
                    }
                }
                return _workroomStartPageLink;

            }
        }

        /// <summary>
        /// Gets the <see cref="EPiServer.Core.PageData"/> for the start page of this workroom.
        /// </summary>
        /// <remarks>An exception is thrown if the WorkroomStart property is not defined.</remarks>
        public virtual PageData WorkroomStartPage
        {
            get 
            {
                if (_workroomStartPage == null)
                {
                    _workroomStartPage = GetPage(WorkroomStartPageLink);
                }
                return _workroomStartPage;
            }
        }

        /// <summary>
        /// Gets the workrooms root page reference
        /// </summary>
        public PageReference WorkroomsRoot
        {
            get
            {
                if (WorkroomStartPage.PageTypeName==WorkroomListPageTypeName)
                    return WorkroomStartPageLink;
                return WorkroomStartPage.ParentLink;
            }
        }

        /// <summary>
        /// Gets reference to the workroom templates
        /// </summary>
        public PageReference WorkroomTemplates
        {
            get
            {
                PageData workroomsRootPage = GetPage(WorkroomsRoot);
                return workroomsRootPage[TemplateContainer] as PageReference;
            }
        }

        /// <summary>
        /// Create a string with an absolute link to the page. Checks if furl is enabled and rewrites the link if appropriate.
        /// </summary>
        /// <param name="page">A PageData to create an absolute link from.</param>
        /// <returns>An absolute link to page</returns>
        public static string CreateExternalLink(PageData page)
        {
            UrlBuilder url = new UrlBuilder(UriSupport.Combine(Settings.Instance.SiteUrl.ToString(), page.LinkURL));
            return url.ToString();
        }

        private static PageReference GetWorkroomStartPageLink(IPageSource page)
        {            
            if (page.CurrentPage.PageTypeName.Equals(WorkroomListPageTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return page.CurrentPage.PageLink;
            }
            else
            {
                return page.CurrentPage[_workroomStartProperty] as PageReference;
            }
        }

        /// <summary>
        /// Changes left menu and breadcrumbs if page is in workroom
        /// Redirects to profile page if needed
        /// <param name="page">Page for initialization</param>
        /// </summary>
        public static void InitializeWorkroomPage(PageBase page)
        {
            InitializeWorkroomPage(page, true);
        }

        /// <summary>
        /// Changes left menu and breadcrumbs if page is in workroom
        /// Redirects to profile page if needed
        /// </summary>
        /// <param name="page">page for initialization</param>
        /// <param name="enableRedirect">If redirect is enabled</param>
        private static void InitializeWorkroomPage(PageBase page, bool enableRedirect)
        {
            //Check if page is in workroom
            PageReference start = GetWorkroomStartPageLink(page);
            if (PageReference.IsNullOrEmpty(start))
            {
                return;
            }

            /// Redirect ot profile page
            if (enableRedirect)
            {
                RedirectToUserProfilePage(page);
            }

            ///Update left menu tree
            PageTree tree = page.Master.FindControl(_subMenuControlID) as PageTree;
            if (tree != null)
            {
                tree.PageLink = start;
                tree.NumberOfLevels = 3;
                tree.DataBind();
            }
        }

        /// <summary>
        /// Changes left menu and breadcrumbs if page is in workroom
        /// </summary>
        public void InitializePage()
        {
            InitializeWorkroomPage(this, RedirectEnabled);
        }

        #region Workroom security

        /// <summary>
        /// Gets a value indicating whether the current user has at least the supplied membership level for the Workroom.
        /// </summary>
        /// <param name="membershipLevel">The membership level to test.</param>
        /// <returns><c>true</c> if the user has sufficent membership level; otherwise <c>false</c>.</returns>
        public bool QueryDistinctMembershipLevel(MembershipLevels membershipLevel) 
        {
            return Membership.QueryDistinctMembershipLevel(CurrentPage, membershipLevel);
        }

        /// <summary>
        /// The <see cref="EPiServer.Templates.Advanced.Workroom.Core.MembershipLevel"/> required to access this page.
        /// </summary>
        public virtual MembershipLevels RequiredMembershipLevel() 
        {
            return MembershipLevels.Read;
        }

        /// <summary>
        /// The <see cref="EPiServer.Templates.Advanced.Workroom.Core.MembershipLevel"/> required to access this page.
        /// </summary>
        public override Security.AccessLevel RequiredAccess()
        {
            return Membership.ConvertAccessLevel(RequiredMembershipLevel());
        }

        #endregion

        #region Helper methods

 
        /// <summary>
        /// Deletes all page versions with state not equal to Published state
        /// </summary>
        /// <param name="pageRef"></param>
        protected static void DeleteNotPublishedPageVersions(PageReference pageRef)
        {
            var versions = DataFactory.Instance.ListVersions(pageRef)
                .Cast<PageVersion>().Where(pageVer => pageVer.Status != VersionStatus.Published);
            versions.ToList().ForEach(pageVer => DataFactory.Instance.DeleteVersion(pageVer.ID));
        }

        #endregion
    }
}

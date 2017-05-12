#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Filters;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Web;
using log4net;

namespace EPiServer.Templates.AlloyTech.Blog
{
    
    /// <summary>
    /// Updates the UsageStatistics property on the tag listing pages.
    /// Deletes any unused tag listing pages and any unused category.
    /// </summary>
    [ScheduledPlugIn(DisplayName = "Update tag statistics", LanguagePath = "/blog/tagstatisticsupdater")]
    public static class TagStatisticsUpdater
    {
        private static ILog _log = LogManager.GetLogger(typeof(TagStatisticsUpdater));

        /// <summary>
        /// Updates the UsageStatistics property on the tag listing pages.
        /// Cleans up unused categories.
        /// </summary>
        /// <returns></returns>
        public static string Execute()
        {
            Dictionary<string, int> globalTagStats = CreateTagStats();;
            Dictionary<string, int> teamTagStats;
            PageDataCollection teamStartPages = GetPages(PageReference.RootPage, PageType.Load(BlogUtility.TeamStartPageTypeName).ID);

            foreach (PageData teamStartPage in teamStartPages) 
            {
                teamTagStats = CreateTagStats();
                foreach (PageData startPage in DataFactory.Instance.GetChildren(teamStartPage.PageLink).Where(page => String.Equals(page.PageTypeName, BlogUtility.PersonalStartPageTypeName, StringComparison.OrdinalIgnoreCase)))
                {
                    CalculateCategoryStats(startPage, teamTagStats, globalTagStats);
                }
                //Add team level statistics
                SetStatsToPages(teamTagStats, teamStartPage, true);
            }

            //Special handling of personal blogs without a team blog as parent
            PageDataCollection personalStartPages = GetPages(PageReference.RootPage, PageType.Load(BlogUtility.PersonalStartPageTypeName).ID);

            var startPages = from page in personalStartPages
                                where
                                    !String.Equals(DataFactory.Instance.GetPage(page.ParentLink).PageTypeName, BlogUtility.TeamStartPageTypeName, StringComparison.OrdinalIgnoreCase)
                                select page;

            foreach (PageData startPage in startPages)
            {
                CalculateCategoryStats(startPage, CreateTagStats(), globalTagStats);
            }

            CleanUnusedCategories(globalTagStats);
            return "Job executed successfully";
        }

        /// <summary>
        /// Goes through all the tag statistics and deletes any categories that are not used by any blog.
        /// </summary>
        /// <param name="globalTagStats">A dictionary with statistics on tag usage across the whole site.</param>
        private static void CleanUnusedCategories(Dictionary<string, int> globalTagStats)
        {
            foreach (string key in globalTagStats.Keys)
            {
                if (globalTagStats[key] == 0)
                {
                    Category blogRoot = Category.Find(BlogUtility.RootCategoryName);
                    Category category = blogRoot.FindChild(key);
                    try
                    {
                        category.Delete();
                    }
                    catch (Exception)
                    {
                        _log.Error(String.Format(CultureInfo.InvariantCulture, "Unable to delete category \"{0}\".", key));
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Goes through Blog items and checks what categories are used adding them to a statistics dictionary.
        /// </summary>
        /// <param name="startPage">The start page used to search for blog item.</param>
        /// <param name="teamTagStats">The stats for team level.</param>
        /// <param name="globalTagStats">The stats for global level.</param>
        private static void CalculateCategoryStats(PageData startPage, Dictionary<string, int> teamTagStats, Dictionary<string, int> globalTagStats)
        {
            Dictionary<string, int> tagStats = CreateTagStats();
            PageDataCollection blogItemPages = GetPages(startPage.PageLink, PageType.Load(BlogUtility.ItemPageTypeName).ID);
            
            Category currentPageCategory;
            Category blogRoot = Category.Find(BlogUtility.RootCategoryName);

            foreach (PageData blogItem in blogItemPages)
            {
                foreach (int catID in blogItem.Category)
                {
                    currentPageCategory = Category.Find(catID);

                    if (currentPageCategory != null && currentPageCategory.Parent.ID == blogRoot.ID) 
                    {
                        //Update the stats on personl, team and global level.
                        tagStats[currentPageCategory.Name]++;
                        teamTagStats[currentPageCategory.Name]++;
                        globalTagStats[currentPageCategory.Name]++;
                    }
                }
            }
            SetStatsToPages(tagStats, startPage, false);
        }

        /// <summary>
        /// Set statistics to the tag pages. Adds a tag page if required.
        /// Also deletes any tag pages if the corresponding category has been deleted manually.
        /// </summary>
        /// <param name="tagStats">The usage statistics collection.</param>
        /// <param name="startPage">The start page for the blog.</param>
        /// <param name="isTeamLevel">If set to <c>true</c> we are on a team level blog.</param>
        private static void SetStatsToPages(Dictionary<string, int> tagStats, PageData startPage, bool isTeamLevel)
        {
            if (startPage[BlogUtility.TagContainerPropertyName] == null) return;

            PageData tagContainer = DataFactory.Instance.GetPage((PageReference)startPage[BlogUtility.TagContainerPropertyName]);
            PageDataCollection tagPages = DataFactory.Instance.GetChildren(tagContainer.PageLink);
            PageData tagPage;
            Dictionary<string, int> tagCloudValues = CreateTagCloudValues(tagStats);

            foreach (string key in tagStats.Keys)
            {
                if (FindPageByName(key, tagPages, out tagPage)) 
                {
                    if (tagStats[key] > 0)
                    {
                        tagPage = tagPage.CreateWritableClone();
                        tagPage[BlogUtility.UsageStatisticsPropertyName] = tagStats[key];
                        tagPage[BlogUtility.TagCloudValuePropertyName] = tagCloudValues[key];
                        DataFactory.Instance.Save(tagPage, SaveAction.Publish, AccessLevel.NoAccess);
                    }
                    else
                    {
                        DataFactory.Instance.Delete(tagPage.PageLink, true, AccessLevel.NoAccess);
                    }
                }
                else if (tagStats[key] > 0)
                {
                    //Create tag Page
                    tagPage = DataFactory.Instance.GetDefaultPageData(tagContainer.PageLink, BlogUtility.ListPageTypeName);
                    tagPage.PageName = key;
                    tagPage.URLSegment = UrlSegment.CreateUrlSegment(tagPage);
                    tagPage[BlogUtility.IsTagListingPropertyName] = true;
                    tagPage[BlogUtility.IsTeamLevelPropertyName] = isTeamLevel;
                    tagPage[BlogUtility.UsageStatisticsPropertyName] = tagStats[key];
                    tagPage[BlogUtility.TagCloudValuePropertyName] = tagCloudValues[key];
                    DataFactory.Instance.Save(tagPage, SaveAction.Publish, AccessLevel.NoAccess);
                }
            }

            //If a category has been deleted we should remove its corresponding tag page.
            foreach (PageData page in tagPages.Where(p => !tagStats.ContainsKey(p.PageName)))
            {
                DataFactory.Instance.Delete(page.PageLink, true, AccessLevel.NoAccess);
            }
        }

        /// <summary>
        /// Convertes the tag usage statistics into values suitable for the tag cloud.
        /// </summary>
        /// <param name="tagStats">A Dictionary with the tag usage statistics.</param>
        /// <returns>A Dictionary with values suitable for the tag cloud.</returns>
        private static Dictionary<string, int> CreateTagCloudValues(Dictionary<string, int> tagStats)
        {
            int tagSize;
            
            Dictionary<string, int> tagCloudValues = new Dictionary<string, int>();

            double max = tagStats.Values.Max();

            double tagBlockSize = max / 6;

            foreach (string key in tagStats.Keys)
            {
                tagSize = (int)(7 - (Math.Ceiling((double)tagStats[key] / tagBlockSize)));
                tagCloudValues.Add(key, tagSize);
            }
            
            return tagCloudValues;
        }

        /// <summary>
        /// Goes through the collection of pages and checks for a matching page by page name.
        /// </summary>
        /// <param name="name">The page name to search for.</param>
        /// <param name="pages">The PageDataCollection to search in.</param>
        /// <param name="pageToReturn">Any successful match.</param>
        /// <returns>True if page is found, otherwise false</returns>
        private static bool FindPageByName(string name, PageDataCollection pages, out PageData pageToReturn)
        {
            pageToReturn = pages.Where(p => p.PageName.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return pageToReturn != null;
        }
        /// <summary>
        /// Creates a dictionary of all available categories under the "Blogroot".
        /// </summary>
        /// <returns>A Dictionary containing the avaliable categories.</returns>
        private static Dictionary<string, int> CreateTagStats()
        {
            Dictionary<string, int> tagStats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Category blogRoot = Category.Find(BlogUtility.RootCategoryName);
            foreach (Category tag in blogRoot.Categories)
            {
                tagStats.Add(tag.Name, 0);
            }
            return tagStats;
        }

        /// <summary>
        /// Gets pages by a specific PageTypeId under a certain starting point.
        /// </summary>
        /// <param name="startPageReference">The page reference used as a starting point for the search.</param>
        /// <param name="pageTypeId">The page type id to search for.</param>
        /// <returns>A PageDataCollection containing all pages below the startPageReference of the page type pageTypeId.</returns>
        private static PageDataCollection GetPages(PageReference startPageReference, int pageTypeId)
        {
            PropertyCriteriaCollection pcc = new PropertyCriteriaCollection();
            pcc.Add(BlogUtility.CreateCriteria(CompareCondition.Equal, "PageTypeID", PropertyDataType.PageType, pageTypeId.ToString(CultureInfo.InvariantCulture), true));
            return DataFactory.Instance.FindPagesWithCriteria(startPageReference, pcc);
        }
    }
}

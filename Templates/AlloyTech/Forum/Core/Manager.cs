#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Text.RegularExpressions;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Filters;
using EPiServer.Security;

namespace EPiServer.Templates.AlloyTech.Forum.Core
{
    /// <summary>
    /// Help class to create/delete forum items and get information about the forum.
    /// </summary>
    public static class Manager
    {
        private static readonly TimeSpan _archiveTimeSpan = new TimeSpan(30, 0, 0, 0); //Thirty days

        private static PageData _startPage;

        /// <summary>
        /// Gets the start page.
        /// </summary>
        /// <value>The start page.</value>
        public static PageData StartPage
        {
            get
            {
                if (_startPage == null)
                {
                    _startPage = DataFactory.Instance.GetPage(PageReference.StartPage);
                }
                return _startPage;
            }
        }


        /// <summary>
        /// ForumPageTypeName
        /// </summary>
        public static string ForumPageTypeName
        {
            get
            {
                if (StartPage["ForumPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["ForumPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Forum";
                }
            }
        }

        /// <summary>
        /// ThreadPageTypeName
        /// </summary>
        public static string ThreadPageTypeName
        {
            get
            {
                if (StartPage["ThreadPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["ThreadPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Forum thread";
                }
            }
        }

        /// <summary>
        /// PostPageTypeName
        /// </summary>
        public static string PostPageTypeName
        {
            get
            {
                if (StartPage["PostPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["PostPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Forum post";
                }
            }
        }

        /// <summary>
        /// ThreadContainerPageTypeName
        /// </summary>
        public static string ThreadContainerPageTypeName
        {
            get
            {
                if (StartPage["ThreadContainerPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["ThreadContainerPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Forum thread container";
                }
            }
        }

        /// <summary>
        /// Gets the latest updated threads. Will search through all the forums beneath the supplied root.
        /// </summary>
        /// <param name="pageRef">The root page to look for threads under.</param>
        /// <param name="nrOfThreads">The number of threads to return.</param>
        /// <returns>A PageDataCollection containing threads.</returns>
        public static PageDataCollection GetLatestUpdatedThreads(PageReference pageRef, int nrOfThreads)
        {
            PropertyCriteriaCollection criterias = new PropertyCriteriaCollection();
            criterias.Add("PageTypeName", ThreadContainerPageTypeName, CompareCondition.Equal);
            PageDataCollection pages = DataFactory.Instance.FindPagesWithCriteria(pageRef, criterias);

            PageDataCollection threads = new PageDataCollection();
            FilterPublished publishedFilter = new FilterPublished();

            foreach (PageData threadContainer in pages)
            {
                foreach (PageData page in DataFactory.Instance.GetChildren(threadContainer.PageLink, LanguageSelector.AutoDetect(), 0, nrOfThreads))
                {
                    if (!publishedFilter.ShouldFilter(page))
                    {
                        threads.Add(page);
                    }
                }
            }

            new FilterPropertySort("PageChanged", FilterSortDirection.Descending).Filter(threads);
            new FilterCount(nrOfThreads).Filter(threads);

            return threads;
        }

        /// <summary>
        /// Gets the latest reply in a thread.
        /// </summary>
        /// <param name="threadRef">A reference to the thread.</param>
        /// <returns>The latest reply in the thread.</returns>
        public static PageData GetLatestReply(PageReference threadRef)
        {
            PageDataCollection replies = DataFactory.Instance.GetChildren(threadRef, LanguageSelector.AutoDetect(), 0, 1);
            return replies[0];
        }

        /// <summary>
        /// Creates a reply to a thread.
        /// </summary>
        /// <param name="threadReference">A reference to the thread.</param>
        /// <param name="forumStartPage">A reference to the forum the thread belongs to.</param>
        /// <param name="text">The content of the reply.</param>
        /// <returns>Returns true if the page has been moved from the archive to the active container; Otherwise false.</returns>
        public static bool CreateReply(PageReference threadReference, PageData forumStartPage, string text)
        {
            CreateReplyPage(threadReference, text);
            return UpdateThread(threadReference, forumStartPage);
        }

        /// <summary>
        /// Creates a thread in a forum.
        /// </summary>
        /// <param name="forumStartPage">A reference to the forum</param>
        /// <param name="heading">The heading for the thread.</param>
        /// <param name="text">The content of first entry of the thread.</param>
        /// <returns>The newly created thread page.</returns>
        public static PageData CreateThread(PageData forumStartPage, string heading, string text)
        {
            //Create thread page
            PageReference activeThreadContainerReference = (PageReference)forumStartPage["ActiveThreadContainer"];
            if (PageReference.IsNullOrEmpty(activeThreadContainerReference))
            {
                throw new EPiServerException("ActiveThreadContainer",
                                                "'Container for active threads' property is not defined on a forum start page.");
            }
            PageData newPage = DataFactory.Instance.GetDefaultPageData(activeThreadContainerReference, ThreadPageTypeName);
            newPage.PageName = heading.Trim();
            newPage.StartPublish = DateTime.Now.AddMinutes(-1);
            newPage.ArchiveLink = (PageReference)forumStartPage["ArchiveThreadContainer"];
            newPage.StopPublish = DateTime.Now.Add(_archiveTimeSpan);
            newPage["PageChildOrderRule"] = FilterSortOrder.CreatedDescending;
            PageReference pageRef = DataFactory.Instance.Save(newPage, SaveAction.Publish);

            //Create first reply
            CreateReplyPage(pageRef, text);

            return newPage;
        }

        /// <summary>
        /// Creates a new forum.
        /// </summary>
        /// <remarks>
        /// Sets up three subpages to the forum page. One container for active threads, one for sticky threads and one for archived threads.
        /// </remarks>
        /// <param name="parent">The parent to the forum.</param>
        /// <param name="forumName">Name of the forum.</param>
        /// <param name="allowThreads">If set to <c>true</c> the foum allows threads as sub pages.</param>
        /// <param name="allowForums">If set to <c>true</c> the foum allows other forums as sub pages.</param>
        /// <param name="icon">The icon to use for this forum.</param>
        /// <returns>A reference to the newly created forum.</returns>
        public static PageReference CreateForum(PageReference parent, string forumName, bool allowThreads, bool allowForums, string icon)
        {
            PageData newPage = DataFactory.Instance.GetDefaultPageData(parent, ForumPageTypeName);
            newPage.PageName = forumName; 
            newPage.StartPublish = DateTime.Now.AddMinutes(-1);
            newPage["AllowThreads"] = allowThreads;
            newPage["AllowForums"] = allowForums;
            newPage["Icon"] = icon;
            return SetupForumStructure(DataFactory.Instance.Save(newPage, SaveAction.Publish, AccessLevel.Administer));
        }


        /// <summary>
        /// Sets up the structure for a forum
        /// </summary>
        /// <remarks>
        /// Sets up three subpages to the forum page. One container for active threads, one for sticky threads and one for archived threads.
        /// </remarks>
        /// <param name="forumReference">The reference to the forum.</param>
        public static PageReference SetupForumStructure(PageReference forumReference)
        {
            PageData activeContainer = DataFactory.Instance.GetDefaultPageData(forumReference, ThreadContainerPageTypeName);
            PageData archiveContainer = DataFactory.Instance.GetDefaultPageData(forumReference, ThreadContainerPageTypeName);
            PageData stickyContainer = DataFactory.Instance.GetDefaultPageData(forumReference, ThreadContainerPageTypeName);

            PageData forum = DataFactory.Instance.GetPage(forumReference).CreateWritableClone();

            activeContainer.PageName = "Active";
            archiveContainer.PageName = "Archive";
            stickyContainer.PageName = "Sticky";
            activeContainer.VisibleInMenu = false;
            archiveContainer.VisibleInMenu = false;
            stickyContainer.VisibleInMenu = false;
            activeContainer["PageChildOrderRule"] = FilterSortOrder.ChangedDescending;
            archiveContainer["PageChildOrderRule"] = FilterSortOrder.ChangedDescending;
            stickyContainer["PageChildOrderRule"] = FilterSortOrder.ChangedDescending;

            forum["ActiveThreadContainer"] = DataFactory.Instance.Save(activeContainer, SaveAction.Publish, AccessLevel.Administer);
            forum["ArchiveThreadContainer"] = DataFactory.Instance.Save(archiveContainer, SaveAction.Publish, AccessLevel.Administer);
            forum["StickyThreadContainer"] = DataFactory.Instance.Save(stickyContainer, SaveAction.Publish, AccessLevel.Administer);
            DynamicProperty forumStart = DynamicProperty.Load(forum.PageLink, "ForumStart");
            forumStart.PropertyValue.Value = forum.PageLink;
            forumStart.Save();

            return DataFactory.Instance.Save(forum, SaveAction.Publish, AccessLevel.Administer);
        }


        /// <summary>
        /// Deletes a reply.
        /// </summary>
        /// <param name="replyReference">A reference to the reply.</param>
        /// <param name="threadReference">A reference to the page.</param>
        public static void DeleteReply(PageReference replyReference, PageReference threadReference)
        {
            PageData thread = DataFactory.Instance.GetPage(threadReference).CreateWritableClone();
            thread["NumberOfReplies"] = (int)thread["NumberOfReplies"] - 1;
            DataFactory.Instance.Save(thread, SaveAction.Publish);

            DeleteItem(replyReference);
        }

        /// <summary>
        /// Deletes a thread.
        /// </summary>
        /// <param name="threadReference">A reference to the thread.</param>
        public static void DeleteThread(PageReference threadReference)
        {
            DeleteItem(threadReference);
        }

        /// <summary>
        /// Deletes a forum.
        /// </summary>
        /// <param name="forumReference">A reference to the forum.</param>
        public static void DeleteForum(PageReference forumReference)
        {
            DeleteItem(forumReference);
        }

        public static string RemoveAllTags(string inputHtml)
        {
            Regex removeTagsRegex = new Regex(@"(?<tag></?\w+.*?>)");
            return removeTagsRegex.Replace(inputHtml, String.Empty);
        }

        /// <summary>
        /// Deletes a page.
        /// </summary>
        /// <param name="pageReference">A reference to the page.</param>
        private static void DeleteItem(PageReference pageReference)
        {
            DataFactory.Instance.Delete(pageReference, true);
        }

        /// <summary>
        /// Creates the actual page that contains the reply data.
        /// </summary>
        /// <param name="threadReference">A reference to the thread that this is a reply to.</param>
        /// <param name="text">The content of the reply.</param>
        private static void CreateReplyPage(PageReference threadReference, string text)
        {
            PageData newPage = DataFactory.Instance.GetDefaultPageData(threadReference, PostPageTypeName);
            newPage.PageName = DateTime.Now.AddMinutes(-1).ToString("yyyyMMdd'T'HHmmss");
            newPage["MainBody"] = text.Trim().ToSafeString();
            newPage.StartPublish = DateTime.Now.AddMinutes(-1);
            DataFactory.Instance.Save(newPage, EPiServer.DataAccess.SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);
        }

        /// <summary>
        /// Updates the changed date and number of replies on a thread when a reply is made.
        /// </summary>
        /// <param name="threadReference">A reference to the thread.</param>
        /// <param name="forumStartPage">A reference to the forum the thread belongs to.</param>
        /// <returns>Returns true if the page has been moved from the archive to the active container; Otherwise false.</returns>
        private static bool UpdateThread(PageReference threadReference, PageData forumStartPage)
        {
            PageData thread = DataFactory.Instance.GetPage(threadReference).CreateWritableClone();
            thread["PageChangedOnPublish"] = true;
            thread.StopPublish = DateTime.Now.Add(_archiveTimeSpan);
            if (thread["NumberOfReplies"] == null)
            {
                thread["NumberOfReplies"] = 1;
            }
            else
            {
                thread["NumberOfReplies"] = (int)thread["NumberOfReplies"] + 1;
            }

            DataFactory.Instance.Save(thread, SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);

            PageReference archiveThreadContainerReference = (PageReference)forumStartPage["ArchiveThreadContainer"];
            if (PageReference.IsNullOrEmpty(archiveThreadContainerReference))
            {
                throw new EPiServerException("ArchiveThreadContainer",
                    "'Container for archived threads' property is not defined on a forum start page.");
            }
            if (thread.ParentLink.CompareToIgnoreWorkID(archiveThreadContainerReference))
            {
                DataFactory.Instance.Move(thread.PageLink, (PageReference)forumStartPage["ActiveThreadContainer"]);
                return true;
            }
            return false;
        }
    }
}

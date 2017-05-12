#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using EPiServer.Filters;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace EPiServer.Templates.AlloyTech.Blog
{
    /// <summary>
    /// Utility methods and fields used by the blog templates.
    /// </summary>
    public static class BlogUtility
    {
        private static PageData _startPage;

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
        // Page types
        /// <summary>
        /// ItemPageTypeName
        /// </summary>
        public static string ItemPageTypeName
        {
            get
            {
                if (StartPage["ItemPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["ItemPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Blog item";
                }
            }
        }
   
        /// <summary>
        /// TeamStartPageTypeName
        /// </summary>
        public static string TeamStartPageTypeName
        {
            get
            {
                if (StartPage["TeamStartPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["TeamStartPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Blog team start";
                }
            }
        }
         /// <summary>
        /// PersonalStartPageTypeName
        /// </summary>
        public static string PersonalStartPageTypeName
        {
            get
            {
                if (StartPage["PersonalStartPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["PersonalStartPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Blog personal start";
                }
            }
        }
       
        /// <summary>
        /// ListPageTypeName
        /// </summary>
        public static string ListPageTypeName
        {
            get
            {
                if (StartPage["ListPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["ListPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Blog list";
                }
            }
        }
      
        /// <summary>
        /// CommentPageTypeName
        /// </summary>
        public static string CommentPageTypeName
        {
            get
            {
                if (StartPage["CommentPageTypeName"] != null)
                {
                    int pagetypeid = (int)StartPage["CommentPageTypeName"];
                    return PageType.Load(pagetypeid).Name;
                }
                else
                {
                    return "[AlloyTech] Blog comment";
                }
            }
        }
    
        // Page names
        /// <summary>
        /// TagContainerName
        /// </summary>
        public const string TagContainerName = "Tags";
        /// <summary>
        /// DateContainerName
        /// </summary>
        public const string DateContainerName = "Dates";


        // Property names
        /// <summary>
        /// "BlogStart" (PropertyPageReference)[Required] Used to define the start page for of the blog that the current item belongs to.
        /// Is a dynamic property
        /// </summary>
        public const string StartPropertyName = "BlogStart";
        
        /// <summary>
        /// "DateContainer" (PropertyPageReference)[Required] Points out the container page for the date listings for a blog.
        /// Is used on Team start and Personal start
        /// </summary>
        public const string DateContainerPropertyName = "DateContainer";
        
        /// <summary>
        /// "TagContainer" (PropertyPageReference)[Required] Points out the container page for the tag listing pages for a blog.
        /// Is used on Team start and Personal start
        /// </summary>
        public const string TagContainerPropertyName = "TagContainer";
        
        /// <summary>
        /// "IsTagListing" (PropertyBool)[Optional] Set to true if a listing page is used to list tags.
        /// Is used on Listing pages
        /// </summary>
        public const string IsTagListingPropertyName = "IsTagListing";
        
        /// <summary>
        /// "IsDateListing" (PropertyBool)[Optional] Set to true if a listing page is used to list dates.
        /// Is used on Listing pages
        /// </summary>
        public const string IsDateListingPropertyName = "IsDateListing";
        
        /// <summary>
        /// "IsTeamLevel" (PropertyBool)[Optional] Set to true if a listing page is used on team level.
        /// Is used on Listing pages
        /// </summary>
        public const string IsTeamLevelPropertyName = "IsTeamLevel";
        
        /// <summary>
        /// "DisableAnonymousComments" (PropertyBool)[Optional] Set to true if anonymous users should not be allowed to post comments.
        /// Is used on Personal start
        /// </summary>
        public const string DisableAnonymousCommentsPropertyName = "DisableAnonymousComments";
        
        /// <summary>
        /// "FeedCount" (PropertyNumber)[Optional] Defines how many items that should be listed in a RSS/Atom feed.
        /// Can be used on any page
        /// </summary>
        public const string FeedCountPropertyName = "FeedCount";

        /// <summary>
        /// "UsageStatistics" (PropertyNumber)[Required] Defines how many blog items has the tag that this page represents.
        /// Is used on Listing pages
        /// </summary>
        public const string UsageStatisticsPropertyName = "UsageStatistics";

        /// <summary>
        /// "TagCloudValue" (PropertyNumber)[Required] Defines what size that the tag in the tag cloud should be.
        /// Is used on Listing pages
        /// </summary>
        public const string TagCloudValuePropertyName = "TagCloudValue";

        /// <summary>
        /// "SummaryTextLength" (PropertyNumber)[Optional] Defines the length the text of ItemSummarey blo items.
        /// Is used on Listing, Personal start and Team start pages
        /// </summary>
        public const string SummaryTextLengthPropertyName = "SummaryTextLength";

        /// <summary>
        /// "MaxCount" (PropertyNumber)[Optional] Defines how many blog items that will be listed on the page. 
        /// Is used on Personal start and Team start pages
        /// </summary>
        public const string MaxCountPropertyName = "MaxCount";

        /// <summary>
        /// "HistoryLength" (PropertyNumber)[Optional] Defines the age of blog items that should be listed on a team start page.
        /// Is used on Team start
        /// </summary>
        public const string HistoryLengthPropertyName = "HistoryLength";

        /// <summary>
        /// Category RootCategoryName
        /// </summary>
        public const string RootCategoryName = "BlogRoot";

        /// <summary>
        /// Creates a PropertyCriteria.
        /// </summary>
        /// <param name="condition">The condition for the comparison.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="dataType">Type of property data.</param>
        /// <param name="value">The value to evalute the property against.</param>
        /// <param name="required">If set to <c>true</c> the criteria is required, which makes it an AND search.</param>
        /// <returns></returns>
        public static PropertyCriteria CreateCriteria(CompareCondition condition, string name, PropertyDataType dataType, string value, bool required)
        {
            PropertyCriteria criteria = new PropertyCriteria();
            criteria.Condition = condition;
            criteria.Name = name;
            criteria.Type = dataType;
            criteria.Value = value;
            criteria.Required = required;

            return criteria;
        }
    }
}

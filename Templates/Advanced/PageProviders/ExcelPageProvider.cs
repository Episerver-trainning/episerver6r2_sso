#region Copyright
// Copyright © 1996-2010 EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace EPiServer.Templates.Advanced.PageProviders
{
    /// <summary>
    /// Page provider that handles pages stores in an Excel file
    /// </summary>
    public class ExcelPageProvider : PageProviderBase
    {
        #region Fields and Properties

        private const string FilePathConfigParam = "filePath";
        private const string IdColumnConfigParam = "idColumn";
        private const string PageNameColumnConfigParam = "pageName";
        private const string PageTypeNameConfigParam = "pageTypeName";
        private const string SheetNameConfigParam = "sheetName";

        private const string PageNameProperty = "PageName";

        private string _excelFilePath;
        private string _idColumn;
        private string _pageNameColumn;
        private string _pageTypeName;
        private string _sheetName;

        private Guid _baseGuid = new Guid("{13BCDF2D-A903-4078-A50B-FBDC58018C65}");

        #endregion

        #region Overrides

        /// <summary>
        /// Initializes the provider from configuration settings
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="config">Config params</param>
        public override void Initialize(string name, NameValueCollection configParams)
        {
            base.Initialize(name, configParams);

            if (configParams[FilePathConfigParam] == null)
            {
                throw new ConfigurationErrorsException("ExcelPageProvider requires configuration attribute filePath to be set.");
            }

            if (configParams[IdColumnConfigParam] == null)
            {
                throw new ConfigurationErrorsException("ExcelPageProvider requires configuration attribute idColumn to be set.");
            }

            if (configParams[PageNameColumnConfigParam] == null)
            {
                throw new ConfigurationErrorsException("ExcelPageProvider requires configuration attribute pageName to be set.");
            }

            if (configParams[PageTypeNameConfigParam] == null)
            {
                throw new ConfigurationErrorsException("ExcelPageProvider requires configuration attribute pageTypeName to be set.");
            }

            if (configParams[SheetNameConfigParam] == null)
            {
                throw new ConfigurationErrorsException("ExcelPageProvider requires configuration attribute sheetName to be set.");
            }

            _excelFilePath = HttpContext.Current.Server.MapPath(configParams[FilePathConfigParam]);
            _idColumn = configParams[IdColumnConfigParam];
            _pageNameColumn = configParams[PageNameColumnConfigParam];
            _pageTypeName = configParams[PageTypeNameConfigParam];
            _sheetName = configParams[SheetNameConfigParam];
        }

        /// <summary>
        /// Reads data from the backing XML file and creates a PageData object from the data.
        /// </summary>
        /// <remarks>
        /// The language selector is used to see if a specific language is preferred, if language does not
        /// exist or a specific language was not given master language branch is returned.
        /// </remarks>
        /// <param name="pageLink">The page link.</param>
        /// <param name="selector">The selector which states the preffered language</param>
        /// <returns>A fully initialized PageData object</returns>
        protected override PageData GetLocalPage(PageReference pageLink, ILanguageSelector languageSelector)
        {
            List<string> pageLanguages = new List<string>();
            PageData pageData = new PageData();
            Dictionary<string, string> properties = new ExcelParser(_excelFilePath, _idColumn, _pageNameColumn, _sheetName).GetPageProperties(pageLink.ID);

            base.InitializePageData(pageData, properties[_pageNameColumn], _pageTypeName, GetGuidFromID(pageLink.ID), pageLink.CreateWritableClone(), EntryPoint, pageLanguages);

            foreach (KeyValuePair<string, string> keyValuePair in properties)
            {
                if (keyValuePair.Key != _idColumn && keyValuePair.Key != _pageNameColumn)
                {
                    pageData.SetValue(keyValuePair.Key, keyValuePair.Value);
                }
            }
            SetPageStatus(pageData, VersionStatus.Published);
            SetDynamicProperties(pageData);

            return pageData;
        }

        /// <summary>
        /// Resolves the PageReference based identifier for a page and the URI (in "classic" format) for a page given the guid based identifier.
        /// </summary>
        /// <remarks>The returned URI </remarks>
        /// <param name="pageLink">The page link.</param>
        /// <param name="pageGuid">The page GUID.</param>
        /// <returns></returns>
        protected override Uri ResolveLocalPage(Guid pageGuid, out PageReference pageLink)
        {
            if (!IsExcelPageProviderGuid(pageGuid))
            {
                pageLink = PageReference.EmptyReference;
                return null;
            }
            pageLink = base.ConstructPageReference(GetIDFromGuid(pageGuid));
            return base.ConstructPageUri(PageType.Load(_pageTypeName).ID, pageLink);
        }

        /// <summary>
        /// Resolves the Guid based identifier for a page and the URI (in "classic" format) for a page given the PageReference.
        /// </summary>
        /// <remarks>The returned URI </remarks>
        /// <param name="pageLink">The page link.</param>
        /// <param name="pageGuid">The page GUID.</param>
        /// <returns></returns>
        protected override Uri ResolveLocalPage(PageReference pageLink, out Guid guid)
        {
            if (pageLink.RemoteSite != this.Name)
            {
                guid = Guid.Empty;
                return null;
            }
            guid = GetGuidFromID(pageLink.ID);
            return base.ConstructPageUri(PageType.Load(_pageTypeName).ID, pageLink);
        }

        /// <summary>
        /// Returns references to all children of the specific page.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        /// <returns>Reference to all children</returns>
        /// <remarks>This is expected to return all children regardless of languages</remarks>
        protected override PageReferenceCollection GetChildrenReferences(PageReference pageLink, string languageID)
        {
            PageReferenceCollection result = new PageReferenceCollection();
            if (pageLink == EntryPoint)
            {
                ExcelParser parser = new ExcelParser(_excelFilePath, _idColumn, _pageNameColumn, _sheetName);
                parser.GetPageIDs().ForEach(item => result.Add(base.ConstructPageReference(item)));
            }
            return result;
        }

        /// <summary>
        /// Sets the cache settings.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        protected override void SetCacheSettings(PageData page, CacheSettings cacheSettings)
        {
            cacheSettings.FileNames.Add(_excelFilePath);
        }

        /// <summary>
        /// Sets the cache settings.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        /// <param name="childrenReferences">The children references.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        protected override void SetCacheSettings(PageReference pageLink, PageReferenceCollection childrenReferences, CacheSettings cacheSettings)
        {
            cacheSettings.FileNames.Add(_excelFilePath);
        }

        #endregion

        #region Help Methods

        private void SetDynamicProperties(PageData pageData)
        {            
            foreach (DynamicProperty property in DynamicProperty.ListForPage(pageData.ParentLink))
            {
                DynamicProperty curProperty = property;
                if (pageData.Property[curProperty.PropertyValue.Name] == null)
                {
                    if (curProperty.Status == DynamicPropertyStatus.Inherited)
                    {
                        curProperty = DynamicProperty.Load(curProperty.InheritedPageLink, curProperty.PropertyValue.Name);
                    }
                    pageData.Property.Add(curProperty.PropertyValue);
                }                
            }
        }

        private Guid GetGuidFromID(int id)
        {
            byte[] bg = _baseGuid.ToByteArray();
            Guid guid = new Guid(id, BitConverter.ToInt16(bg, 4), BitConverter.ToInt16(bg, 6), bg.Skip(8).Take(8).ToArray());
            return guid;
        }

        private int GetIDFromGuid(Guid guid)
        {
            return BitConverter.ToInt32(guid.ToByteArray(), 0);
        }

        private bool IsExcelPageProviderGuid(Guid guid)
        {
            byte[] excelPageProviderGuidBytes = _baseGuid.ToByteArray().Skip(4).Take(12).ToArray();
            byte[] guidToCheckBytes = guid.ToByteArray().Skip(4).Take(12).ToArray();
            return excelPageProviderGuidBytes == guidToCheckBytes;
        }

        #endregion
    }
}

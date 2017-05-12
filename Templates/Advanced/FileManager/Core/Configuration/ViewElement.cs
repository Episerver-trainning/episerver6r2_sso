#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using EPiServer.Configuration;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{

 
    /// <summary>
    /// Represents a single view definition for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    public class ViewElement : ConfigurationElementBase
    {
        private object _regionLock = new object();
        private object _frameworkSourceLock = new object();
        private string _fallbackViewName;

        private volatile string _frameworkSource = null;
        private volatile Dictionary<string, RegionDefinition> _regions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewElement"/> class.
        /// </summary>
        public ViewElement() : base("view") 
        { }

        /// <summary>
        /// Gets the name of the fallback view if one has been defined.
        /// </summary>
        public string FallbackViewName
        {
            get { return _fallbackViewName; }
            internal set { _fallbackViewName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the source of the framework definition control.
        /// </summary>
        [ConfigurationProperty("frameworkSource", IsRequired = false)]
        public String FrameworkSource
        {
            get 
            {
                // This is to avoid entering a lock after the parameter has been initialized.
                // Should be safe since the variable is only set from within locked regions
                // and set only after all initialization has been done.
                if (_frameworkSource != null)
                { 
                    return _frameworkSource;
                }

                lock (_frameworkSourceLock)
                {
                    // If system was waiting to enter this section because another thread was
                    // doing initialization, we first check if the initialization has been done
                    // to avoid unnecessary reloads.
                    if (String.IsNullOrEmpty(_frameworkSource))
                    {
                        string src = (String)this["frameworkSource"];
                        if (String.IsNullOrEmpty(src) && !String.IsNullOrEmpty(FallbackViewName) &&
                            !String.Equals(Name, FallbackViewName, StringComparison.OrdinalIgnoreCase))
                        {
                            src = GetFallbackView(FallbackViewName).FrameworkSource;
                        }
                        _frameworkSource = src;
                    }
                    return _frameworkSource;
                }
            }
        }

        /// <summary>
        /// Gets the regions registered with this view.
        /// </summary>
        [ConfigurationProperty("regions", IsDefaultCollection = true, IsRequired = true)]
        protected RegionElementCollection RegionElements
        {
            get { return (RegionElementCollection)base["regions"]; }
        }

        /// <summary>
        /// Gets the regions, including fallback regions, registered with this view.
        /// </summary>
        public IDictionary<string, RegionDefinition> Regions
        {
            get
            {
                // This is to avoid entering a lock after the collection has been initialized.
                // Should be safe since the variable is marked volatile and is only set from 
                // within locked regions only after all initialization has been done.
                if (_regions != null)
                {
                    return _regions;
                }

                // Only one thread may change the collection 
                lock (_regionLock)
                {
                    // If system was waiting to enter this section because another thread was
                    // doing initialization, we first check if the initialization has been done
                    // to avoid unnecessary reloads.
                    if (_regions == null)
                    {
                        Dictionary<string, RegionDefinition> tmpRegions = new Dictionary<string, RegionDefinition>();

                        // Add all explicit region declarations
                        foreach (RegionElement element in (RegionElementCollection)base["regions"])
                        {
                            tmpRegions.Add(element.Id, new RegionDefinition(element.Id, element.ContentSource));
                        }

                        // If there is a fallback view defined and this is not it.
                        if (!String.IsNullOrEmpty(FallbackViewName) && !String.Equals(Name, FallbackViewName, StringComparison.OrdinalIgnoreCase))
                        {
                            ViewElement fallbackView = GetFallbackView(FallbackViewName);

                            foreach (RegionDefinition fallbackRegion in fallbackView.Regions.Values.Where(region => !tmpRegions.ContainsKey(region.Id)))
                            {
                                tmpRegions.Add(fallbackRegion.Id, fallbackRegion);
                            }
                        }
                        _regions = tmpRegions;
                    }
                    return _regions;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="ViewElement"/> from configurqation and throws an <see cref="ConfigurationErrorsException"/> if no view with the requested name is found.
        /// </summary>
        /// <param name="viewName">Name of the view element to return.</param>
        /// <returns>A view element with the supplied name; an exception is thrown if no wiev is found.</returns>
        private static ViewElement GetFallbackView(string viewName) 
        {
            ViewElement fallbackView = FileManagerSection.Section.Views.GetView(viewName);

            if (fallbackView == null)
            {
                throw new ConfigurationErrorsException(String.Format(CultureInfo.InvariantCulture, "Can not find fallbackView named [{0}]", viewName));
            }

            return fallbackView;
        }
    }
}

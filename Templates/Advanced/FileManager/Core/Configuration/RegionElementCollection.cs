#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using EPiServer.Configuration;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{
    /// <summary>
    /// Represent a collection of view regions for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    [ConfigurationCollection(typeof(RegionElement), AddItemName = "region",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class RegionElementCollection : GenericConfigurationElementCollection<RegionElement>
    {
        /// <summary>
        /// Gets the name used to identify this collection of elements in the configuration file.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the collection.</returns>
        protected override string ElementName
        {
            get { return "region"; }
        }
    }
}

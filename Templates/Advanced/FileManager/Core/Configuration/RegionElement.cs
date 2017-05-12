#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using EPiServer.Configuration;
using System.Configuration;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{
    /// <summary>
    /// Represents a single region definition for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    public class RegionElement : ConfigurationElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionElement"/> class.
        /// </summary>
        public RegionElement()
            : base("region")
        {
        }

        /// <summary>
        /// Gets or sets the ID of a control implementing <see cref="EPiServer.Templates.Advanced.FileManager.Core.IFileManagerRegion"/>.
        /// </summary>
        /// <value>The ID string.</value>
        [ConfigurationProperty("id", IsRequired = true)]
        public String Id
        {
            get { return (String)this["id"]; }
            set { this["id"] = value; }
        }

        /// <summary>
        /// Gets or sets the location of the content control loaded into this content region with the ID matching <see cref="Id"/>.
        /// </summary>
        /// <value>The location of a web control.</value>
        [ConfigurationProperty("contentSource", IsRequired = true)]
        public String ContentSource
        {
            get { return (String)this["contentSource"]; }
            set { this["contentSource"] = value; }
        }
    }
}

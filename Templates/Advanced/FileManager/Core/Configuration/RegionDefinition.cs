#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{
    /// <summary>
    /// Represents a single region definition for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    [Serializable]
    public class RegionDefinition
    {
        private string _id;
        private string _contentSource;


        /// <summary>
        /// Initializes a new instance of the <see cref="RegionDefinition"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="contentSource">The content source.</param>
        public RegionDefinition(string id, string contentSource)
        {
            _id = id;
            _contentSource = contentSource;
        }

        /// <summary>
        /// Gets or sets the ID of a control implementing <see cref="EPiServer.Templates.Advanced.FileManager.Core.IFileManagerRegion"/>.
        /// </summary>
        /// <value>The ID string.</value>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets the location of the content control loaded into this content region with the ID matching <see cref="ID"/>.
        /// </summary>
        /// <value>The location of a web control.</value>
        public string ContentSource
        {
            get { return _contentSource; }
            set { _contentSource = value; }
        }
    }
}

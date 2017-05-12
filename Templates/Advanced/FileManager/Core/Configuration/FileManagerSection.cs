#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using System.Xml;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{
    /// <summary>
    /// Represents the configuration used for loading user controls when instatiating the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>.
    /// </summary>
    public class FileManagerSection : ConfigurationSection
    {
        //It is safe to keep the section cached since a change to config file reloads application.
        private static FileManagerSection _section;

        /// <summary>
        /// Gets the currently active configuration for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>.
        /// </summary>
        /// <returns>The currently active configuration for the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>.</returns>
        public static FileManagerSection Section
        {
            get
            {
                if (_section == null)
                {
                    FileManagerSection section = (FileManagerSection)WebConfigurationManager.GetSection("episerverModules/episerver.FileManager");
                    if (section == null ||section.Views.Count == 0)
                    {
                        //No custom views defined, load default configuration
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EPiServer.Templates.Advanced.FileManager.views.config"))
                        {
                            XmlTextReader xmlReader = new XmlTextReader(stream);
                            section = new FileManagerSection();
                            section.DeserializeElement(xmlReader, false);
                        }
                    }
                    _section = section;
                }

                return _section;
            }
        }


        /// <summary>
        /// Gets a collection of the configured view definitions for the file manager FileManager.
        /// </summary>
        /// <value>A collection of view configurations.</value>
        [ConfigurationProperty("views", IsDefaultCollection = true, IsRequired = true)]
        public ViewElementCollection Views
        {
            get { return (ViewElementCollection)base["views"]; }
        }
    }
}

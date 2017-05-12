#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using EPiServer.Configuration;

namespace EPiServer.Templates.Advanced.FileManager.Core.Configuration
{
    /// <summary>
    /// Represent a collection of views loaded by the <see cref="EPiServer.Templates.Advanced.FileManager.Core.WebControls.FileManagerControl"/>
    /// </summary>
    [ConfigurationCollection(typeof(ViewElement), AddItemName = "view",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ViewElementCollection : GenericConfigurationElementCollection<ViewElement>
    {
        /// <summary>
        /// Gets the name used to identify this collection of elements in the configuration file.
        /// </summary>
        /// <returns>The name of the collection.</returns>
        protected override string ElementName
        {
            get { return "view"; }
        }

        /// <summary>
        /// Gets or sets the name of the fallback view.
        /// </summary>
        /// <value>The name of the fallback view.</value>
        [ConfigurationProperty("fallbackView", IsRequired = true)]
        public String FallbackViewName
        {
            get { return (String)this["fallbackView"]; }
            set { this["fallbackView"] = value; }
        }

        /// <summary>
        /// Finds a view configuration by its name parameter.
        /// </summary>
        /// <param name="name">The name of the view to find.</param>
        /// <returns>A view configuration; or null if no view existst with the reauested name.</returns>
        public ViewElement GetView(string name)
        {
            return this.Cast<ViewElement>().SingleOrDefault(element => String.Equals(element.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a configuration element to the configuration element collection.
        /// </summary>
        /// <param name="index">The index location at which to add the specified <see cref="T:System.Configuration.ConfigurationElement"/>.</param>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to add.</param>
        protected override void BaseAdd(int index, ConfigurationElement element)
        {
            InitializeChildElement(element);
            base.BaseAdd(index, element);
        }

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to add.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            InitializeChildElement(element);
            base.BaseAdd(element);
        }

        /// <summary>
        /// Initializes the child element with the configured fallback view.
        /// </summary>
        /// <param name="element">The configuration element to initialize.</param>
        private void InitializeChildElement(ConfigurationElement element)
        {
            ViewElement viewElement = element as ViewElement;
            if (viewElement != null) 
            {
                viewElement.FallbackViewName = FallbackViewName;
            }
        }
    }
}

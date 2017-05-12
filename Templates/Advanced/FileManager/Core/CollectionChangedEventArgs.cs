#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// Enum that defines a list of reasons to why the collection has been changed.
    /// </summary>
    public enum CollectionChangeType
    {
        /// <summary>
        /// The item collection has been cleared.
        /// </summary>
        ItemsCleared,
        /// <summary>
        /// A new item has been inserted or added.
        /// </summary>
        ItemInserted,
        /// <summary>
        /// An item has been removed.
        /// </summary>
        ItemRemoved,
        /// <summary>
        /// An item in the collection has been replaced.
        /// </summary>
        ItemSet
    }

    /// <summary>
    /// Event argument class for <see cref="EventDrivenCollection{T}"/> that specifies why the event has been triggered.
    /// </summary>
    public class CollectionChangedEventArgs : EventArgs
    {
        private CollectionChangeType _changeType;

        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public CollectionChangeType ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        public CollectionChangedEventArgs(CollectionChangeType changeType) : base()
        {
            _changeType = changeType;
        }
    }
}
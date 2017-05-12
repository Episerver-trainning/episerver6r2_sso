#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// A generic collection that triggers a <see cref="CollectionChanged"/> event when the collection has changed. 
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> of the items.</typeparam>
    public class EventDrivenCollection<T> : Collection<T>
    {
        private event EventHandler<CollectionChangedEventArgs> _collectionChanged;

        /// <summary>
        /// An event that is triggered when the internal collection is changed.
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs> CollectionChanged
        {
            add
            {
                _collectionChanged += value;
            }
            remove
            {
                _collectionChanged -= value;
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(CollectionChangeType.ItemsCleared);
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(CollectionChangeType.ItemInserted);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnCollectionChanged(CollectionChangeType.ItemRemoved);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(CollectionChangeType.ItemSet);
        }

        /// <summary>
        /// Called when the collection has been changed.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <remarks>Triggers the <see cref="CollectionChanged"/> event.</remarks>
        private void OnCollectionChanged(CollectionChangeType changeType)
        {
            OnCollectionChanged(this, new CollectionChangedEventArgs(changeType));
        }

        private void OnCollectionChanged(object source, CollectionChangedEventArgs e)
        {
            if (_collectionChanged != null)
            {
                _collectionChanged(source, e);
            }
        }
    }
}
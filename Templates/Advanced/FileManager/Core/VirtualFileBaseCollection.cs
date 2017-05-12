#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;
using System.Runtime.Serialization;
using EPiServer.Web.Hosting;
using System.Globalization;
using System.Security.Permissions;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// A collection class that stores files and folders and that is Serializable.
    /// </summary>
    [Serializable]
    public class VirtualFileBaseCollection : EventDrivenCollection<VirtualFileBase>, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualFileBaseCollection"/> class.
        /// </summary>
        public VirtualFileBaseCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualFileBaseCollection"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected VirtualFileBaseCollection(SerializationInfo info, StreamingContext context)
            : base()
        {
            int count = info.GetInt32("count");
            string path;

            for (int i = 0; i < count; i++ )
            {
                path = info.GetString("item" + i.ToString(CultureInfo.InvariantCulture));
                VirtualFileBase file;
                // TODO: Check that path only can be separated with /
                if (path.EndsWith("/", StringComparison.Ordinal))
                {
                    file = HostingEnvironment.VirtualPathProvider.GetDirectory(path) as VirtualFileBase;
                }
                else
                {
                    file = HostingEnvironment.VirtualPathProvider.GetFile(path) as VirtualFileBase;
                    if (file == null)
                    {
                        file = HostingEnvironment.VirtualPathProvider.GetDirectory(path) as VirtualFileBase;
                    }
                }
                // The file may have been removed or renamed
                if (file != null)
                {
                    Add(file);
                }
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("count", Count);
            int i = 0;

            foreach (VirtualFileBase fileBase in this)
            {
                info.AddValue("item" + (i++).ToString(CultureInfo.InvariantCulture), fileBase.VirtualPath);
            }            
        }

        /// <summary>
        /// Determines whether the collection contains an item with the specified virtual path.
        /// </summary>
        /// <param name="virtualFileBase">A reference to a virtual file.</param>
        /// <returns>
        /// 	<c>true</c> if the collection contains an item with the specified virtual path; otherwise, <c>false</c>.
        /// </returns>
        public new bool Contains(VirtualFileBase virtualFileBase)
        {
            return IndexOf(virtualFileBase.VirtualPath) >= 0;
        }

        /// <summary>
        /// Determines whether the collection contains an item with the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>
        /// 	<c>true</c> if the collection contains an item with the specified virtual path; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string virtualPath)
        {
            return IndexOf(virtualPath) >= 0;
        }


        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <param name="virtualFileBase">The <see cref="VirtualFileBase"/> that you want the index for.</param>
        /// <returns>The index of the item.</returns>
        /// <remarks>If an item with the same path does not exist in the collection -1 will be returned.</remarks>
        public new int IndexOf(VirtualFileBase virtualFileBase)
        {
            return IndexOf(virtualFileBase.VirtualPath);
        }

        /// <summary>
        /// Gets the index of the item with the given virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>The index of the item with the given virtual path.</returns>
        /// <remarks>If the path does not exist in the collection -1 will be returned.</remarks>
        public int IndexOf(string virtualPath)
        {
            int i = 0;
            foreach (VirtualFileBase fileBase in this)
            {
                if (fileBase.VirtualPath == virtualPath)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Copies the items in the collection to a target collection.
        /// </summary>
        /// <param name="targetCollection">The target collection.</param>
        public void CopyTo(VirtualFileBaseCollection targetCollection)
        {
            foreach (VirtualFileBase fileItem in this)
            {
                targetCollection.Add(fileItem);
            }
        }
    }
}
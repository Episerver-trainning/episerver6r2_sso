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
using System.Runtime.Serialization;
using System.Globalization;
using System.Security.Permissions;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// File is not found exception
    /// </summary>
    [Serializable]
    public class FileIsCheckedOutException : Exception
    {
        private readonly string _checkedOutBy;

        /// <summary>
        /// Gets the user checked out by.
        /// </summary>
        /// <value>The user name.</value>
        public string CheckedOutBy
        {
            get
            {
                return _checkedOutBy;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIsCheckedOutException"/> class.
        /// </summary>
        public FileIsCheckedOutException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIsCheckedOutException"/> class.
        /// </summary>
        /// <param name="checkedOutBy">The checked out by.</param>
        public FileIsCheckedOutException(string checkedOutBy)
            : base()
        {
            _checkedOutBy = checkedOutBy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIsCheckedOutException"/> class.
        /// </summary>
        /// <param name="checkedOutBy">The checked out by.</param>
        /// <param name="message">The message.</param>
        public FileIsCheckedOutException(string checkedOutBy, string message)
            : base(message)
        {
            _checkedOutBy = checkedOutBy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIsCheckedOutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public FileIsCheckedOutException(string message, Exception exception)
            : base(message, exception)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIsCheckedOutException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected FileIsCheckedOutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CheckedOutBy", _checkedOutBy);
            base.GetObjectData(info, context);
        }
    }

    /// <summary>
    /// Required version is not found exception
    /// </summary>
    [Serializable]
    public class VersionNotFoundException : Exception
    {
        private readonly string _versionId;

        /// <summary>
        /// Gets the version ID.
        /// </summary>
        /// <value>The version ID.</value>
        public string VersionId
        {
            get
            {
                return _versionId;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionIsNotFoundException"/> class.
        /// </summary>
        public VersionNotFoundException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionIsNotFoundException"/> class.
        /// </summary>
        /// <param name="versionId">The version ID.</param>
        public VersionNotFoundException(string versionId)
            : base(GetErrorMessage(versionId))
        {
            _versionId = versionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionIsNotFoundException"/> class.
        /// </summary>
        /// <param name="versionId">The version ID.</param>
        /// <param name="exception">The exception.</param>
        public VersionNotFoundException(string versionId, Exception exception)
            : base(GetErrorMessage(versionId), exception)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionIsNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected VersionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        private static string GetErrorMessage(string versionId)
        {
            return String.Format(CultureInfo.InvariantCulture, "Required version '{0}' could not be found", versionId);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("VersionId", _versionId);
            base.GetObjectData(info, context);
        }
    }

    /// <summary>
    /// Unable to perform operation with versions
    /// </summary>
    [Serializable]
    public class InvalidVersioningOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVersioningOperationException"/> class.
        /// </summary>
        public InvalidVersioningOperationException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVersioningOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidVersioningOperationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVersioningOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public InvalidVersioningOperationException(string message, Exception exception)
            : base(message, exception)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVersioningOperationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected InvalidVersioningOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

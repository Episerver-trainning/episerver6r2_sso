#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using EPiServer.Templates.Advanced.FileManager.Core.WebControls;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    interface IFileManagerRegion
    {
        string ContentSource { get; }
        WebControls.FileManagerControl FileManager { get; }

        void Initialize(WebControls.FileManagerControl fileManager, string contentSource);
    }
}

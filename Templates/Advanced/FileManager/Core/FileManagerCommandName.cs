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
    /// Built-in File Manager commands which will be handled by the <see cref="WebControls.FileManagerControl"/>.
    /// </summary>
    public struct FileManagerCommandName
    {
        /// <summary>
        /// Loads a new view declared in the config section <c>episerverModules/episerver.FileManager</c> of <c>web.config</c>
        /// </summary>
        public const string LoadView = "LoadView";

        /// <summary>
        /// Changes the currently selected folder.
        /// </summary>
        public const string SelectFolder = "SelectFolder";

        /// <summary>
        /// Copies the selected items to the Paste puffer in preparation for a copy/paste operation
        /// </summary>
        public const string CopySelection = "CopySelection";

        /// <summary>
        /// Copies the selected items to the Paste buffer in preparation for a move/paste operation.
        /// </summary>
        public const string CutSelection = "CutSelection";

        /// <summary>
        /// Moves or copies the contents of the Paste buffer to the currently selected directory.
        /// </summary>
        public const string PasteItems = "PasteItems";

        /// <summary>
        /// Deletes the items currently in the <see cref="WebControls.FileManagerControl.SelectedItems"/>.
        /// </summary>
        public const string DeleteSelection = "DeleteSelection";

        /// <summary>
        /// Deletes a special version for the selected file in the <see cref="WebControls.FileManagerControl.SelectedItems"/>.
        /// </summary>
        public const string DeleteVersion = "DeleteVersion";

        /// <summary>
        /// Restores a specail version for the selected file in the <see cref="WebControls.FileManagerControl.SelectedItems"/>.
        /// </summary>
        public const string RestoreVersion = "RestoreVersion";

        /// <summary>
        /// Denotes that the child collection of a folder has been changed.
        /// </summary>
        public const string ChildCollectionChanged = "ChildCollectionChanged";

        /// <summary>
        /// ChecksOut the items currently in the <see cref="WebControls.FileManagerControl.SelectedItems"/>.
        /// </summary>
        public const string CheckoutSelection = "CheckOutSelection";

        /// <summary>
        /// Undo CheckOut the items currently in the <see cref="WebControls.FileManagerControl.SelectedItems"/>.
        /// </summary>
        public const string UndoCheckoutSelection = "UndoCheckOutSelection";
    }
}

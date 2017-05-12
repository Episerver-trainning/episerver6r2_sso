#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using EPiServer.Web.Hosting;
using System.Web.Hosting;
using System.Web;
using System.IO;
using EPiServer.BaseLibrary.IO;
using EPiServer.Security;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;

namespace EPiServer.Templates.Advanced.FileManager.Core
{
    /// <summary>
    /// Indicates the action for a paste operation
    /// </summary>
    public enum PasteMode
    {
        /// <summary>
        /// Used to copy the source files whena paste operation occurs 
        /// </summary>
        Copy,
        /// <summary>
        /// Used to move the source files when a paste operation occurs.
        /// </summary>
        Cut
    }

    /// <summary>
    /// test
    /// </summary>
    public static class FileSystemUtility
    {
        /// <summary>
        /// Name of temp file
        /// </summary>
        public const string TempFileName = "~$EPI";

        /// <summary>
        /// Extension of temp file
        /// </summary>
        public const string TempFileExtension = ".tmp";

        // TODO: For nice flexibility this should be a wrapper with plug-in provider support.
        // Since the base provider API doesn't support any modifications at all, we need custom implementations for every modification.
        // The UnifiedFile and UnifiedDirectory provides abstract API for modifications, but it's likely that the individual inheritors 
        // are incompatible with each other. Meaning that moving a file between different providers are likely to fail.

         

        /// <summary>
        /// Pastes the files in the source collection to the supplied target directory.
        /// </summary>
        /// <param name="sourceItems">A collection of source items to paste.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        /// <param name="pasteMode">
        /// Determines how the source files are treated when the paste operation is performed. 
        /// <list type="table">
        ///     <listheader>
        ///         <term>pasteMode</term><description>Result</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PasteMode.Copy"/></term>
        ///         <description>The source items are copied to the new location.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PasteMode.Cut"/></term> 
        ///         <description>The source items are moved from their original location to the new location.</description>
        ///     </item>
        /// </list>
        /// </param>
        public static void PasteFiles(VirtualFileBaseCollection sourceItems, UnifiedDirectory targetDirectory, PasteMode pasteMode)
        {
            // Have to assume unified file
            // The Versioning VPP has copy/move support built in, but only for operations inside the Versioning file systems.
            foreach (VirtualFileBase sourceItem in sourceItems)
            {
                try
                {
                    if (pasteMode == PasteMode.Copy)
                    {
                        CopyItem(sourceItem, targetDirectory);
                    }
                    else if (pasteMode == PasteMode.Cut)
                    {
                        MoveItem(sourceItem, targetDirectory);
                    }
                }
                catch (FileIsCheckedOutException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Saves the source stream to a unified file and adds a comment if the file implrments IVersioningFile.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="comment">A comment to add when checking in a versioning file.</param>
        public static void WriteToFile(Stream sourceStream, UnifiedFile targetFile, string comment)
        {
            // It may be a versioning file
            IVersioningFile versioningFile = targetFile as IVersioningFile;
            if (versioningFile != null)
            {
                ThrowIfCheckedOut(versioningFile);
                if (!versioningFile.IsCheckedOut)
                {
                    versioningFile.CheckOut();
                }                
            }

            // Copy the source stream to the target file stream.
            using (Stream writeStream = targetFile.Open(FileMode.Create, FileAccess.Write))
            {
                StreamConsumer.CopyToEnd(sourceStream, writeStream);
            }

            // If versioning, then check in with the supplied comment.
            if (versioningFile != null)
            {
                versioningFile.CheckIn(comment ?? String.Empty);
            }
        }

        /// <summary>
        /// Writes to file the content from other file.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="targetFile">The target file.</param>
        /// <param name="comment">The comment.</param>
        public static void WriteToFile(UnifiedFile sourceFile, UnifiedFile targetFile, string comment)
        {
            using (Stream stream = sourceFile.Open())
            {
                WriteToFile(stream, targetFile, comment);
            }
        }

        /// <summary>
        /// Updates the file summary.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="updatedData">The updated data.</param>
        public static void UpdateFileSummary(UnifiedFile file, NameValueCollection updatedData)
        {
            IVersioningFile versioningFile = file as IVersioningFile;
            if (versioningFile != null)
            {
                ThrowIfCheckedOut(versioningFile);
            }
            if (updatedData != null)
            {
                IDictionary dict = file.Summary.Dictionary;
                foreach (string key in updatedData)
                {
                    dict[key] = updatedData[key];
                }
            }
            file.Summary.SaveChanges();
        }

        #region Create/Delete Functionality


        /// <summary>
        /// Creates a file using the contents of the supplied stream.
        /// </summary>
        /// <param name="directory">The directory in which to create the file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="checkInComment">An optional check in comment.</param>
        /// <param name="fileContent">Contents of the file.</param>
        /// <returns>The newly created file.</returns>
        public static UnifiedFile CreateFile(UnifiedDirectory directory, string fileName, string checkInComment, Stream fileContent)
        {
            UnifiedFile file = directory.CreateFile(fileName);
            WriteToFile(fileContent, file, checkInComment);
            return file;
        }

        /// <summary>
        /// Gets a named file from the supplied directory.
        /// </summary>
        /// <param name="directory">The directory in which to look for the file.</param>
        /// <param name="fileName">Name of the file to retreive.</param>
        /// <returns>A reference to a file; or null if no file existst with the requested name.</returns>
        public static VirtualFile GetFile(VirtualDirectory directory, string fileName)
        {
            string virtualFilePath = VirtualPathUtility.Combine(directory.VirtualPath, fileName);
            return HostingEnvironment.VirtualPathProvider.GetFile(virtualFilePath);
        }

        /// <summary>
        /// Creates a new directory in the supplied parent directory.
        /// </summary>
        /// <param name="parentDirectory">The parent directory in which to create a sub directory.</param>
        /// <param name="directoryName">Name of the new directory.</param>
        public static void CreateDirectory(UnifiedDirectory parentDirectory, string directoryName)
        {
            parentDirectory.CreateSubdirectory(directoryName);
        }

        /// <summary>
        /// Deletes the items supplied in the <see cref="VirtualFileBaseCollection"/>.
        /// </summary>
        /// <param name="items">The items to delete.</param>
        public static void DeleteItems(VirtualFileBaseCollection items)
        {
            //Can only delete items inheriting from Unified[Directory/File]
            foreach (VirtualFileBase fileItem in items)
            {
                if (CanEdit(fileItem))
                {
                    UnifiedDirectory directory = fileItem as UnifiedDirectory;
                    if (directory != null)
                    {
                        directory.Delete();
                        continue;
                    }
                    UnifiedFile file = fileItem as UnifiedFile;
                    if (file != null)
                    {
                        file.Delete();
                        continue;
                    }
                }
                else
                {
                    //TODO: What do we do with delete operation on unknown items.
                    // Throw exception? Silently ignore? Logging!
                }
            }
        }

        /// <summary>
        /// Restore a specific file version as the current version.
        /// </summary>
        /// <param name="versioningFileItem">A versioning file instance containing the version to restore.</param>
        /// <param name="versionId">Version id of the version to restore.</param>
        public static void RestoreVersion(IVersioningFile versioningFileItem, string versionId)
        {
            Validate.RequiredParameter("versioningFileItem", versioningFileItem);
            if (String.IsNullOrEmpty(versionId))
            {
                throw new ArgumentException("versionId can not be null or empty", "versionId");
            }

            ThrowIfCheckedOut(versioningFileItem);
            IList<UnifiedVersion> versions = versioningFileItem.GetVersions();
            if (versions.Count == 1)
            {
                throw new InvalidVersioningOperationException("Can not restore version when only one version exists.");
            }            
            UnifiedVersion targetVersion = versions.SingleOrDefault<UnifiedVersion>(v => v.Id.ToString() == versionId);
            if (targetVersion == null)
            {
                throw new VersionNotFoundException(versionId);
            }
            var query = from version in versions orderby version.Changed descending select version;
            UnifiedVersion lastVersion = query.First<UnifiedVersion>();
            if (lastVersion.Id.ToString() == targetVersion.Id.ToString())
            {
                throw new InvalidVersioningOperationException("The version to be restored is already the currently active version.");
            }
            targetVersion.Restore();
        }

        /// <summary>
        /// Deletes a specific file version.
        /// </summary>
        /// <param name="versioningFileItem">File version.</param>
        /// <param name="versionId">File version Id.</param>
        public static void DeleteVersion(IVersioningFile versioningFileItem, string versionId)
        {
            Validate.RequiredParameter("versioningFileItem", versioningFileItem);
            if (String.IsNullOrEmpty(versionId))
            {
                throw new ArgumentException("versionId can not be null or empty", "versionId");
            }

            ThrowIfCheckedOut(versioningFileItem);
            IList<UnifiedVersion> versions = versioningFileItem.GetVersions();
            if (versions.Count == 1)
            {
                throw new InvalidVersioningOperationException();
            }
            UnifiedVersion targetVersion = versions.SingleOrDefault<UnifiedVersion>(v => v.Id.ToString() == versionId);
            if (targetVersion == null)
            {
                throw new VersionNotFoundException(versionId);
            }
            targetVersion.Delete();
        }

        #endregion

        #region Check Out/Check In

        /// <summary>
        /// Determines whether the file is checked out to another user than the one currently logged on.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the versioing file is checked out to another user; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCheckedOutBySomeoneElse(IVersioningFile file) 
        {
            return file.IsCheckedOut && !IsCheckedOutByCurrentUser(file);
        }

        /// <summary>
        /// Determines if a file is checked out by the user currently logged on.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if file is checked out by the active user; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCheckedOutByCurrentUser(IVersioningFile file)
        {
            if (!file.IsCheckedOut || String.IsNullOrEmpty(file.CheckedOutBy))
            {
                return false;
            }
            string checkedOutByName = file.CheckedOutBy;
            int slashCheckedOutByName = checkedOutByName.IndexOf("\\", StringComparison.Ordinal);
            string currentUserName = PrincipalInfo.Current.Name;
            int slashCurrentUserName = currentUserName.IndexOf("\\", StringComparison.Ordinal);
            if (slashCheckedOutByName > 0 && slashCurrentUserName == -1)
            {
                checkedOutByName = checkedOutByName.Substring(slashCheckedOutByName + 1);
            }
            if (slashCheckedOutByName == -1 && slashCurrentUserName > 0)
            {
                currentUserName = currentUserName.Substring(slashCurrentUserName + 1);
            }
            return String.Equals(checkedOutByName, currentUserName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the currently logged on user has edit permissions on the specific item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if the currently logged on user has edit permissions on the specific item; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanEdit(VirtualFileBase item)
        {
            if (item == null)
            {
                return false;
            }

            UnifiedDirectory dir = item as UnifiedDirectory;
            if (dir != null)
            {
                return PrincipalInfo.HasAdminAccess || dir.QueryDistinctAccess(AccessLevel.Edit);
            }
            
            UnifiedFile file = item as UnifiedFile;
            if (file != null && (PrincipalInfo.HasAdminAccess || file.QueryDistinctAccess(AccessLevel.Edit)))
            {
                IVersioningFile versioningFile = file as IVersioningFile;
                return versioningFile == null || !IsCheckedOutBySomeoneElse(versioningFile);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether can perform operations with versions of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if can perform operations with versions of the specified file; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanChangeVersions(IVersioningFile file)
        {
            Validate.RequiredParameter("file", file);
            return file.GetVersions().Count > 1 && CanEdit(file as VirtualFileBase);
        }

        /// <summary>
        /// Determines whether the currently logged on user can check out the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the currently logged on user is permitted to check out the specified file; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanCheckOut(VirtualFileBase file)
        {
            Validate.RequiredParameter("virtualFileBase", file);
            if (!CanEdit(file))
            {
                return false;
            }
            
            IVersioningFile versioningFile = file as IVersioningFile;
            return versioningFile == null ? false : !versioningFile.IsCheckedOut;
        }

        /// <summary>
        /// Determines whether the currently logged on user can undo a check out of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the user is allowed to undo check out of the specified file; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Administrators are always allowed to undo a check out files</remarks>
        public static bool CanUndoCheckOut(VirtualFileBase file)
        {
            Validate.RequiredParameter("file", file);
            IVersioningFile versioningFile = file as IVersioningFile;

            if (versioningFile != null) 
            {
                // Admins are allowed to undo other users check outs
                return (versioningFile.IsCheckedOut && PrincipalInfo.HasAdminAccess) || IsCheckedOutByCurrentUser(versioningFile);
            }
            return false;
        }


        /// <summary>
        /// Ensures that versioning file can be updated.
        /// </summary>
        /// <param name="file">The file.</param>
        private static void ThrowIfCheckedOut(IVersioningFile file)
        {
            Validate.RequiredParameter("file", file);
            if (IsCheckedOutBySomeoneElse(file))
            {
                throw new FileIsCheckedOutException(file.CheckedOutBy);
            }
        }

        /// <summary>
        /// Ensures that versioning directory can be updated.
        /// </summary>
        /// <param name="directory">The directory.</param>
        private static void ThrowIfCheckedOut(VersioningDirectory directory)
        {
            Validate.RequiredParameter("directory", directory);

            foreach (IVersioningFile file in directory.Files)
            {
                ThrowIfCheckedOut(file);
            }
            foreach (VersioningDirectory childDirectory in directory.Directories)
            {
                ThrowIfCheckedOut(childDirectory);
            }
        }

        /// <summary>
        /// Checks out all <see cref="IVersioningFile"/> items in the <see cref="VirtualFileBaseCollection"/>. 
        /// Items not implementing IVersioningFile will be ignored.
        /// </summary>
        /// <param name="items">The items to be checked out.</param>
        public static void CheckoutItems(VirtualFileBaseCollection items)
        {
            foreach (VirtualFileBase fileItem in items)
            {
                if (CanCheckOut(fileItem))
                {
                    ((IVersioningFile)fileItem).CheckOut();
                }
            }   
        }

        ///<summary>
        /// Undo Check Out of all files that are checked out in the supplied <see cref="VirtualFileBaseCollection"/>.
        ///</summary>
        /// <param name="items">The items to perform undo checkout on.</param>
        /// <remarks>Files not implementing <see cref="IVersioningFile"/> in are ignored.</remarks>
        public static void UndoCheckoutItems(VirtualFileBaseCollection items)
        {
            foreach (VirtualFileBase fileItem in items)
            {
                if (CanUndoCheckOut(fileItem))
                {
                    ((IVersioningFile)fileItem).UndoCheckOut();
                }
            }
        }      

        #endregion

        #region Copy Functionality

        /// <summary>
        /// Copies a virtual file or folder to the target directory.
        /// </summary>
        /// <param name="sourceItem">The item to copy.</param>
        /// <param name="targetDirectory">The target directory.</param>
        public static void CopyItem(VirtualFileBase sourceItem, UnifiedDirectory targetDirectory)
        {
            if (sourceItem.IsDirectory)
            {
                CopyDirectory((VirtualDirectory)sourceItem, targetDirectory);
            }
            else
            {
                CopyFile((VirtualFile)sourceItem, targetDirectory);
            }
        }

        /// <summary>
        /// Copies a file to the target folder.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="targetDirectory">The target directory.</param>
        public static void CopyFile(VirtualFile sourceFile, UnifiedDirectory targetDirectory)
        {
            CopyFile(sourceFile, targetDirectory, null);
        }

        /// <summary>
        /// Copies a file to the target folder, optionally with a new file name.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="newName">The new name. Supply <c>null</c> to use the name of the source file.</param>
        public static void CopyFile(VirtualFile sourceFile, UnifiedDirectory targetDirectory, string newName)
        {
            if (String.IsNullOrEmpty(newName))
            {
                newName = sourceFile.Name;
            }
            
            string targetVirtualFilePath = VirtualPathUtility.Combine(targetDirectory.VirtualPath, newName);
            targetVirtualFilePath = FindAvailableVirtualPath(targetVirtualFilePath);

            UnifiedFile unifiedSourceFile = sourceFile as UnifiedFile;
            if (unifiedSourceFile != null)
            {
                // If the source file is a UnifiedFile use its copy implementation
                unifiedSourceFile.CopyTo(targetVirtualFilePath);
            }
            else
            {
                // If the source is unknown. i.e. implementing VirtualFile but not UnifiedFile.
                // Use the support methods of UnifiedFile to copy the source VirtualFile.
                UnifiedFile.CopyTo(sourceFile, targetVirtualFilePath);
            }
            //TODO: Implement copying of VersioningFile to a non-versioning provider.
            // The versioning providers overrides the [Copy/Move]To methods of UnifiedFile, 
            // but throws an exception if trying to move or copy a file FROM a versioning 
            // provider to another provider
        }

        /// <summary>
        /// Copies the source directory to the target directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="targetDirectory">The target directory.</param>
        public static void CopyDirectory(VirtualDirectory sourceDirectory, UnifiedDirectory targetDirectory)
        {
            CopyDirectory(sourceDirectory, targetDirectory, null);
        }

        /// <summary>
        /// Copies a directory to a new location, optionally with a new name.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="newName">The optional new name of the directory. Supply <c>null</c> to use the name of the source directory.</param>
        public static void CopyDirectory(VirtualDirectory sourceDirectory, UnifiedDirectory targetDirectory, string newName)
        {
            if (String.IsNullOrEmpty(newName))
            {
                newName = sourceDirectory.Name;
            }

            string targetVirtualDirectoryPath = VirtualPathUtility.Combine(targetDirectory.VirtualPath, newName);

            // TODO: Move this check into VirtualPath implementation. It may be OK depending on the implementation.
            // Copying a snapshot will work, but a recursive "live" copy operation will fail with infinite recursion.
            if (!ValidatePathStructureForMove(sourceDirectory.VirtualPath, targetVirtualDirectoryPath))
            {
                throw new ArgumentException("Copying a folder below itself is not allowed");
            }

            // Check if we are pasting a folder to same location it's copied from.
            if (VirtualPathUtility.GetDirectory(sourceDirectory.VirtualPath) == targetDirectory.VirtualPath)
            {
                throw new ArgumentException("Pasting a folder to the same location as the source folder is not allowed");
            }

            if (!IsPathAvailable(targetVirtualDirectoryPath))
            {
                throw new ArgumentException("Pasting a folder with the same name as a file or a folder in the target directory is not allowed");
            }

            UnifiedDirectory unifiedSourceDirectory = sourceDirectory as UnifiedDirectory;
            if (unifiedSourceDirectory != null)
            {
                unifiedSourceDirectory.CopyTo(targetVirtualDirectoryPath);
            }
            else
            {
                UnifiedDirectory newDirectory = UnifiedDirectory.CreateDirectory(targetVirtualDirectoryPath);
                foreach (VirtualFileBase childItem in sourceDirectory.Children)
                {
                    CopyItem(childItem, newDirectory);
                }
            }
        }

        #endregion
        
        #region Move Functionality

        /// <summary>
        /// Renames a virtual file or folder.
        /// </summary>
        /// <param name="sourceItem">The item to rename.</param>
        /// <param name="newName">The new name.</param>
        public static void RenameItem(VirtualFileBase sourceItem, string newName)
        {
            // Get the parent directory of the source item.
            string sourceItemParentPath = VirtualPathUtility.GetDirectory(sourceItem.VirtualPath);
            UnifiedDirectory targetDirectory = HostingEnvironment.VirtualPathProvider.GetDirectory(sourceItemParentPath) as UnifiedDirectory;
            MoveItem(sourceItem, targetDirectory, newName);
        }

        /// <summary>
        /// Moves a virtual file or folder to a new location.
        /// </summary>
        /// <param name="sourceItem">The source item to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        public static void MoveItem(VirtualFileBase sourceItem, UnifiedDirectory targetDirectory)
        {
            MoveItem(sourceItem, targetDirectory, null);
        }

        /// <summary>
        /// Moves a virtual file or folder to a new location, optionally renaming it at the same time.
        /// </summary>
        /// <param name="sourceItem">The source item to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        /// <param name="newName">An optional new name. Supply <c>null</c> to use the original name of the source folder.</param>
        public static void MoveItem(VirtualFileBase sourceItem, UnifiedDirectory targetDirectory, string newName)
        {
            if (sourceItem.IsDirectory)
            {
                MoveDirectory((VirtualDirectory)sourceItem, targetDirectory, newName);
            }
            else
            {
                MoveFile((VirtualFile)sourceItem, targetDirectory, newName);
            }
        }

        /// <summary>
        /// Moves a virtual file to another directory.
        /// </summary>
        /// <param name="sourceFile">The source file to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        public static void MoveFile(VirtualFile sourceFile, UnifiedDirectory targetDirectory)
        {
            MoveFile(sourceFile, targetDirectory, null);
        }

        /// <summary>
        /// Moves a virtual file to a new directory, optioanlly renaming it in the process.
        /// </summary>
        /// <param name="sourceFile">The source file to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        /// <param name="newName">An optional new name. Supply <c>null</c> to use the original name of the source file.</param>
        public static void MoveFile(VirtualFile sourceFile, UnifiedDirectory targetDirectory, string newName)
        {
            if (String.IsNullOrEmpty(newName))
            {
                newName = sourceFile.Name;
            }

            string targetVirtualFilePath = VirtualPathUtility.Combine(targetDirectory.VirtualPath, newName);

            UnifiedFile unifiedSourceFile = sourceFile as UnifiedFile;
            if (unifiedSourceFile != null)
            {
                IVersioningFile versioningFile = sourceFile as IVersioningFile;
                if (versioningFile != null)
                {
                    ThrowIfCheckedOut(versioningFile);
                }

                if (!IsPathAvailable(targetVirtualFilePath))
                {
                    throw new ArgumentException("Moving a file with the same name as a file or a folder in the target directory is not allowed");
                }

                unifiedSourceFile.MoveTo(targetVirtualFilePath);
            }
            // Can't move a file unless its a UnifiedFile. VirtualFile does not specify any remove methods.
        }



        /// <summary>
        /// Moves a virtual directopry to a new location.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        public static void MoveDirectory(VirtualDirectory sourceDirectory, UnifiedDirectory targetDirectory)
        {
            MoveDirectory(sourceDirectory, targetDirectory, null);
        }

        /// <summary>
        /// Moves a virtual directopry to a new location, optioanlly renaming it in the process.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to be moved.</param>
        /// <param name="targetDirectory">The destination directory.</param>
        /// <param name="newName">An optional new name. Supply <c>null</c> to use the original name of the source file.</param>
        public static void MoveDirectory(VirtualDirectory sourceDirectory, UnifiedDirectory targetDirectory, string newName)
        {
            if (String.IsNullOrEmpty(newName))
            {
                newName = sourceDirectory.Name;
            }

            string targetVirtualDirectoryPath = VirtualPathUtility.Combine(targetDirectory.VirtualPath, newName);

            // TODO: Move this logic into UnifiedDirectoey it shouldn't be possible to orphan directories.
            if (!ValidatePathStructureForMove(sourceDirectory.VirtualPath, targetVirtualDirectoryPath))
            {
                throw new ArgumentException("Moving a folder below itself is not allowed.");
            }

            UnifiedDirectory unifiedSourceDirectory = sourceDirectory as UnifiedDirectory;
            if (unifiedSourceDirectory != null)
            {
                if (unifiedSourceDirectory.Parent.VirtualPath != targetDirectory.VirtualPath)
                {
                    VersioningDirectory versioningDirectory = sourceDirectory as VersioningDirectory;
                    if (versioningDirectory != null)
                    {
                        ThrowIfCheckedOut(versioningDirectory);
                    }
                }

                unifiedSourceDirectory.MoveTo(targetVirtualDirectoryPath);
            }
            // If source directory isn't a UnifiedDirectory we can't move it.
        }

        #endregion

        #region Path allocation utility methods

        /// <summary>
        /// Validates that that a move operation won't cause invalid path structures. e.g. Moving a folder into itself.
        /// </summary>
        /// <param name="sourceVirtualPath">The source virtual path.</param>
        /// <param name="destinationVirtualPath">The destination virtual path.</param>
        /// <returns><c>true</c> if a move will give a valid structure; otherwise <c>false</c>.</returns>
        public static bool ValidatePathStructureForMove(string sourceVirtualPath, string destinationVirtualPath)
        {
            // Directories can't be moved to a location below itself
            string targetParentDirectory = VirtualPathUtility.GetDirectory(destinationVirtualPath);
            return !targetParentDirectory.StartsWith(sourceVirtualPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Finds a non-existing file or directory from a supplied suggestion by adding "[ X ]" at the end of the name. X is a number starting at 1.
        /// </summary>
        /// <param name="virtualPath">The prefered new directory or file name..</param>
        /// <returns>The complete virtual path to an unused file or folder.</returns>
        public static string FindAvailableVirtualPath(string virtualPath)
        {
            string freePath = virtualPath;
            string extension = VirtualPathUtility.GetExtension(virtualPath);
            string fileName = VirtualPathUtility.GetFileName(virtualPath);
            if (!String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(extension))
            {
                fileName = fileName.Replace(extension, String.Empty);
            }

            int i = 1;
            while (!IsPathAvailable(freePath))
            {
                string newName = fileName + " [" + i++ + "]" + extension;
                freePath = VirtualPathUtility.Combine(virtualPath, newName);
            }
            return freePath;
        }

        /// <summary>
        /// Determines whether a virtual path points an existing file or folder, or if it's unused.
        /// </summary>
        /// <param name="virtualPath">The virtual path to check.</param>
        /// <returns>
        /// 	<c>true</c> if the path is available; otherwise <c>false</c>.
        /// </returns>
        public static bool IsPathAvailable(string virtualPath)
        {
            return !HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualPath) && 
                !HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
        }

        /// <summary>
        /// Checks whether a file with the supplied name exists in the supplied directory.
        /// </summary>
        /// <param name="directory">The directory to check in.</param>
        /// <param name="fileName">Name of the file to check for.</param>
        /// <returns><c>true</c> if a file with the supplied name exists in the directory; otherwise <c>false</c>.</returns>
        public static bool FileExists(VirtualDirectory directory, string fileName)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(VirtualPathUtility.Combine(directory.VirtualPath, fileName));
        }

        /// <summary>
        /// Creates a temporary file in the supplied directory.
        /// </summary>
        /// <param name="baseDirectory">The directory where the temporary file is created.</param>
        /// <returns>A Unified File</returns>
        public static UnifiedFile CreateTempFile(UnifiedDirectory baseDirectory)
        {
            string virtualPath = FindAvailableVirtualPath(VirtualPathUtility.Combine(baseDirectory.VirtualPath, TempFileName + TempFileExtension));
            return baseDirectory.CreateFile(VirtualPathUtility.GetFileName(virtualPath));
        }

        /// <summary>
        /// Creates a temporary file in the supplied directory and writes stream content to file.
        /// </summary>
        /// <param name="baseDirectory">The directory where the temporary file is created.</param>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns>A Unified File</returns>        
        public static UnifiedFile CreateTempFile(UnifiedDirectory baseDirectory, Stream fileContent)
        { 
            UnifiedFile file = CreateTempFile(baseDirectory);
            WriteToFile(fileContent, file, null);
            return file;
        }

        #endregion

    }

}

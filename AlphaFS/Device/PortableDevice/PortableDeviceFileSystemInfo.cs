﻿/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Alphaleonis.Win32.Filesystem;
using File = Alphaleonis.Win32.Filesystem.File;
using FileSystemInfo = System.IO.FileSystemInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Alphaleonis.Win32.Device
{
   /// <summary>Provides the base class for both <see cref="T:PortableDeviceFileInfo"/> and <see cref="T:PortableDeviceDirectoryInfo"/> objects.</summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class PortableDeviceFileSystemInfo : MarshalByRefObject
   {
      #region Fields

      private FileSystemEntryInfo _entryInfo;


      #region .NET

      /// <summary>Represents the fully qualified path of the file or directory.</summary>
      /// <remarks>
      ///   <para>Classes derived from <see cref="Filesystem.FileSystemInfo"/> can use the FullPath field</para>
      ///   <para>to determine the full path of the object being manipulated.</para>
      /// </remarks>
      [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      protected string FullPath;

      /// <summary>The path originally specified by the user, whether relative or absolute.</summary>
      [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      protected string OriginalPath;

      #endregion // .NET


      // We use this field in conjunction with the Refresh methods, if we succeed
      // we store a zero, on failure we store the HResult in it so that we can
      // give back a generic error back.
      [NonSerialized]
      internal int DataInitialised = -1;


      // The pre-cached FileSystemInfo information.
      [NonSerialized]
      internal Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA Win32AttributeData;

      #endregion // Fields
      

      #region Properties

      #region .NET

      /// <summary>Gets or sets the attributes for the current file or directory.</summary>
      /// <returns><see cref="T:System.IO.FileAttributes"/> of the current <see cref="T:FileSystemInfo"/>.</returns>
      public FileAttributes Attributes
      {
         [SecurityCritical]
         get
         {
            if (DataInitialised == -1)
            {
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
               Refresh();
            }

            // MSDN: .NET 3.5+: IOException: Refresh cannot initialize the data. 
            if (DataInitialised != 0)
               NativeError.ThrowException(DataInitialised, FullPath);

            return Win32AttributeData.dwFileAttributes;
         }

         [SecurityCritical]
         set
         {
            File.SetAttributesCore(null, IsDirectory, FullName, value, PathFormat.FullPath);
            Reset();
         }
      }


      /// <summary>Gets or sets the creation time of the current file or directory.</summary>
      /// <returns>The creation date and time of the current <see cref="T:FileSystemInfo"/> object.</returns>
      /// <remarks>This value is expressed in local time.</remarks>
      public DateTime CreationTime
      {
         [SecurityCritical] get { return CreationTimeUtc.ToLocalTime(); }

         [SecurityCritical] set { CreationTimeUtc = value.ToUniversalTime(); }
      }


      /// <summary>Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.</summary>
      /// <returns>The creation date and time in UTC format of the current <see cref="T:FileSystemInfo"/> object.</returns>
      /// <remarks>This value is expressed in UTC time.</remarks>
      [ComVisible(false)]
      public DateTime CreationTimeUtc
      {
         [SecurityCritical]
         get
         {
            if (DataInitialised == -1)
            {
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
               Refresh();
            }

            // MSDN: .NET 3.5+: IOException: Refresh cannot initialize the data. 
            if (DataInitialised != 0)
               NativeError.ThrowException(DataInitialised, FullName);

            return DateTime.FromFileTimeUtc(Win32AttributeData.ftCreationTime);
         }

         [SecurityCritical]
         set
         {
            File.SetFsoDateTimeCore(null, FullName, value, null, null, false, PathFormat.FullPath);
            Reset();
         }
      }
      

      /// <summary>Gets a value indicating whether the file or directory exists.</summary>
      /// <returns><c>true</c> if the file or directory exists; otherwise, <c>false</c>.</returns>
      /// <remarks>The <see cref="T:Exists"/> property returns <c>false</c> if any error occurs while trying to determine if the specified file or directory exists. This can occur in situations that raise exceptions such as passing a directory- or file name with invalid characters or too many characters, a failing or missing disk, or if the caller does not have permission to read the file or directory.</remarks>
      public abstract bool Exists { get; }


      /// <summary>Gets the extension part of the file.</summary>
      /// <returns>The <see cref="T:System.IO.FileSystemInfo"/> extension.</returns>
      public string Extension
      {
         get { return Path.GetExtension(FullPath, false); }
      }


      /// <summary>Gets the full path of the file or directory.</summary>
      /// <returns>The full path.</returns>
      public virtual string FullName
      {
         [SecurityCritical]
         get { return FullPath; }
      }


      /// <summary>Gets or sets the time the current file or directory was last accessed.</summary>
      /// <returns>The time that the current file or directory was last accessed.</returns>
      /// <remarks>This value is expressed in local time.</remarks>
      /// <remarks>When first called, <see cref="T:FileSystemInfo"/> calls Refresh and returns the cached information on APIs to get attributes and so on. On subsequent calls, you must call Refresh to get the latest copy of the information. 
      /// If the file described in the <see cref="T:FileSystemInfo"/> object does not exist, this property will return 12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC), adjusted to local time. 
      /// </remarks>
      public DateTime LastAccessTime
      {
         [SecurityCritical]
         get { return LastAccessTimeUtc.ToLocalTime(); }

         [SecurityCritical]
         set { LastAccessTimeUtc = value.ToUniversalTime(); }
      }


      /// <summary>Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.</summary>
      /// <returns>The UTC time that the current file or directory was last accessed.</returns>
      /// <remarks>This value is expressed in UTC time.</remarks>
      /// <remarks>When first called, <see cref="T:FileSystemInfo"/> calls Refresh and returns the cached information on APIs to get attributes and so on. On subsequent calls, you must call Refresh to get the latest copy of the information. 
      /// If the file described in the <see cref="T:FileSystemInfo"/> object does not exist, this property will return 12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC), adjusted to local time. 
      /// </remarks>
      [ComVisible(false)]
      public DateTime LastAccessTimeUtc
      {
         [SecurityCritical]
         get
         {
            if (DataInitialised == -1)
            {
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
               Refresh();
            }

            // MSDN: .NET 3.5+: IOException: Refresh cannot initialize the data. 
            if (DataInitialised != 0)
               NativeError.ThrowException(DataInitialised, FullName);

            return DateTime.FromFileTimeUtc(Win32AttributeData.ftLastAccessTime);
         }

         [SecurityCritical]
         set
         {
            File.SetFsoDateTimeCore(null, FullName, null, value, null, false, PathFormat.FullPath);
            Reset();
         }
      }


      /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
      /// <returns>The time the current file was last written.</returns>
      /// <remarks>This value is expressed in local time.</remarks>
      public DateTime LastWriteTime
      {
         get { return LastWriteTimeUtc.ToLocalTime(); }
         set { LastWriteTimeUtc = value.ToUniversalTime(); }
      }


      /// <summary>Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.</summary>
      /// <returns>The UTC time when the current file was last written to.</returns>
      /// <remarks>This value is expressed in UTC time.</remarks>
      [ComVisible(false)]
      public DateTime LastWriteTimeUtc
      {
         [SecurityCritical]
         get
         {
            if (DataInitialised == -1)
            {
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
               Refresh();
            }

            // MSDN: .NET 3.5+: IOException: Refresh cannot initialize the data. 
            if (DataInitialised != 0)
               NativeError.ThrowException(DataInitialised, FullName);

            return DateTime.FromFileTimeUtc(Win32AttributeData.ftLastWriteTime);
         }

         [SecurityCritical]
         set
         {
            File.SetFsoDateTimeCore(null, FullName, null, null, value, false, PathFormat.FullPath);
            Reset();
         }
      }


      /// <summary>For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. Otherwise, the Name property gets the name of the directory.</summary>
      /// <returns>The name of the parent directory, the name of the last directory in the hierarchy, or the name of a file, including the file name extension.</returns>
      public abstract string Name { get; internal set; }

      #endregion // .NET


      #region AlphaFS

      /// <summary>Returns the path as a string.</summary>
      protected internal string DisplayPath { get; protected set; }
      

      /// <summary>[AlphaFS] Gets the instance of the <see cref="T:FileSystemEntryInfo"/> class.</summary>
      public FileSystemEntryInfo EntryInfo
      {
         [SecurityCritical]
         get
         {
            if (null == _entryInfo)
            {
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
               RefreshEntryInfo();
            }

            // MSDN: .NET 3.5+: IOException: Refresh cannot initialize the data. 
            if (DataInitialised > 0)
               NativeError.ThrowException(DataInitialised, FullName);

            return _entryInfo;
         }

         internal set
         {
            _entryInfo = value;

            DataInitialised = value == null ? -1 : 0;

            if (DataInitialised == 0)
               Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA(_entryInfo.Win32FindData);
         }
      }


      /// <summary>[AlphaFS] The initial "IsDirectory" indicator that was passed to the constructor.</summary>
      protected internal bool IsDirectory { get; set; }


      /// <summary>
      /// 
      /// </summary>
      public Guid ContentType { get; internal set; }


      /// <summary>
      /// 
      /// </summary>
      public string Id { get; set; }


      /// <summary>
      /// 
      /// </summary>
      public string OriginalFileName { get; set; }


      /// <summary>
      /// 
      /// </summary>
      public string ParentId { get; set; }

      
      ///// <summary>
      ///// 
      ///// </summary>
      //public PortableDeviceInfo PortableDeviceInfo { get; set; }

      #endregion // AlphaFS

      #endregion // Properties


      #region Methods
      
      #region .NET

      /// <summary>Deletes a file or directory.</summary>
      [SecurityCritical]
      public abstract void Delete();


      /// <summary>Refreshes the state of the object.</summary>
      /// <remarks>
      ///   <para>FileSystemInfo.Refresh() takes a snapshot of the file from the current file system.</para>
      ///   <para>Refresh cannot correct the underlying file system even if the file system returns incorrect or outdated information.</para>
      ///   <para>This can happen on platforms such as Windows 98.</para>
      ///   <para>Calls must be made to Refresh() before attempting to get the attribute information, or the information will be
      ///   outdated.</para>
      /// </remarks>
      [SecurityCritical]
      public void Refresh()
      {
         DataInitialised = File.FillAttributeInfoCore(null, FullName, ref Win32AttributeData, false, false);

         IsDirectory = File.IsDirectory(Win32AttributeData.dwFileAttributes);
      }


      /// <summary>Returns a string that represents the current object.</summary>
      /// <remarks>
      ///   ToString is the major formatting method in the .NET Framework. It converts an object to its string representation so that it is
      ///   suitable for display.
      /// </remarks>
      /// <returns>Returns a string that represents this instance.</returns>
      public override string ToString()
      {
         // "Alphaleonis.Win32.Filesystem.FileSystemInfo"
         return GetType().ToString();
      }


      /// <summary>Determines whether the specified Object is equal to the current Object.</summary>
      /// <param name="obj">Another object to compare to.</param>
      /// <returns><c>true</c> if the specified Object is equal to the current Object; otherwise, <c>false</c>.</returns>
      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         var other = obj as FileSystemInfo;

         return null != other && null != other.Name &&
                other.FullName.Equals(FullName, StringComparison.OrdinalIgnoreCase) && other.Attributes.Equals(Attributes) &&
                other.CreationTimeUtc.Equals(CreationTimeUtc) && other.LastWriteTimeUtc.Equals(LastWriteTimeUtc);
      }


      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>Returns a hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         return null != FullName ? FullName.GetHashCode() : 0;
      }


      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(PortableDeviceFileSystemInfo left, PortableDeviceFileSystemInfo right)
      {
         return ReferenceEquals(left, null) && ReferenceEquals(right, null) ||
                !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
      }


      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(PortableDeviceFileSystemInfo left, PortableDeviceFileSystemInfo right)
      {
         return !(left == right);
      }

      #endregion // .NET


      #region AlphaFS

      /// <summary>[AlphaFS] Refreshes the current <see cref="FileSystemInfo"/> instance (<see cref="PortableDeviceDirectoryInfo"/> or <see cref="PortableDeviceFileInfo"/>) with a new destination path.</summary>
      internal void UpdateSourcePath(string destinationPath, string destinationPathLp)
      {
         FullPath = null != destinationPathLp ? Path.GetRegularPathCore(FullName, GetFullPathOptions.None, false) : null;

         OriginalPath = destinationPath;
         DisplayPath = null != OriginalPath ? Path.GetRegularPathCore(OriginalPath, GetFullPathOptions.None, false) : null;

         // Flush any cached information about the FileSystemInfo instance.
         Reset();
      }


      /// <summary>Refreshes the state of the <see cref="PortableDeviceFileSystemInfo"/> EntryInfo instance.</summary>
      /// <remarks>
      ///   <para>FileSystemInfo.RefreshEntryInfo() takes a snapshot of the file from the current file system.</para>
      ///   <para>Refresh cannot correct the underlying file system even if the file system returns incorrect or outdated information.</para>
      ///   <para>This can happen on platforms such as Windows 98.</para>
      ///   <para>Calls must be made to Refresh() before attempting to get the attribute information, or the information will be outdated.</para>
      /// </remarks>
      [SecurityCritical]
      protected void RefreshEntryInfo()
      {
         _entryInfo = File.GetFileSystemEntryInfoCore(null, IsDirectory, FullName, true, PathFormat.LongFullPath);

         if (null == _entryInfo)
            DataInitialised = -1;

         else
         {
            DataInitialised = 0;
            Win32AttributeData = new Filesystem.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA(_entryInfo.Win32FindData);
         }
      }


      /// <summary>[AlphaFS] Resets the state of the file system object to uninitialized.</summary>
      internal void Reset()
      {
         DataInitialised = -1;
      }


      /// <summary>[AlphaFS] Initializes the specified file name.</summary>
      /// <param name="isFolder">Specifies that <paramref name="objectId"/> is a file or directory.</param>
      /// <param name="objectId">The full path and name of the file.</param>
      /// <param name="fullPath"></param>
      internal void InitializeCore(bool isFolder, string objectId, string fullPath)
      {
         //if (Utils.IsNullOrWhiteSpace(fullPath))
         //   throw new ArgumentNullException("fullPath");

         if (Utils.IsNullOrWhiteSpace(objectId))
            objectId = PortableDeviceConstants.DeviceObjectId;

         Id = objectId;

         //FullPath = !Utils.IsNullOrWhiteSpace(fullPath) ? fullPath : objectId;
         FullPath = fullPath;

         IsDirectory = isFolder;

         OriginalPath = objectId;

         DisplayPath = OriginalPath.Length != 2 || OriginalPath[1] != Path.VolumeSeparatorChar
            ? OriginalPath
            : Path.CurrentDirectoryPrefix;
      }

      #endregion // AlphaFS

      #endregion // Methods
   }
}
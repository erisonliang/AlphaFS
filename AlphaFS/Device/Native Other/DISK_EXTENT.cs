/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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

using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Device
{
   internal static partial class NativeMethods
   {
      // MSDN: Regardless of the disk type, a disk can contain one or more disk extents.
      // A disk extent is a contiguous range of logical blocks exposed by the disk. For example, a disk extent can represent an entire volume,
      // one portion of a spanned volume, one member of a striped volume, or one plex of a mirrored volume.




      /// <summary>Represents a disk extent.</summary>
      /// <remarks>
      ///   Minimum supported client: Windows XP [desktop apps only]
      ///   Minimum supported server: Windows Server 2003 [desktop apps only]
      /// </remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct DISK_EXTENT
      {
         /// <summary>The number of the disk that contains this extent. This is the same number that is used to construct the name of the disk,
         /// for example, the X in "\\.\PhysicalDriveX" or "\\?\HarddiskX".</summary>
         [MarshalAs(UnmanagedType.U4)] public uint DiskNumber;

         /// <summary>The offset from the beginning of the disk to the extent, in bytes.</summary>
         [MarshalAs(UnmanagedType.I8)] public long StartingOffset;

         /// <summary>The number of bytes in this extent.</summary>
         [MarshalAs(UnmanagedType.I8)] public long ExtentLength;
      }


      /// <summary>Represents a disk extent.</summary>
      /// <remarks>
      ///   Minimum supported client: Windows XP [desktop apps only]
      ///   Minimum supported server: Windows Server 2003 [desktop apps only]
      /// </remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct DISK_EXTENT_SINGLE
      {
         /// <summary>The number of disks in the volume (a volume can span multiple disks).</summary>
         [MarshalAs(UnmanagedType.U4)] public readonly uint NumberOfDiskExtents;

         public readonly DISK_EXTENT Extent;
      }
   }
}
using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   public static partial class Path
   {
      /// <summary>Combines an array of strings into a path.</summary>
      /// <param name="paths">An array of parts of the path.</param>
      /// <returns>The combined paths.</returns>
      ///
      /// <exception cref="ArgumentException">
      ///   One of the strings in the array contains invalid characters, is empty, or contains only white spaces.
      /// </exception>
      /// <exception cref="ArgumentNullException">One of the strings in the array is <see langword="null"/>.</exception>
      [SecurityCritical]
      public static string Combine(params string[] paths)
      {
         return CombineInternal(true, paths);
      }

      /// <summary>Unified method Combine() to combine an array of strings into a path.</summary>
      /// <remarks>
      ///   <para>The parameters are not parsed if they have white space.</para>
      ///   <para>Therefore, if path2 includes white space (for example, " c:\\ "),</para>
      ///   <para>the Combine method appends path2 to path1 instead of returning only path2.</para>
      /// </remarks>
      /// <exception cref="ArgumentNullException">One of the strings in the array is <see langword="null"/>.</exception>
      /// <param name="checkInvalidPathChars">
      ///   <see langword="true"/> will not check <paramref name="paths"/> for invalid path characters.
      /// </param>
      /// <param name="paths">An array of parts of the path.</param>
      /// <returns>Returns the combined paths.</returns>
      ///
      /// <exception cref="ArgumentException">
      ///   One of the strings in the array contains one or more of the invalid characters defined in <see cref="GetInvalidPathChars"/>.
      /// </exception>
      [SecurityCritical]
      internal static string CombineInternal(bool checkInvalidPathChars, params string[] paths)
      {
         if (paths == null)
            throw new ArgumentNullException("paths");

         int capacity = 0;
         int num = 0;
         for (int index = 0, l = paths.Length; index < l; ++index)
         {
            if (paths[index] == null)
               throw new ArgumentNullException("paths");

            if (paths[index].Length != 0)
            {
               if (IsPathRooted(paths[index], checkInvalidPathChars))
               {
                  num = index;
                  capacity = paths[index].Length;
               }
               else
                  capacity += paths[index].Length;

               char ch = paths[index][paths[index].Length - 1];

               if (!IsDVsc(ch, null))
                  ++capacity;
            }
         }

         var buffer = new StringBuilder(capacity);
         for (int index = num; index < paths.Length; ++index)
         {
            if (paths[index].Length != 0)
            {
               if (buffer.Length == 0)
                  buffer.Append(paths[index]);

               else
               {
                  char ch = buffer[buffer.Length - 1];

                  if (!IsDVsc(ch, null))
                     buffer.Append(DirectorySeparatorChar);

                  buffer.Append(paths[index]);
               }
            }
         }

         return buffer.ToString();
      }
   }
}

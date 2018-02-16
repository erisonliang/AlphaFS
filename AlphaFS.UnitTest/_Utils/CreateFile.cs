/*  Copyright (C) 2008-2017 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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

namespace AlphaFS.UnitTest
{
   public static partial class UnitTestConstants
   {
      public static System.IO.FileInfo CreateFile(string rootFolder, int fileLength = 0)
      {
         var file = System.IO.Path.Combine(rootFolder, GetRandomFileName());

         using (var fs = System.IO.File.Create(file))
         {
            if (fileLength <= 0)
               fileLength = new Random().Next(0, 10485760);

            fs.SetLength(fileLength);
         }

         return new System.IO.FileInfo(file);
      }
   }
}
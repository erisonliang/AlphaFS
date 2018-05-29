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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace AlphaFS.UnitTest
{
   public partial class EnumerationTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_Host_EnumerateDfsLinks_Network_Success()
      {
         UnitTestConstants.PrintUnitTestHeader(true);


         var cnt = 0;
         var noDomainConnection = true;
         try
         {
            foreach (var dfsNamespace in Alphaleonis.Win32.Network.Host.EnumerateDomainDfsRoot())
            {
               noDomainConnection = false;

               Console.Write("#{0:000}\tDFS Root: [{1}]\n", ++cnt, dfsNamespace);
               var cnt2 = 0;

               try
               {
                  foreach (var dfsInfo in Alphaleonis.Win32.Network.Host.EnumerateDfsLinks(dfsNamespace).OrderBy(d => d.EntryPath))
                     Console.Write("\n\t#{0:000}\tDFS Link: [{1}]", ++cnt2, dfsInfo.EntryPath);
               }
               catch (NetworkInformationException ex)
               {
                  Console.WriteLine("NetworkInformationException #1: [{0}]", ex.Message.Replace(Environment.NewLine, "  "));
               }
               catch (Exception ex)
               {
                  Console.WriteLine("\nCaught (UNEXPECTED #1) {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
               }

               Console.WriteLine();
            }
         }
         catch (NetworkInformationException ex)
         {
            Console.WriteLine("NetworkInformationException #2: [{0}]", ex.Message.Replace(Environment.NewLine, "  "));
         }
         catch (Exception ex)
         {
            Console.WriteLine("\nCaught (UNEXPECTED #2) {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
         }


         if (noDomainConnection)
            UnitTestAssert.Inconclusive("Test ignored because the computer is either not connected to a domain or no DFS root exists.");

         else if (cnt == 0)
            UnitTestAssert.InconclusiveBecauseEnumerationIsEmpty();


         Console.WriteLine();
      }
   }
}

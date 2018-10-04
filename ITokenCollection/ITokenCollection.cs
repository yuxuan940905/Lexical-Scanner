/////////////////////////////////////////////////////////////////////////
// ITokenCollection.cs  -  ITokenCollection                            //
//              served as scanner interface of SemiExp                 //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 2017, .Net Framework 4.6.1           //
// Application: Pr#2 , CSE681, Fall 2018                               //
// Author:      Yuxuan Xing, Syracuse University                       //
//              yxing05@syr.edu                                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This package provides a method get(), serverd as an interface of SemiExp
 * This interface provide extendibility for further development
 * 
 * Public Interface
 * ================
 *  ITokenCollection.get();            
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   SemiExp.cs
 * 
 * 
 * Maintenance History
 * ===================
 * ver 1.0 : 30 Sep 2018
 * - first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CToker;

namespace ITokenCollection
{
    public class ITCollection
    {
        Toker get() { return null; }

        static void Main()
        {
            
        }
    }
}

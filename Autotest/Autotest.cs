/////////////////////////////////////////////////////////////////////////
// Autotest.cs  -  AutoTest                                            //
//              Some autotest cases of the project function            //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 2017, .Net Framework 4.6.1           //
// Application: Pr#2 , CSE681, Fall 2018                               //
// Author:      Yuxuan Xing, Syracuse University                       //
//              yxing05@syr.edu                                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * This package provides several autotest classes for pro#2 requirement,
 * 
 * 
 * Public Interface
 * ================
 * // - Auto test cases
 * Requirement3();
 * Requirement4();
 * Requirement5();
 * Requirement6();
 * Requirement7();
 * Requirement8();
 * Requirement9();
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   Tokenizer.cs
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
using SemiExpression;
using CToker;

namespace Autotest
{
    class TestDemo
    {
        static System.IO.StreamReader fs_;

        static bool Requirement3()
        {
            Console.Write("\n\n  Demonstrate requirement 3: ");
            Console.Write("\n ============================\n");
            Console.Write("The packages of this project include:\n");
            Console.Write("Tokenizer, SemiExp, ITokenCollection, AutoTest \n");
            return true;
        }

        static bool Requirement4(string file)
        {
            Console.Write("\n\n  Demonstrate requirement 4: ");
            Console.Write("\n ============================\n");

            Console.Write(" The file content: \n\n");
            fs_ = new System.IO.StreamReader("../../"+file, true);
            int ch;
            while (!fs_.EndOfStream)
            {
                ch = fs_.Read();
                Console.Write("{0}",(char)ch);
            }
            fs_.Close();
            Console.Write("\n ============================\n");
            Console.Write("The output: \n");

            Toker toker = new Toker();

            string fqf = System.IO.Path.GetFullPath("../../"+file);
            if (!toker.open(fqf))
            {
                Console.Write("\n can't open {0}\n", fqf);
                return false;
            }
            else
            {
                Console.Write("\n  processing file: {0}", fqf);
            }
            while (!toker.isDone())
            {
                StringBuilder tok = toker.getTok();
                Console.Write("\n -- line#{0, 4} : {1}", toker.lineCount(), tok);
            }
            toker.close();
            
            return true;
        }

        static bool Requirement5()
        {
            Console.Write("\n\n  Demonstrate requirement 5: ");
            Console.Write("\n ============================\n");
            Console.Write("The Implementation of function getTok() is located on Tokenizer.cs \n");
            Console.Write("Line 129, 269, 303, 370\n");
            return true;
        }

        static bool Requirement6(string file)
        {
            Console.Write("\n\n  Demonstrate requirement 6: ");
            Console.Write("\n ============================\n");

            Console.Write(" The file content: \n\n");
            fs_ = new System.IO.StreamReader("../../" + file, true);
            int ch;
            while (!fs_.EndOfStream)
            {
                ch = fs_.Read();
                Console.Write("{0}", (char)ch);
            }
            fs_.Close();
            Console.Write("\n ============================\n");
            Console.Write("The output: \n");

            SemiExp test = new SemiExp();
            test.returnNewLines = true;
            test.displayNewLines = true;
            string path = "../../"+file;

            if (!test.open(path))
                Console.Write("\n  Can't open file {0}", path);
            while (test.GetSemi())
                test.display();
            test.close();
            return true;
        }

        static bool Requirement7(string file)
        {
            Console.Write("\n\n  Demonstrate requirement 7: ");
            Console.Write("\n ============================\n");

            Console.Write(" The file content: \n\n");
            fs_ = new System.IO.StreamReader("../../" + file, true);
            int ch;
            while (!fs_.EndOfStream)
            {
                ch = fs_.Read();
                Console.Write("{0}", (char)ch);
            }
            fs_.Close();
            Console.Write("\n ============================\n");
            Console.Write("The output: \n");

            SemiExp test = new SemiExp();
            test.returnNewLines = true;
            test.displayNewLines = true;
            string path = "../../" + file;

            if (!test.open(path))
                Console.Write("\n  Can't open file {0}", path);
            while (test.GetSemi())
                test.display();
            test.close();
            return true;
        }

        static bool Requirement8(string file)
        {
            Console.Write("\n\n  Demonstrate requirement 8: ");
            Console.Write("\n ============================\n");

            Console.Write(" The file content: \n\n");
            fs_ = new System.IO.StreamReader("../../" + file, true);
            int ch;
            while (!fs_.EndOfStream)
            {
                ch = fs_.Read();
                Console.Write("{0}", (char)ch);
            }
            fs_.Close();
            Console.Write("\n ============================\n");
            Console.Write("The output: \n");

            SemiExp test = new SemiExp();
            test.returnNewLines = true;
            test.displayNewLines = true;
            string path = "../../" + file;

            if (!test.open(path))
                Console.Write("\n  Can't open file {0}", path);
            while (test.GetSemi())
                test.display();
            test.close();
            return true;
        }

        static bool Requirement9()
        {
            Console.Write("\n\n  Demonstrate requirement 9: ");
            Console.Write("\n ============================\n");
            Console.Write("The get function is located on ITokenCollection Line 44 \n");
            Console.Write("But it is not used for this project \n");
            return true;
        }

        static void Main(string[] args)
        {
            Console.Write("\n  Auto test cases for the project: ");
            Console.Write("\n ============================\n");

            Requirement3();
            Requirement4(args[0]);
            Requirement5();
            Requirement6(args[1]);
            Requirement7(args[2]);
            Requirement8(args[3]);
            Requirement9();

            Console.ReadKey();
        }
    }
}

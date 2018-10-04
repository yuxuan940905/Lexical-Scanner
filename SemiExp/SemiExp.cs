/////////////////////////////////////////////////////////////////////////
// SemiExp.cs  -  SemiExpression                                       //
//              Group tokens into sets                                 //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 2017, .Net Framework 4.6.1           //
// Application: Pr#2 , CSE681, Fall 2018                               //
// Author:      Yuxuan Xing, Syracuse University                       //
//              yxing05@syr.edu                                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * Retrieve collections of tokens by calling Toker::getTok() repeatedly until one of the SemiExpression termination conditions, 
 * Termination conditions:
 * Extracting any of the single character tokens: 
 * semicolon, 
 * open brace, 
 * closed brace. 
 * Also on extracting newline if a '#' is the first token on that line.
 * Provide a rule to ignore the (two) semicolons within parentheses in a for(;;) expression
 * 
 * Public Interface
 * ================
 * SemiExp semi = new SemiEx;();      // constructs SemiExp object
 * if(semi.open(fileName)) ...        // attaches semi to specified file
 * semi.close();                      // closes file stream
 * if(semi.Equals(se)) ...            // do these semiExps have same tokens?
 * int hc = semi.GetHashCode()        // returns hashcode
 * if(getSemi()) ...                  // extracts and stores next semiExp
 * int len = semi.count;              // length property
 * semi.verbose = true;               // verbose property - shows tokens
 * string tok = semi[2];              // access a semi token
 * string tok = semi[1];              // extract token
 * semi.flush();                      // removes all tokens
 * semi.insert(2,tok);                // inserts token as third element
 * semi.Add(tok);                     // appends token
 * semi.Add(tokArray);                // appends array of tokens
 * semi.display();                    // sends tokens to Console
 * string show = semi.displayStr();   // returns tokens as single string
 * semi.returnNewLines = false;       // property defines newline handling             
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   Toker.cs
 *   ITokenCollection.cs
 *   SemiExp.cs
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
using ITokenCollection;

namespace SemiExpression
{
    public class SemiExp : ITCollection
    {
        Toker toker = null;
        List<string> semiExp = null;
        string semi="";
        StringBuilder strb;
        //string prevTok;

        //------< line count property >------------------------------

        public int LineCount
        {
            get { return toker.lineCount(); }
        }

        //------< constructor >--------------------------------------

        public SemiExp()
        {
            toker = new Toker();
            semiExp = new List<string>();
            discardComments = true;
            returnNewLines = true;
            displayNewLines = false;
        }

        //------< test for equality >----------------------------------------

        override public bool Equals(Object semi)
        {
            SemiExp temp = (SemiExp)semi;
            if (temp.count != this.count)
                return false;
            for (int i = 0; i < temp.LineCount && i < this.count;i++)
            {
                if (this[i] != temp[i])
                    return false;
            }
            return true;
        }

        //---< pos of first str in semi-expression if found, -1 otherwise >--

        public int FindFirst(string str)
        {
            for (int i = 0; i < count - 1; ++i)
                if (this[i] == str)
                    return i;
            return -1;
        }
        //---< pos of last str in semi-expression if found, -1 otherwise >--- 

        public int FindLast(string str)
        {
            for (int i = this.count - 1; i >= 0; --i)
                if (this[i] == str)
                    return i;
            return -1;
        }

        //----< deprecated: here to avoid breakage with old code >----------- 

        public int Contains(string str)
        {
            return FindLast(str);
        }

        //----< opens member tokenizer with specified file >-----------------

        public bool open(string fileName)
        {
            return toker.open(fileName);
        }
        //----< close file stream >------------------------------------------

        public void close()
        {
            toker.close();
        }

        //----< is this the last token in the current semiExpression? >------

        bool isTerminator(string tok)
        {
            switch(tok)
            {
                case ";": return true;
                case "{": return true;
                case "}": return true;
                case "\n":
                    if (this.FindFirst("#") != -1)  // expensive - may wish to cache in get
                        return true;
                    return false;
                default: return false;
            }
        }

        //----< collect semiExpression from filtered token stream >----------

        public bool isWhiteSpace(string tok)
        {
            Char ch = tok[0];
            return Char.IsWhiteSpace(tok[0]);
        }

        //----< is this token a comment? >-----------------------------------

        public bool isComment(string tok)
        {
            if (tok.Length > 1)
                if (tok[0] == '/')
                    if (tok[1] == '/' || tok[1] == '*')
                        return true;
            return false;
        }

        //----< is this character a punctuator> >----------------------------

        bool IsPunc(char ch)
        {
            return (Char.IsPunctuation(ch) || Char.IsSymbol(ch));
        }

        //----< insert token - fails if out of range and returns false>------

        public bool insert(int loc, string tok)
        {
            if (0 <= loc && loc < semiExp.Count)
            {
                semiExp.Insert(loc, tok);
                return true;
            }
            return false;
        }
        //----< append token to end of semiExp >-----------------------------

        public SemiExp Add(string token)
        {
            semiExp.Add(token);
            return this;
        }
        //----< load semiExp from array of strings >-------------------------

        public void Add(string[] source)
        {
            foreach (string tok in source)
                semiExp.Add(tok);
        }

        //----< make a copy of semiEpression >-------------------------------

        public SemiExp clone()
        {
            SemiExp copy = new SemiExp();
            for (int i = 0; i < count; ++i)
            {
                copy.Add(this[i]);
            }
            return copy;
        }

        //----< remove all contents of semiExp >-----------------------------

        public void flush()
        {
            semiExp.RemoveRange(0, semiExp.Count);
        }
        //----< remove a token from semiExpression >-------------------------

        public bool remove(int i)
        {
            if (0 <= i && i < semiExp.Count)
            {
                semiExp.RemoveAt(i);
                return true;
            }
            return false;
        }
        //----< remove a token from semiExpression >-------------------------

        public bool remove(string token)
        {
            if (semiExp.Contains(token))
            {
                semiExp.Remove(token);
                return true;
            }
            return false;
        }

        //----< display semiExpression on Console >--------------------------

        public void display()
        {
            Console.Write("\n -- ");
            Console.Write(displayStr());
        }
        //----< return display string >--------------------------------------

        public string displayStr()
        {
            StringBuilder disp = new StringBuilder("");
            foreach (string tok in semiExp)
            {
                disp.Append(tok);
                if (tok.IndexOf('\n') != tok.Length - 1)
                    disp.Append(" ");
            }
            return disp.ToString();
        }

        //----< have to override GetHashCode() >-----------------------------

        override public System.Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        //----< indexer for semiExpression >---------------------------------

        public string this[int i]
        {
            get { return semiExp[i]; }
            set { semiExp[i] = value; }
        }

        //----< get length property >----------------------------------------

        public int count
        {
            get { return semiExp.Count; }
        }

        //----< announce tokens when verbose is true >-----------------------

        public bool verbose
        {
            get;
            set;
        }
        //----< determines whether new lines are returned with semi >--------

        public bool returnNewLines
        {
            get;
            set;
        }
        //----< determines whether new lines are displayed >-----------------

        public bool displayNewLines
        {
            get;
            set;
        }
        //----< determines whether comments are discarded >------------------

        public bool discardComments
        {
            get;
            set;
        }

        public void trim()
        {
            SemiExp temp = new SemiExp();
            foreach (string tok in semiExp)
            {
                if (isWhiteSpace(tok))
                    continue;
                temp.Add(tok);
            }
            semiExp = temp.semiExp;
        }

        //---------< get a set of a semi >------------------------------------
        public bool GetSemi()
        {
            semiExp.RemoveRange(0, semiExp.Count);  // empty container

            do
            {
                strb = toker.getTok();
                if (strb != null)
                    semi = strb.ToString();
                if (isComment(semi))    //throw the comment
                    continue;
                if (strb==null)    //end of file
                    return false;
                if (returnNewLines || !semi.Equals("\n"))
                    semiExp.Add(semi);
            } while (!isTerminator(semi) || count == 0);

            trim();

            if(semiExp.Contains("for"))
            {
                SemiExp se = clone();
                GetSemi();              //recursively
                se.Add(semiExp.ToArray());
                GetSemi();
                se.Add(semiExp.ToArray());
                semiExp.Clear();
                for (int i = 0; i < se.count; i++)
                    semiExp.Add(se[i]);
            }
            return (semiExp.Count > 0);
        }

#if (TEST_SEMIEXP)

        //----< test stub >--------------------------------------------------
        class DemoSemi
        {
            static bool TestSemi(string path)
            {
                SemiExp test = new SemiExp();
                test.returnNewLines = true;
                test.displayNewLines = true;

                if(!test.open(path))
                    Console.Write("\n  Can't open file {0}", path);
                while (test.GetSemi())
                    test.display();
                test.close();
                return true;
            }

            [STAThread]
            static void Main(string[] args)
            {
                Console.Write("\n  Testing semiExp Operations");
                Console.Write("\n ============================\n");

                TestSemi("../../TestSemiExp.txt");
            }
        }
        
#endif
    }
}

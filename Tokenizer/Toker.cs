/////////////////////////////////////////////////////////////////////
// Toker.cs - Extract tokens from source file                      //
// ver 1.2                                                         //
// Author: Yuxuan Xing, Syracuse University                        //
// Source: Jim Fawcett,                                            //
//         CSE681 - Software Modeling and Analysis, Fall 2018      //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Demonstrates how to build a tokenizer based on the State Pattern.
 * Intentionally does not provide a lot of the required facilities
 * for Project #2.
 * 
 * Required Files:
 * ---------------
 * Toker.cs
 * 
 * Public Interface:
 * -----------------
 * ITokenSource                 // - Declares operations expected of any source of tokens
 * ITokenState                  // - Declares operations expected of any token gathering state
 * Toker tok= new Toker();      // - Construct new Toker class
 * tok.getTok()                 // - Extract tokens from source file
 * tok.open(path)               // - Open a file
 * tok.close()                  // - Close a file
 * tok.isDone()                 // - Whether a file reading is finished
 * TokenState
 * isWhiteSpace(int)            // - if it is a WhiteSpaceState
 * isPunctuation(int)           // - if it is a PunctuationState
 * isLetterOrDigit(int)         // - if it is a AlphaState
 * DemoToker.testToker(string)  // - Run test cases of Tokenizer
 * 
 * Maintenance History
 * -------------------
 * ver 1.2 : 03 Sep 2018
 * - added comments just above the definition of derived states, near line #209
 * ver 1.1 : 02 Sep 2018
 * - Changed Toker, TokenState, TokenFileSource, and TokenContext to fix a bug
 *   in setting the initial state.  These changes are cited, below.
 * - Removed TokenState state_ from toker so only TokenContext instance manages 
 *   the current state.
 * - Changed TokenFileSource() to TokenFileSource(TokenContext context) to allow the 
 *   TokenFileSource instance to set the initial state correctly.
 * - Changed TokenState.nextState() to static TokenState.nextState(TokenContext context).
 *   That allows TokenFileSource to use nextState to set the initial state correctly.
 * - Changed TokenState.nextState(context) to treat everything that is not whitespace
 *   and is not a letter or digit as punctuation.  Char.IsPunctuation was not inclusive
 *   enough for Toker.
 * - changed current_ to currentState_ for readability
 * ver 1.0 : 30 Aug 2018
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CToker
{
    using Token = StringBuilder;

    ///////////////////////////////////////////////////////////////////
    // ITokenSource interface
    // - Declares operations expected of any source of tokens
    // - Typically we would use either files or strings.  This demo
    //   provides a source only for Files, e.g., TokenFileSource, below.

    public interface ITokenSource
    {
        bool open(string path);
        void close();
        int next();
        int peek(int n = 0);
        bool end();
        int lineCount { get; set; }
    }

    ///////////////////////////////////////////////////////////////////
    // ITokenState interface
    // - Declares operations expected of any token gathering state

    public interface ITokenState
    {
        Token getTok();
        bool isDone();
    }

    ///////////////////////////////////////////////////////////////////
    // Toker class
    // - applications need to use only this class to collect tokens

    public class Toker
    {
        private TokenContext context_;       // holds single instance of all states and token source

        //----< initialize state machine >-------------------------------

        public Toker()
        {
            context_ = new TokenContext();      // context is the glue that holds all of the state machine parts 
        }
        //----< attempt to open source of tokens >-----------------------
        /*
         * If src is successfully opened, it uses TokenState.nextState(context_)
         * to set the initial state, based on the source content.
         */
        public bool open(string path)
        {
            TokenSourceFile src = new TokenSourceFile(context_);
            context_.src = src;
            return src.open(path);
        }
        //----< close source of tokens >---------------------------------

        public void close()
        {
            context_.src.close();
        }
        //----< extract a token from source >----------------------------

        private bool isWhiteSpaceToken(Token tok)
        {
            return (tok.Length > 0 && Char.IsWhiteSpace(tok[0]));
        }

        public Token getTok()
        {
            Token tok = null;
            while (!isDone())
            {
                tok = context_.currentState_.getTok();
                context_.currentState_ = TokenState.nextState(context_);
                if (!isWhiteSpaceToken(tok))
                    break;
            }
            return tok;
        }
        //----< has Toker reached end of its source? >-------------------

        public bool isDone()
        {
            if (context_.currentState_ == null)
                return true;
            return context_.currentState_.isDone();
        }
        public int lineCount() { return context_.src.lineCount; }
    }
    ///////////////////////////////////////////////////////////////////
    // TokenContext class
    // - holds all the tokenizer states
    // - holds source of tokens
    // - internal qualification limits access to this assembly

    public class TokenContext
    {
        internal TokenContext()
        {
            ws_ = new WhiteSpaceState(this);
            ps_ = new PunctState(this);
            as_ = new AlphaState(this);
            // more states here
            currentState_ = ws_;
        }
        internal WhiteSpaceState ws_ { get; set; }
        internal PunctState ps_ { get; set; }
        internal AlphaState as_ { get; set; }
        // more states here

        internal TokenState currentState_ { get; set; }
        internal ITokenSource src { get; set; }  // can hold any derived class
    }

    ///////////////////////////////////////////////////////////////////
    // TokenState class
    // - base for all the tokenizer states

    public abstract class TokenState : ITokenState
    {

        internal TokenContext context_ { get; set; }  // derived classes store context ref here

        //----< delegate source opening to context's src >---------------

        public bool open(string path)
        {
            return context_.src.open(path);
        }
        //----< pass interface's requirement onto derived states >-------

        public abstract Token getTok();

        //----< derived states don't have to know about other states >---

        static public TokenState nextState(TokenContext context)
        {
            int nextItem = context.src.peek();
            if (nextItem < 0)
                return null;
            char ch = (char)nextItem;

            if (Char.IsWhiteSpace(ch))
                return context.ws_;
            if (Char.IsLetterOrDigit(ch))
                return context.as_;

            // Test for strings and comments here since we don't
            // want them classified as punctuators.

            // toker's definition of punctuation is anything that
            // is not whitespace and is not a letter or digit
            // Char.IsPunctuation is not inclusive enough

            return context.ps_;
        }
        //----< has tokenizer reached the end of its source? >-----------

        public bool isDone()
        {
            if (context_.src == null)
                return true;
            return context_.src.end();
        }
    }
    ///////////////////////////////////////////////////////////////////
    // Derived State Classes
    /* - WhiteSpaceState          Token with space, tab, and newline chars
     * - AlphaNumState            Token with letters and digits
     * - PunctuationState         Token holding anything not included above
     * ----------------------------------------------------------------
     * - Each state class accepts a reference to the context in its
     *   constructor and saves in its inherited context_ property.
     * - It is only required to provide a getTok() method which
     *   returns a token conforming to its state, e.g., whitespace, ...
     * - getTok() assumes that the TokenSource's first character 
     *   matches its type e.g., whitespace char, ...
     * - The nextState() method ensures that the condition, above, is
     *   satisfied.
     * - The getTok() method promises not to extract characters from
     *   the TokenSource that belong to another state.
     * - These requirements lead us to depend heavily on peeking into
     *   the TokenSource's content.
     */
    ///////////////////////////////////////////////////////////////////
    // WhiteSpaceState class
    // - extracts contiguous whitespace chars as a token
    // - will be thrown away by tokenizer

    public class WhiteSpaceState : TokenState
    {
        public WhiteSpaceState(TokenContext context)
        {
            context_ = context;
        }
        //----< manage converting extracted ints to chars >--------------

        bool isWhiteSpace(int i)
        {
            int nextItem = context_.src.peek();
            if (nextItem < 0)
                return false;
            char ch = (char)nextItem;
            return Char.IsWhiteSpace(ch);
        }
        //----< keep extracting until get none-whitespace >--------------

        override public Token getTok()
        {
            Token tok = new Token();
            tok.Append((char)context_.src.next());     // first is WhiteSpace

            while (isWhiteSpace(context_.src.peek()))  // stop when non-WhiteSpace
            {
                tok.Append((char)context_.src.next());
            }
            return tok;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // PunctState class
    // - extracts contiguous punctuation chars as a token

    public class PunctState : TokenState
    {
        public PunctState(TokenContext context)
        {
            context_ = context;
        }
        //----< manage converting extracted ints to chars >--------------

        bool isPunctuation(int i)
        {
            int nextItem = context_.src.peek();
            if (nextItem < 0)
                return false;
            char ch = (char)nextItem;
            return (!Char.IsWhiteSpace(ch) && !Char.IsLetterOrDigit(ch));
        }

        //-----< set the special two char punctuators >------------------

        public void SetSpecialCharPairs(string scp)
        {
            
        }
        //----< keep extracting until get none-punctuator >--------------

        override public Token getTok()
        {
            Token tok = new Token();
            tok.Append((char)context_.src.next());       // first is punctuator

            if (tok[0] == '\"')
            {
                while (context_.src.peek() != '\"')
                {
                    tok.Append((char)context_.src.next());
                }
                tok.Append((char)context_.src.next());
            }

            else if(tok[0]=='\'')
            {
                while (context_.src.peek() != '\'')
                {
                    tok.Append((char)context_.src.next());
                }
                tok.Append((char)context_.src.next());
            }
            
            else if (tok[0] == '/')
            {
                if (context_.src.peek() == '/')     //single line comment
                {
                    while (context_.src.peek() != '\n')
                    {
                        tok.Append((char)context_.src.next());
                    }
                }

                else if (context_.src.peek() == '*')    //Multi-line comments
                {
                    tok.Append((char)context_.src.next());
                    while(!(tok[tok.Length-1]=='*' && context_.src.peek()=='/'))
                    {
                        tok.Append((char)context_.src.next());
                    }
                    tok.Append((char)context_.src.next());
                }

                else if (context_.src.peek() == '=')
                {
                    tok.Append((char)context_.src.next());
                }
            }

            //  special two pairs
            else if (tok[0] == '<')
            {
                if (context_.src.peek() == '<')
                {
                    tok.Append((char)context_.src.next());
                }
            }

            else if (tok[0] == '>')
            {
                if (context_.src.peek() == '>')
                {
                    tok.Append((char)context_.src.next());
                }
            }

            else if (tok[0] == ':')
            {
                if (context_.src.peek() == ':')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '=')
            {
                if (context_.src.peek() == '=')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '*')
            {
                if (context_.src.peek() == '=')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '|')
            {
                if (context_.src.peek() == '|')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '&')
            {
                if (context_.src.peek() == '&')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '+')
            {
                if (context_.src.peek() == '+'|| context_.src.peek() == '=')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            else if (tok[0] == '-')
            {
                if (context_.src.peek() == '-' || context_.src.peek() == '=')
                {
                    tok.Append((char)context_.src.next());
                }
            }
            return tok;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // AlphaState class
    // - extracts contiguous letter and digit chars as a token

    public class AlphaState : TokenState
    {
        public AlphaState(TokenContext context)
        {
            context_ = context;
        }
        //----< manage converting extracted ints to chars >--------------

        bool isLetterOrDigit(int i)
        {
            int nextItem = context_.src.peek();
            if (nextItem < 0)
                return false;
            char ch = (char)nextItem;
            return Char.IsLetterOrDigit(ch);
        }
        //----< keep extracting until get none-alpha >-------------------

        override public Token getTok()
        {
            Token tok = new Token();
            tok.Append((char)context_.src.next());          // first is alpha

            while (isLetterOrDigit(context_.src.peek())||context_.src.peek()=='_')    // stop when non-alpha
            {
                tok.Append((char)context_.src.next());
            }
            return tok;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // TokenSourceFile class
    // - extracts integers from token source
    // - Streams often use terminators that can't be represented by
    //   a character, so we collect all elements as ints
    // - keeps track of the line number where a token is found
    // - uses StreamReader which correctly handles byte order mark
    //   characters and alternate text encodings.

    public class TokenSourceFile : ITokenSource
    {
        public int lineCount { get; set; } = 1;
        private System.IO.StreamReader fs_;           // physical source of text
        private List<int> charQ_ = new List<int>();   // enqueing ints but using as chars
        private TokenContext context_;

        public TokenSourceFile(TokenContext context)
        {
            context_ = context;
        }
        //----< attempt to open file with a System.IO.StreamReader >-----

        public bool open(string path)
        {
            try
            {
                fs_ = new System.IO.StreamReader(path, true);
                context_.currentState_ = TokenState.nextState(context_);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}\n", ex.Message);
                return false;
            }
            return true;
        }
        //----< close file >---------------------------------------------

        public void close()
        {
            fs_.Close();
        }
        //----< extract the next available integer >---------------------
        /*
         *  - checks to see if previously enqueued peeked ints are available
         *  - if not, reads from stream
         */
        public int next()
        {
            int ch;
            if (charQ_.Count == 0)  // no saved peeked ints
            {
                if (end())
                    return -1;
                ch = fs_.Read();
            }
            else                    // has saved peeked ints, so use the first
            {
                ch = charQ_[0];
                charQ_.Remove(ch);
            }
            if ((char)ch == '\n')   // track the number of newlines seen so far
                ++lineCount;
            return ch;
        }
        //----< peek n ints into source without extracting them >--------
        /*
         *  - This is an organizing prinicple that makes tokenizing easier
         *  - We enqueue because file streams only allow peeking at the first int
         *    and even that isn't always reliable if an error occurred.
         *  - When we look for two punctuator tokens, like ==, !=, etc. we want
         *    to detect their presence without removing them from the stream.
         *    Doing that is a small part of your work on this project.
         */
        public int peek(int n = 0)
        {
            if (n < charQ_.Count)  // already peeked, so return
            {
                return charQ_[n];
            }
            else                  // nth int not yet peeked
            {
                for (int i = charQ_.Count; i <= n; ++i)
                {
                    if (end())
                        return -1;
                    charQ_.Add(fs_.Read());  // read and enqueue
                }
                return charQ_[n];   // now return the last peeked
            }
        }
        //----< reached the end of the file stream? >--------------------

        public bool end()
        {
            return fs_.EndOfStream;
        }
    }

#if (TEST_TOKER)

    class DemoToker
    {
        static bool testToker(string path)
        {
            Toker toker = new Toker();

            string fqf = System.IO.Path.GetFullPath(path);
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
                Token tok = toker.getTok();
                Console.Write("\n -- line#{0, 4} : {1}", toker.lineCount(), tok);
            }
            toker.close();
            return true;
        }
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrate Toker class");
            Console.Write("\n =========================");

            StringBuilder msg = new StringBuilder();
            msg.Append("\n  Some things this demo does not do for CSE681 Project #2:");
            msg.Append("\n  - collect comments as tokens");
            msg.Append("\n  - collect double quoted strings as tokens");
            msg.Append("\n  - collect single quoted strings as tokens");
            msg.Append("\n  - collect specified single characters as tokens");
            msg.Append("\n  - collect specified character pairs as tokens");
            msg.Append("\n  - integrate with a SemiExpression collector");
            msg.Append("\n  - provide the required package structure");
            msg.Append("\n");

            Console.Write(msg);

            testToker("../../TestToker.txt");
            testToker("../../Toker.cs");

            Console.Write("\n\n");
        }
    }
}

#endif

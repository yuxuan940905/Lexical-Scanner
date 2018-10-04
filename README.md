# Lexical-Scanner
This is a Lexical Scanner,whose function is extracting tokens from source file, handling them, 
and grouping them into sets.
This is a start project for futher development

Instrcutions:
------------------------------------
1.Double-click compile.bat
2.Double-click run.bat
  An auto test will be executive

This project includes following features:
---------------------------------------------------
1. Use Visual Studio 2017 and its C# Windows Console Projects, as provided in the ECS computer labs.
2. Use the .Net System.IO and System.Text for all I/O.
3. Provide C# packages for Tokenizing, collecting SemiExpressions, and a scanner interface, ITokCollection.
4. Provide a Tokenizer package that declares and defines a Toker class 
   that implements the State Pattern with an abstract ConsumeState class and 
   derived classes for collecting the following token types:
   - alphanumeric tokens
   - punctuator tokens
   - special one and two character tokens with defaults
   - Single-line comments returned as a single token, e.g., //
   - Multi-line comments returned as a single token, e.g., /* ... */
   - quoted strings
5. The Toker class, contained in the Tokenizer package, shall produce one token for each call to a member function getTok().
6. Provide a SemiExpression package that contains a class SemiExp 
   used to retrieve collections of tokens by calling Toker::getTok() repeatedly 
   until one of the SemiExpression termination conditions, below, is satisfied.
7. Terminate a token collection after extracting any of the single character tokens: 
   semicolon, open brace, closed brace. Also on extracting newline if a '#' is the first token on that line.
8. Provide a facility providing rules to ignore certain termination characters under special circumstances. 
   Provide a rule to ignore the (two) semicolons within parentheses in a for(;;) expression.
9. The SemiExp class implemented the interface ITokenCollection with a declared method get().
10. Include an automated unit test suite that exercises all of the special cases 
    that seem appropriate for these two packages.

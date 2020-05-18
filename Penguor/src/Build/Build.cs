/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

using System.Collections.Generic;
using System.IO;
using Penguor.Lexing;
using Penguor.Parsing;
using Penguor.Debugging;
using Penguor.Parsing.AST;
using Penguor.ASM;

namespace Penguor.Build
{
    /// <summary>
    /// automated Builder
    /// </summary>
    public class Builder
    {
        List<Token> tokens;

        private Lexer lexer; // the lexer
        private Parser parser;
        private AsmGenerator iLGenerator;

        /// <summary>
        /// Initialize a new builder with the given values
        /// </summary>
        public Builder()
        {
            lexer = new Lexer();
            iLGenerator = new AsmGenerator();
        }

        /// <summary>
        /// Build a program from a single file
        /// </summary>
        /// <param name="fileName"></param>
        public void BuildFromSource(string fileName)
        {
            Debug.Log("Building from source", LogLevel.Info);
            // the tokens constructed by the Lexer

            tokens = lexer.Tokenize(fileName);

            parser = new Parser(tokens);

            foreach (Token token in tokens)
            {
                Debug.Log(token.ToString(), LogLevel.Debug);
            }

            //     Stmt program = parser.Parse();

            //     Library ilCode = iLGenerator.GenerateFromAST(program);
        }
    }
}
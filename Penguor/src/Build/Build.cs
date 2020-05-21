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
using Penguor.Lexing;
using Penguor.Parsing;
using Penguor.Debugging;
using Penguor.ASM;
using Penguor.Parsing.AST;

namespace Penguor.Build
{
    public class Builder
    {
        List<Token> tokens;

        private Lexer lexer; // the lexer
        private Parser parser;
        private AsmGenerator iLGenerator;

        public static int ExitCode = 0;
        public static string ActiveFile { get; set; }

        /// <summary>
        /// Initialize a new Builder with the given values
        /// </summary>
        public Builder()
        {
            lexer = new Lexer();
            iLGenerator = new AsmGenerator();
        }

        /// <summary>
        /// Build a program from a single file
        /// </summary>
        /// <param name="fileName">the source file to build from</param>
        public void BuildFromSource(string fileName)
        {
            Debugging.Debug.Log("Building from source", LogLevel.Info);
            // the tokens constructed by the Lexer

            ActiveFile = fileName;
            tokens = lexer.Tokenize(fileName);

            if (ExitCode > 0) System.Environment.Exit(ExitCode);

            parser = new Parser(tokens);

            Stmt program = parser.Parse();
            if (ExitCode > 0) System.Environment.Exit(ExitCode);
            

            //     Library ilCode = iLGenerator.GenerateFromAST(program);
        }
    }
}
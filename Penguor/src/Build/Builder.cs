/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# all rights reserved
# 
*/

using System;
using System.Collections.Generic;
using Penguor.Debugging;
using Penguor.Compiler.Lexing;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Transpiling;

using IOFile = System.IO.File;

namespace Penguor.Compiler.Build
{

    /// <summary>
    /// This class handles the management for building a Penguor source file
    /// </summary>
    public class Builder
    {
        private int exitCode = 0;
        List<Token>? tokens;
        Decl? program;

        /// <summary>
        /// has the lexer run?
        /// </summary>
        public bool lexerFinished = false;
        /// <summary>
        /// has the parser run?
        /// </summary>
        public bool parserFinished = false;

        /// <summary>
        /// the source file this builder builds
        /// </summary>
        /// <value></value>
        public string File { get; }

        /// <summary>
        /// The code the program exits with. If it isn't 0, an error has occurred
        /// </summary>
        public int ExitCode { get { return exitCode; } }

        /// <summary>
        /// create a new instance of the Builder class
        /// </summary>
        /// <param name="file">the source file this builder is using</param>
        public Builder(string file)
        {
            if (IOFile.Exists(file)) File = file;
            else throw new PenguorException(5, 0);
        }

        /// <summary>
        /// Build a program from a single file
        /// </summary>
        public int Build()
        {
            Lex();
            CheckExit();
            Parse();
            CheckExit();

            return ExitCode;
        }

        /// <summary>
        /// Split up the source file into tokens
        /// </summary>
        /// <returns>a list of tokens created from the source file</returns>
        public List<Token> Lex()
        {
            Lexer lexer = new Lexer(File, this);
            tokens = lexer.Tokenize();
            return tokens;
        }

        /// <summary>
        /// parses the Tokens produced by Lex()
        /// </summary>
        /// <returns>the ASt of the parsed program</returns>
        public Decl Parse()
        {
            if (!lexerFinished) Lex();
            if (tokens == null) throw new PenguorCSException(1);
            Parser parser = new Parser(tokens, this);

            try
            {
                program = parser.Parse();
            }
            catch (PenguorException)
            {
                if (ExitCode != 0) Environment.Exit(ExitCode);
                else Environment.Exit(1);
            }
            return program;
        }

        /// <summary>
        /// transpiles a file to another language
        /// </summary>
        /// <param name="lang">the language to transpile to</param>
        /// <param name="output">where to write the output file to</param>
        public void Transpile(TranspileLanguage lang, string output)
        {
            if (!parserFinished) Parse();
            if (program == null) throw new PenguorCSException(1);

            switch (lang)
            {
                case TranspileLanguage.CSHARP:
                    CSharptTranspiler transpiler = new CSharptTranspiler((ProgramDecl)program);
                    transpiler.Transpile(output);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="offset"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void Exception(int msg, int offset, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            Debug.CastPGR(msg, offset, File, arg0, arg1, arg2, arg3);
        }

        private void CheckExit()
        {
            if (ExitCode > 0) System.Environment.Exit(ExitCode);
        }
    }
}
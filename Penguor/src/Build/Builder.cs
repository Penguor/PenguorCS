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
using Penguor.Lexing;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

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

        public string File { get; }

        /// <summary>
        /// The code the program exits with. If it isn't 0, an error has occurred
        /// </summary>
        public int ExitCode { get { return exitCode; } }

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

        public List<Token> Lex()
        {
            Lexer lexer = new Lexer(File, this);
            tokens = lexer.Tokenize();
            return tokens;
        }

        public Decl Parse()
        {
            if (tokens == null) throw new PenguorCSException(1);
            Parser parser = new Parser(tokens, this);

            Decl program = null!;
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
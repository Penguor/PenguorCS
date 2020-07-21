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

using Penguor.Compiler.Debugging;
using Penguor.Compiler.Lexing;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Analysis;
using Penguor.Compiler.IR;
using Penguor.Compiler.Transpiling;

using IOFile = System.IO.File;

namespace Penguor.Compiler.Build
{
    /// <summary>
    /// This class handles the management for building a Penguor source file
    /// </summary>
    public class Builder
    {
        private List<Token>? tokens;
        private Decl? program;

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
        public int ExitCode { get; private set; }

        /// <summary>
        /// create a new instance of the Builder class
        /// </summary>
        /// <param name="file">the source file this builder is using</param>
        public Builder(string file)
        {
            Exceptions = new List<PenguorException>();
            if (!IOFile.Exists(file))
            {
                Debug.CastPGRCS(6, file);
                Exit(1);
            }
            File = file;
        }

        /// <summary>
        /// Build a program from a single file
        /// </summary>
        public int Build()
        {
            Lex();
            Parse();
            Analyse();
            return ExitCode;
        }

        /// <summary>
        /// Split up the source file into tokens
        /// </summary>
        /// <returns>a list of tokens created from the source file</returns>
        public List<Token> Lex()
        {
            Lexer lexer = new Lexer(File, this);
            try
            {
                tokens = lexer.Tokenize();
                SubmitAllExceptions();
            }
            catch (LexingException e)
            {
                e.Log(File);
                Exit(1);
            }
            catch (PenguorCSException)
            {
                Exit(1);
            }
            lexerFinished = true;
            return tokens!;
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
                SubmitAllExceptions();
            }
            catch (ParsingException e)
            {
                e.Log(File);
                Exit(1);
            }
            parserFinished = true;
            return program ?? throw new NullReferenceException();
        }

        /// <summary>
        /// perform semantic analysis on the abstract syntax tree
        /// </summary>
        public void Analyse()
        {
            if (!lexerFinished) Lex();
            if (!parserFinished) Parse();

            SemanticAnalyser analyser = new SemanticAnalyser((ProgramDecl)program!);

            Decl analysed = analyser.Analyse();
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
                    CSharpTranspiler transpiler = new CSharpTranspiler((ProgramDecl)program);
                    transpiler.Transpile(output);
                    break;
            }
        }

        // Below this is the code for handling errors in the Penguor source code

        /// <summary>
        /// contains all caught exceptions
        /// </summary>
        /// <value></value>
        public List<PenguorException> Exceptions { get; protected set; }

        /// <summary>
        /// submit a single exception
        /// </summary>
        public void SubmitException()
        {
            if (Exceptions.Count == 0) return;
            PenguorException p = Exceptions[0];
            Exceptions.RemoveAt(0);
            p.Log(File);
        }

        /// <summary>
        /// submit all exceptions
        /// </summary>
        public void SubmitAllExceptions()
        {
            foreach (var e in Exceptions)
                e.Log(File);
            Exceptions.Clear();
        }

        /// <summary>
        /// exit the program after submitting all exceptions
        /// </summary>
        /// <param name="exitCode"></param>
        public void Exit(int exitCode = 0)
        {
            SubmitAllExceptions();
            ExitCode = exitCode;

            Environment.Exit(exitCode);
        }
    }
}
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
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Penguor.Compiler.Debugging;
using Penguor.Compiler.Lexing;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Analysis;

namespace Penguor.Compiler.Build
{
    /// <summary>
    /// This class handles the management for building a Penguor source file
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// contains the SymbolTableManager for the compilation process
        /// </summary>
        public SymbolTableManager TableManager { get; }

        private List<Token>? tokens;
        private ProgramDecl? program;

        /// <summary>
        /// has the lexer run?
        /// </summary>
        public bool lexerFinished;
        /// <summary>
        /// has the parser run?
        /// </summary>
        public bool parserFinished;

        /// <summary>
        /// the source file this builder builds
        /// </summary>
        public string SourceFile { get; }

        public string Input { get; set; }

        /// <summary>
        /// The code the program exits with. If it isn't 0, an error has occurred
        /// </summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// create a new instance of the Builder class
        /// </summary>
        /// <param name="tableManager">the Manager which contains the symbols</param>
        /// <param name="file">the source file to compile</param>
        public Builder(ref SymbolTableManager tableManager, string file)
        {
            TableManager = tableManager;
            Exceptions = new List<PenguorException>();
            try
            {
                Input = File.ReadAllText(file);
                SourceFile = file;
            }
            catch (FileNotFoundException)
            {
                throw new PenguorCSException(6, file);
            }
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
        public void Lex()
        {
            Lexer lexer = new Lexer(this);
            try
            {
                tokens = lexer.Tokenize();
                SubmitAllExceptions();
            }
            catch (LexingException e)
            {
                e.Log(SourceFile);
                Exit(1);
            }
            catch (PenguorCSException)
            {
                Exit(1);
            }
            lexerFinished = true;
        }

        /// <summary>
        /// Split up the source file into tokens and put the result into <c>tokens</c>
        /// </summary>
        /// <param name="tokens">where to</param>
        public void Lex(out List<Token> tokens)
        {
            Lex();
            if (this.tokens != null) tokens = this.tokens;
            else throw new NullReferenceException();
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
                e.Log(SourceFile);
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

            SemanticAnalyser analyser = new SemanticAnalyser(program ?? throw new ArgumentNullException(nameof(program)), this);

            Decl analysed = analyser.Analyse();
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
            p.Log(SourceFile);
        }

        /// <summary>
        /// submit all exceptions
        /// </summary>
        public void SubmitAllExceptions()
        {
            foreach (var e in Exceptions)
                e.Log(SourceFile);
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
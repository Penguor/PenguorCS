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
using Penguor.Debugging;
using Penguor.Lexing;
using Penguor.Parsing;
using Penguor.Parsing.AST;

//! Builder should maybe be static
namespace Penguor.Build
{

    /// <summary>
    /// This class handles the management for building a Penguor source file
    /// </summary>
    public class Builder
    {
        private static string? activeFile;
        List<Token>? tokens;

        /// <summary>
        /// The code the program exits with. If it isn't 0, an error has occurred
        /// </summary>
        public static int ExitCode = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static string ActiveFile
        {
            get
            {
                if (activeFile != null) return activeFile;
                throw new PenguorCSException(9);
            }
            set => activeFile = value;
        }

        /// <summary>
        /// Build a program from a single file
        /// </summary>
        /// <param name="fileName">the source file to build from</param>
        public void BuildFromSource(string fileName)
        {
            Debugging.Debug.Log("Building from source", LogLevel.Info);

            ActiveFile = fileName;
            Lexer lexer = new Lexer(fileName);
            CheckExit();
            tokens = lexer.Tokenize();
            CheckExit();

            Parser parser = new Parser(tokens);
            CheckExit();
            Decl program = parser.Parse();
            CheckExit();
        }

        private void CheckExit()
        {
            if (ExitCode > 0) System.Environment.Exit(ExitCode);
        }
    }
}
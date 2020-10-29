/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System;

using Penguor.Compiler.Parsing;

namespace Penguor.Compiler.Debugging
{
    /// <summary>
    /// An exception which occurred while parsing a Penguor source file
    /// </summary>
    public class ParsingException : PenguorException
    {
        /// <summary>Gets the expected <c>TokenType</c>(s)</summary>
        public TokenType[] ExpectedTypes { get; }
        /// <summary>Gets the found <c>Token</c></summary>
        public Token ActualToken { get; }

        /// <summary>
        /// create a new ParsingException
        /// </summary>
        /// <param name="msg">the error message number</param>
        /// <param name="actual">the actual Token</param>
        /// <param name="expected">the expected Token, optional</param>
        public ParsingException(uint msg, Token actual, TokenType[] expected) : base(msg, actual.Offset)
        {
            ExpectedTypes = expected;
            ActualToken = actual;
        }
    }
}
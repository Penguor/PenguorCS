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
    public class LexingException : PenguorException
    {
        public string Found { get; }

        /// <summary>
        /// create a new LexingException
        /// </summary>
        /// <param name="msg">the error message number</param>
        public LexingException(uint msg, int offset, string found) : base(msg, offset)
        {
            Found = found;
        }
    }
}
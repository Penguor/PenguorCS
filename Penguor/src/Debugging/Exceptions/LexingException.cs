/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

namespace Penguor.Compiler.Debugging
{
    /// <summary>
    /// An exception which occurred while parsing a Penguor source file
    /// </summary>
    public class LexingException : PenguorException
    {
        /// <summary>
        /// the character which was found
        /// </summary>
        public string Found { get; }

        /// <summary>
        /// create a new LexingException
        /// </summary>
        /// <param name="msg">the error message number</param>
        /// <param name="offset">the offset where the exception occurred</param>
        /// <param name="found"></param>
        public LexingException(uint msg, int offset, string found) : base(msg, offset)
        {
            Found = found;
        }
    }
}
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

namespace Penguor.Parsing
{
    /// <summary>
    /// a Penguor token
    /// </summary>
    public struct Token
    {
        /// <summary>
        /// the type of token
        /// </summary>
        public readonly TokenType type;

        /// <summary>
        /// the content
        /// </summary>
        public readonly string token;

        /// <summary>
        /// line number of the token
        /// </summary>
        public readonly int lineNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="line"></param>
        public Token(TokenType type, string token, int line)
        {
            this.type = type;
            this.token = token;
            lineNumber = line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a string of the token with line number and type</returns>
        public override string ToString()
        {
            return ("type: " + type.ToString() + ", token: " + token + ", line number: " + lineNumber);
        }
    }
}
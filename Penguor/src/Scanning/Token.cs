/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
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
        /// offset of the token
        /// </summary>
        public readonly int offset;
        /// <summary>
        /// length of the token
        /// </summary>
        public readonly int length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="offset">the offset of the token in the source file</param>
        /// <param name="length">the length of the token</param>
        public Token(TokenType type, string token, int offset, int length)
        {
            this.type = type;
            this.token = token;
            this.offset = offset;
            this.length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a string of the token with line number and type</returns>
        public override string ToString()
        {
            return ($"type: {type.ToString()}, token: {token}, offset: {offset}, length: {length}");
        }
    }
}
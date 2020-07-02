/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

namespace Penguor.Compiler.Parsing
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

        public string TokenTypeToString() => TokenTypeToString(type);

        public static string TokenTypeToString(TokenType type) => type switch
        {
            TokenType.HASHTAG => "#",
            TokenType.USING => "using",
            TokenType.SAFETY => "safety",
            TokenType.PUBLIC => "public",
            TokenType.PRIVATE => "private",
            TokenType.PROTECTED => "protected",
            TokenType.RESTRICTED => "restricted",
            TokenType.STATIC => "static",
            TokenType.DYNAMIC => "dynamic",
            TokenType.ABSTRACT => "abstract",
            TokenType.CONST => "const",
            TokenType.LPAREN => "(",
            TokenType.RPAREN => ")",
            TokenType.LBRACE => "{",
            TokenType.RBRACE => "}",
            TokenType.LBRACK => "[",
            TokenType.RBRACK => "]",
            TokenType.PLUS => "+",
            TokenType.MINUS => "-",
            TokenType.MUL => "*",
            TokenType.DIV => "/",
            TokenType.PERCENT => "%",
            TokenType.DPLUS => "++",
            TokenType.DMINUS => "--",
            TokenType.GREATER => ">",
            TokenType.LESS => "<",
            TokenType.GREATER_EQUALS => ">=",
            TokenType.LESS_EQUALS => "<=",
            TokenType.EQUALS => "==",
            TokenType.NEQUALS => "!=",
            TokenType.AND => "&&",
            TokenType.OR => "||",
            TokenType.XOR => "^^",
            TokenType.BW_AND => "&",
            TokenType.BW_OR => "|",
            TokenType.BW_XOR => "^",
            TokenType.BW_NOT => "~",
            TokenType.BS_LEFT => "<<",
            TokenType.BS_RIGHT => ">>",
            TokenType.ASSIGN => "=",
            TokenType.ADD_ASSIGN => "+=",
            TokenType.SUB_ASSIGN => "-=",
            TokenType.MUL_ASSIGN => "*=",
            TokenType.DIV_ASSIGN => "/=",
            TokenType.PERCENT_ASSIGN => "%=",
            TokenType.BW_AND_ASSIGN => "&=",
            TokenType.BW_OR_ASSIGN => "|=",
            TokenType.BW_XOR_ASSIGN => "^=",
            TokenType.BS_LEFT_ASSIGN => "<<=",
            TokenType.BS_RIGHT_ASSIGN => ">>=",
            TokenType.NULL => "null",
            TokenType.COLON => ":",
            TokenType.SEMICOLON => ";",
            TokenType.ENDING => "newline",
            TokenType.DOT => ".",
            TokenType.COMMA => ",",
            TokenType.EXCL_MARK => "!",
            TokenType.NUM => "number",
            TokenType.STRING => "string",
            TokenType.IDF => "identifier",
            TokenType.TRUE => "true",
            TokenType.FALSE => "false",
            TokenType.CONTAINER => "container",
            TokenType.SYSTEM => "system",
            TokenType.DATATYPE => "datatype",
            TokenType.LIBRARY => "library",
            TokenType.IF => "if",
            TokenType.ELIF => "elif",
            TokenType.ELSE => "else",
            TokenType.FOR => "for",
            TokenType.WHILE => "while",
            TokenType.DO => "do",
            TokenType.SWITCH => "switch",
            TokenType.CASE => "case",
            TokenType.DEFAULT => "default",
            TokenType.EOF => "end of file",
            TokenType.RETURN => "return",
            _ => throw new System.ArgumentException()
        };

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
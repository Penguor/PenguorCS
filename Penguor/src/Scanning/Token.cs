/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using static Penguor.Compiler.Parsing.TokenType;

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
        /// create a new Token with the given values
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
        /// returns the TokenType as string
        /// </summary>
        public override string ToString() => ToString(type);

        /// <summary>
        /// returns <c>type</c> as string
        /// </summary>
        /// <param name="type">the type which should be turned into a string</param>

        public static string ToString(TokenType type) => type switch
        {
            HASHTAG => "#",
            USING => "using",
            SAFETY => "safety",
            PUBLIC => "public",
            PRIVATE => "private",
            PROTECTED => "protected",
            RESTRICTED => "restricted",
            STATIC => "static",
            DYNAMIC => "dynamic",
            ABSTRACT => "abstract",
            CONST => "const",
            LPAREN => "(",
            RPAREN => ")",
            LBRACE => "{",
            RBRACE => "}",
            LBRACK => "[",
            RBRACK => "]",
            PLUS => "+",
            MINUS => "-",
            MUL => "*",
            DIV => "/",
            PERCENT => "%",
            DPLUS => "++",
            DMINUS => "--",
            GREATER => ">",
            LESS => "<",
            GREATER_EQUALS => ">=",
            LESS_EQUALS => "<=",
            EQUALS => "==",
            NEQUALS => "!=",
            AND => "&&",
            OR => "||",
            XOR => "^^",
            BW_AND => "&",
            BW_OR => "|",
            BW_XOR => "^",
            BW_NOT => "~",
            BS_LEFT => "<<",
            BS_RIGHT => ">>",
            ASSIGN => "=",
            ADD_ASSIGN => "+=",
            SUB_ASSIGN => "-=",
            MUL_ASSIGN => "*=",
            DIV_ASSIGN => "/=",
            PERCENT_ASSIGN => "%=",
            BW_AND_ASSIGN => "&=",
            BW_OR_ASSIGN => "|=",
            BW_XOR_ASSIGN => "^=",
            BS_LEFT_ASSIGN => "<<=",
            BS_RIGHT_ASSIGN => ">>=",
            NULL => "null",
            COLON => ":",
            SEMICOLON => ";",
            ENDING => "newline",
            DOT => ".",
            COMMA => ",",
            EXCL_MARK => "!",
            NUM => "number",
            STRING => "string",
            IDF => "identifier",
            TRUE => "true",
            FALSE => "false",
            DATA => "data",
            SYSTEM => "system",
            TYPE => "type",
            LIBRARY => "library",
            IF => "if",
            ELIF => "elif",
            ELSE => "else",
            FOR => "for",
            WHILE => "while",
            DO => "do",
            SWITCH => "switch",
            CASE => "case",
            DEFAULT => "default",
            EOF => "end of file",
            RETURN => "return",
            _ => throw new System.ArgumentException()
        };

        /// <summary>
        /// return a string with all the values of the token
        /// </summary>
        public string ToPrettyString()
        {
            return $"type: {type}, token: {token}, offset: {offset}, length: {length}";
        }
    }
}
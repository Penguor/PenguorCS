

using System.Collections.Generic;
using System.Text;

using Penguor.Compiler.Build;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Debugging;

namespace Penguor.Compiler.Lexing
{
    /// <summary>
    /// This class contains everything to split Penguor files up into tokens
    /// </summary>
    public class Lexer
    {
        private readonly Builder builder; // the builder which is holding the Lexer

        private readonly string source; // the source code
        private List<Token> tokens; // the list containing the tokens from the source file

        private int current; // the location in the source code
        private int offset; // the offset where the token scanned atm began

        /// <summary>
        /// creates a new instance of the Lexer class
        /// </summary>
        /// <param name="builder">the Builder this class is executed from</param>
        public Lexer(Builder builder)
        {
            this.builder = builder;
            source = builder.Input;

            tokens = new List<Token>();
        }

        /// <summary>
        /// scans a text file and breaks it into tokens
        /// </summary>
        public ref List<Token> Tokenize()
        {
            current = 0; // start from the beginning of the file

            StringBuilder stringBuilder = new StringBuilder();
            while (!AtEnd())
            {
                stringBuilder.Clear();
                while (char.IsWhiteSpace(Peek()) && Peek() != '\n') Advance(); // skip leading whitespace

                // the offset is set to the offset in the source file at the beginning of the token
                offset = current;

                // lex keywords and identifiers
                if (char.IsLetter(Peek()) || Peek() == '_')
                {
                    stringBuilder.Append(Advance());
                    while (char.IsLetterOrDigit(Peek()) || Peek() == '_') stringBuilder.Append(Advance());
                    string idf = stringBuilder.ToString();

                    switch (idf)
                    {
                        case "asm":
                            AddToken(TokenType.ASM);
                            continue;
                        case "null":
                            AddToken(TokenType.NULL);
                            continue;
                        case "using":
                            AddToken(TokenType.USING);
                            continue;
                        case "system":
                            AddToken(TokenType.SYSTEM);
                            continue;
                        case "data":
                            AddToken(TokenType.DATA);
                            continue;
                        case "type":
                            AddToken(TokenType.TYPE);
                            continue;
                        case "library":
                            AddToken(TokenType.LIBRARY);
                            continue;
                        case "if":
                            AddToken(TokenType.IF);
                            continue;
                        case "elif":
                            AddToken(TokenType.ELIF);
                            continue;
                        case "else":
                            AddToken(TokenType.ELSE);
                            continue;
                        case "while":
                            AddToken(TokenType.WHILE);
                            continue;
                        case "for":
                            AddToken(TokenType.FOR);
                            continue;
                        case "do":
                            AddToken(TokenType.DO);
                            continue;
                        case "safety":
                            AddToken(TokenType.SAFETY);
                            continue;
                        case "public":
                            AddToken(TokenType.PUBLIC);
                            continue;
                        case "private":
                            AddToken(TokenType.PRIVATE);
                            continue;
                        case "protected":
                            AddToken(TokenType.PROTECTED);
                            continue;
                        case "restricted":
                            AddToken(TokenType.RESTRICTED);
                            continue;
                        case "static":
                            AddToken(TokenType.STATIC);
                            continue;
                        case "dynamic":
                            AddToken(TokenType.DYNAMIC);
                            continue;
                        case "abstract":
                            AddToken(TokenType.ABSTRACT);
                            continue;
                        case "const":
                            AddToken(TokenType.CONST);
                            continue;
                        case "true":
                            AddToken(TokenType.TRUE);
                            continue;
                        case "false":
                            AddToken(TokenType.FALSE);
                            continue;
                        case "switch":
                            AddToken(TokenType.SWITCH);
                            continue;
                        case "case":
                            AddToken(TokenType.CASE);
                            continue;
                        case "default":
                            AddToken(TokenType.DEFAULT);
                            continue;
                        case "return":
                            AddToken(TokenType.RETURN);
                            continue;
                        default:
                            // if none of the keywords is matched, the Token is an identifier
                            AddToken(TokenType.IDF, idf);
                            continue;
                    }
                }

                // lex numbers
                // TODO: add all types of numbers and notations Penguor supports
                if (char.IsDigit(Peek()) || (Peek() == '.' && char.IsDigit(PeekNext())))
                {
                    string numBase = "10";
                    bool hasBase = true;

                    while (char.IsDigit(Peek())) stringBuilder.Append(Advance());

                    if (stringBuilder.ToString() == "0" && "bohd".Contains(Peek()))
                    {
                        switch (Advance())
                        {
                            case 'b':
                                numBase = "2";
                                break;
                            case 'o':
                                numBase = "8";
                                break;
                            case 'h':
                                numBase = "16";
                                break;
                            case 'd':
                                numBase = "10";
                                break;
                            default:
                                throw new System.Exception();
                        }
                    }
                    else if (Peek() == 'x')
                    {
                        Advance();
                        numBase = stringBuilder.ToString();
                    }
                    else
                    {
                        hasBase = false;
                    }

                    if (hasBase)
                    {
                        AddToken(TokenType.NUM_BASE, numBase);
                        stringBuilder.Clear();
                    }
                    else
                    {
                        AddToken(TokenType.NUM_BASE, numBase);
                    }

                    while (char.IsLetterOrDigit(Peek())) stringBuilder.Append(Advance());
                    if (Peek() == '.')
                    {
                        stringBuilder.Append(Advance());
                        if (!char.IsLetterOrDigit(Peek())) throw new System.Exception();
                        while (char.IsLetterOrDigit(Peek())) stringBuilder.Append(Advance());
                    }

                    if (stringBuilder.Length == 0) throw new System.Exception();
                    AddToken(TokenType.NUM, stringBuilder.ToString().ToUpper());
                    continue;
                }

                // lex strings
                if (Match('"'))
                {
                    while (!Match('"')) stringBuilder.Append(Advance());
                    AddToken(TokenType.STRING, stringBuilder.ToString());
                    continue;
                }

                // lex all other tokens
                char c;
                if (!AtEnd()) c = Advance();
                else break;

                switch (c)
                {
                    case '+':
                        AddToken(Match('=') ? TokenType.ADD_ASSIGN : Match('+') ? TokenType.DPLUS : TokenType.PLUS);
                        break;
                    case '-':
                        AddToken(Match('=') ? TokenType.SUB_ASSIGN : Match('-') ? TokenType.DMINUS : TokenType.MINUS);
                        break;
                    case '*':
                        AddToken(Match('=') ? TokenType.MUL_ASSIGN : TokenType.MUL);
                        break;
                    case '/':
                        if (Match('/'))
                        {
                            while (!Match('\n') && current < source.Length) Advance();
                        }
                        else if (Match('*'))
                        {
                            int nested = 1;
                            while (nested > 0)
                            {
                                if (Match('/') && Match('*')) nested++;
                                else if (Match('*') && Match('/')) nested--;
                                else Advance();
                            }
                        }
                        else
                        {
                            AddToken(Match('=') ? TokenType.DIV_ASSIGN : TokenType.DIV);
                        }

                        break;
                    case '%':
                        AddToken(Match('=') ? TokenType.PERCENT_ASSIGN : TokenType.PERCENT);
                        break;
                    case '!':
                        AddToken(TokenType.EXCL_MARK);
                        break;
                    case '~':
                        AddToken(TokenType.BW_NOT);
                        break;
                    case '(':
                        AddToken(TokenType.LPAREN);
                        break;
                    case ')':
                        AddToken(TokenType.RPAREN);
                        break;
                    case '{':
                        AddToken(TokenType.LBRACE);
                        break;
                    case '}':
                        AddToken(TokenType.RBRACE);
                        break;
                    case '[':
                        while (char.IsWhiteSpace(Peek())) Advance();
                        AddToken(Match(']') ? TokenType.ARRAY : TokenType.LBRACK);
                        break;
                    case ']':
                        AddToken(TokenType.RBRACK);
                        break;
                    case '.':
                        AddToken(TokenType.DOT);
                        break;
                    case ',':
                        AddToken(TokenType.COMMA);
                        break;
                    case ':':
                        AddToken(TokenType.COLON);
                        break;
                    case ';':
                        AddToken(TokenType.SEMICOLON);
                        break;
                    case '\n':
                        AddToken(TokenType.ENDING);
                        break;
                    case '<':
                        AddToken(Match('=') ? TokenType.LESS_EQUALS : Match('<') ? (Match('=') ? TokenType.BS_LEFT_ASSIGN : TokenType.BS_LEFT) : TokenType.LESS);
                        break;
                    case '>':
                        AddToken(Match('=') ? TokenType.GREATER_EQUALS : Match('>') ? (Match('=') ? TokenType.BS_RIGHT_ASSIGN : TokenType.BS_RIGHT) : TokenType.GREATER);
                        break;
                    case '&':
                        AddToken(Match('=') ? TokenType.BW_AND_ASSIGN : Match('&') ? TokenType.AND : TokenType.BW_AND);
                        break;
                    case '|':
                        AddToken(Match('=') ? TokenType.BW_OR_ASSIGN : Match('|') ? TokenType.OR : TokenType.BW_OR);
                        break;
                    case '^':
                        AddToken(Match('=') ? TokenType.BW_XOR_ASSIGN : TokenType.BW_XOR);
                        break;
                    case '#':
                        AddToken(TokenType.HASHTAG);
                        break;
                    case '=':
                        AddToken(Match('=') ? TokenType.EQUALS : TokenType.ASSIGN);
                        break;
                    default:
                        Logger.Log(new Notification(builder.SourceFile, offset, 7, MsgType.PGR, c.ToString()));
                        break;
                }
            }
            // add an end of file token at the end
            // it will be used by the Parser to verify that all tokens have been parsed
            AddToken(TokenType.EOF);

            return ref tokens;
        }

        /// <summary>
        /// returns whether the end of the source file is reached
        /// </summary>
        private bool AtEnd() => current >= source.Length;

        /// <summary>
        /// advances the position by one
        /// </summary>
        /// <returns>the character at the previous position after advancing</returns>
        private char Advance()
        {
            current++;
            if (current > source.Length) Logger.Log(new Notification(builder.SourceFile, offset, 9, MsgType.PGR));
            return source[current - 1];
        }

        /// <summary>
        /// matches the current character against one of the expected characters
        /// </summary>
        /// <param name="expected">the character array which should be matched against</param>
        /// <returns>
        /// whether the expected character is equal to the current char in the source file
        /// and <c>false</c> when the end of the file is reached
        /// </returns>
        private bool Match(params char[] expected)
        {
            if (AtEnd()) return false;
            foreach (var i in expected)
                if (Peek() != i) return false;

            current++;
            return true;
        }

        /// <summary>
        /// returns the character at the current position without advancing
        /// </summary>
        private char Peek()
        {
            if (AtEnd()) return '\0';
            return source[current];
        }

        /// <summary>
        /// returns the next character
        /// </summary>
        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        // add tokens to the List
        private void AddToken(TokenType type) => AddToken(type, "");
        private void AddToken(TokenType type, string literal) =>
            tokens.Add(new Token(type, literal, offset, current - offset));
    }
}
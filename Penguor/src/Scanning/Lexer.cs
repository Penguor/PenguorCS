/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
*/

using System.Collections.Generic;
using System.Text;
using System.IO;
using Penguor.Debugging;
using Penguor.Parsing;

namespace Penguor.Lexing
{
    /// <summary>
    /// This class contains everything to scan Penguor files and return a token list
    /// </summary>
    public class Lexer
    {
        private List<Token> tokens = new List<Token>();

        string source;

        int current = 0;
        int offset = 0;


        /// <summary>
        /// scans a text file and breaks it into tokens
        /// </summary>
        /// <param name="filePath">the file to scan</param>
        /// <returns>a list of tokens</returns>
        public List<Token> Tokenize(string filePath)
        {
            Debug.Log("Starting Tokenize()", LogLevel.Info);
            current = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                source = reader.ReadToEnd();
                reader.Close();
            }

            StringBuilder stringBuilder = new StringBuilder();

            while (!AtEnd())
            {
                stringBuilder.Clear();
                while (char.IsWhiteSpace(Peek())) Advance();

                offset = current;

                if (char.IsLetter(Peek()) || Peek() == '_')
                {
                    stringBuilder.Append(Advance());
                    while (char.IsLetterOrDigit(Peek()) || Peek() == '_') stringBuilder.Append(Advance());
                    string idf = stringBuilder.ToString();
                    switch (idf)
                    {
                        case "null":
                            AddToken(TokenType.NULL);
                            break;
                        case "system":
                            AddToken(TokenType.SYSTEM);
                            break;
                        case "container":
                            AddToken(TokenType.CONTAINER);
                            break;
                        case "datatype":
                            AddToken(TokenType.DATATYPE);
                            break;
                        case "library":
                            AddToken(TokenType.LIBRARY);
                            break;
                        case "if":
                            AddToken(TokenType.IF);
                            break;
                        case "elif":
                            AddToken(TokenType.ELIF);
                            break;
                        case "else":
                            AddToken(TokenType.ELSE);
                            break;
                        case "while":
                            AddToken(TokenType.WHILE);
                            break;
                        case "for":
                            AddToken(TokenType.FOR);
                            break;
                        case "do":
                            AddToken(TokenType.DO);
                            break;
                        case "from":
                            AddToken(TokenType.FROM);
                            break;
                        case "include":
                            AddToken(TokenType.INCLUDE);
                            break;
                        case "safety":
                            AddToken(TokenType.SAFETY);
                            break;
                        case "public":
                            AddToken(TokenType.PUBLIC);
                            break;
                        case "private":
                            AddToken(TokenType.PRIVATE);
                            break;
                        case "protected":
                            AddToken(TokenType.PROTECTED);
                            break;
                        case "restricted":
                            AddToken(TokenType.RESTRICTED);
                            break;
                        case "static":
                            AddToken(TokenType.STATIC);
                            break;
                        case "dynamic":
                            AddToken(TokenType.DYNAMIC);
                            break;
                        case "abstract":
                            AddToken(TokenType.ABSTRACT);
                            break;
                        case "const":
                            AddToken(TokenType.CONST);
                            break;
                        case "true":
                            AddToken(TokenType.TRUE);
                            break;
                        case "false":
                            AddToken(TokenType.FALSE);
                            break;
                        case "switch":
                            AddToken(TokenType.SWITCH);
                            break;
                        case "case":
                            AddToken(TokenType.CASE);
                            break;
                        case "default":
                            AddToken(TokenType.DEFAULT);
                            break;
                        default:
                            AddToken(TokenType.IDF, idf);
                            break;
                    }
                    continue;
                }

                if (char.IsDigit(Peek()))
                {
                    stringBuilder.Append(Advance());
                    while (char.IsDigit(Peek())) stringBuilder.Append(Advance());
                    if (Peek() == '.') do stringBuilder.Append(Advance()); while (char.IsDigit(Peek()));
                    AddToken(TokenType.NUM, stringBuilder.ToString());
                    continue;
                }
                else if (Peek() == '.' && char.IsDigit(PeekNext()))
                {
                    stringBuilder.Append(Advance());
                    do stringBuilder.Append(Advance()); while (char.IsDigit(Peek()));
                    AddToken(TokenType.NUM, stringBuilder.ToString());
                    continue;
                }

                if (Match('"'))
                {
                    while (!Match('"')) stringBuilder.Append(Advance());
                    AddToken(TokenType.STRING, stringBuilder.ToString());
                    continue;
                }

                char c = '\0';
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
                            while (!(Match('\r') && Match('\n')) && !Match('\r') && !Match('\n')) Advance();
                        else if (Match('*'))
                            while (!(Match('*') && !Match('/'))) Advance();
                        else AddToken(Match('=') ? TokenType.DIV_ASSIGN : TokenType.DIV);
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
                        AddToken(TokenType.LBRACK);
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
                        AddToken(Match('=') ? TokenType.BW_XOR_ASSIGN : Match('^') ? TokenType.XOR : TokenType.BW_XOR);
                        break;
                    case '#':
                        AddToken(TokenType.HASHTAG);
                        break;
                    case '=':
                        AddToken(Match('=') ? TokenType.EQUALS : TokenType.ASSIGN);
                        break;
                    default:
                        Debug.Log($"unexpected character '{c}'", LogLevel.Error);
                        break;
                }
            }

            AddToken(TokenType.EOF);
            return tokens;
        }

        private bool AtEnd() => current >= source.Length;

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private bool Match(char expected)
        {
            if (AtEnd()) return false;
            if ((Peek() != expected)) return false;

            current++;
            return true;
        }

        private char Peek()
        {
            if (AtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private void AddToken(TokenType type) => AddToken(type, "");
        private void AddToken(TokenType type, string literal) => tokens.Add(new Token(type, literal, offset, current - offset));
    }
}
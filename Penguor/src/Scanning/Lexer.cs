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

        private string idf;

        /// <summary>
        /// scans a text file and breaks it into tokens
        /// </summary>
        /// <param name="filePath">the file to scan</param>
        /// <returns>a list of tokens</returns>
        public List<Token> Tokenize(string filePath)
        {
            Debug.Log("Starting Tokenize()", LogLevel.Info);

            StreamReader reader = new StreamReader(filePath);

            StringBuilder idfBuilder = new StringBuilder();
            StringBuilder numBuilder = new StringBuilder();
            int line = 1;

            while (!reader.EndOfStream)
            {
                char current = ' ';

                while (current == ' ')
                {
                    current = (char)reader.Read();
                }

                if (char.IsLetter(current))
                {
                    idfBuilder.Append(current);
                    while (char.IsLetterOrDigit((current = (char)reader.Read())))
                    {
                        idfBuilder.Append(current);
                    }

                    idf = idfBuilder.ToString();
                    idfBuilder.Clear();

                    switch (idf)
                    {
                        case "fn":
                            tokens.Add(new Token(TokenType.FN, "fn", line));
                            break;
                        case "null":
                            tokens.Add(new Token(TokenType.NULL, "null", line));
                            break;
                        case "system":
                            tokens.Add(new Token(TokenType.SYSTEM, "system", line));
                            break;
                        case "component":
                            tokens.Add(new Token(TokenType.COMPONENT, "component", line));
                            break;
                        case "datatype":
                            tokens.Add(new Token(TokenType.DATATYPE, "datatype", line));
                            break;
                        case "if":
                            tokens.Add(new Token(TokenType.IF, "if", line));
                            break;
                        case "while":
                            tokens.Add(new Token(TokenType.WHILE, "while", line));
                            break;
                        case "for":
                            tokens.Add(new Token(TokenType.FOR, "for", line));
                            break;
                        case "do":
                            tokens.Add(new Token(TokenType.DO, "do", line));
                            break;
                        case "from":
                            tokens.Add(new Token(TokenType.FROM, "from", line));
                            break;
                        case "include":
                            tokens.Add(new Token(TokenType.INCLUDE, "include", line));
                            break;
                        case "var":
                            tokens.Add(new Token(TokenType.VAR, "var", line));
                            break;
                        case "true":
                            tokens.Add(new Token(TokenType.TRUE, "true", line));
                            break;
                        case "false":
                            tokens.Add(new Token(TokenType.FALSE, "false", line));
                            break;
                        default:
                            tokens.Add(new Token(TokenType.IDF, idf, line));
                            break;
                    }
                }
                if (char.IsDigit(current))
                {
                    do
                    {
                        numBuilder.Append(current);
                        current = (char)reader.Read();
                    } while (char.IsDigit(current) || current == '.');
                    tokens.Add(new Token(TokenType.NUM, numBuilder.ToString(), line));
                    numBuilder.Clear();
                }
                if (current == '"')
                {
                    while ('"' != (current = (char)reader.Read()))
                    {
                        idfBuilder.Append(current);
                    }

                    tokens.Add(new Token(TokenType.STRING, idfBuilder.ToString(), line));
                    idfBuilder.Clear();
                }
                if (current == '\n')
                {
                    line++;
                }
                switch (current)
                {
                    case '/':
                        if ('/' == (current = (char)reader.Read()))
                        {
                            do
                            {
                                current = (char)reader.Read();
                            } while (current != '\n');
                            break;
                        }
                        else if ('*' == (current = (char)reader.Read()))
                        {
                            bool comment = true;
                            do
                            {
                                current = (char)reader.Read();
                                if ('*' == current)
                                {
                                    if ('/' == (current = (char)reader.Read())) comment = false;
                                }
                            } while (comment);
                        }
                        else tokens.Add(new Token(TokenType.DIV, "/", line));
                        break;
                    case '!':
                        if ('#' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.HEADSTART, "!#", line));
                        else if ('=' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.NEQUALS, "!=", line));
                        else tokens.Add(new Token(TokenType.NOT, "!", line));
                        break;
                    case '#':
                        if ('!' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.HEADEND, "#!", line));
                        break;
                    case '|':
                        if ('|' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.OR, "||", line));
                        else Debug.CastPGR(1, line);
                        break;
                    case '&':
                        if ('&' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.AND, "&&", line));
                        else Debug.CastPGR(1, line);
                        break;
                    case '=':
                        if ('=' == (current = (char)reader.Read())) tokens.Add(new Token(TokenType.EQUALS, "==", line));
                        else tokens.Add(new Token(TokenType.ASSIGN, "=", line));
                        break;
                    case '+':
                        tokens.Add(new Token(TokenType.PLUS, "+", line));
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.MINUS, "-", line));
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.MUL, "*", line));
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LPAREN, "(", line));
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RPAREN, ")", line));
                        break;
                    case '{':
                        tokens.Add(new Token(TokenType.LBRACE, "{", line));
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RBRACE, "}", line));
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LBRACK, "[", line));
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RBRACK, "]", line));
                        break;
                    case ':':
                        tokens.Add(new Token(TokenType.COLON, ":", line));
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.SEMICOLON, ";", line));
                        break;
                    case '<':
                        tokens.Add(new Token(TokenType.LESS, "<", line));
                        break;
                    case '.':
                        tokens.Add(new Token(TokenType.DOT, ".", line));
                        break;
                    default:
                        if (!char.IsWhiteSpace(current) && current != '"')
                            tokens.Add(new Token(TokenType.OTHER, current.ToString(), line));
                        break;
                }
            }
            tokens.Add(new Token(TokenType.EOF, "EOF", line));
            return tokens;
        }
    }
}
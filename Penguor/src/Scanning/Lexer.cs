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

            StringBuilder idfBuilder = new StringBuilder();
            StringBuilder numBuilder = new StringBuilder();

            while (!AtEnd())
            {
                idfBuilder.Clear();
                while (char.IsWhiteSpace(source[current])) Advance();

                offset = current;

                if (char.IsLetter(source[current]) || source[current] == '_')
                {
                    idfBuilder.Append(Advance());
                    while (char.IsLetterOrDigit(source[current]) || source[current] == '_') idfBuilder.Append(Advance());
                    string idf = idfBuilder.ToString();
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

                switch (source[current])
                {
                    case '+':
                        AddToken(Match('=') ? TokenType.ADD_ASSIGN : Match('+') ? TokenType.DPLUS : TokenType.PLUS);
                        break;
                }
            }

            AddToken(TokenType.EOF);
            return tokens;
        }

        private bool AtEnd() => current < source.Length;

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private bool Match(char expected)
        {
            if (AtEnd()) return false;
            if ((current != expected)) return false;

            Advance();
            return true;
        }

        private void AddToken(TokenType type) => AddToken(type, "");

        private void AddToken(TokenType type, string literal) => tokens.Add(new Token(type, literal, offset, current - offset));
    }
}
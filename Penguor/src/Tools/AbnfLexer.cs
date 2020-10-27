/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
#
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Penguor.Compiler.Tools.Lexer
{
    internal enum AbnfToken : byte
    {
        RULE,
        NEWLINE, NEWLINE_WITH_SPACE,

        LPAREN, RPAREN,
        LBRACK, RBRACK,
        LABRACK, RABRACK,

        ASSIGN, ALT_ASSIGN,
        ALTERNATIVE,

        STRING, STRICT_STRING,

    }

    internal class AbnfLexer
    {
        private int current; // the location in the source code
        private int offset; // the offset where the token scanned atm began

        private string source;
        readonly List<(AbnfToken, string, int)> tokens;

        public AbnfLexer(string file)
        {
            using StreamReader reader = new StreamReader(file);
            source = reader.ReadToEnd();

            tokens = new List<(AbnfToken, string, int)>();
        }

        public void Tokenize()
        {

            StringBuilder builder = new StringBuilder();

            while (!AtEnd())
            {
                builder.Clear();
                while (char.IsWhiteSpace(Peek()) && Peek() != '\n') Advance(); // skip leading whitespace

                // the offset is set to the offset in the source file at the beginning of the token
                offset = current;

                // lex rulenames
                if (char.IsLetter(Peek()))
                {
                    builder.Append(Advance());
                    while (char.IsLetterOrDigit(Peek()) || Peek() == '-') builder.Append(Advance());
                    string idf = builder.ToString().ToLower();
                    tokens.Add((AbnfToken.RULE, idf, offset));
                    continue;
                }

                switch (Advance())
                {
                    case '<':
                        builder.Append(Advance());
                        while (char.IsLetterOrDigit(Peek()) || Peek() == '-') builder.Append(Advance());
                        Match('>');
                        string idf = builder.ToString().ToLower();
                        tokens.Add((AbnfToken.RULE, idf, offset));
                        continue;
                    case '%':
                        switch (Advance())
                        {
                            case 'i':
                                LexString(false);
                                continue;
                            case 's':
                                LexString(true);
                                continue;
                            case 'b':
                                LexNumberChar(2);
                                continue;
                            case 'x':
                                LexNumberChar(16);
                                continue;
                            case 'd':
                                LexNumberChar(10);
                                continue;
                        }
                        continue;
                    case '"':
                        current--;
                        LexString(false);
                        continue;
                    case ';':
                        while (Peek() == '\n') Advance();
                        continue;
                    case '\n':
                        if (char.IsWhiteSpace(PeekNext())) tokens.Add((AbnfToken.NEWLINE_WITH_SPACE, "\n ", offset));
                        else tokens.Add((AbnfToken.NEWLINE, "\n", offset));
                        continue;
                    case '/':
                        if (Match('=')) tokens.Add((AbnfToken.ALT_ASSIGN, "/=", offset));
                        else tokens.Add((AbnfToken.ALTERNATIVE, "/", offset));
                        continue;
                    case '=':
                        tokens.Add((AbnfToken.ASSIGN, "=", offset));
                        continue;
                }
            }
        }

        private void LexNumberChar(int numBase)
        {
            StringBuilder numBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();

        loopStart:
            while (char.IsDigit(Peek())) numBuilder.Append(Advance());
            builder.Append(Convert.ToInt32(numBuilder.ToString(), numBase));
            while (char.IsWhiteSpace(Peek())) Advance();
            if (Match('.')) goto loopStart;

            tokens.Add((AbnfToken.STRICT_STRING, builder.ToString(), offset));
        }

        // lexes a string
        private void LexString(bool caseSensitive)
        {
            StringBuilder builder = new StringBuilder();
            Consume('"');
            while (!Match('"')) builder.Append(Advance());

            if (caseSensitive) tokens.Add((AbnfToken.STRICT_STRING, builder.ToString(), offset));
            else tokens.Add((AbnfToken.STRING, builder.ToString(), offset));
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
            if (current > source.Length) throw new System.Exception();
            return source[current - 1];
        }

        /// <summary>
        /// matches the current character against the expected character
        /// </summary>
        /// <param name="expected">the character which should be matched against</param>
        /// <returns>
        /// whether the expected character is equal to the current char in the source file
        /// and <c>false</c> when the end of the file is reached
        /// </returns>
        private bool Match(char expected)
        {
            if (AtEnd()) return false;
            if (Peek() != expected) return false;

            current++;
            return true;
        }

        private void Consume(char expected)
        {
            if (AtEnd()) throw new System.Exception();
            if (Peek() != expected) throw new System.Exception();

            current++;
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
    }
}
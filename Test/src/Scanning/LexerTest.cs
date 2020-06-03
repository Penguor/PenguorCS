/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using Penguor.Lexing;
using Penguor.Parsing;

namespace Penguor.Compiler.Tests
{
    public class LexerTest
    {
        [Fact]
        public void TestHelloWorld()
        {
            Lexer lexer = new Lexer(@"src\Files\HelloWorld\HelloWorld.pgr");
            List<Token> tokens = lexer.Tokenize();
            Assert.Equal(TokensToString(ref tokens), GetTokens(@"src\Files\HelloWorld\HelloWorld.pgr.lexout"));
        }

        [Fact]
        public void TestFibonacci()
        {
            Lexer lexer = new Lexer(@"src\Files\Fibonacci\Fibonacci.pgr");
            List<Token> tokens = lexer.Tokenize();
            Assert.Equal(TokensToString(ref tokens), GetTokens(@"src\Files\Fibonacci\Fibonacci.pgr.lexout"));
        }

        private List<string> GetTokens(string file)
        {
            using StreamReader reader = new StreamReader(file);
            List<String> tokens = new List<string>();
            while (!reader.EndOfStream) tokens.Add(reader.ReadLine());
            return tokens;
        }

        private void WriteTokens(ref List<Token> tokens, string folder, string file)
        {
            Directory.CreateDirectory(folder);
            using StreamWriter writer = new StreamWriter(Path.Combine(folder, file));
            foreach (var t in tokens) writer.WriteLine(t.ToString());
        }

        private List<String> TokensToString(ref List<Token> tokens)
        {
            List<String> stringTokens = new List<string>();
            foreach (var t in tokens) stringTokens.Add(t.ToString());
            return stringTokens;
        }
    }
}
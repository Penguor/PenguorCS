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

using Penguor.Compiler.Build;
using Penguor.Compiler.Parsing;

namespace Penguor.Compiler.Tests
{
    public class LexerTest
    {
        [Fact]
        public void TestHelloWorld()
        {
            Builder builder = new Builder(@"src/Files/HelloWorld/HelloWorld.pgr");
            List<Token> tokens = builder.Lex();
            Assert.Equal(TokensToString(ref tokens), GetTokens(@"src/Files/HelloWorld/HelloWorld.pgr.lexout"));
        }

        [Fact]
        public void TestFibonacci()
        {
            Builder builder = new Builder(@"src/Files/Fibonacci/Fibonacci.pgr");
            List<Token> tokens = builder.Lex();
            Assert.Equal(TokensToString(ref tokens), GetTokens(@"src/Files/Fibonacci/Fibonacci.pgr.lexout"));
        }

        private List<string> GetTokens(string file)
        {
            using StreamReader reader = new StreamReader(file);
            List<String> tokens = new List<string>();
            while (!reader.EndOfStream) tokens.Add(reader.ReadLine());
            return tokens;
        }

        private void WriteTokens(ref List<Token> tokens, string file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            using StreamWriter writer = new StreamWriter(file);
            foreach (var t in tokens) writer.WriteLine(t.ToString());
        }

        private List<string> TokensToString(ref List<Token> tokens)
        {
            List<string> stringTokens = new List<string>();
            foreach (var t in tokens) stringTokens.Add(t.ToString());
            return stringTokens;
        }
    }
}
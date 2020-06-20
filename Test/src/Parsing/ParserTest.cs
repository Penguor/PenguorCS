/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Xunit;

using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Build;
using System.Text;
using System.Xml;

namespace Penguor.Compiler.Tests
{

    public class ParserTest
    {
        [Fact]
        public void TestHelloWorld()
        {
            Builder builder = new Builder(@"src\Files\HelloWorld\HelloWorld.pgr");
            Decl program = builder.Parse();
            WriteDecl(ref program, @"src\Files\HelloWorld\HelloWorld.pgr.parseout");
        }

        [Fact]
        public void TestFibonacci()
        {
            Builder builder = new Builder(@"src\Files\Fibonacci\Fibonacci.pgr");
            Decl program = builder.Parse();
        }

        public Decl GetDecl(string file)
        {
            // XmlSerializer serializer = new XmlSerializer(typeof(Decl));
            // using Stream stream = new FileStream(file, FileMode.Open);
            // return (Decl) serializer.Deserialize(stream);
            throw new System.NotImplementedException();
        }

        public void WriteDecl(ref Decl decl, string file)
        {
            // XmlSerializer serializer = new XmlSerializer(typeof(Decl));
            // using Stream stream = new FileStream(file, FileMode.Create);
            // serializer.Serialize(stream, decl);
        }
    }
}
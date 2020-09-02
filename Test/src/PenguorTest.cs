/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using Xunit;

using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Build;

namespace Penguor.Compiler.Tests
{
    public class PenguorTest
    {
        [Fact]
        public void TestHelloWorld()
        {
            SymbolTableManager manager = new SymbolTableManager();
            Builder builder = new Builder(ref manager, "src/Files/HelloWorld/HelloWorld.pgr");
            builder.Build();
        }

        [Fact]
        public void TestFibonacci()
        {
            SymbolTableManager manager = new SymbolTableManager();
            Builder builder = new Builder(ref manager, "src/Files/Fibonacci/Fibonacci.pgr");
            Assert.Equal(0, builder.Build());
        }
    }
}
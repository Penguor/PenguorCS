

using Penguor.Compiler.Build;
using Penguor.Compiler.Parsing.AST;
using Xunit;

namespace Penguor.Compiler.Tests
{
    public class PenguorTest
    {
        [Fact]
        public void TestHelloWorld()
        {
            BuildManager.TableManager = new SymbolTableManager();
            BuildManager.SmartBuild("Files/HelloWorld/HelloWorld.pgr", null, true);
        }

        [Fact]
        public void TestFizzBuzz()
        {
            BuildManager.TableManager = new SymbolTableManager();
            BuildManager.SmartBuild("Files/FizzBuzz/FizzBuzz.pgr", null, true);
        }

        // [Fact]
        public void TestFibonacci()
        {
            BuildManager.TableManager = new SymbolTableManager();
            BuildManager.SmartBuild("Files/Fibonacci/Fibonacci.pgr", null, false);
        }
    }
}
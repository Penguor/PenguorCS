using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Penguor.Compiler.Build;
using Xunit;
using Xunit.Abstractions;

namespace Penguor.Compiler.Tests
{
    public class PenguorTest
    {
        private readonly StringWriter output;
        public PenguorTest()
        {
            output = new StringWriter();
            Console.SetOut(output);
        }

        [Theory,
        MemberData(nameof(GetFiles))]
        public void TestFiles(string directory)
        {
            BuildManager.TableManager = new SymbolTableManager();
            BuildManager.SmartBuild(Path.Combine(directory, "Program.pgr"), null, true);
            Assert.Equal(File.ReadAllText(Path.Combine(directory, "out")), output.ToString().Replace("\r", "").Trim());
        }

        public static IEnumerable<object[]> GetFiles()
        {
            return Directory.GetDirectories("Files").Select(s => new object[] { s });
        }
    }
}
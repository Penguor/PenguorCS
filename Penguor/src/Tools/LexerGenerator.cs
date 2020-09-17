/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
#
*/

using System.IO;

namespace Penguor.Compiler.Tools
{
    internal class LexerGenerator
    {
        private StreamReader? reader;

        public void Generate(string file)
        {
            reader = new StreamReader(file);
        }
    }
}
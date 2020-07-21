/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
#
#
*/

using Penguor.Compiler.Parsing;

namespace Penguor.Compiler.IR
{
    public class IRBlock : IRDecl
    {
        public IRBlock(State name) : base(name)
        {
        }
    }
}
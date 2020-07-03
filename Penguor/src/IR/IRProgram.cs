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

using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.IR
{
    public class IRProgram : IRStruct
    {
        public ProgramDecl Program { get; }

        public IRProgram(ProgramDecl decl) : base(new State(new string[0]))
        {
            Program = decl;
        }
    }
}
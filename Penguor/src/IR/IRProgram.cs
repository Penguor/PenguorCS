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
    public class IRProgram : IRDecl
    {

        public IRProgram() : base(new State(new AddressFrame[0]))
        {
        }
    }
}
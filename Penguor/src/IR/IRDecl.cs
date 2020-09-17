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

using Penguor.Compiler.Parsing;

namespace Penguor.Compiler.IR
{
    public class IRDecl : IRStruct
    {
        public List<IRDecl> Structures { get; set; }

        public State Name { get; }

        public IRDecl(State name)
        {
            Name = name;
            Structures = new List<IRDecl>();
        }

        public IRDecl ValidateCall()
        {
            throw new NotImplementedException();
        }
    }
}
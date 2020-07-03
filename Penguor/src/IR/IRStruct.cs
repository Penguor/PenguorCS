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
    public class IRStruct
    {
        public List<IRStruct> Structures { get; set; }

        public State Name { get; }

        public IRStruct(State name)
        {
            Name = name;
            Structures = new List<IRStruct>();
        }

        public IRStruct ValidateCall()
        {
            throw new NotImplementedException();
        }
    }
}
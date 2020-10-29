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

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// A statement modifying some data
    /// </summary>
    public record IRStatement
    {
        public OPCode Code { get; init; }

        public string[] Operands { get; init; }

        public IRStatement(OPCode code, string[] operands)
        {
            Code = code;
            Operands = operands;
        }
    }
}
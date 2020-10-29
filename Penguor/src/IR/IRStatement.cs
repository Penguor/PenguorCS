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
        /// <summary>
        /// the OPCode of this statement
        /// </summary>
        public OPCode Code { get; init; }

        /// <summary>
        /// the operands which get passed to the OPCode
        /// </summary>
        public string[] Operands { get; init; }

        /// <summary>
        /// Initializes a new instance of the IRStatement
        /// </summary>
        /// <param name="code">the OPCode of this IRStatement</param>
        /// <param name="operands">the operands of this IRStatement</param>
        public IRStatement(OPCode code, string[] operands)
        {
            Code = code;
            Operands = operands;
        }
    }
}
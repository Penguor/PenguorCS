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

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// the base of a program
    /// </summary>
    public record IRProgram : IRStruct
    {
        /// <summary>
        /// Initialize a new instance of IRProgram
        /// </summary>
        public IRProgram() : base(new State(Array.Empty<AddressFrame>())) { }
    }
}
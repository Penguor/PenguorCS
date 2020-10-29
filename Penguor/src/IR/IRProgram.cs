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

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// the base of a program
    /// </summary>
    public record IRProgram : IRStruct
    {
        /// <summary>
        /// The child declarations of the program
        /// </summary>
        public List<IRStruct> Children { get; init; }

        /// <summary>
        /// Initialize a new instance of IRProgram
        /// </summary>
        public IRProgram() : base(new State(Array.Empty<AddressFrame>()))
        {
            Children = new();
        }
    }
}
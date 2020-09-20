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
    /// base class for ir structs
    /// </summary>
    public abstract record IRStruct
    {
        /// <summary>
        /// Contains the children of the struct
        /// </summary>
        public List<IRStruct> Children { get; }

        /// <summary>
        /// the statements to execute when the struct gets called
        /// </summary>
        public List<IRStatement> Statements { get; }

        /// <summary>
        /// the state of the IRStruct, used to identify it
        /// </summary>
        public State State { get; init; }

        /// <summary>
        /// Initializes a new instance of the <c>State</c> class
        /// </summary>
        /// <param name="state">the state of this instance</param>
        public IRStruct(State state)
        {
            Children = new();
            Statements = new();

            State = state;
        }
    }
}
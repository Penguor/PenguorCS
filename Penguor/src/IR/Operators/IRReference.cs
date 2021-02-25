using System;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRReference : IRArgument
    {
        public uint Referenced { get; init; }

        public IRReference(uint referenced)
        {
            Referenced = referenced;
        }

        /// <inheritdoc/>
        public override string ToString() => $"({Referenced})";
    }
}
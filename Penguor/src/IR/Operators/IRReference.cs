using System;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRReference : IRArgument
    {
        public int Referenced { get; init; }

        public IRReference(int referenced)
        {
            Referenced = referenced;
        }

        /// <inheritdoc/>
        public override string ToString() => $"({Referenced})";
    }
}
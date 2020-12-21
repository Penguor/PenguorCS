namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record Reference : IRArgument
    {
        public uint Referenced { get; }

        public Reference(uint referenced)
        {
            Referenced = referenced;
        }

        /// <inheritdoc/>
        public override string ToString() => $"({Referenced})";
    }
}
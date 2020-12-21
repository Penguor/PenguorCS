namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record Null : IRArgument
    {
        /// <inheritdoc/>
        public override string ToString() => "$null";
    }
}
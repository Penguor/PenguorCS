namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRNull : IRArgument
    {
        /// <inheritdoc/>
        public override string ToString() => "$null";
    }
}
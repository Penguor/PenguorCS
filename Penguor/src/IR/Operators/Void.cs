namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record Void : IRArgument
    {
        /// <inheritdoc/>
        public override string ToString() => "$void";
    }
}
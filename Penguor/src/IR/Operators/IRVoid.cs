namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRVoid : IRArgument
    {
        /// <inheritdoc/>
        public override string ToString() => "$void";
    }
}
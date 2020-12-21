

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public abstract record IRArgument
    {
        /// <inheritdoc/>
        public abstract override string ToString();
    }
}
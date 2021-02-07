namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRChar : IRArgument
    {
        /// <summary>
        /// the value of the argument
        /// </summary>
        public char Value { get; }

        /// <summary>
        /// Initialize a new Instance of this IRArgument with the given value
        /// </summary>
        /// <param name="value">the value of this argument</param>
        public IRChar(char value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
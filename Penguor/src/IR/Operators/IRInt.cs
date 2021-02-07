namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRInt : IRArgument
    {
        /// <summary>
        /// the value of the argument
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initialize a new Instance of this IRArgument with the given value
        /// </summary>
        /// <param name="value">the value of this argument</param>
        public IRInt(int value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
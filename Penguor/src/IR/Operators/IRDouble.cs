namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRDouble : IRArgument
    {
        /// <summary>
        /// the value of the argument
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initialize a new Instance of this IRArgument with the given value
        /// </summary>
        /// <param name="value">the value of this argument</param>
        public IRDouble(double value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRByte : IRArgument
    {
        /// <summary>
        /// the value of the argument
        /// </summary>
        public byte Value { get; }

        /// <summary>
        /// Initialize a new Instance of this IRArgument with the given value
        /// </summary>
        /// <param name="value">the value of this argument</param>
        public IRByte(byte value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
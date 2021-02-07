namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record String : IRArgument
    {
        /// <summary>
        /// the value of the argument
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initialize a new Instance of this IRArgument with the given value
        /// </summary>
        /// <param name="value">the value of this argument</param>
        public String(string value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override string ToString() => '"' + Value + '"';
    }
}
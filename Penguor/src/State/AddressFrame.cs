namespace Penguor.Compiler
{
    /// <summary>
    /// represents a part of an address in a Penguor program
    /// </summary>
    public record AddressFrame
    {
        /// <summary>
        /// the name of the object the AddressFrame is pointing to
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The type of address
        /// </summary>
        public AddressType Type { get; init; }

        /// <summary>
        /// create a new instance of AddressFrame
        /// </summary>
        public AddressFrame(string symbol, AddressType type)
        {
            Symbol = symbol;
            Type = type;
        }

        /// <summary>
        /// returns <c>Symbol</c>
        /// </summary>
        public override string ToString() => Symbol;
    }
}
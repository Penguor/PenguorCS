using Penguor.Compiler.Parsing;

namespace Penguor.Compiler
{
    /// <summary>
    /// Symbols contain the data in the symbol tables
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// the name of the object the AddressFrame is pointing to
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// The type of address
        /// </summary>
        public AddressType AdType { get; init; }
        /// <summary>
        /// The data type this symbol is/returns
        /// </summary>
        public State? DataType { get; set; }
        /// <summary>
        /// the visibility of the symbol
        /// </summary>
        public TokenType? AccessMod { get; set; }
        /// <summary>
        /// the non-accessmods, e.g. static
        /// </summary>
        public TokenType[]? NonAccessMods { get; set; }

        /// <summary>
        /// if the symbol is a declaration, this represents the parent declaration it inherits from
        /// </summary>
        public State? Parent { get; set; }
        /// <summary>
        /// Initialize a new Instance of Symbol with the given values
        /// </summary>
        /// <param name="name">the name of the symbol</param>
        /// <param name="type">the type of address</param>
        public Symbol(string name, AddressType type)
        {
            Name = name;
            AdType = type;
        }

        /// <summary>
        /// Initialize a new Instance of Symbol with the given values
        /// </summary>
        /// <param name="name">the name of the symbol</param>
        /// <param name="type">the type of address</param>
        /// <param name="dataType">the penguor data type of the symbol</param>
        public Symbol(string name, AddressType type, State dataType)
        {
            Name = name;
            AdType = type;
            DataType = dataType;
        }
    }
}
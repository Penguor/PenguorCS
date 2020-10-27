namespace Penguor.Compiler
{
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

        public State? DataType { get; set; }

        public Symbol(string name, AddressType type)
        {
            Name = name;
            AdType = type;
        }

        public Symbol(string name, AddressType type, State dataType)
        {
            Name = name;
            AdType = type;
            DataType = dataType;
        }
    }
}
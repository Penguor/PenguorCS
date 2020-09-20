/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

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
        /// the data type of the object
        /// the AddressFrame is pointing to
        /// </summary>
        public State? DataType { get; init; }

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
            DataType = null;
            Type = type;
        }

        /// <summary>
        /// create a new instance of AddressFrame
        /// </summary>
        public AddressFrame(string symbol, AddressType type, State dataType)
        {
            Symbol = symbol;
            DataType = dataType;
            Type = type;
        }


        /// <summary>
        /// returns <c>Symbol</c>
        /// </summary>
        public override string ToString() => Symbol;
    }
}
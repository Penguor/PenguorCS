/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Penguor.Compiler.Parsing;

namespace Penguor.Compiler
{
    /// <summary>
    /// represents a part of an address in a Penguor program
    /// </summary>
    public struct AddressFrame
    {
        /// <summary>
        /// the name of the object the AddressFrame is pointing to
        /// </summary>
        public Token Symbol { get; set; }

        /// <summary>
        /// the data type of the object
        /// the AddressFrame is pointing to
        /// </summary>
        public State? DataType { get; set; }

        /// <summary>
        /// The type of address
        /// </summary>
        public AddressType Type { get; }

        /// <summary>
        /// should be set to true if the AddressFrame is the last item in a state
        /// </summary>
        public bool IsLastItem { get; set; }

        /// <summary>
        /// lists all the children
        /// </summary>
        public Dictionary<string, AddressFrame> Children { get; }

        /// <summary>
        /// create a new instance of AddressFrame
        /// </summary>
        public AddressFrame(Token symbol, AddressType type, bool isLastItem = false)
        {
            Symbol = symbol;
            DataType = null;
            Type = type;
            IsLastItem = isLastItem;
            Children = new Dictionary<string, AddressFrame>();
        }

        /// <summary>
        /// create a new instance of AddressFrame
        /// </summary>
        public AddressFrame(Token symbol, AddressType type, State dataType, bool isLastItem = false)
        {
            Symbol = symbol;
            DataType = dataType;
            Type = type;
            IsLastItem = isLastItem;
            Children = new Dictionary<string, AddressFrame>();
        }


        /// <summary>
        /// returns <c>Symbol</c>
        /// </summary>
        public override string ToString() => Symbol.token ?? "";
    }
}
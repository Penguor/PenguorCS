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
    /// represents a Symbol in a Penguor program
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// the name of the symbol, must be unique inside of a scope
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the type of the symbol
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// initializes a new Instance of the <c>Symbol</c> class with the given values
        /// </summary>
        /// <param name="name">the name of the symbol</param>
        public Symbol(string name)
        {
            Name = name;
        }
    }
}
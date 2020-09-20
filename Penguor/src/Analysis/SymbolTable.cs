/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System;
using System.Collections.Generic;

using Penguor.Compiler.Debugging;

namespace Penguor.Compiler
{
    /// <summary>
    /// a table containing the symbols for a specified scope in the penguor source code
    /// </summary>
    public class SymbolTable
    {
        /// <summary>
        /// the depth of the symbol table
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// the dictionary containing the symbols
        /// </summary>
        public Dictionary<string, Symbol> Symbols { get; init; }

        /// <summary>
        /// initializes a new instance of the SymbolTable class
        /// </summary>
        /// <param name="level">the level of the SymbolTable, equivalent to the number of scope address frames</param>
        public SymbolTable(int level)
        {
            Level = level;

            Symbols = new Dictionary<string, Symbol>();
        }

        /// <summary>
        /// insert a new Symbol into the symbol table
        /// </summary>
        /// <param name="symbol">the symbol to insert into the table</param>
        public void Insert(Symbol symbol)
        {
            bool succeeded = Symbols.TryAdd(symbol.Name, symbol);
            if (!succeeded) throw new PenguorCSException(1);
        }

        /// <summary>
        /// look up a value in the symbol table
        /// </summary>
        /// <param name="name">the identifier to search for</param>
        /// <param name="symbol">the variable into which the result is copied to</param>
        public void Lookup(string name, out Symbol symbol)
        {
            Symbols.TryGetValue(name, out Symbol? outSymbol);

            symbol = outSymbol ?? throw new PenguorCSException(1);
        }
    }
}
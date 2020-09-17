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

namespace Penguor.Compiler
{
    /// <summary>
    /// Manages all the symbol tables during a build, 
    /// ensuring that analysis is only started once all symbol tables are available
    /// </summary>
    public class SymbolTableManager
    {
        private readonly Dictionary<State, SymbolTable> tables;

        /// <summary>
        /// initialize a new Instance of the SymbolTableManager class
        /// </summary>
        public SymbolTableManager()
        {
            tables = new Dictionary<State, SymbolTable>();
        }

        /// <summary>
        /// add a symbol to a table
        /// </summary>
        /// <param name="scope">the scope where the Symbol occurs</param>
        /// <param name="symbol">the Symbol to add</param>
        public bool AddSymbol(State scope, Symbol symbol)
        {
            if (tables.ContainsKey(scope))
            {
                tables[scope].Insert(symbol);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create a new SymbolTable for the specified scope
        /// </summary>
        /// <param name="scope">the scope the SymbolTable will be used for</param>
        public void AddTable(State scope)
        {
            tables.Add(scope, new SymbolTable(scope.Count));
        }


        /// <summary>
        /// returns the AddressFrame described by the string a
        /// </summary>
        public SymbolTable this[State a]
        {
            get => tables[a];
            set => tables[a] = value;
        }
    }
}
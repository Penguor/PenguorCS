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
        /// returns the Symbol if it exists, otherwise throws an exception
        /// </summary>
        /// <param name="scope">the scope in which the symbol exists</param>
        /// <param name="symbol">the symbol to look for</param>
        public Symbol LookupSymbolInScope(State scope, Symbol symbol)
        {
            bool exists = tables.ContainsKey(scope);
            if (exists)
            {
                exists = tables[scope].Lookup(symbol.Name, out Symbol? outSym);
                return exists ? outSym! : throw new System.Exception();
            }
            else throw new System.Exception();
        }

        /// <summary>
        /// Looks up a Symbol
        /// </summary>
        /// <param name="scope">the scope in which the symbol exists</param>
        /// <param name="symbol">the symbol to look for</param>
        /// <param name="outSymbol">the Symbol to put the output to</param>
        /// <returns></returns>
        public bool TryLookupSymbolInScope(State scope, Symbol symbol, out Symbol? outSymbol)
        {
            bool exists = tables.ContainsKey(scope);
            if (exists)
            {
                exists = tables[scope].Lookup(symbol.Name, out outSymbol);
                if (!exists) return false;
                return true;
            }
            outSymbol = null;
            return false;
        }

        public bool FindSymbol(Symbol symbol, State scope)
        {
            for (int i = scope.Count - 1; i >= 0; i--)
            {
                bool found = TryLookupSymbolInScope(scope, symbol, out _);
                if (found) return true;
                else if (scope[i].Type == AddressType.LibraryDecl) return false;
                else scope.Pop();
            }
            throw new System.Exception();
        }

        public bool FindSymbol(Symbol symbol, params State[] scopes)
        {
            bool found;
            foreach (var i in scopes)
            {
                found = FindSymbol(symbol, i);
                if (found) return true;
            }
            return false;
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
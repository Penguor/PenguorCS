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
using Penguor.Compiler.Parsing;

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
        public bool AddSymbol(State scope, AddressFrame symbol)
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
        public Symbol LookupSymbolInScope(State scope, AddressFrame symbol)
        {
            bool exists = tables.ContainsKey(scope);
            if (exists)
            {
                exists = tables[scope].Lookup(symbol.Symbol, out Symbol? outSym);
                return exists ? (outSym ?? throw new Exception()) : throw new System.Exception();
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Looks up a Symbol
        /// </summary>
        /// <param name="scope">the scope in which the symbol exists</param>
        /// <param name="symbol">the symbol to look for</param>
        /// <param name="outSymbol">the Symbol to put the output to</param>
        /// <returns></returns>
        public bool TryLookupSymbolInScope(State scope, AddressFrame symbol, out Symbol? outSymbol)
        {
            bool exists = tables.ContainsKey(scope);
            if (exists)
                return tables[scope].Lookup(symbol.Symbol, out outSymbol);
            outSymbol = null;
            return false;
        }

        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        public Symbol? GetSymbol(AddressFrame symbol, State scope)
        {
            if (scope.Count == 0)
            {
                TryLookupSymbolInScope(scope, symbol, out Symbol? outSym);
                return outSym;
            }
            for (int i = scope.Count - 1; i >= 0; i--)
            {
                TryLookupSymbolInScope(scope, symbol, out Symbol? outSym);
                if (outSym != null) return outSym;
                else if (scope[i].Type == AddressType.LibraryDecl) return null;
                else scope.Pop();
            }
            return null;
        }

        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        public Symbol? GetSymbol(AddressFrame symbol, State[] scopes)
        {
            Symbol? outSym;
            foreach (var i in scopes)
                outSym = GetSymbol(symbol, i);
            return null;
        }

        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        public Symbol? GetSymbol(State symbol, State scope) => GetSymbol(symbol.Pop(), scope + symbol);
        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        public Symbol? GetSymbol(State symbol, State[] scopes)
        {
            var frame = symbol.Pop();
            var array = (State[])scopes.Clone();

            var allScopes = new State[array.Length + 1];
            allScopes[0] = symbol;
            array.CopyTo(allScopes, 1);

            return GetSymbol(frame, allScopes);
        }

        /// <summary>
        /// Search for a symbol
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        /// <returns>true if the Symbol was found, otherwise false</returns>
        public bool FindSymbol(AddressFrame symbol, State scope)
        {
            if (scope.Count == 0) return TryLookupSymbolInScope(scope, symbol, out _);
            for (int i = scope.Count - 1; i >= 0; i--)
            {
                bool found = TryLookupSymbolInScope(scope, symbol, out _);
                if (found) return true;
                else if (scope[i].Type == AddressType.LibraryDecl) return false;
                else scope.Pop();
            }
            return false;
        }

        /// <summary>
        /// Search for a symbol
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        /// <returns>true if the Symbol was found, otherwise false</returns>
        public bool FindSymbol(AddressFrame symbol, State[] scopes)
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
        /// Search for a symbol
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        /// <returns>true if the Symbol was found, otherwise false</returns>
        public bool FindSymbol(State symbol, State scope) => FindSymbol(symbol.Pop(), scope + symbol);
        /// <summary>
        /// Search for a symbol
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        /// <returns>true if the Symbol was found, otherwise false</returns>
        public bool FindSymbol(State symbol, State[] scopes)
        {
            var frame = symbol.Pop();
            var array = (State[])scopes.Clone();

            var allScopes = new State[array.Length + 1];
            allScopes[0] = symbol;
            array.CopyTo(allScopes, 1);

            return FindSymbol(frame, allScopes);
        }
        /// <summary>
        /// Search for a SymbolTable
        /// </summary>
        /// <param name="scope">the State corresponding to the SymbolTable</param>
        /// <returns>true if the table exists, otherwise false</returns>
        public bool FindTable(State scope) => tables.ContainsKey(scope);

        /// <summary>
        /// Create a new SymbolTable for the specified scope
        /// </summary>
        /// <param name="scope">the scope the SymbolTable will be used for</param>
        public void AddTable(State scope)
        {
            if (tables.ContainsKey(scope)) return;
            AddressFrame[] frames = new AddressFrame[scope.Count];
            scope.CopyTo(frames, 0);
            State newState = new State(frames);

            tables.Add(newState, new SymbolTable(scope.Count));
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

using System;
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
        /// add a new symbol equivalent to state
        /// </summary>
        /// <param name="state">the address of the symbol</param>
        /// <returns>returns true if the insert was successful, otherwise false</returns>
        public bool AddSymbol(State state)
        {
            return AddSymbol(new State(((State)state.Clone())[..^1]), ((State)state.Clone())[^1]);
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
                return exists ? (outSym ?? throw new Exception()) : throw new Exception();
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
        public bool TryLookupSymbolInScope(State scope, AddressFrame symbol, out Symbol? outSymbol)
        {
            bool exists = tables.ContainsKey(scope);
            if (exists)
                return tables[scope].Lookup(symbol.Symbol, out outSymbol);
            outSymbol = null;
            return false;
        }

        /// <summary>
        /// looks up the symbol of a scope
        /// </summary>
        /// <param name="symbol">the scope whose symbol to get</param>
        /// <returns>the symbol of the scope</returns>
        public Symbol GetSymbol(State symbol)
        {
            var localSymbol = (State)symbol.Clone();
            var frame = localSymbol.Pop();
            tables[localSymbol].Lookup(frame.Symbol, out Symbol? returned);
            return returned ?? throw new Exception();
        }

        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        public Symbol? GetSymbol(AddressFrame symbol, State scope)
        {
            var localScope = (State)scope.Clone();
            if (localScope.Count == 0)
            {
                TryLookupSymbolInScope(localScope, symbol, out Symbol? outSym);
                return outSym;
            }
            for (int i = localScope.Count - 1; i >= 0; i--)
            {
                TryLookupSymbolInScope(localScope, symbol, out Symbol? outSym);
                if (outSym != null) return outSym;
                else if (localScope[i].Type == AddressType.LibraryDecl) return null;
                else localScope.Pop();
            }
            return null;
        }

        /// <summary>
        /// Looks up a symbol and returns it
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        public Symbol? GetSymbol(AddressFrame symbol, IEnumerable<State> scopes)
        {
            Symbol? outSym = null;
            foreach (var i in scopes)
            {
                outSym = GetSymbol(symbol, i);
                if (outSym != null) return outSym;
            }
            return outSym;
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
            var symbolArray = (State)symbol.Clone();
            var frame = symbolArray.Pop();
            var array = (State[])scopes.Clone();

            var allScopes = new State[array.Length + 1];
            allScopes[0] = symbolArray;
            array.CopyTo(allScopes, 1);

            return GetSymbol(frame, allScopes);
        }

        /// <summary>
        /// Looks up a symbol and returns the full state
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        public State? GetStateBySymbol(AddressFrame symbol, State scope)
        {
            State localScope = (State)scope.Clone();
            if (localScope.Count == 0)
            {
                TryLookupSymbolInScope(localScope, symbol, out Symbol? outSym);
                if (outSym != null) return localScope + new AddressFrame(outSym.Name, outSym.AdType);
                else return null;
            }
            for (int i = localScope.Count - 1; i >= 0; i--)
            {
                TryLookupSymbolInScope(localScope, symbol, out Symbol? outSym);
                if (outSym != null) return localScope + new AddressFrame(outSym.Name, outSym.AdType);
                else if (localScope[i].Type == AddressType.LibraryDecl) return null;
                else localScope.Pop();
            }
            return null;
        }

        /// <summary>
        /// Looks up a symbol and returns the full state
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        public State? GetStateBySymbol(AddressFrame symbol, IEnumerable<State> scopes)
        {
            State? outSym = null;
            foreach (var i in scopes)
            {
                outSym = GetStateBySymbol(symbol, i);
                if (outSym != null) break;
            }
            return outSym;
        }

        /// <summary>
        /// Looks up a symbol and returns the full state
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        public State? GetStateBySymbol(State symbol, State scope)
        {
            var symbolArray = (State)symbol.Clone();
            return GetStateBySymbol(symbolArray.Pop(), scope + symbol);
        }

        /// <summary>
        /// Looks up a symbol and returns the full state
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scopes">the scopes to search in</param>
        public State? GetStateBySymbol(State symbol, State[] scopes)
        {
            var symbolArray = (State)symbol.Clone();
            var frame = symbolArray.Pop();
            var array = (State[])scopes.Clone();

            var allScopes = new State[array.Length + 1];
            allScopes[0] = symbolArray;
            array.CopyTo(allScopes, 1);

            return GetStateBySymbol(frame, allScopes);
        }

        /// <summary>
        /// Search for a symbol
        /// </summary>
        /// <param name="symbol">the symbol to search for</param>
        /// <param name="scope">the scope to search in</param>
        /// <returns>true if the Symbol was found, otherwise false</returns>
        public bool FindSymbol(AddressFrame symbol, State scope)
        {
            State clonedScope = (State)scope.Clone();
            if (clonedScope.Count == 0) return TryLookupSymbolInScope(clonedScope, symbol, out _);
            for (int i = clonedScope.Count - 1; i >= 0; i--)
            {
                bool found = TryLookupSymbolInScope(clonedScope, symbol, out _);
                if (found) return true;
                else if (clonedScope[i].Type == AddressType.LibraryDecl) return false;
                else clonedScope.Pop();
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
            var symbolArray = (State)symbol.Clone();
            var frame = symbolArray.Pop();

            var array = (State[])scopes.Clone();

            var allScopes = new State[array.Length + 1];
            allScopes[0] = symbolArray;
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

            if (scope.Count == 0)
                return;
            var symbolState = (State)newState.Clone();
            var symbol = symbolState.Pop();
            tables[symbolState].Insert(symbol);
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
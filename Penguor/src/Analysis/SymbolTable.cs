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
    public class SymbolTable
    {
        public int Level { get; set; }
        // public State Scope { get; set; }

        public Dictionary<string, Symbol> symbols { get; }

        public SymbolTable(int level)
        {
            Level = level;
            // Scope = scope;

            symbols = new Dictionary<string, Symbol>();
        }

        public void Insert(Symbol symbol)
        {
            bool succeeded = symbols.TryAdd(symbol.Name!, symbol);
            if (!succeeded) throw new PenguorCSException(1);
        }

        public void Lookup(string name, out Symbol symbol)
        {
            symbols.TryGetValue(name, out Symbol? outSymbol);

            symbol = outSymbol ?? throw new PenguorCSException(1);
        }
    }
}
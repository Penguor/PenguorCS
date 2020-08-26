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
using System.Collections;

using Penguor.Compiler.Parsing;
using Penguor.Compiler.Debugging;

namespace Penguor.Compiler.Analysis
{
    /// <summary>
    /// Manages all the symbol tables during a build, 
    /// ensuring that analysis is only started once all symbol tables are available
    /// </summary>
    public class SymbolTableManager
    {
        private Dictionary<string, AddressFrame> table;
        Hashtable hTable = new Hashtable();

        public SymbolTableManager()
        {
            table = new Dictionary<string, AddressFrame>();
        }

        public void AddSymbol(string name)
        {
            table.Add(name, new AddressFrame());
        }

        public void AddDeclaration()
        {
            State address;
        }

        public void AddDeclaration(State address)
        {
            Dictionary<string, AddressFrame> tmp = table;
            for (int i = 0; i < address.Count; i++)
            {
                if (!address[i].IsLastItem)
                {
                    if (tmp.ContainsKey(address[i].Symbol.token))
                    {
                        tmp = address[i].Children;
                    }
                    else
                    {
                        tmp.Add(address[i].Symbol.token, address[i]);
                        tmp = tmp[address[i].Symbol.token].Children;
                    }
                }
                else
                {
                    if (tmp.ContainsKey(address[i].Symbol.token))
                    {
                        throw new PenguorException(1, address[i].Symbol.offset);
                    }
                    else
                    {
                        tmp.Add(address[i].Symbol.token, address[i]);
                    }
                }
            }
        }
    }
}
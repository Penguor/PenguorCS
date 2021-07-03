using System;
using System.Collections.Generic;
using System.Linq;
using Penguor.Compiler.Debugging;
using static Penguor.Compiler.IR.IROPCode;

namespace Penguor.Compiler.IR
{
    public class IROptimizer
    {
        IRProgram program;

        public IROptimizer(IRProgram input)
        {
            program = input;
        }

        public IRProgram Optimize()
        {
            /*foreach (var function in program.Functions)
            {
                SortedSet<int> toRemove = new();
                for (int i = 0; i < function.Statements.Count; i++)
                {
                    var current = function.Statements[i];

                    // optimize load args
                    if (current.Code is LOADARG)
                    {
                        var referencedIndex = function.Statements.FindIndex(s => s.Number == ((IRReference)current.Operands[1]).Referenced);
                        var referenced = function.Statements[referencedIndex];
                        if (referenced?.Code is LOAD)
                        {
                            if (referenced.Number == (current.Number - 1))
                            {
                                function.Statements[i] = current with { Code = LDARGDIR, Operands = new IRArgument[] { current.Operands[0], referenced.Operands[0] } };
                                if (!function.Statements.Any(s => s.Operands.Any(op => op is IRReference reference && reference.Referenced == referenced.Number)))
                                {
                                    toRemove.Add(referencedIndex);
                                }
                            }
                            else if (!function.Statements.Any(s => s.Operands.Any(op => op is IRReference reference && reference.Referenced == referenced.Number)))
                            {
                                function.Statements[i] = current with { Code = LDARGDIR, Operands = new IRArgument[] { current.Operands[0], referenced.Operands[0] } };
                                toRemove.Add(referencedIndex);
                            }
                        }
                    }
                    // strip unused loads
                    else if (current.Code is LOAD && !function.Statements.Any(s => s.Operands.Any(op => op is IRReference reference && reference.Referenced == current.Number)))
                    {
                        toRemove.Add(i);
                    }
                    // remove unnecessary mov instructions
                    else if (current.Code is MOV && current.Operands[0] == current.Operands[1])
                    {
                        toRemove.Add(i);
                    }
                }
                var toRemoveArray = toRemove.ToArray();
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    function.Statements.RemoveAt(toRemoveArray[i]);
                }
            }
            if (program.Functions.Count != 0) Logger.Log(program.ToString(), LogLevel.Debug);*/
            return program;
        }
    }
}
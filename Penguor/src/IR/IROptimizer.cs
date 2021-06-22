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
            LoadArg();
            StripLoads();
            if (program.Functions.Count != 0) Logger.Log(program.ToString(), LogLevel.Debug);
            return program;
        }

        private void LoadArg()
        {
            foreach (var function in program.Functions)
            {
                List<int> toRemove = new();
                for (int i = 0; i < function.Statements.Count; i++)
                {
                    if (function.Statements[i].Code is LOADARG)
                    {
                        var referencedIndex = function.Statements.FindIndex(s => s.Number == ((IRReference)function.Statements[i].Operands[1]).Referenced);
                        var referenced = function.Statements[referencedIndex];
                        if (referenced?.Code is LOAD)
                        {
                            function.Statements[i] = function.Statements[i] with { Code = LDARGDIR, Operands = new IRArgument[] { function.Statements[i].Operands[0], referenced.Operands[0] } };
                            if (!function.Statements.Any(s => s.Operands.Any(op => op is IRReference reference && reference.Referenced == referenced.Number)))
                            {
                                toRemove.Add(referencedIndex);
                            }
                        }
                    }
                }
                toRemove.Sort();
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    function.Statements.RemoveAt(toRemove[i]);
                }
            }
        }

        private void StripLoads()
        {
            foreach (var function in program.Functions)
            {
                List<int> toRemove = new();
                for (int i = 0; i < function.Statements.Count; i++)
                {
                    if (function.Statements[i].Code is LOAD && !function.Statements.Any(s => s.Operands.Any(op => op is IRReference reference && reference.Referenced == function.Statements[i].Number)))
                    {
                        toRemove.Add(i);
                    }
                }
                toRemove.Sort();
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    function.Statements.RemoveAt(toRemove[i]);
                }
            }
        }
    }
}
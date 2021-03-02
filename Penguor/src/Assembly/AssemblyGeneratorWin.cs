using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.IR;
using static Penguor.Compiler.Assembly.RegisterAmd64;
namespace Penguor.Compiler.Assembly
{
    /// <summary>
    /// generate assembly code for windows
    /// </summary>
    public sealed class AssemblyGeneratorWinAmd64 : AssemblyGenerator
    {
        private readonly Builder builder;
        private readonly List<IRFunction> functions;
        private AsmProgram program = new AsmProgram();
        private int i;

        private readonly Dictionary<IRReference, IRArgument> loaded = new();

        /// <summary>
        /// create a new instance of the AssemblyGenerator for windows
        /// </summary>
        /// <param name="program">the input ir program</param>
        /// <param name="builder">the builder which compiles this unit</param>
        public AssemblyGeneratorWinAmd64(IRProgram program, Builder builder) : base(program)
        {
            this.builder = builder;
            functions = program.Functions;
        }

        /// <inheritdoc/>
        public override void Generate()
        {
            program.AddGlobalLabel("main");

            foreach (var i in functions)
            {
                GetRegisters(i);
            }
        }


        private void GetRegisters(IRFunction function)
        {
            List<(int, BitArray)> lifetimes = ComputeLifetime(function);

            int[,] weight = ComputeWeight(lifetimes, function);

            ComputeRegisters(lifetimes, weight);
        }

        private List<(int, BitArray)> ComputeLifetime(IRFunction function)
        {
            List<(int, BitArray)> lifetimes = new();

            for (int i = 0; i < function.Statements.Count; i++)
            {
                uint number = function.Statements[i].Number;
                IRReference numReference = new IRReference(number);

                var bits = new BitArray(function.Statements.Count);

                uint refCount = 0;

                for (int j = 0; j < function.Statements.Count; j++)
                {
                    bool referenced = false;
                    if (j != i)
                    {
                        foreach (var operand in function.Statements[j].Operands)
                        {
                            referenced = (operand is IRPhi phiOperand && phiOperand.Operands.Contains(numReference))
                                || (operand is IRReference refOperand && refOperand == numReference);
                        }
                    }
                    else
                    {
                        referenced = true;
                    }
                    if (referenced) refCount++;
                    bits[j] = referenced;
                }

                // do not add variables whose only occurrence are themselves
                if (refCount > 1)
                    lifetimes.Add((i, bits));
            }

            Console.WriteLine(function.Statements[0].ToString());
            foreach (var row in lifetimes)
            {
                Console.Write(string.Format("{0,-4}", row.Item1));
                foreach (var item in row.Item2)
                {
                    Console.Write(item.ToString() == "True" ? 1 : 0);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            return lifetimes;
        }

        private int[,] ComputeWeight(List<(int, BitArray)> lifetime, IRFunction function)
        {
            int[,] weight = new int[lifetime.Count, function.Statements.Count];

            for (int y = 0; y < lifetime.Count; y++)
            {
                int localWeight = 0;
                bool first = true;
                for (int x = 0; x < function.Statements.Count; x++)
                {
                    if (lifetime[y].Item2[x] && !first)
                    {
                        for (; localWeight > 0; localWeight--)
                        {
                            weight[y, x - localWeight] = localWeight;
                        }
                    }
                    else if (lifetime[y].Item2[x] && first)
                    {
                        for (; localWeight > 0; localWeight--)
                        {
                            weight[y, x - localWeight] = -1;
                        }
                        first = false;
                    }
                    else
                    {
                        weight[y, x] = -2;
                        localWeight++;
                    }
                }
            }

            Console.WriteLine(function.Statements[0].ToString());
            for (int y = 0; y < lifetime.Count; y++)
            {
                for (int x = 0; x < function.Statements.Count; x++)
                {
                    Console.Write(string.Format("{0,-3}", weight[y, x]));
                }
                Console.WriteLine();
            }

            return weight;
        }

        private void ComputeRegisters(List<(int, BitArray)> lifetimes, int[,] weight)
        {
            int registerCount = 3;

            int[] registerOccupied = new int[registerCount];
            int[,] registerMap = new int[lifetimes.Count, lifetimes[0].Item2.Count];

            int xMax = lifetimes[0].Item2.Count;
            List<int> finished = new();
            for (int x = 0; x < xMax; x++)
            {

                for (int y = 0; y < lifetimes.Count; y++)
                {
                    if (!finished.Contains(y))
                    {
                        (int statement, BitArray row) = lifetimes[y];
                        if (row[x])
                        {
                            if (registerMap[y, x - 1] <= 0)
                            {
                                bool foundEmpty = false;
                                for (int i = 0; i < registerOccupied.Length; i++)
                                {
                                    if (registerOccupied[i] == 0)
                                    {
                                        registerMap[y, x] = i + 1;
                                        registerOccupied[i] = statement;
                                        foundEmpty = true;
                                        i = registerOccupied.Length;
                                    }
                                }
                                if (!foundEmpty)
                                {
                                    int highestWeight = 0;
                                    int highestY = 0;
                                    for (int innerY = 0; innerY < lifetimes.Count; innerY++)
                                    {
                                        if (lifetimes[innerY].Item1 != statement && weight[innerY, x] > highestWeight)
                                        {
                                            highestWeight = weight[innerY, x];
                                            highestY = innerY;
                                        }
                                    }

                                    int register = 0;
                                    if (highestY > y)
                                    {
                                        register = registerMap[highestY, x - 1];
                                        registerMap[highestY, x - 1] = -1;
                                    }
                                    else
                                    {
                                        register = registerMap[highestY, x];
                                        registerMap[highestY, x] = -1;
                                    }

                                    registerMap[y, x] = register;
                                    registerOccupied[register - 1] = statement;
                                }
                            }
                            else
                            {
                                registerMap[y, x] = registerMap[y, x - 1];
                            }
                        }
                        else if (weight[y, x] == -2)
                        {
                            registerOccupied[registerMap[y, x - 1] - 1] = 0;
                            finished.Add(y);
                        }
                        else if (x != 0)
                        {
                            registerMap[y, x] = registerMap[y, x - 1];
                        }
                    }
                }
            }

            Console.WriteLine();
            for (int y = 0; y < registerMap.Length / xMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    Console.Write(string.Format("{0,-3}", registerMap[y, x]));
                }
                Console.WriteLine();
            }
        }

        // generate assembly for one statement
        private void GenerateStatement()
        {
        }
    }
}
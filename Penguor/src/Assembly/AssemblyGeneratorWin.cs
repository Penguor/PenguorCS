using System;
using System.Collections;
using System.Collections.Generic;
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
        public override AsmProgram Generate()
        {
            program.AddGlobalLabel(new State("main"));

            AsmTextSection text = new();

            foreach (var i in functions)
            {
                (var registers, var statements) = GetRegisters(i);
                text.AddFunction(GenerateAssembly(i, registers, statements));
            }

            return new AsmProgram { Text = text };
        }

        private (int[,], int[]) GetRegisters(IRFunction function)
        {
            (int[] statements, BitArray[] lifetimes) = ComputeLifetime(function);

            int[,] weight = ComputeWeight(lifetimes);

            return ComputeRegisters(weight, statements, function);
        }

        private (int[], BitArray[]) ComputeLifetime(IRFunction function)
        {
            List<int> statements = new();
            List<BitArray> lifetimes = new();

            for (int i = 0; i < function.Statements.Count; i++)
            {
                int number = function.Statements[i].Number;
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
                            if (!referenced)
                            {
                                referenced = (operand is IRPhi phiOperand && phiOperand.Operands.Contains(numReference))
                                    || (operand is IRReference refOperand && refOperand == numReference);
                            }
                        }
                    }
                    else
                    {
                        referenced = true;
                    }
                    if (referenced) refCount++;
                    bits[j] = referenced;
                }

                if (function.Statements[i].Code is IROPCode.LOADARG or IROPCode.LOADPARAM or IROPCode.BCALL or IROPCode.CALL)
                {
                    statements.Add(i);
                    lifetimes.Add(bits);
                }
                // do not add variables whose only occurrence are themselves
                else if (refCount > 1)
                {
                    statements.Add(i);
                    lifetimes.Add(bits);
                }
            }

            Console.WriteLine(function.Statements[0].ToString());
            for (int i = 0; i < lifetimes.Count; i++)
            {
                Console.Write(string.Format("{0,-4}", statements[i]));
                foreach (bool item in lifetimes[i])
                {
                    Console.Write(Convert.ToByte(item));
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            return (statements.ToArray(), lifetimes.ToArray());
        }

        private int[,] ComputeWeight(BitArray[] lifetimes)
        {
            if (lifetimes.Length == 0) return new int[0, 0];
            int[,] weight = new int[lifetimes.Length, lifetimes[0].Length];

            for (int y = 0; y < lifetimes.Length; y++)
            {
                int localWeight = 0;
                bool first = true;
                for (int x = 0; x < weight.GetLength(1); x++)
                {
                    if (lifetimes[y][x] && !first)
                    {
                        for (; localWeight > 0; localWeight--)
                        {
                            weight[y, x - localWeight] = localWeight;
                        }
                    }
                    else if (lifetimes[y][x] && first)
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

            Console.WriteLine();
            for (int y = 0; y < weight.GetLength(0); y++)
            {
                for (int x = 0; x < weight.GetLength(1); x++)
                {
                    Console.Write(string.Format("{0,-4}", weight[y, x].ToString("+0;-#")));
                }
                Console.WriteLine();
            }

            return weight;
        }

        private (int[,], int[]) ComputeRegisters(int[,] weight, int[] statements, IRFunction function)
        {
            if (statements.Length == 0) return (new int[0, 0], statements);

            Dictionary<RegisterAmd64, int> registerOccupied = new();
            foreach (RegisterAmd64 register in (RegisterAmd64[])Enum.GetValues(typeof(RegisterAmd64)))
                registerOccupied[register] = 0;

            int[,] registerMap = new int[weight.GetLength(0), weight.GetLength(1)];

            int xMax = weight.GetLength(1);

            Stack<int> calls = new();
            int paramCount = 1;
            int argCount = 1;

            List<int> finished = new();
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < statements.Length; y++)
                {
                    if (!finished.Contains(y))
                    {
                        int statement = statements[y];
                        if (weight[y, x] == 0)
                        {
                            if (function.Statements[x].Code is IROPCode.BCALL)
                            {
                                calls.Push(argCount);
                                argCount = 1;
                            }
                            else if (function.Statements[x].Code is IROPCode.CALL)
                            {
                                argCount = calls.Pop();
                            }
                            else if (function.Statements[x].Code is IROPCode.LOADPARAM)
                            {
                                RegisterAmd64 newRegister = 0;
                                switch (function.Statements[x].Operands[0].ToString())
                                {
                                    case "byte" or "short" or "int" or "long":
                                        newRegister = paramCount switch
                                        {
                                            1 => RCX,
                                            2 => RDX,
                                            3 => R8,
                                            4 => R9,
                                            _ => STACK,
                                        };
                                        break;
                                }
                                AssignRegister(x, y, newRegister);
                                paramCount++;
                            }
                            else if (registerMap[y, x - 1] <= 0)
                            {
                                AssignRegister(x, y, (RegisterAmd64)FindAndEmptyRegister(y, x));
                            }
                            else
                            {
                                registerMap[y, x] = registerMap[y, x - 1];
                            }
                        }
                        else if (weight[y, x] == -2)
                        {
                            registerOccupied[(RegisterAmd64)registerMap[y, x - 1]] = 0;
                            finished.Add(y);
                        }
                        else if (x != 0)
                        {
                            registerMap[y, x] = registerMap[y, x - 1];
                        }
                    }
                }
            }

            PrintRegisters();

            for (int x = 0; x < xMax; x++)
            {
                HashSet<int> ints = new HashSet<int>(registerMap.Length);
                for (int y = 0; y < registerMap.Length / xMax; y++)
                {
                    if (registerMap[y, x] != 0 && !ints.Add(registerMap[y, x]))
                        throw new Exception();
                }
            }

            return (registerMap, statements);

            void PrintRegisters()
            {
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

            void AssignRegister(int x, int y, RegisterAmd64 register)
            {
                if (registerOccupied[register] == 0)
                {
                    registerMap[y, x] = (int)register;
                    registerOccupied[register] = statements[y];
                }
                else
                {
                    int newY = Array.IndexOf(statements, registerOccupied[register]);
                    var newRegister = FindAndEmptyRegister(newY, x);

                    registerMap[newY, x] = newRegister;
                    registerOccupied[(RegisterAmd64)newRegister] = statements[newY];
                }
                registerMap[y, x] = (int)register;
                registerOccupied[register - 1] = statements[y];
            }

            int FindAndEmptyRegister(int y, int x)
            {
                IRStatement statement = function.Statements[statements[y]];

                RegisterAmd64[] validRegisters = GetRegisterSetFromStatement(statement);
                int newRegister = 0;

                foreach ((var register, var currentStatement) in registerOccupied)
                {
                    if (Array.Exists(validRegisters, element => element == register) && currentStatement == 0)
                    {
                        newRegister = (int)register;
                        break;
                    }
                }

                if (newRegister != 0) return newRegister;

                int highestWeight = 0;
                int highestY = 0;

                for (int innerY = 0; innerY < weight.GetLength(0); innerY++)
                {
                    if (innerY > y && weight[innerY, x] > highestWeight && registerMap[innerY, x - 1] != -1)
                    {
                        highestWeight = weight[innerY, x - 1];
                        highestY = innerY;
                    }
                    else if (innerY < y && weight[innerY, x] > highestWeight && registerMap[innerY, x] != -1)
                    {
                        highestWeight = weight[innerY, x];
                        highestY = innerY;
                    }
                }

                if (highestY > y)
                {
                    newRegister = registerMap[highestY, x - 1];
                    registerMap[highestY, x - 1] = -1;
                }
                else
                {
                    newRegister = registerMap[highestY, x];
                    registerMap[highestY, x] = -1;
                }

                return newRegister;
            }

            RegisterAmd64[] GetRegisterSetFromStatement(IRStatement statement)
            {
                // Console.WriteLine(statement);
                return statement.Code switch
                {
                    IROPCode.LOADARG or IROPCode.LOADPARAM => statement.Operands[0].ToString() switch
                    {
                        "byte" or "short" or "int" or "long" or "string" => RegisterSetAmd64.GeneralPurpose,
                        "double" => RegisterSetAmd64.XMM
                    },
                    IROPCode.LOAD => statement.Operands[0] switch
                    {
                        IRInt or IRString => RegisterSetAmd64.GeneralPurpose,
                    },
                    IROPCode.PHI => GetRegisterSetFromStatement(
                            function.Statements.Find(s => s.Number == ((IRPhi)statement.Operands[0]).Operands[0].Referenced) ?? throw new NullReferenceException()),
                    IROPCode.ADD or IROPCode.MUL or IROPCode.SUB or IROPCode.DIV => GetRegisterSetFromStatement(
                            function.Statements.Find(s => s.Number == ((IRReference)statement.Operands[0]).Referenced) ?? throw new NullReferenceException()),
                    IROPCode.CALL => builder.TableManager.GetSymbol(((IRState)statement.Operands[0]).State).DataType?.ToString() switch
                    {
                        "byte" or "short" or "int" or "long" or "string" or "void" => RegisterSetAmd64.Return64,
                        null => throw new NullReferenceException(),
                        string s => throw new PenguorCSException(s)
                    },
                    _ => throw new PenguorCSException(statement.ToString())
                };
            }
        }

        private AsmFunctionAmd64 GenerateAssembly(IRFunction irFunction, int[,] registers, int[] statements)
        {
            AsmFunctionAmd64 function = new AsmFunctionAmd64(irFunction.Name.ToString());

            int x = 0;
            foreach (var statement in irFunction.Statements)
            {
                switch (statement.Code)
                {
                    case IROPCode.FUNC:
                    case IROPCode.LABEL:
                        function.AddInstruction(new AsmLabelAmd64(((IRState)statement.Operands[0]).State));
                        break;
                    case IROPCode.ASM:
                        function.AddInstruction(new AsmRawInstructionAmd64(((IRString)statement.Operands[0]).Value));
                        break;
                    case IROPCode.ADD:
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister((RegisterAmd64)registers[DecodeStatement(x), x]),
                            new AsmRegister((RegisterAmd64)registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        ));
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.ADD,
                            new AsmRegister((RegisterAmd64)registers[DecodeStatement(x), x]),
                            new AsmRegister((RegisterAmd64)registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        ));
                        break;
                }
                x++;
            }

            return function;

            int DecodeStatement(int index)
            {
                return Array.IndexOf(statements, index);
            }

            int DecodeStatementFromReference(IRArgument argument)
            {
                if (argument is not IRReference reference)
                    throw new Exception();
                else
                    return Array.IndexOf(statements, irFunction.Statements.FindIndex(s => s.Number == reference.Referenced));
            }
        }
    }
}
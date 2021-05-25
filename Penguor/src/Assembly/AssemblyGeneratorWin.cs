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
            program.AddExtern(new State("printf"));

            AsmTextSection text = new();

            foreach (var i in functions)
            {
                var registers = GetRegisters(i);
                text.AddFunction(GenerateAssembly(i, registers));
            }

            program.Text = text;

            return program;
        }

        private RegisterAmd64[,] GetRegisters(IRFunction function)
        {
            var lifetimes = AnalyseLifetime(function);
            var weight = ComputeWeight(lifetimes, function);
            return ComputeRegisters(weight, function);
        }

        private BitArray[] AnalyseLifetime(IRFunction function)
        {
            List<BitArray> lifetimes = new();

            for (int x = 0; x < function.Statements.Count; x++)
            {
                int number = function.Statements[x].Number;
                IRReference numReference = new IRReference(number);

                var bits = new BitArray(function.Statements.Count);

                for (int y = 0; y < function.Statements.Count; y++)
                {
                    if (x == y)
                    {
                        bits[y] = true;
                        continue;
                    }

                    bool referenced = false;
                    foreach (var operand in function.Statements[y].Operands)
                    {
                        referenced = (operand is IRPhi phiOperand && phiOperand.Operands.Contains(numReference))
                            || (operand is IRReference refOperand && refOperand == numReference);
                        if (referenced) break;
                    }

                    bits[y] = referenced;
                }
                lifetimes.Add(bits);
            }

            Console.WriteLine(function.Statements[0].ToString());
            for (int i = 0; i < lifetimes.Count; i++)
            {
                foreach (bool item in lifetimes[i])
                {
                    Console.Write(Convert.ToByte(item));
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            return lifetimes.ToArray();
        }

        private int[,] ComputeWeight(BitArray[] lifetimes, IRFunction function)
        {
            if (lifetimes.Length == 0) return new int[0, 0];
            int[,] weight = new int[lifetimes.Length, lifetimes[0].Length];

            for (int y = 0; y < lifetimes.Length; y++)
            {
                int localWeight = 0;
                bool first = true;
                int refCount = 0;
                for (int x = 0; x < weight.GetLength(1); x++)
                {
                    if (lifetimes[y][x] && !first)
                    {
                        for (; localWeight > 0; localWeight--)
                        {
                            weight[y, x - localWeight] = localWeight;
                        }
                        refCount++;
                    }
                    else if (lifetimes[y][x] && first)
                    {
                        for (; localWeight > 0; localWeight--)
                        {
                            weight[y, x - localWeight] = -1;
                        }
                        first = false;
                        refCount++;
                    }
                    else
                    {
                        weight[y, x] = -2;
                        localWeight++;
                    }
                }

                if (refCount == 1 && !(function.Statements[y].Code is
                            IROPCode.BCALL or
                            IROPCode.CALL or
                            IROPCode.LOADPARAM or
                            IROPCode.JMP or
                            IROPCode.JTR or
                            IROPCode.JFL or
                            IROPCode.JE or
                            IROPCode.JG or
                            IROPCode.JGE or
                            IROPCode.JL or
                            IROPCode.JLE))
                {
                    for (int x = 0; x < weight.GetLength(1); x++)
                    {
                        if (weight[y, x] == 0)
                        {
                            weight[y, x] = -2;
                            break;
                        }
                        weight[y, x] = -2;
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
                Console.WriteLine(function.Statements[y].ToString());
            }

            return weight;
        }

        private RegisterAmd64[,] ComputeRegisters(int[,] weight, IRFunction function)
        {
            if (weight.Length == 0) return new RegisterAmd64[0, 0];

            Dictionary<RegisterAmd64, int[]> registerOccupied = new();
            foreach (RegisterAmd64 register in (RegisterAmd64[])Enum.GetValues(typeof(RegisterAmd64)))
                registerOccupied[register] = new int[function.Statements.Count];

            RegisterAmd64[,] registerMap = new RegisterAmd64[weight.GetLength(0), weight.GetLength(1)];

            int xMax = weight.GetLength(1);

            Stack<int> calls = new();
            int paramCount = 1;
            int argCount = 1;

            List<int> finished = new();
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < weight.GetLength(0); y++)
                {
                    if (finished.Contains(y))
                    {
                        continue;
                    }
                    if (weight[y, x] == 0)
                    {
                        if (function.Statements[x].Code is IROPCode.BCALL && x == y)
                        {
                            Console.WriteLine("bcall " + function.Statements[x].Number + " " + calls.Count);
                            calls.Push(argCount);
                            argCount = 1;
                        }
                        else if (function.Statements[x].Code is IROPCode.CALL && x == y)
                        {
                            Console.WriteLine("call " + function.Statements[x].Number + " " + calls.Count);
                            argCount = calls.Pop();
                        }
                        else if (function.Statements[x].Code is IROPCode.LOADPARAM or IROPCode.LOADARG)
                        {
                            RegisterAmd64 newRegister = 0;
                            switch (function.Statements[x].Operands[0].ToString())
                            {
                                case "byte" or "short" or "int" or "long" or "string":
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
                        else if (function.Statements[x].Code is
                            IROPCode.JMP or
                            IROPCode.JTR or
                            IROPCode.JFL or
                            IROPCode.JE or
                            IROPCode.JG or
                            IROPCode.JGE or
                            IROPCode.JL or
                            IROPCode.JLE)
                        {
                            var labelIndex = function.Statements.FindIndex(s => s.Operands[0].Equals(function.Statements[x].Operands[0]));

                            if (labelIndex > x)
                            {
                                for (int innerY = 0; innerY < weight.GetLength(0); innerY++)
                                {
                                    AssignRegister(x, innerY, registerMap[innerY, x - 1]);
                                    AssignRegister(labelIndex, innerY, registerMap[innerY, x]);
                                }
                            }
                            else
                            {
                                for (int innerY = 0; innerY < weight.GetLength(0); innerY++)
                                {
                                    AssignRegister(x, innerY, registerMap[innerY, labelIndex]);
                                }
                            }
                        }
                        else if (registerMap[y, x] == 0 && registerMap[y, x - 1] <= 0)
                        {
                            AssignRegister(x, y, FindAndEmptyRegister(y, x));
                        }
                        else if (x > 0)
                        {
                            registerMap[y, x] = registerMap[y, x - 1];
                            if (registerMap[y, x] != 0)
                                registerOccupied[registerMap[y, x]][x] = y;
                        }
                    }
                    else if (weight[y, x] == -2)
                    {
                        if (x > 0 && registerMap[y, x - 1] != 0)
                            registerOccupied[registerMap[y, x - 1]][x] = 0;
                        finished.Add(y);
                    }
                    else if (x != 0)
                    {
                        registerMap[y, x] = registerMap[y, x - 1];
                        if (registerMap[y, x] != 0)
                            registerOccupied[registerMap[y, x]][x] = y;
                    }
                }
            }

            PrintRegisters();

            for (int x = 0; x < xMax; x++)
            {
                HashSet<int> ints = new HashSet<int>(registerMap.Length);
                for (int y = 0; y < registerMap.Length / xMax; y++)
                {
                    if (registerMap[y, x] != 0 && !ints.Add((int)registerMap[y, x]))
                        throw new Exception(y.ToString() + ", " + x);
                }
            }

            return registerMap;

            void PrintRegisters()
            {
                Console.WriteLine();
                for (int y = 0; y < registerMap.Length / xMax; y++)
                {
                    Console.Write(string.Format("{0,-6}", y));
                    for (int x = 0; x < xMax; x++)
                    {
                        Console.Write(string.Format("{0,-5}", registerMap[y, x]));
                    }
                    Console.Write(function.Statements[y]);
                    Console.WriteLine();
                }
            }

            void AssignRegister(int x, int y, RegisterAmd64 register)
            {
                if (register == 0) return;
                if (registerOccupied[register][x] == 0)
                {
                    registerMap[y, x] = register;
                    registerOccupied[register][x] = y;
                    return;
                }
                else
                {
                    int newY = registerOccupied[register][x];
                    var newRegister = FindAndEmptyRegister(newY, x);

                    registerMap[newY, x] = newRegister;
                    registerOccupied[newRegister][x] = newY;
                }
                registerMap[y, x] = register;
                registerOccupied[register][x] = y;
            }

            RegisterAmd64 FindAndEmptyRegister(int y, int x)
            {
                IRStatement statement = function.Statements[y];

                RegisterAmd64[] validRegisters = GetRegisterSetFromStatement(statement);
                RegisterAmd64 newRegister = 0;

                foreach ((var register, var currentStatement) in registerOccupied)
                {
                    if (Array.Exists(validRegisters, element => element == register))
                    {
                        var xMinus = currentStatement[x - 1];
                        var xCurrent = currentStatement[x];

                        if (xMinus == 0 && xCurrent == 0)
                        {
                            newRegister = register;
                            break;
                        }
                        else if (xMinus != 0 && xCurrent == 0)
                        {
                            if (xMinus > x)
                            {
                                continue;
                            }
                            newRegister = register;
                            break;
                        }
                    }
                }

                if (newRegister != 0) return newRegister;

                int highestWeight = 0;
                int highestY = 0;

                for (int innerY = 0; innerY < weight.GetLength(0); innerY++)
                {
                    if (innerY > y && weight[innerY, x] > highestWeight && registerMap[innerY, x - 1] != STACK)
                    {
                        highestWeight = weight[innerY, x - 1];
                        highestY = innerY;
                    }
                    else if (innerY < y && weight[innerY, x] > highestWeight && registerMap[innerY, x] != STACK)
                    {
                        highestWeight = weight[innerY, x];
                        highestY = innerY;
                    }
                }

                if (highestY > y)
                {
                    newRegister = registerMap[highestY, x - 1];
                    registerMap[highestY, x - 1] = STACK;
                }
                else
                {
                    newRegister = registerMap[highestY, x];
                    registerMap[highestY, x] = STACK;
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
                        "i8" or "i16" or "i32" or "i64" or "u8" or "u16" or "u32" or "u64" or "str" => RegisterSetAmd64.GeneralPurpose,
                        "f64" => RegisterSetAmd64.XMM
                    },
                    IROPCode.LOAD => statement.Operands[0] switch
                    {
                        IRInt or IRString or IRBool => RegisterSetAmd64.GeneralPurpose,
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
                    IROPCode.LESS => new RegisterAmd64[] { 0 },
                    _ => throw new PenguorCSException(statement.ToString())
                };
            }
        }

        private AsmFunctionAmd64 GenerateAssembly(IRFunction irFunction, RegisterAmd64[,] registers)
        {
            AsmFunctionAmd64 function = new AsmFunctionAmd64(irFunction.Name.ToString());

            int x = 0;

            int stackCounter = 0;

            foreach (var statement in irFunction.Statements)
            {
                for (int y = 0; y < registers.GetLength(0); y++)
                {
                    if (x != 0 && registers[y, x] != registers[y, x - 1])
                    {
                        if (registers[y, x] == 0 || registers[y, x - 1] == 0)
                        {
                            continue;
                        }
                        else if (registers[y, x - 1] == STACK && registers[y, x] != STACK)
                        {
                        }
                        else if (registers[y, x - 1] != STACK && registers[y, x] == STACK)
                        {

                        }
                        else
                        {
                            function.AddInstruction(
                                AsmMnemonicAmd64.MOV,
                                new AsmRegister(registers[y, x]),
                                new AsmRegister(registers[y, x - 1])
                            );
                        }
                    }
                }
                switch (statement.Code)
                {
                    case IROPCode.FUNC:
                        function.AddInstruction(new AsmLabelAmd64(((IRState)statement.Operands[0]).State));
                        //stack frame
                        function.AddInstruction(AsmMnemonicAmd64.PUSH, new AsmRegister(RBP));
                        function.AddInstruction(AsmMnemonicAmd64.MOV, new AsmRegister(RBP), new AsmRegister(RSP));
                        break;
                    case IROPCode.LABEL:
                        function.AddInstruction(new AsmLabelAmd64(((IRState)statement.Operands[0]).State));
                        break;
                    case IROPCode.ASM:
                        function.AddInstruction(new AsmRawInstructionAmd64(((IRString)statement.Operands[0]).Value));
                        break;
                    case IROPCode.ADD:
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        ));
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.ADD,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        ));
                        break;
                    case IROPCode.LOAD:
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            statement.Operands[0] switch
                            {
                                IRInt num => new AsmNumber(num.Value),
                                IRString str => new AsmString(GetString(str)),
                                IRBool bl => new AsmNumber(bl.Value ? 1 : 0),
                                _ => new AsmString("0")
                            }
                        ));
                        break;
                    case IROPCode.LESS:
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.CMP,
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        ));
                        break;
                    case IROPCode.PHI:

                    case IROPCode.LOADPARAM:
                    case IROPCode.BCALL:
                        break;
                    case IROPCode.JMP:
                        function.AddInstruction(AsmMnemonicAmd64.JMP, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JGE:
                        function.AddInstruction(AsmMnemonicAmd64.JGE, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JNL:
                        function.AddInstruction(AsmMnemonicAmd64.JNL, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.MUL:
                        function.AddInstruction(new AsmInstructionAmd64(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        ));
                        function.AddInstruction(
                            AsmMnemonicAmd64.IMUL,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        );
                        break;
                    case IROPCode.RET:
                        function.AddInstruction(AsmMnemonicAmd64.MOV, new AsmRegister(RSP), new AsmRegister(RBP));
                        function.AddInstruction(AsmMnemonicAmd64.POP, new AsmRegister(RBP));
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(RAX),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        function.AddInstruction(AsmMnemonicAmd64.RET);
                        break;
                    case IROPCode.RETN:
                        function.AddInstruction(AsmMnemonicAmd64.MOV, new AsmRegister(RSP), new AsmRegister(RBP));
                        function.AddInstruction(AsmMnemonicAmd64.POP, new AsmRegister(RBP));
                        function.AddInstruction(AsmMnemonicAmd64.RET);
                        break;
                    case IROPCode.LOADARG:
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        );
                        break;
                    case IROPCode.CALL:
                        function.AddInstruction(
                            AsmMnemonicAmd64.CALL,
                            new AsmString(statement.Operands[0].ToString())
                        );
                        break;
                    case IROPCode.MOV:
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        break;
                    // case IROPCode.JFL:
                    //     var register = registers[DecodeStatementFromReference(statement.Operands[1]), x - 1];
                    //     function.AddInstruction(o)
                    //     function.AddInstruction(
                    //         AsmMnemonicAmd64.JZ,


                    //     );
                    //     break;
                    default:
                        throw new PenguorCSException(statement.ToString());
                }
                x++;
            }

            return function;

            int DecodeStatementFromReference(IRArgument argument)
            {
                if (argument is not IRReference reference)
                    throw new Exception();
                else
                    return irFunction.Statements.FindIndex(s => s.Number == reference.Referenced);
            }
        }

        private readonly Dictionary<string, int> stringConstants = new();
        private int _stringNum;
        private int StringNum { get => _stringNum++; }

        private string GetString(IRString s)
        {
            if (!stringConstants.ContainsKey(s.Value))
            {
                string[] values = s.Value.Split('\\');
                var labelNum = StringNum;
                var labelName = $"@string{labelNum}";
                stringConstants.Add(s.Value, labelNum);

                List<string> processedValues = new();

                for (int i2 = 0; i2 < values.Length; i2++)
                {
                    if (i2 == 0)
                    {
                        if (values[i2].Length > 0) processedValues.Add($"'{values[i2]}'");
                    }
                    else
                    {
                        processedValues.Add(values[i2][0] switch
                        {
                            'n' => "10",
                            //todo: proper offset for ir statements
                            char escape => builder.Except("error", 16, 0, escape.ToString())
                        });
                    }
                }
                processedValues.Add("0");

                program.Data.AddFunction(new AsmVariableAmd64(labelName, "db", processedValues.ToArray()));
                return labelName;
            }
            else
            {
                return $"@string{stringConstants[s.Value]}";
            }
        }
    }
}
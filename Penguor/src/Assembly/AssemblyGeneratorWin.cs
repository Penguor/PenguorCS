using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly IROPCode[] jumpCodes = {
                    IROPCode.JMP, IROPCode.JTR, IROPCode.JFL,
        IROPCode.JL, IROPCode.JNL, IROPCode.JLE, IROPCode.JNLE,
        IROPCode.JG, IROPCode.JNG, IROPCode.JGE, IROPCode.JNGE,
        IROPCode.JE, IROPCode.JNE,
        };

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
            BitArray[] lifetimes = new BitArray[function.Statements.Count];
            for (int i = 0; i < lifetimes.Length; i++)
            {
                lifetimes[i] = new BitArray(lifetimes.Length);
            }

            Tuple<int, int>[] ranges = new Tuple<int, int>[function.Statements.Count];

            for (int x = 0; x < function.Statements.Count; x++)
            {
                if (ranges[x] == null)
                {
                    ranges[x] = new Tuple<int, int>(x, x);
                }
                else
                {
                    ranges[x] = new Tuple<int, int>(ranges[x].Item1, x);
                }

                lifetimes[x][x] = true;

                foreach (var operand in function.Statements[x].Operands)
                {
                    if (operand is IRReference refOperand)
                    {
                        int index = function.Statements.FindIndex(s => s.Number == refOperand.Referenced);
                        lifetimes[index][x] = true;
                        if (ranges[index] == null)
                        {
                            ranges[index] = new Tuple<int, int>(x, x);
                        }
                        else
                        {
                            ranges[index] = new Tuple<int, int>(ranges[index].Item1, x);
                        }
                    }
                    else if (operand is IRPhi phiOperand)
                    {
                        foreach (var phi in phiOperand.Operands)
                        {
                            int index = function.Statements.FindIndex(s => s.Number == phi.Referenced);
                            lifetimes[index][x] = true;

                            if (ranges[index] == null)
                            {
                                ranges[index] = new Tuple<int, int>(x, x);
                            }
                            else
                            {
                                ranges[index] = new Tuple<int, int>(ranges[index].Item1, x);
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < function.Statements.Count; x++)
            {
                if (jumpCodes.Contains(function.Statements[x].Code))
                {
                    var labelIndex = function.Statements.FindIndex(s => s.Operands.Length > 0 && s.Code == IROPCode.LABEL && s.Operands[0].Equals(function.Statements[x].Operands[0]));

                    for (int y = 0; y < function.Statements.Count; y++)
                    {
                        if (x > labelIndex && ranges[y].Item1 < labelIndex && ranges[y].Item2 > labelIndex)
                        {
                            lifetimes[y][x] = true;
                        }
                    }
                }
            }

            // Console.WriteLine(function.Statements[0].ToString());
            // for (int i = 0; i < lifetimes.Count; i++)
            // {
            //     foreach (bool item in lifetimes[i])
            //     {
            //         Console.Write(Convert.ToByte(item));
            //         Console.Write(' ');
            //     }
            //     Console.WriteLine();
            // }
            // Console.WriteLine();
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
                            IROPCode.LOADPARAM || jumpCodes.Contains(function.Statements[y].Code)))
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

            // for (int y = 0; y < weight.GetLength(0); y++)
            // {
            //     for (int x = 0; x < weight.GetLength(1); x++)
            //     {
            //         Console.Write(string.Format("{0,-4}", weight[y, x].ToString("+0;-#")));
            //     }
            //     Console.WriteLine(function.Statements[y].ToString());
            // }

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
                            calls.Push(argCount);
                            argCount = 1;
                        }
                        else if (function.Statements[x].Code is IROPCode.CALL && x == y)
                        {
                            argCount = calls.Pop();
                        }
                        else if (function.Statements[x].Code is IROPCode.LOADPARAM or IROPCode.LOADARG && x == y)
                        {
                            RegisterAmd64 newRegister = 0;
                            switch (function.Statements[x].Operands[0].ToString())
                            {
                                case "i8" or "i16" or "i32" or "i64" or "u8" or "u16" or "u32" or "u64" or "str":
                                    newRegister = argCount switch
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
                            argCount++;
                        }
                        else if (jumpCodes.Contains(function.Statements[x].Code))
                        {
                            var labelIndex = function.Statements.FindIndex(s => s.Operands.Length > 0 && s.Operands[0].Equals(function.Statements[x].Operands[0]));

                            if (labelIndex < x)
                            {
                                AssignRegister(x, y, registerMap[y, labelIndex]);
                            }
                            else
                            {
                                AssignRegister(x, y, registerMap[y, x - 1]);
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

            for (int x = 0; x < xMax; x++)
            {
                HashSet<int> ints = new(registerMap.Length);
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
                    Console.Write(string.Format("{0,-5}", y));
                    for (int x = 0; x < xMax; x++)
                    {
                        Console.Write(string.Format("{0,-4}", registerMap[y, x]));
                    }
                    // Console.Write(function.Statements[y]);
                    Console.WriteLine();
                }
            }

            void AssignRegister(int x, int y, RegisterAmd64 register)
            {
                if (register == 0) return;
                if (!IsRegisterOccupied(x, y, register))
                {
                    SetRegister(x, y, register);
                    return;
                }
                else
                {
                    int newY = GetStatementOccupyingRegister(x, y, register);
                    var newRegister = FindAndEmptyRegister(newY, x);

                    SetRegister(x, newY, newRegister);
                }
                SetRegister(x, y, register);
            }

            bool IsRegisterOccupied(int x, int y, RegisterAmd64 register)
            {
                if (register == 0)
                {
                    return false;
                }
                int current = registerOccupied[register][x];
                int last = registerOccupied[register][x - 1];
                if (last > y)
                {
                    return last != 0;
                }
                else
                {
                    return current != 0;
                }
            }

            int GetStatementOccupyingRegister(int x, int y, RegisterAmd64 register)
            {
                int current = registerOccupied[register][x];
                int last = registerOccupied[register][x - 1];
                if (last > y)
                {
                    return last;
                }
                else
                {
                    return current;
                }
            }

            void SetRegister(int x, int y, RegisterAmd64 register)
            {
                registerMap[y, x] = register;
                registerOccupied[register][x] = y;
            }

            RegisterAmd64 FindAndEmptyRegister(int y, int x)
            {
                IRStatement statement = function.Statements[y];

                RegisterAmd64[] validRegisters = GetRegisterSetFromStatement(statement);
                RegisterAmd64 newRegister = STACK;

                foreach (var register in validRegisters)
                {
                    if (!IsRegisterOccupied(x, y, register))
                    {
                        newRegister = register;
                        break;
                    }
                }

                if (newRegister != STACK) return newRegister;

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
                    registerOccupied[newRegister][x - 1] = 0;
                }
                else
                {
                    newRegister = registerMap[highestY, x];
                    registerMap[highestY, x] = STACK;
                    registerOccupied[newRegister][x] = 0;
                }

                return newRegister;
            }

            RegisterAmd64[] GetRegisterSetFromStatement(IRStatement statement)
            {
                return statement.Code switch
                {
                    IROPCode.LOADARG or IROPCode.LOADPARAM => statement.Operands[0].ToString() switch
                    {
                        "i8" or "i16" or "i32" or "i64" or "u8" or "u16" or "u32" or "u64" or "str" => RegisterSetAmd64.GeneralPurpose,
                        "f64" => RegisterSetAmd64.XMM
                    },
                    IROPCode.LOAD => statement.Operands[0] switch
                    {
                        IRInt or IRString or IRBool or IRChar => RegisterSetAmd64.GeneralPurpose,
                    },
                    IROPCode.PHI => GetRegisterSetFromStatement(
                            function.Statements.Find(s => s.Number == ((IRPhi)statement.Operands[0]).Operands[0].Referenced) ?? throw new NullReferenceException()),
                    IROPCode.ADD or IROPCode.MUL or IROPCode.SUB => GetRegisterSetFromStatement(
                            function.Statements.Find(s => s.Number == ((IRReference)statement.Operands[0]).Referenced) ?? throw new NullReferenceException()),
                    IROPCode.DIV => new RegisterAmd64[] { RAX },
                    IROPCode.REMAINDER => new RegisterAmd64[] { RDX },
                    IROPCode.CALL => builder.TableManager.GetSymbol(((IRState)statement.Operands[0]).State).DataType?.ToString() switch
                    {
                        "i8" or "i16" or "i32" or "i64" or "u8" or "u16" or "u32" or "u64" or "str" or "void" => RegisterSetAmd64.Return64,
                        null => throw new NullReferenceException(),
                        string s => throw new PenguorCSException(s)
                    },
                    IROPCode.LESS or IROPCode.LESS_EQUALS or IROPCode.GREATER or IROPCode.EQUALS or IROPCode.INVERT => RegisterSetAmd64.GeneralPurpose,
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
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.ADD,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        );
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
                                IRChar chr => new AsmNumber(chr.Value),
                                _ => throw new Exception()
                            }
                        ));
                        break;
                    case IROPCode.INVERT:
                        function.AddInstruction(
                            AsmMnemonicAmd64.XOR,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[x, x])
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(GetRegisterBySize(registers[x, x], RegisterSize.BYTE)),
                            new AsmRegister(GetRegisterBySize(registers[DecodeStatementFromReference(statement.Operands[0]), x], RegisterSize.BYTE))
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.TEST,
                            new AsmRegister(GetRegisterBySize(registers[x, x], RegisterSize.BYTE)),
                            new AsmRegister(GetRegisterBySize(registers[x, x], RegisterSize.BYTE))

                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.SETZ,
                            new AsmRegister(GetRegisterBySize(registers[x, x], RegisterSize.BYTE))
                        );
                        break;
                    case IROPCode.LESS:
                    case IROPCode.LESS_EQUALS:
                    case IROPCode.GREATER:
                    case IROPCode.EQUALS:
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
                        function.AddInstruction(AsmMnemonicAmd64.JNZ, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JTR:
                        function.AddInstruction(
                            AsmMnemonicAmd64.TEST,
                            new AsmRegister(GetRegisterBySize(registers[DecodeStatementFromReference(statement.Operands[1]), x], RegisterSize.BYTE)),
                            new AsmRegister(GetRegisterBySize(registers[DecodeStatementFromReference(statement.Operands[1]), x], RegisterSize.BYTE))
                        );
                        function.AddInstruction(AsmMnemonicAmd64.JZ, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JGE:
                        function.AddInstruction(AsmMnemonicAmd64.JGE, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JE:
                        function.AddInstruction(AsmMnemonicAmd64.JE, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JNE:
                        function.AddInstruction(AsmMnemonicAmd64.JNE, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JNL:
                        function.AddInstruction(AsmMnemonicAmd64.JNL, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JNLE:
                        function.AddInstruction(AsmMnemonicAmd64.JNLE, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JL:
                        function.AddInstruction(AsmMnemonicAmd64.JL, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.JNG:
                        function.AddInstruction(AsmMnemonicAmd64.JNG, new AsmString(statement.Operands[0].ToString()));
                        break;
                    case IROPCode.MUL:
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.IMUL,
                            new AsmRegister(registers[x, x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x])
                        );
                        break;
                    case IROPCode.REMAINDER:
                    case IROPCode.DIV:
                        function.AddInstruction(
                            AsmMnemonicAmd64.PUSH,
                            new AsmRegister(RDX)
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.XOR,
                            new AsmRegister(RDX),
                            new AsmRegister(RDX)
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(RAX),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        function.AddInstruction(
                            AsmMnemonicAmd64.IDIV,
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
                        Stack<RegisterAmd64> registersToPop = new();
                        for (int y = 0; y < registers.GetLength(0); y++)
                        {
                            if (RegisterSetAmd64.Volatile.Contains(registers[y, x]))
                            {
                                registersToPop.Push(registers[y, x]);
                                function.AddInstruction(AsmMnemonicAmd64.PUSH, new AsmRegister(registers[y, x]));
                            }
                        }
                        function.AddInstruction(
                            AsmMnemonicAmd64.CALL,
                            new AsmString(statement.Operands[0].ToString())
                        );
                        for (int y = 0; y < registers.GetLength(0); y++)
                        {
                            if (RegisterSetAmd64.Volatile.Contains(registers[y, x]))
                            {
                                function.AddInstruction(AsmMnemonicAmd64.POP, new AsmRegister(registersToPop.Pop()));
                            }
                        }
                        break;
                    case IROPCode.MOV:
                        function.AddInstruction(
                            AsmMnemonicAmd64.MOV,
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[1]), x]),
                            new AsmRegister(registers[DecodeStatementFromReference(statement.Operands[0]), x])
                        );
                        break;
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

        private RegisterAmd64 GetRegisterBySize(RegisterAmd64 register, RegisterSize size) => size switch
        {
            RegisterSize.BYTE => register switch
            {
                AL or AX or EAX or RAX => AL,
                BL or BX or EBX or RBX => BL,
                CL or CX or ECX or RCX => CL,
                DL or DX or EDX or RDX => DL,
                DIL or DI or EDI or RDI => DIL,
                SIL or SI or ESI or RSI => SIL,
                BPL or BP or EBP or RBP => BPL,
                SPL or SP or ESP or RSP => SPL,
                R8B or R8W or R8D or R8 => R8B,
                R9B or R9W or R9D or R9 => R9B,
                R10B or R10W or R10D or R10 => R10B,
                R11B or R11W or R11D or R11 => R11B,
                R12B or R12W or R12D or R12 => R12B,
                R13B or R13W or R13D or R13 => R13B,
                R14B or R14W or R14D or R14 => R14B,
                R15B or R15W or R15D or R15 => R15B,
            },
            RegisterSize.WORD => register switch
            {
                AL or AX or EAX or RAX => AX,
                BL or BX or EBX or RBX => BX,
                CL or CX or ECX or RCX => CX,
                DL or DX or EDX or RDX => DX,
                DIL or DI or EDI or RDI => DI,
                SIL or SI or ESI or RSI => SI,
                BPL or BP or EBP or RBP => BP,
                SPL or SP or ESP or RSP => SP,
                R8B or R8W or R8D or R8 => R8W,
                R9B or R9W or R9D or R9 => R9W,
                R10B or R10W or R10D or R10 => R10W,
                R11B or R11W or R11D or R11 => R11W,
                R12B or R12W or R12D or R12 => R12W,
                R13B or R13W or R13D or R13 => R13W,
                R14B or R14W or R14D or R14 => R14W,
                R15B or R15W or R15D or R15 => R15W,
            },
            RegisterSize.DWORD => register switch
            {
                AL or AX or EAX or RAX => EAX,
                BL or BX or EBX or RBX => EBX,
                CL or CX or ECX or RCX => ECX,
                DL or DX or EDX or RDX => EDX,
                DIL or DI or EDI or RDI => EDI,
                SIL or SI or ESI or RSI => ESI,
                BPL or BP or EBP or RBP => EBP,
                SPL or SP or ESP or RSP => ESP,
                R8B or R8W or R8D or R8 => R8D,
                R9B or R9W or R9D or R9 => R9D,
                R10B or R10W or R10D or R10 => R10D,
                R11B or R11W or R11D or R11 => R11D,
                R12B or R12W or R12D or R12 => R12D,
                R13B or R13W or R13D or R13 => R13D,
                R14B or R14W or R14D or R14 => R14D,
                R15B or R15W or R15D or R15 => R15D,
            },
            RegisterSize.QWORD => register switch
            {
                AL or AX or EAX or RAX => RAX,
                BL or BX or EBX or RBX => RBX,
                CL or CX or ECX or RCX => RCX,
                DL or DX or EDX or RDX => RDX,
                DIL or DI or EDI or RDI => RDI,
                SIL or SI or ESI or RSI => RSI,
                BPL or BP or EBP or RBP => RBP,
                SPL or SP or ESP or RSP => RSP,
                R8B or R8W or R8D or R8 => R8,
                R9B or R9W or R9D or R9 => R9,
                R10B or R10W or R10D or R10 => R10,
                R11B or R11W or R11D or R11 => R11,
                R12B or R12W or R12D or R12 => R12,
                R13B or R13W or R13D or R13 => R13,
                R14B or R14W or R14D or R14 => R14,
                R15B or R15W or R15D or R15 => R15,
            },
        };

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
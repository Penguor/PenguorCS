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

        private readonly StringBuilder pre = new StringBuilder();
        private readonly StringBuilder text = new StringBuilder();
        private readonly StringBuilder data = new StringBuilder();
        private readonly StringBuilder bss = new StringBuilder();

        private int i;
        private readonly List<IRStatement> stmts;

        private Dictionary<Reference, IRArgument> loaded = new();

        /// <summary>
        /// create a new instance of the AssemblyGenerator for windows
        /// </summary>
        /// <param name="program">the input ir program</param>
        /// <param name="builder">the builder which compiles this unit</param>
        public AssemblyGeneratorWinAmd64(IRProgram program, Builder builder) : base(program)
        {
            this.builder = builder;
            stmts = program.Statements;
        }

        /// <inheritdoc/>
        public override void Generate()
        {
            pre.AppendLine("global main");
            pre.AppendLine("extern printf");

            AsmProgram program = new AsmProgram();

            for (i = 0; i < stmts.Count; i++)
            {
                GenerateStatement();
            }
            string final = "\n" + pre.ToString() + "\nsection .data\n\n" + data.ToString() + "\nsection .bss\n\n" + bss.ToString() + "\nsection .text\n\n" + text.ToString();
            if (!(data.Length == 0 && bss.Length == 0 && text.Length == 0)) Logger.Log(final, LogLevel.Debug);
            BuildManager.asmData.Append(data);
            BuildManager.asmText.Append(text);
            BuildManager.asmBss.Append(bss);
        }

        // generate assembly for one statement
        private void GenerateStatement()
        {
            RegisterAmd64? register;
            switch (stmts[i])
            {
                case var s when s.Code == IROPCode.ASM:
                    text.AppendLine(((String)s.Operands[0]).Value);
                    break;
                case var s when s.Code == IROPCode.FUNC:
                    CreateLabel(s.Operands[0]);
                    Push(RBP);
                    Move(RSP, RBP);
                    break;
                case var s when s.Code == IROPCode.LOADPARAM && s.Operands[0] is IRState irState:
                    var symbol = builder.TableManager.GetSymbol(irState.State);
                    System.Console.WriteLine(symbol.DataType?.ToString());
                    register = GetRegister(symbol.DataType?.ToString(), ((Int)s.Operands[1]).Value);
                    symbol.AsmInfo = new AsmInfoWindowsAmd64
                    {
                        ParamNumber = ((Int)s.Operands[1]).Value,
                        Register = register
                    };

                    break;
                case var s when s.Code == IROPCode.LOAD:
                    loaded.Add(new Reference((uint)i), s.Operands[0]);
                    break;
                case var s when s.Code == IROPCode.LOADARG && s.Operands[0] is String or Int:
                    GetValueOrPointer(s.Operands[0], GetRegister(s.Operands[0], (Int)s.Operands[1]));
                    break;
                /* case var s when s.Code == OPCode.LOADARG && s.Operands[0] is Reference reference && stmts[(int)reference.Referenced].Operands[0] is String or Int:
                    IRArgument argument = loaded.GetValueOrDefault(reference) ?? throw new System.Exception();
                    register = (argument, ((Int)s.Operands[1]).Value) switch
                    {
                        (String or Int, 1) => "rcx"
                    };
                    string get = argument switch
                    {
                        String str => GetString(str),
                        Int i => i.ToString()
                    };
                    if (register != "stack")
                        text.Append("mov ").Append(register).Append(", ").AppendLine(get);
                    else
                        text.Append("push ").AppendLine(get);
                    break; */
                /* case var s when s.Code == OPCode.LOADARG && s.Operands[0] is Reference reference && stmts[(int)reference.Referenced].Operands[0] is IRState state:
                    Symbol param = builder.TableManager.GetSymbol(state.State);
                    var asmInfo = (AsmInfoWindowsAmd64?)param.AsmInfo ?? throw new System.Exception();
                    register = (param.DataType?.ToString(), ((Int)s.Operands[1]).Value) switch
                    {
                        ("int", 1) => "rcx",
                        ("string", 1) => "rcx"
                    };
                    text.Append("mov ").Append(register).Append(", ").AppendLine(asmInfo.Register);
                    break; */
                case var s when s.Code == IROPCode.CALL && s.Operands[0] is IRState state:
                    text.Append("call ").Append(state.State).AppendLine();
                    break;
                case var s when s.Code == IROPCode.RETN:
                    text.AppendLine("mov rsp, rbp");
                    text.AppendLine("pop rbp");
                    text.AppendLine("ret");
                    return;
            }
        }

        private uint _stringNum;
        private uint StringNum { get => _stringNum++; }
        private readonly Dictionary<string, uint> stringConstants = new();

        private string GetString(String s)
        {
            if (!stringConstants.ContainsKey(s.Value))
            {
                string[] values = s.Value.Split('\\');
                var labelNum = StringNum;
                var labelName = $"@string{labelNum}";
                stringConstants.Add(s.Value, labelNum);
                data.Append(labelName).Append(" db ");

                for (int i2 = 0; i2 < values.Length; i2++)
                {
                    if (i2 == 0)
                    {
                        if (values[i2].Length > 0) data.Append('\'').Append(values[i2]).Append("', ");
                    }
                    else
                    {
                        data.Append(values[i2][0] switch
                        {
                            'n' => 10,
                            //todo: proper offset for ir statements
                            char escape => builder.Except(16, 0, escape)
                        }).Append(", ");
                    }
                }
                data.AppendLine("0");
                return labelName;
            }
            else
            {
                return $"@string{stringConstants[s.Value]}";
            }
        }

        private void GetValueOrPointer(IRArgument argument, RegisterAmd64 toRegister)
        {
            switch (argument)
            {
                case String str:
                    string name = GetString(str);
                    text.Append("mov ").Append(toRegister).Append(", ").AppendLine(name);
                    break;
                case Int num:
                    text.Append("mov ").Append(toRegister).Append(", ").AppendLine(num.Value.ToString());
                    break;
            }
        }

        private void CreateLabel(IRArgument labelName) => CreateLabel(labelName.ToString());
        private void CreateLabel(string labelName) => text.Append(labelName).AppendLine(":");

        private void Move(RegisterAmd64 from, RegisterAmd64 to) => text.Append("mov ").Append(to).Append(", ").Append(from).AppendLine();

        private void Push(RegisterAmd64 register)
        {
            if (register != STACK)
                text.Append("push ").Append(register).AppendLine();
            else throw new System.Exception();
        }

        private RegisterAmd64 GetRegister(IRArgument argument, Int paramNumber) => (argument, paramNumber.Value) switch
        {
            (String or Int or Double or Float, 1) => RCX,
            (String or Int or Double or Float, 2) => RDX,
            (String or Int or Double or Float, 3) => R8,
            (String or Int or Double or Float, 4) => R9,
            (String or Int or Double or Float, _) => STACK,
        };

        private RegisterAmd64 GetRegister(IRState state, int paramNumber) => GetRegister(state.ToString(), paramNumber);

        private RegisterAmd64 GetRegister(string? type, int paramNumber) => (type, paramNumber) switch
        {
            ("string" or "int" or "double" or "float", 1) => RCX,
            ("string" or "int" or "double" or "float", 2) => RDX,
            ("string" or "int" or "double" or "float", 3) => R8,
            ("string" or "int" or "double" or "float", 4) => R9,
            ("string" or "int" or "double" or "float", _) => STACK,
        };
    }
}
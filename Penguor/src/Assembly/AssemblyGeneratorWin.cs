using System.Collections.Generic;
using System.Text;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.IR;

namespace Penguor.Compiler.Assembly
{
    /// <summary>
    /// generate assembly code for windows
    /// </summary>
    public sealed class AssemblyGeneratorWin : AssemblyGenerator
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
        public AssemblyGeneratorWin(IRProgram program, Builder builder) : base(program)
        {
            this.builder = builder;
            stmts = program.Statements;
        }

        /// <inheritdoc/>
        public override void Generate()
        {
            pre.AppendLine("global main");
            pre.AppendLine("extern printf");

            for (i = 0; i < stmts.Count; i++)
            {
                AnalyseStatement();
            }

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

        private void AnalyseStatement()
        {
            switch (stmts[i].Code)
            {
                case OPCode.FUNC:
                case OPCode.LABEL:
                    builder.TableManager.GetSymbol(((IRState)stmts[i].Operands[0]).State).AsmInfo = new AsmInfoWindowsAmd64 { Get = stmts[i].Operands[0].ToString() };
                    break;
            }
        }

        // generate assembly for one statement
        private void GenerateStatement()
        {
            string? register = null;
            switch (stmts[i])
            {
                case var s when s.Code == OPCode.FUNC:
                    DefineFunction();
                    break;
                case var s when s.Code == OPCode.LOAD:
                    loaded.Add(new Reference((uint)i), s.Operands[0]);
                    break;
                case var s when s.Code == OPCode.LOADARG && s.Operands[0] is Reference reference && stmts[(int)reference.Referenced].Operands[0] is String or Int:
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
                    break;
                case var s when s.Code == OPCode.LOADARG && s.Operands[0] is Reference reference && stmts[(int)reference.Referenced].Operands[0] is IRState state:
                    Symbol param = builder.TableManager.GetSymbol(state.State);
                    var asmInfo = (AsmInfoWindowsAmd64?)param.AsmInfo ?? throw new System.Exception();
                    register = (param.DataType?.ToString(), ((Int)s.Operands[1]).Value) switch
                    {
                        ("int", 0) => "rcx",
                        ("string", 0) => "rcx"
                    };
                    text.Append("mov ").Append(register).Append(", ").AppendLine(asmInfo.Get);
                    break;
                case var s when s.Code == OPCode.CALL && s.Operands[0] is IRState state:
                    text.Append("call ").Append(state.State).AppendLine();
                    break;
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

        private void DefineFunction()
        {
            text.Append(stmts[i].Operands[0]).AppendLine(":");
            text.AppendLine("push rbp");
            text.AppendLine("mov rbp, rsp");
            if (stmts[i].Operands[0] is IRState s && s.ToString() == "print")
                text.AppendLine("CALL printf");
            while (true)
            {
                i++;
                switch (stmts[i].Code)
                {
                    case OPCode.RETN:
                        text.AppendLine("mov rsp, rbp");
                        text.AppendLine("pop rbp");
                        text.AppendLine("ret");
                        return;
                    default:
                        GenerateStatement();
                        break;
                }
            }
        }
    }
}
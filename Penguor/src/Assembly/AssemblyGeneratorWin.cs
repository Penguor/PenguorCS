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

        int i = 0;
        private readonly List<IRStatement> stmts;

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
            string final = "\n" + pre.ToString() + "\nsection .data\n\n" + data.ToString() + "\nsection .bss\n\n" + bss.ToString() + "\nsection .text\n\n" + text.ToString();
            if (!(data.Length == 0 && bss.Length == 0 && text.Length == 0)) Logger.Log(final, LogLevel.Debug);
            BuildManager.asmData.Append(data);
            BuildManager.asmText.Append(text);
            BuildManager.asmBss.Append(bss);
        }

        // analyses one statement and writes the appropriate assembly
        private void AnalyseStatement()
        {
            string val1;
            string val2;
            switch (stmts[i].Code)
            {
                case OPCode.FUNC:
                    DefineFunction();
                    break;
                case OPCode.LABEL:
                    text.Append(stmts[i].Operands[0]).AppendLine(":");
                    break;
                case OPCode.LIB:
                    text.Append("; ").AppendLine(stmts[i].Operands[0].ToString());
                    break;
                case OPCode.DEF:
                    if (stmts[i].Operands[0] is IRState state)
                    {
                        data.Append(stmts[i].Operands[0]).Append(
                           ' ').Append(builder.TableManager.GetSymbol(state.State).DataType?.ToString() switch
                           {
                               "bool" or "byte" or "char" or "string" => "db",
                               "short" => "dw",
                               "int" => "dd",
                               "long" => "dq",
                               "double" => "dq",
                               _ => throw new System.Exception(),
                           }).Append(' ');
                        data.Append(stmts[i].Operands[1] switch
                        {
                            String a => $"'{a.Value}', 0",
                            Double a => a.Value,
                            _ => " "
                        });
                        data.AppendLine();
                    }

                    break;
                case OPCode.DFE:
                    bss.Append(stmts[i].Operands[0]).Append(": ");
                    bss.Append(builder.TableManager.GetSymbol(((IRState)stmts[i].Operands[1]).State).DataType?.ToString() switch
                    {
                        "byte" => "resb",
                        "short" => "resw",
                        "int" => "resd",
                        "long" => "resq",
                        "float" => "resd",
                        "double" => "resq",
                        "char" => "resb",
                        // "string" => "",
                        "bool" => "resb",
                        // "void" => "",
                        _ => throw new System.Exception(),
                    }).AppendLine(" 1");
                    break;
                case OPCode.DFL:
                    throw new System.Exception();
                case OPCode.ASSIGN:
                    val1 = Get(stmts[i].Operands[0], true, true);
                    val2 = Get(stmts[i].Operands[1], false, true);
                    text.Append("mov ").Append(val1).Append(", ").AppendLine(val2);
                    break;
                case OPCode.CALL:
                    break;
                case OPCode.LOADARG:
                    CallFunction();
                    break;
                case OPCode.ADD:
                    val1 = Get(stmts[i].Operands[0], true, true);
                    val2 = Get(stmts[i].Operands[1], false, true);
                    text.Append("add ").Append(val1).Append(", ").AppendLine(val2);
                    break;
                case OPCode.SUB:
                    val1 = Get(stmts[i].Operands[0], true, true);
                    val2 = Get(stmts[i].Operands[1], false, true);
                    text.Append("add ").Append(val1).Append(", ").AppendLine(val2);
                    break;
                case OPCode.MUL:
                    val1 = Get(stmts[i].Operands[0], true, true);
                    val2 = Get(stmts[i].Operands[1], false, true);
                    text.Append("mov rax, ").AppendLine(val1);
                    text.Append("mul ").Append(val1).Append(", ").AppendLine(val2);
                    break;
                case OPCode.DIV:
                    val1 = Get(stmts[i].Operands[0], false, false);
                    val2 = Get(stmts[i].Operands[1], false, true);
                    text.Append("mov rax, ").AppendLine(val1);
                    text.Append("div ").AppendLine(val2);
                    break;
            }
        }

        private uint stringNum;
        private uint StringNum { get => stringNum++; }
        private readonly Dictionary<string, uint> stringConstants = new();

        private string GetString(String s)
        {
            if (!stringConstants.ContainsKey(s.Value))
            {
                var labelNum = StringNum;
                var labelName = $"@string{labelNum}";
                stringConstants.Add(s.Value, labelNum);
                data.Append(labelName).Append(" db '").Append(s.Value).AppendLine("', 0");
                return labelName;
            }
            else
            {
                return $"@string{stringConstants[s.Value]}";
            }
        }

        private string Get(IRArgument arg, bool result, bool getValue)
        {
            switch (arg)
            {
                case Byte a:
                    text.Append("mov r10, ").Append(a.Value).AppendLine();
                    return "r10";
                case Short a:
                    text.Append("mov r10, ").Append(a.Value).AppendLine();
                    return "r10";
                case Int a:
                    text.Append("mov r10, ").Append(a.Value).AppendLine();
                    return "r10";
                case Long a:
                    text.Append("mov r10, ").Append(a.Value).AppendLine();
                    return "r10";
                case Float a:
                    text.Append("mov xmm3, ").Append(a.Value).AppendLine();
                    return "xmm3";
                case Double a:
                    text.Append("mov xmm3, ").Append(a.Value).AppendLine();
                    return "xmm3";
                case IRState a:
                    var info = builder.TableManager.GetSymbol(a.State).AsmInfo;
                    return ((AsmInfoWindows)info!).Get ?? throw new System.Exception();
                case String a:
                    return GetString(a);
                case Reference or Char:
                    return "!notimplemented";
                default:
                    throw new System.Exception();
            }
        }

        private void DefineFunction()
        {
            int stack = 0;
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
                    case OPCode.DFL:
                        var symbol = builder.TableManager.GetSymbol(((IRState)stmts[i].Operands[0]).State);
                        text.Append("sub rsp, ");
                        text.Append(symbol.DataType?.ToString() switch
                        {
                            "byte" => SubStack(1),
                            "short" => SubStack(2),
                            "int" => SubStack(4),
                            "long" => SubStack(8),
                            "float" => SubStack(4),
                            "double" => SubStack(8),
                            "char" => SubStack(1),
                            "bool" => SubStack(1),
                            "string" => SubStack(System.Text.RegularExpressions.Regex.Unescape(((String)stmts[i].Operands[1]).Value).Length),
                            _ => throw new System.Exception(),
                        }).AppendLine();
                        symbol.AsmInfo = new AsmInfoWindows { StackOffset = stack, Get = $"rbp-{stack}" };
                        text.Append("MOV [rbp-").Append(stack).Append("], ").AppendLine(symbol.DataType?.ToString() switch
                        {
                            "byte" => "",
                            "short" => "",
                            "int" => "",
                            "long" => "",
                            "float" => "",
                            "double" => "",
                            "char" => "",
                            "bool" => ((Bool)stmts[i].Operands[1]).Value ? "1" : "0",
                            "string" => GetString((String)stmts[i].Operands[1]),
                            _ => throw new System.Exception(),
                        });
                        break;
                    case OPCode.RET:
                    case OPCode.RETN:
                        text.AppendLine("mov rsp, rbp");
                        text.AppendLine("pop rbp");
                        text.AppendLine("ret");
                        return;
                    default:
                        AnalyseStatement();
                        break;
                }
            }

            int SubStack(int amount)
            {
                stack += amount;
                return amount;
            }
        }

        private void CallFunction()
        {
            int count = 0;
            while (true)
            {
                switch (stmts[i].Code)
                {
                    case OPCode.LOADARG:
                        count++;
                        text.AppendLine(
                            count switch
                            {
                                1 => "MOV rcx, rax",
                                _ => "PUSH rax"
                            }
                        );
                        break;
                    case OPCode.LOAD:
                        text.AppendLine("MOV rax, ").AppendLine(stmts[i].Operands[0].ToString());
                        break;
                    case OPCode.CALL:
                        text.Append("CALL ").AppendLine(stmts[i].Operands[0].ToString());
                        return;
                    default:
                        throw new System.Exception();
                }
                i++;
            }
        }
    }
}
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
                switch (stmts[i].Code)
                {
                    case OPCode.FUNC:
                        text.Append(stmts[i].Operands[0]).AppendLine(":");
                        text.AppendLine("PUSH RBP");
                        text.AppendLine("MOV RSP, RBP");
                        if (stmts[i].Operands[0] is String s && s.Value == "print")
                            text.AppendLine("CALL printf");
                        break;
                    case OPCode.LABEL:
                        text.Append(stmts[i].Operands[0]).AppendLine(":");
                        break;
                    case OPCode.LIB:
                        text.Append("; ").AppendLine(stmts[i].Operands[0].ToString());
                        break;
                    case OPCode.USE:
                        break;
                    case OPCode.LOAD:
                        text.Append("MOV rax, ").Append(stmts[i].Operands[0]).AppendLine();
                        break;
                    case OPCode.LOADPARAM:
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
                    case OPCode.ASSIGN:
                        text.Append("MOV [").Append(stmts[i].Operands[0]).AppendLine("], rax");
                        break;
                    case OPCode.CALL:
                        break;
                    case OPCode.LOADARG:
                        CallFunction();
                        break;
                    case OPCode.RET:
                        text.AppendLine("RET");
                        break;
                    case OPCode.ADD:
                        text.Append("ADD [").Append(stmts[i].Operands[0]).AppendLine("], rax");
                        break;
                    case OPCode.SUB:
                        text.Append("SUB [").Append(stmts[i].Operands[0]).AppendLine("], rax");
                        break;
                    case OPCode.MUL:
                    case OPCode.DIV:
                    case OPCode.LESS:
                    case OPCode.GREATER:
                    case OPCode.JTR:
                    case OPCode.JFL:
                        break;
                }
            }
            string final = "\n" + pre.ToString() + "\nsection .data\n\n" + data.ToString() + "\nsection .bss\n\n" + bss.ToString() + "\nsection .text\n\n" + text.ToString();
            if (!(data.Length == 0 && bss.Length == 0 && text.Length == 0)) Logger.Log(final, LogLevel.Debug);
            BuildManager.asmData.Append(data);
            BuildManager.asmText.Append(text);
            BuildManager.asmBss.Append(bss);
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
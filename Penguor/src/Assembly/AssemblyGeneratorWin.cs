using System.Collections.Generic;
using System.Text;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.IR;

namespace Penguor.Compiler.Assembly
{
    public class AssemblyGeneratorWin : IAssemblyGenerator
    {
        public IRProgram Program { get; }

        public Builder Builder { get; }

        readonly StringBuilder pre = new StringBuilder();
        readonly StringBuilder text = new StringBuilder();
        readonly StringBuilder data = new StringBuilder();

        int i = 0;
        List<IRStatement> stmts;
        public AssemblyGeneratorWin(IRProgram program, Builder builder)
        {
            Program = program;
            Builder = builder;
            stmts = program.Statements;
        }

        /// <inheritdoc/>
        public void Generate()
        {
            pre.AppendLine("global main");
            pre.AppendLine("extern printf");

            foreach (var i in Program.Statements)
            {
                switch (i.Code)
                {
                    case OPCode.DEFINT:
                        data.Append(i.Operands[0]).Append(" dd ").Append(i.Operands[1]);
                        break;
                    case OPCode.DEFSTR:
                        data.Append(i.Operands[0]).Append(" db ").Append(i.Operands[1]).Append(", 0");
                        break;
                    default: continue;
                }
                data.AppendLine();
            }

            for (i = 0; i < stmts.Count; i++)
            {
                switch (stmts[i].Code)
                {
                    case OPCode.LABEL:
                        text.Append(stmts[i].Operands[0]).AppendLine(":");
                        if (stmts[i].Operands[0] == "print")
                            text.Append("CALL printf");
                        break;
                    case OPCode.LIB:
                        text.Append("; ").Append(stmts[i].Operands[0]);
                        break;
                    case OPCode.USE:
                        break;
                    case OPCode.LOAD:
                        text.Append("MOV rax, ").AppendJoin(' ', stmts[i].Operands);
                        break;
                    case OPCode.LOADPARAM:
                    case OPCode.DEF:
                    case OPCode.DFE:
                    case OPCode.ASSIGN:
                        break;
                    case OPCode.CALL:
                        GenericCall();
                        break;
                    case OPCode.LOADARG:
                        CallFunction();
                        break;
                    case OPCode.RETURN:
                        text.Append("RET");
                        break;
                    case OPCode.ADD:
                    case OPCode.SUB:
                    case OPCode.MUL:
                    case OPCode.DIV:
                    case OPCode.LESS:
                    case OPCode.GREATER:
                    case OPCode.JTR:
                        break;
                }
                text.AppendLine();
            }

            string final = "\n" + pre.ToString() + "\nsection .data\n\n" + data.ToString() + "\nsection .text\n\n" + text.ToString();
            Logger.Log(final, LogLevel.Debug);
        }

        private void GenericCall()
        {

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
                        text.AppendLine("MOV rax, ").AppendLine(stmts[i].Operands[0]);
                        break;
                    case OPCode.CALL:
                        text.Append("CALL ").AppendLine(stmts[i].Operands[0]);
                        return;
                    default:
                        throw new System.Exception();
                }
                i++;
            }
        }

        private void Advance()
        {
            if (i < stmts.Count) i++;
            else throw new System.Exception();
        }
    }
}
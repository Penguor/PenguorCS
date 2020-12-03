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

        public AssemblyGeneratorWin(IRProgram program, Builder builder)
        {
            Program = program;
            Builder = builder;
        }

        /// <inheritdoc/>
        public void Generate()
        {
            StringBuilder text = new StringBuilder();
            StringBuilder data = new StringBuilder();

            foreach (var i in Program.Statements)
            {
                switch (i.Code)
                {
                    case OPCode.DEFINT:
                        data.Append(i.Operands[0]).Append(" dd ").Append(i.Operands[1]);
                        break;
                    case OPCode.DEFSTR:
                        data.Append(i.Operands[0]).Append(" db ").Append(i.Operands[1]).Append(" 0");
                        break;
                    default: continue;
                }
                data.AppendLine();
            }

            foreach (var i in Program.Statements)
            {
                switch (i.Code)
                {
                    case OPCode.LABEL:
                        text.Append(i.Operands[0]).Append(':');
                        break;
                    case OPCode.LIB:
                        text.Append("; ").Append(i.Operands[0]);
                        break;
                    case OPCode.USE:
                        break;
                    case OPCode.LOAD:
                        text.Append("MOV r10 ").AppendJoin(' ', i.Operands);
                        break;
                    case OPCode.LOADARG:
                    case OPCode.LOADPARAM:
                    case OPCode.DEF:
                    case OPCode.DFE:
                    case OPCode.ASSIGN:
                    case OPCode.CALL:
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

            string final = "\nsection .data\n\n" + data.ToString() + "\nsection .text\n\n" + text.ToString();
            Logger.Log(final, LogLevel.Debug);
        }
    }
}
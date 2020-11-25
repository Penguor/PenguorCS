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

        public void Generate()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var i in Program.Statements)
            {
                switch (i.Code)
                {
                    case OPCode.LABEL:
                        builder.Append(i.Operands[0]);
                        break;
                    case OPCode.LIB:
                        builder.Append($"; {i.Operands[0]}");
                        break;
                    case OPCode.USE:
                    case OPCode.LOAD:
                    case OPCode.LOADARG:
                    case OPCode.LOADPARAM:
                    case OPCode.DEF:
                    case OPCode.DFE:
                    case OPCode.ASSIGN:
                    case OPCode.CALL:
                    case OPCode.RETURN:
                    case OPCode.ADD:
                    case OPCode.SUB:
                    case OPCode.MUL:
                    case OPCode.DIV:
                    case OPCode.LESS:
                    case OPCode.GREATER:
                    case OPCode.JTR:
                    default:
                        break;
                }
            }

            Debug.Log(builder.ToString(), LogLevel.Debug);
        }
    }
}
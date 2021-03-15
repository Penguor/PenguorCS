using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmInstructionAmd64 : IEmitter
    {
        AsmMnemonicAmd64 OPCode { get; set; }
        AsmOperand[] Operands { get; set; }

        public AsmInstructionAmd64(AsmMnemonicAmd64 opCode, params AsmOperand[] operands)
        {
            OPCode = opCode;
            Operands = operands;
        }

        public virtual string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            builder.Append(OPCode.ToString());
            builder.Append(' ');
            foreach (var i in Operands)
            {
                builder.Append(i.Emit(syntax));
                builder.Append(", ");
            }
            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }
    }
}
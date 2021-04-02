using System;
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
            for (int i = 0; i < Operands.Length; i++)
            {
                builder.Append(Operands[i].Emit(syntax));
                if (i != Operands.Length - 1) builder.Append(", ");
            }

            return builder.ToString();
        }
    }
}
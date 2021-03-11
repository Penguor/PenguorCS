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
            //todo: implement
            throw new System.NotImplementedException();
        }
    }
}
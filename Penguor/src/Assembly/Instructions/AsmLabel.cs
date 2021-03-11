using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmLabelAmd64 : AsmInstructionAmd64
    {
        State Label { get; init; }

        public AsmLabelAmd64(State label) : base(AsmMnemonicAmd64.LABEL)
        {
            Label = label;
        }

        public override string Emit(AsmSyntax syntax)
        {
            return Label.ToString() + ':';
        }
    }
}
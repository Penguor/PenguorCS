using System;
using System.Text;
using Penguor.Compiler.IR;

namespace Penguor.Compiler.Assembly
{
    public class AsmRawInstructionAmd64 : AsmInstructionAmd64
    {
        string Value { get; init; }

        public AsmRawInstructionAmd64(string value) : base((AsmMnemonicAmd64)Enum.Parse(typeof(AsmMnemonicAmd64), value.Split(' ')[0].ToUpper()))
        {
            Value = value;
        }

        public override string Emit(AsmSyntax syntax)
        {
            return Value;
        }
    }
}
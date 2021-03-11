using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmFunctionAmd64 : IEmitter
    {
        public string Name { get; set; }
        List<AsmInstructionAmd64> Instructions { get; }

        public AsmFunctionAmd64(string name)
        {
            Name = name;
            Instructions = new();
        }

        public void AddInstruction(AsmInstructionAmd64 instruction) => Instructions.Add(instruction);
        public void AddInstruction(AsmMnemonicAmd64 opCode, params AsmOperand[] operands) => Instructions.Add(new AsmInstructionAmd64(opCode, operands));

        public string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            foreach (var instruction in Instructions)
            {
                builder.AppendLine(instruction.Emit(syntax));
            }
            return builder.ToString();
        }
    }
}
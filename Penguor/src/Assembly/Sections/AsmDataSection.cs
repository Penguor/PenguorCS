using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmDataSection : AsmSection
    {
        public List<AsmVariableAmd64> Variables { get; }

        public AsmDataSection() : base("text")
        {
            Variables = new();
        }

        public void AddFunction(AsmVariableAmd64 function) => Variables.Add(function);

        public override string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            builder.AppendLine("section .data\n");

            foreach (var variable in Variables)
            {
                builder.AppendLine(variable.Emit(syntax));
            }
            return builder.ToString();
        }
    }
}
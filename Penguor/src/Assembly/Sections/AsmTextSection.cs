using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmTextSection : AsmSection
    {
        public List<AsmFunctionAmd64> Functions { get; }

        public AsmTextSection() : base("text")
        {
            Functions = new();
        }

        public void AddFunction(AsmFunctionAmd64 function) => Functions.Add(function);

        public override string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            builder.AppendLine("section .text\n");

            foreach (var function in Functions)
            {
                builder.AppendLine(function.Emit(syntax));
            }
            return builder.ToString();
        }
    }
}
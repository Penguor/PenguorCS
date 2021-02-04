using System.Collections.Generic;

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
            throw new System.NotImplementedException();
        }
    }
}
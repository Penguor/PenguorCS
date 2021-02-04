using System.Collections.Generic;

namespace Penguor.Compiler.Assembly
{
    public class AsmProgram : IEmitter
    {
        public List<string> GlobalLabels { get; }

        public AsmTextSection Text { get; set; }

        public AsmProgram()
        {
            GlobalLabels = new();

            Text = new();
        }

        public void AddGlobalLabel(string label) => GlobalLabels.Add(label);

        public string Emit(AsmSyntax syntax)
        {
            throw new System.NotImplementedException();
        }
    }
}
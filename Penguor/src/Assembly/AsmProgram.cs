using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmProgram : IEmitter
    {
        public List<State> GlobalLabels { get; }

        public AsmTextSection Text { get; set; }

        public AsmProgram()
        {
            GlobalLabels = new();

            Text = new();
        }

        public void AddGlobalLabel(State label) => GlobalLabels.Add(label);

        public string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            builder.AppendJoin(", ", GlobalLabels).AppendLine();
            builder.AppendLine(Text.Emit(syntax));
            return builder.ToString();
        }
    }
}
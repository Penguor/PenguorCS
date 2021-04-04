using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.Assembly
{
    public class AsmProgram : IEmitter
    {
        public List<State> GlobalLabels { get; }
        public List<State> Externs { get; }

        public AsmTextSection Text { get; set; }
        public AsmDataSection Data { get; set; }

        public AsmProgram()
        {
            GlobalLabels = new();
            Externs = new();

            Text = new();
            Data = new();
        }

        public void AddGlobalLabel(State label) => GlobalLabels.Add(label);
        public void AddExtern(State externName) => Externs.Add(externName);

        public string Emit(AsmSyntax syntax)
        {
            StringBuilder builder = new();
            if (GlobalLabels.Count > 0)
            {
                builder.Append("global ");
                builder.AppendJoin(", ", GlobalLabels).AppendLine();
            }
            if (Externs.Count > 0)
            {
                builder.Append("extern ");
                builder.AppendJoin(", ", Externs).AppendLine();
            }

            builder.AppendLine(Data.Emit(syntax));
            builder.AppendLine(Text.Emit(syntax));
            return builder.ToString();
        }
    }
}
using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.IR
{
    public record IRFunction
    {
        public List<IRStatement> Statements { get; init; } = new();
        public State Name { get; init; }

        public IRFunction(State name)
        {
            Name = name;
        }

        public void Add(IRStatement statement) => Statements.Add(statement);

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine();
            foreach (var i in Statements)
                builder.AppendLine(i.ToString());
            return builder.ToString();
        }
    }
}
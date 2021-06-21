using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// the base of a program
    /// </summary>
    public class IRProgram
    {
        /// <summary>
        /// The child declarations of the program
        /// </summary>
        public List<IRFunction> Functions { get; init; } = new();
        public List<IRStatement> GlobalStatements { get; init; } = new();

        /// <summary>
        /// append a new statement to the end of the list
        /// </summary>
        /// <param name="function"></param>
        public void Add(IRFunction function) => Functions.Add(function);

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine();
            foreach (var i in GlobalStatements)
                builder.AppendLine(i.ToString());
            builder.AppendLine();
            foreach (var i in Functions)
                builder.AppendLine(i.ToString());
            return builder.ToString();
        }
    }
}
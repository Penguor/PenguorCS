

using System;
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
        public List<IRStatement> Statements { get; init; }

        /// <summary>
        /// Initialize a new instance of IRProgram
        /// </summary>
        public IRProgram()
        {
            Statements = new();
        }

        /// <summary>
        /// Initialize a new instance of IRProgram
        /// </summary>
        public IRProgram(IEnumerable<IRStatement> statements)
        {
            Statements = new(statements);
        }

        /// <summary>
        /// append a new statement to the end of the list
        /// </summary>
        /// <param name="stmt"></param>
        public void Add(IRStatement stmt) => Statements.Add(stmt);

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
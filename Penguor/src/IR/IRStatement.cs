using System.Collections.Generic;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// A statement modifying some data
    /// </summary>
    public record IRStatement
    {
        /// <summary>
        /// the instrution number
        /// </summary>
        public int Number { get; init; }

        /// <summary>
        /// the OPCode of this statement
        /// </summary>
        public IROPCode Code { get; init; }

        /// <summary>
        /// the operands which get passed to the OPCode
        /// </summary>
        public IRArgument[] Operands { get; init; }

        /// <summary>
        /// Initializes a new instance of the IRStatement
        /// </summary>
        /// <param name="number">the instrution number</param>
        /// <param name="code">the OPCode of this IRStatement</param>
        /// <param name="operands">the operands of this IRStatement</param>
        public IRStatement(int number, IROPCode code, params IRArgument[] operands)
        {
            Number = number;
            Code = code;
            Operands = operands;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Code is IROPCode.LABEL or IROPCode.LIB or IROPCode.FUNC)
                return $"({Number:D4}) {Code} {Operands[0]}{':'}";
            return $"({Number:D4})     {Code} {string.Join(' ', (IEnumerable<IRArgument>)Operands)}";
        }
    }
}
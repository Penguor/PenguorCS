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
        public uint Number { get; init; }

        /// <summary>
        /// the OPCode of this statement
        /// </summary>
        public OPCode Code { get; init; }

        /// <summary>
        /// the operands which get passed to the OPCode
        /// </summary>
        public IRArgument[] Operands { get; init; }

        /// <summary>
        /// whether the value of the ir argument is used later on
        /// </summary>
        public bool GetsReferenced { get; set; }

        /// <summary>
        /// get the register where the value is stored if GetsReferenced is true
        /// </summary>
        public string? GetRegister { get; set; }

        /// <summary>
        /// Initializes a new instance of the IRStatement
        /// </summary>
        /// <param name="number">the instrution number</param>
        /// <param name="code">the OPCode of this IRStatement</param>
        /// <param name="operands">the operands of this IRStatement</param>
        public IRStatement(uint number, OPCode code, params IRArgument[] operands)
        {
            Number = number;
            Code = code;
            Operands = operands;
            GetsReferenced = false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Code is OPCode.LABEL or OPCode.LIB or OPCode.FUNC)
                return $"({Number:D4}) {Code} {Operands[0]}{':'}";
            return $"({Number:D4})     {Code} {string.Join(' ', (IEnumerable<IRArgument>)Operands)}";
        }
    }
}
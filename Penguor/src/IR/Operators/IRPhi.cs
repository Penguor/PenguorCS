using System.Collections.Generic;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRPhi : IRArgument
    {
        public List<IRReference> Operands { get; } = new();
        public List<IRReference> Users { get; } = new();
        public BlockID Block { get; }

        public IRPhi(BlockID block)
        {
            Block = block;
        }

        /// <summary>
        /// adds a new operand to the phi function
        /// </summary>
        /// <param name="variable">the operand to add</param>
        public void AppendOperand(IRReference variable) => Operands.Add(variable);

        /// <inheritdoc/>
        public override string ToString() => $"Φ({string.Join(", ", Operands)})";
    }
}
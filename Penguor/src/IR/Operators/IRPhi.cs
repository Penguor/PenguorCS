using System.Collections.Generic;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRPhi : IRArgument
    {
        public HashSet<IRReference> Operands { get; } = new();
        public List<IRReference> Users { get; } = new();
        public State Block { get; }

        public IRPhi(State block)
        {
            Block = block;
        }

        /// <summary>
        /// adds a new operand to the phi function
        /// </summary>
        /// <param name="variable">the operand to add</param>
        public void AppendOperand(IRReference variable)
        {
            if (!Operands.Contains(variable))
                Operands.Add(variable);
        }

        public void AddUser(IRReference user) => Users.Add(user);

        /// <inheritdoc/>
        public override string ToString() => $"Φ({string.Join(", ", Operands)})";
    }
}
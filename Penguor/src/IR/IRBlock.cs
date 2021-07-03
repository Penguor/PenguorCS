using System;
using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// A basic block in penguor ir code
    /// </summary>
    public class IRBlock
    {
        /// <summary>
        /// the id of the block, used to uniquely identify it
        /// </summary>
        public State ID { get; set; }
        /// <summary>
        /// a list of all direct predecessors of a block
        /// </summary>
        public List<State> Predecessors { get; } = new();

        /// <summary>
        /// the <see cref="IRReference"/> of the first statement the block
        /// </summary>
        public IRReference? First { get; set; }

        public Dictionary<IRReference, IRStatement> Phis { get; } = new();

        public IndexedDictionary<IRReference, IRStatement> Statements { get; } = new();

        /// <summary>
        /// creates a new instance of IRBlock
        /// </summary>
        /// <param name="id">the id used to identify this IRBlock</param>
        public IRBlock(State id)
        {
            ID = id;
        }

        /// <summary>
        /// Adds a predecessor to the list
        /// </summary>
        /// <param name="pred">the id of the predecessor</param>
        public void AddPredecessor(State pred) => Predecessors.Add(pred);

        public void AddStmt(int instructionNumber, IROPCode code, params IRArgument[] operands) => Statements.Add(new IRReference(instructionNumber), new IRStatement(instructionNumber, code, operands));
        public void AddPhi(int instructionNumber, IRPhi phi) => Phis.Add(new IRReference(instructionNumber), new IRStatement(instructionNumber, IROPCode.PHI, phi));

        public void Replace(IRReference oldRef, IRReference newRef)
        {
            foreach ((_, var phi) in Phis)
            {
                for (int i = 0; i < phi.Operands.Length; i++)
                {
                    if (phi.Operands[i] is IRPhi irPhi && irPhi.Operands.Remove(oldRef))
                        irPhi.AppendOperand(newRef);
                }
            }
            foreach ((_, var statement) in Statements)
            {
                for (int i = 0; i < statement.Operands.Length; i++)
                {
                    if (statement.Operands[i] is IRPhi irPhi && irPhi.Operands.Remove(oldRef))
                        irPhi.AppendOperand(newRef);
                    else if (statement.Operands[i] is IRReference reference && reference == oldRef)
                        statement.Operands[i] = newRef;
                }
            }
            if (!Phis.Remove(oldRef)) Statements.Remove(oldRef);
        }

        /// <summary>
        /// gets a statement or phi by the ir reference
        /// prioritises IRPhis, but there should never be a duplicate instruction number in an ir block
        /// </summary>
        public IRStatement this[IRReference index]
        {
            get => Phis.ContainsKey(index) ? Phis[index] : Statements.ContainsKey(index) ? Statements[index] : throw new KeyNotFoundException($"the statement or phi with the value {index} could not be found");
            set
            {
                if (Phis.ContainsKey(index))
                    Phis[index] = value;
                else if (Statements.ContainsKey(index))
                    Statements[index] = value;
                else
                    throw new KeyNotFoundException($"the statement or phi with the value {index} could not be found");
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj is IRBlock block)
                return ID.Equals(block.ID);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 254837322 + ID.GetHashCode();
            foreach (var i in Predecessors)
                hashCode *= -4562385 + i.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (var i in Phis)
            {
                builder.AppendLine(i.ToString());
            }
            foreach ((_, var i) in Statements)
            {
                builder.AppendLine(i.ToString());
            }
            return builder.ToString();
        }
    }
}
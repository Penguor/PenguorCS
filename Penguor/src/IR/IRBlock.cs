using System;
using System.Collections.Generic;

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
    }
}
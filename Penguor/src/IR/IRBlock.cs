using System;
using System.Collections.Generic;

namespace Penguor.Compiler.IR
{
    public class IRBlock
    {
        public BlockID ID { get; set; }
        public List<BlockID> Predecessors { get; } = new();

        public IRBlock(BlockID id)
        {
            ID = id;
        }

        public void AddPredecessor(BlockID pred) => Predecessors.Add(pred);


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

        public override int GetHashCode()
        {
            var hashCode = 254837322 + ID.GetHashCode();
            foreach (var i in Predecessors)
                hashCode *= -4562385 + i.GetHashCode();
            return hashCode;
        }
    }

    public record BlockID(int ID, State State);
}
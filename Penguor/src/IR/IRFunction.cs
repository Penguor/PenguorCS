using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Penguor.Compiler.IR
{
    public record IRFunction
    {
        public List<IRStatement> Statements { get; init; } = new();
        public State Name { get; init; }

        private int instructionNumber;
        private int InstructionNumber { get => ++instructionNumber; }

        // ssa stuff
        public Dictionary<State, IRBlock> Blocks { get; } = new();
        public State? CurrentBlock { get; set; }
        private readonly List<State> sealedBlocks = new();

        // phis
        private readonly Dictionary<State, Dictionary<State, IRReference>> incompletePhis = new();

        private readonly Dictionary<State, Dictionary<State, IRReference>> currentDefinition = new();

        public IRFunction(State name)
        {
            Name = name;
        }

        public void ResolveReroutes()
        {
            int hitCount = 1;
            while (hitCount > 0)
            {
                hitCount = 0;
                for (int i = 0; i < Statements.Count; i++)
                {
                    if (Statements[i].Code == IROPCode.REROUTE)
                    {
                        IRReference rerouteReference = new(Statements[i].Number);
                        IRReference newReference = (IRReference)Statements[i].Operands[0];
                        for (int j = 0; j < Statements.Count; j++)
                        {
                            if (Statements[i].Code != Statements[j].Code)
                            {
                                for (int k = 0; k < Statements[j].Operands.Length; k++)
                                {
                                    if (Statements[j].Operands[k] is IRReference reference && reference.Equals(rerouteReference))
                                    {
                                        hitCount++;
                                        Statements[j].Operands[k] = newReference with { };
                                    }
                                    else if (Statements[j].Operands[k] is IRPhi newPhi && newPhi.Operands.Contains(rerouteReference))
                                    {
                                        hitCount++;
                                        newPhi.Operands.Remove(rerouteReference);
                                        newPhi.Operands.Add(newReference);
                                    }
                                }
                            }
                        }
                    }
                    else if (Statements[i].Code == IROPCode.PHI && Statements[i].Operands[0] is IRPhi phi && phi.Operands.Count == 1)
                    {
                        IRReference rerouteReference = new(Statements[i].Number);
                        foreach (var newReference in phi.Operands)
                        {
                            for (int j = 0; j < Statements.Count; j++)
                            {
                                if (Statements[i].Code != Statements[j].Code)
                                {
                                    for (int k = 0; k < Statements[j].Operands.Length; k++)
                                    {
                                        if (Statements[j].Operands[k] is IRReference reference && reference.Equals(rerouteReference))
                                        {
                                            hitCount++;
                                            Statements[j].Operands[k] = newReference with { };
                                        }
                                        else if (Statements[j].Operands[k] is IRPhi newPhi && newPhi.Operands.Contains(rerouteReference))
                                        {
                                            hitCount++;
                                            newPhi.Operands.Remove(rerouteReference);
                                            newPhi.Operands.Add(newReference);
                                        }
                                    }
                                }
                            }
                            Statements[i] = Statements[i] with { Code = IROPCode.REROUTE, Operands = new IRArgument[] { newReference with { } } };
                        }
                    }
                    else if (Statements[i].Code == IROPCode.PHI && Statements[i].Operands[0] is IRPhi phi1 && phi1.Operands.Any(s => s.Referenced == Statements[i].Number))
                    {
                        hitCount++;
                        phi1.Operands.RemoveWhere(op => op == new IRReference(Statements[i].Number));
                    }
                }
            }

            Statements.RemoveAll(s => s.Code == IROPCode.REROUTE || (s.Code == IROPCode.PHI && s.Operands[0] is IRPhi phi && phi.Operands.Count == 1));
        }

        //ssa related stuff
        // paper: Simple and Efficient Construction of Static Single Assignment Form, Braun et al.
        // https://pp.info.uni-karlsruhe.de/uploads/publikationen/braun13cc.pdf

        // blocks

        public State BeginBlock(State block)
        {
            if (!Blocks.ContainsKey(block))
            {
                AddBlock(block);
            }

            CurrentBlock = (State)block.Clone();

            return CurrentBlock;
        }

        public State AddBlock(State block)
        {
            var id = (State)block.Clone();
            Blocks.Add(id, new IRBlock(id));
            incompletePhis.Add(id, new());
            return id;
        }

        public void SealBlock(State block)
        {
            foreach ((var variable, _) in incompletePhis[block])
            {
                AddPhiOperands(variable, incompletePhis[block][variable]);
            }
            sealedBlocks.Add(block);
        }

        public IRBlock GetCurrentBlock() => Blocks[CurrentBlock ?? throw new NullReferenceException("no active block")];

        public IRBlock FindBlock(IRReference reference)
        {
            foreach ((_, var block) in Blocks)
            {
                if (block.Statements.ContainsKey(reference) || block.Phis.ContainsKey(reference)) return block;
            }
            throw new NullReferenceException($"no block containing {reference}");
        }

        // variables

        public void WriteVariable(State variable, State block, IRReference value)
        {
            currentDefinition.TryAdd(variable, new());
            currentDefinition[variable][block] = value;
        }

        public IRReference ReadVariable(State variable, State block)
        {
            if (currentDefinition[variable].ContainsKey(block))
                return currentDefinition[variable][block];
            else
                return ReadVariableRecursive(variable, block);
        }

        private IRReference ReadVariableRecursive(State variable, State block)
        {
            var tempBlock = (State)block.Clone();

            IRReference val;
            if (!sealedBlocks.Contains(tempBlock))
            {
                val = AddPhi(new IRPhi(tempBlock));
                incompletePhis[tempBlock][variable] = val;
            }
            else if (Blocks[tempBlock].Predecessors.Count == 1)
            {
                val = ReadVariable(variable, Blocks[tempBlock].Predecessors[0]);
            }
            else
            {
                val = AddPhi(new IRPhi(tempBlock));
                WriteVariable(variable, tempBlock, val);
            }
            return val;
        }

        private IRReference AddPhiOperands(State variable, IRReference phi)
        {
            var block = FindBlock(phi);
            foreach (var pred in block.Predecessors)
            {
                ((IRPhi)block[phi].Operands[0]).AppendOperand(ReadVariable(variable, pred));
            }
            return TryRemoveTrivialPhi(phi);
        }

        private IRReference TryRemoveTrivialPhi(IRReference phi)
        {
            IRPhi irPhi = (IRPhi)FindBlock(phi)[phi].Operands[0];
            IRReference? same = null;
            foreach (var op in irPhi.Operands)
            {
                if (op == same || op == phi)
                {
                    continue;
                }
                if (same != null)
                {
                    return phi;
                }
                same = op;
            }
            if (same == null)
            {
                same = new IRReference(-1);
            }
            irPhi.Users.Remove(phi);
            Replace(phi, same);

            foreach (var use in irPhi.Users)
            {
                if (this[use].Operands[0] is IRPhi)
                {
                    TryRemoveTrivialPhi(use);
                }
            }
            return same;
        }

        // statements

        public IRReference AddStmt(IROPCode code, params IRArgument[] operands)
        {
            var num = InstructionNumber;
            GetCurrentBlock().AddStmt(num, code, operands);
            foreach (var operand in operands)
            {
                if (operand is IRReference reference && this[reference].Operands[0] is IRPhi phi)
                {
                    phi.AddUser(new IRReference(num));
                }
            }
            return new IRReference(num);
        }

        public IRReference AddPhi(IRPhi phi)
        {
            var num = InstructionNumber;
            GetCurrentBlock().AddPhi(InstructionNumber, phi);
            return new IRReference(num);
        }

        public IRStatement GetLastStatement() => GetCurrentBlock().Statements[^1];

        public int GetInstructionNumber() => instructionNumber;

        public void Replace(IRReference oldRef, IRReference newRef)
        {
            foreach ((_, var block) in Blocks)
            {
                block.Replace(oldRef, newRef);
            }
        }

        public IRStatement this[IRReference index]
        {
            get => FindBlock(index)[index];
            set
            {
                IRBlock? block;
                if ((block = FindBlock(index)) is not null)
                    block[index] = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine();
            foreach ((_, var i) in Blocks)
                builder.AppendLine(i.ToString());
            return builder.ToString();
        }
    }
}
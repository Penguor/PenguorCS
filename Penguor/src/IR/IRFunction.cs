using System;
using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.IR
{
    public record IRFunction
    {
        public IRReference First { get; init; }
        public List<IRStatement> Statements { get; init; } = new();
        public State Name { get; init; }

        public IRFunction(State name, IRReference first)
        {
            Name = name;
            First = first;
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
                }
            }

            Statements.RemoveAll(s => s.Code == IROPCode.REROUTE || (s.Code == IROPCode.PHI && s.Operands[0] is IRPhi phi && phi.Operands.Count == 1));
        }

        public void Add(IRStatement statement) => Statements.Add(statement);

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
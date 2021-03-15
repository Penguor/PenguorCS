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
            List<int> toRemove = new();
            foreach (var statement in Statements)
            {
                if (statement.Code == IROPCode.REROUTE)
                {
                    IRReference rerouteReference = new IRReference(statement.Number);
                    IRReference newReference = (IRReference)statement.Operands[0];
                    for (int i = 0; i < Statements.Count; i++)
                    {
                        if (statement.Code != Statements[i].Code)
                        {
                            for (int i2 = 0; i2 < Statements[i].Operands.Length; i2++)
                            {
                                if (Statements[i].Operands[i2] is IRReference reference && reference.Equals(rerouteReference))
                                {
                                    Statements[i].Operands[i2] = newReference with { };
                                }
                                else if (Statements[i].Operands[i2] is IRPhi newPhi && newPhi.Operands.Contains(rerouteReference))
                                {
                                    newPhi.Operands[newPhi.Operands.FindIndex(op => op.Equals(rerouteReference))] = newReference with { };
                                    newPhi.Operands.Remove(new IRReference(Statements[i].Number));
                                }
                            }
                        }
                    }
                    toRemove.Add(Statements.IndexOf(statement));
                }
                else if (statement.Code == IROPCode.PHI && statement.Operands[0] is IRPhi phi && phi.Operands.Count == 1)
                {
                    IRReference rerouteReference = new IRReference(statement.Number);
                    IRReference newReference = phi.Operands[0];
                    for (int i = 0; i < Statements.Count; i++)
                    {
                        if (statement.Code != Statements[i].Code)
                        {
                            for (int i2 = 0; i2 < Statements[i].Operands.Length; i2++)
                            {
                                if (Statements[i].Operands[i2] is IRReference reference && reference.Equals(rerouteReference))
                                {
                                    Statements[i].Operands[i2] = newReference with { };
                                }
                                else if (Statements[i].Operands[i2] is IRPhi newPhi && newPhi.Operands.Contains(rerouteReference))
                                {
                                    newPhi.Operands[newPhi.Operands.FindIndex(op => op.Equals(rerouteReference))] = newReference with { };
                                    newPhi.Operands.Remove(new IRReference(Statements[i].Number));
                                }
                            }
                        }
                    }
                    toRemove.Add(Statements.IndexOf(statement));
                }
            }
            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                Statements.RemoveAt(toRemove[i]);
            }
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
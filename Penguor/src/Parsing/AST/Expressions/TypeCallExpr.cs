#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A TypeCall Expr
    /// </summary>
    public sealed record TypeCallExpr : Expr
    {
        /// <summary>
        /// creates a new instance of TypeCallExpr
        /// </summary>
        public TypeCallExpr(int id, int offset, State name, List<uint> dimensions)
        {
            Id = id;
            Offset = offset;
            Name = name;
            Dimensions = dimensions;
        }
        public State Name { get; init; }
        public List<uint> Dimensions { get; init; }

        public override string ToString() => "typecall expression";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Expressions
    /// </summary>
    public partial interface IExprVisitor<T>
    {
        /// <summary>
        /// visit a TypeCallExpr
        /// </summary>
        T Visit(TypeCallExpr expr);
    }
}

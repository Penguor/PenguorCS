
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Assign Expr
    /// </summary>
    public sealed record AssignExpr : Expr
    {
        /// <summary>
        /// creates a new instance of AssignExpr
        /// </summary>
        public AssignExpr(int id, int offset, CallExpr lhs, TokenType op, Expr value)
        {
            Id = id;
            Offset = offset;
            Lhs = lhs;
            Op = op;
            Value = value;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public CallExpr Lhs { get; init; }
        public TokenType Op { get; init; }
        public Expr Value { get; init; }


        public override string ToString() => "assign expression";

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
        /// visit a AssignExpr
        /// </summary>
        T Visit(AssignExpr expr);
    }
}

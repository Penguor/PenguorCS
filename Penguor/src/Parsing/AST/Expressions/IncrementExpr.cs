#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Increment Expr
    /// </summary>
    public sealed record IncrementExpr : Expr
    {
        /// <summary>
        /// creates a new instance of IncrementExpr
        /// </summary>
        public IncrementExpr(int id, int offset, CallExpr child, TokenType postfix)
        {
            Id = id;
            Offset = offset;
            Child = child;
            Postfix = postfix;
        }
        public CallExpr Child { get; init; }
        public TokenType Postfix { get; init; }

        public override string ToString() => "increment expression";

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
        /// visit a IncrementExpr
        /// </summary>
        T Visit(IncrementExpr expr);
    }
}

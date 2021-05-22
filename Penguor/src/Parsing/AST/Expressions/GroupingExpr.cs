#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Grouping Expr
    /// </summary>
    public sealed record GroupingExpr : Expr
    {
        /// <summary>
        /// creates a new instance of GroupingExpr
        /// </summary>
        public GroupingExpr(int id, int offset, Expr content)
        {
            Id = id;
            Offset = offset;
            Content = content;
        }
        public Expr Content { get; init; }

        public override string ToString() => "grouping expression";

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
        /// visit a GroupingExpr
        /// </summary>
        T Visit(GroupingExpr expr);
    }
}

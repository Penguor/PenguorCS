
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A String Expr
    /// </summary>
    public sealed record StringExpr : Expr
    {
        /// <summary>
        /// creates a new instance of StringExpr
        /// </summary>
        public StringExpr(int id, int offset, string value)
        {
            Id = id;
            Offset = offset;
            Value = value;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public string Value { get; init; }

        public override string ToString() => "string expression";

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
        /// visit a StringExpr
        /// </summary>
        T Visit(StringExpr expr);
    }
}

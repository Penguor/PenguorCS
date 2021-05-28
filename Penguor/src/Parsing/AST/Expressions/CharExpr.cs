#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Char Expr
    /// </summary>
    public sealed record CharExpr : Expr
    {
        /// <summary>
        /// creates a new instance of CharExpr
        /// </summary>
        public CharExpr(int id, int offset, char value)
        {
            Id = id;
            Offset = offset;
            Value = value;
        }
        public char Value { get; init; }

        public override string ToString() => "char expression";

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
        /// visit a CharExpr
        /// </summary>
        T Visit(CharExpr expr);
    }
}

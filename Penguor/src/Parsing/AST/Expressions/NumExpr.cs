#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Num Expr
    /// </summary>
    public sealed record NumExpr : Expr
    {
        /// <summary>
        /// creates a new instance of NumExpr
        /// </summary>
        public NumExpr(int id, int offset, int numbase, string value, double? numvalue)
        {
            Id = id;
            Offset = offset;
            NumBase = numbase;
            Value = value;
            NumValue = numvalue;
        }
        public int NumBase { get; init; }
        public string Value { get; init; }
        public double? NumValue { get; init; }

        public override string ToString() => "num expression";

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
        /// visit a NumExpr
        /// </summary>
        T Visit(NumExpr expr);
    }
}

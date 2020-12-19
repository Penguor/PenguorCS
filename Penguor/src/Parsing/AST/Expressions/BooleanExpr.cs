/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Boolean Expr
    /// </summary>
    public sealed record BooleanExpr : Expr
    {
        /// <summary>
        /// creates a new instance of BooleanExpr
        /// </summary>
        public BooleanExpr(int id, int offset, bool value)
        {
            Id = id;
            Offset = offset;
            Value = value;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public bool Value { get; init; }


        public override string ToString() => "boolean expression";

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
        /// visit a BooleanExpr
        /// </summary>
        T Visit(BooleanExpr expr);
    }
}

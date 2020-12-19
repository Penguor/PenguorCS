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
    /// A Unary Expr
    /// </summary>
    public sealed record UnaryExpr : Expr
    {
        /// <summary>
        /// creates a new instance of UnaryExpr
        /// </summary>
        public UnaryExpr(int id, int offset, TokenType? op, Expr rhs)
        {
            Id = id;
            Offset = offset;
            Op = op;
            Rhs = rhs;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public TokenType? Op { get; init; }
        public Expr Rhs { get; init; }


        public override string ToString() => "unary expression";

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
        /// visit a UnaryExpr
        /// </summary>
        T Visit(UnaryExpr expr);
    }
}

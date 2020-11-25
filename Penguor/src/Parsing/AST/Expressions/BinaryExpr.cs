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
    /// A BinaryExpr Expr
    /// </summary>
    public sealed record BinaryExpr : Expr
    {
        /// <summary>
        /// creates a new instance of BinaryExpr
        /// </summary>
        public BinaryExpr(int offset, Expr lhs, TokenType op, Expr rhs)
        {
            Offset = offset;
            Lhs = lhs;
            Op = op;
            Rhs = rhs;
        }
        public int Offset { get; }
        public Expr Lhs { get; }
        public TokenType Op { get; }
        public Expr Rhs { get; }

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
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface IExprVisitor<T>
    {
        /// <summary>
        /// visit a BinaryExpr
        /// </summary>
        T Visit(BinaryExpr expr);
    }
}

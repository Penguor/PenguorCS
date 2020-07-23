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
    /// A UnaryExpr Expr
    /// </summary>
    public sealed class UnaryExpr : Expr
    {
        /// <summary>
        /// creates a new instance of UnaryExpr
        /// </summary>
        public UnaryExpr(TokenType? op, Expr rhs)
        {
            Op = op;
            Rhs = rhs;
        }
        public TokenType? Op { get; }
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
        /// visit a UnaryExpr
        /// </summary>
        /// <returns></returns>
        T Visit(UnaryExpr expr);
    }
}

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
    /// A AssignExpr Expr
    /// </summary>
    public sealed class AssignExpr : Expr
    {
        /// <summary>
        /// creates a new instance of AssignExpr
        /// </summary>
        public AssignExpr(int offset, CallExpr lhs, TokenType op, Expr value)
        {
            Offset = offset;
            Lhs = lhs;
            Op = op;
            Value = value;
        }
        public int Offset { get; }
        public CallExpr Lhs { get; }
        public TokenType Op { get; }
        public Expr Value { get; }

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
        /// visit a AssignExpr
        /// </summary>
        /// <returns></returns>
        T Visit(AssignExpr expr);
    }
}

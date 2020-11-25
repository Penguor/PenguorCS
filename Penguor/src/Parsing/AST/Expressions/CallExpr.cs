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
    /// A CallExpr Expr
    /// </summary>
    public sealed record CallExpr : Expr
    {
        /// <summary>
        /// creates a new instance of CallExpr
        /// </summary>
        public CallExpr(int offset, List<Call> callee, TokenType? postfix)
        {
            Offset = offset;
            Callee = callee;
            Postfix = postfix;
        }
        public int Offset { get; init; }
        public List<Call> Callee { get; init; }
        public TokenType? Postfix { get; init; }

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
        /// visit a CallExpr
        /// </summary>
        T Visit(CallExpr expr);
    }
}

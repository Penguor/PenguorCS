/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A CallExpr Expr
    /// </summary>
    public sealed class CallExpr : Expr
    {
        /// <summary>
        /// creates a new instance of CallExpr
        /// </summary>
        public CallExpr(List<Call> callee, TokenType? postfix)
        {
            Callee = callee;
            Postfix = postfix;
        }
        /// <summary></summary>
        public List<Call> Callee { get; }
        /// <summary></summary>
        public TokenType? Postfix { get; }

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
        /// <returns></returns>
        T Visit(CallExpr expr);
    }
}

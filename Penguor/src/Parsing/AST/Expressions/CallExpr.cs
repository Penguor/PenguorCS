/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
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
        public List<Call> Callee { get; private set; }
        /// <summary></summary>
        public TokenType? Postfix { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a CallExpr
        /// </summary>
        /// <returns></returns>
        T Visit(CallExpr expr);
    }
}

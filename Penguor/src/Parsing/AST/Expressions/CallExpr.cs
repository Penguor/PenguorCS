/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A CallExpr expression
    /// </summary>
    public sealed class CallExpr : Expr
    {
        /// <summary>
        /// creates a new instance of CallExpr
        /// </summary>
        public CallExpr(List<Expr> callee, List<Expr> args, TokenType? postfix)
        {
            Callee = callee;
            Args = args;
            Postfix = postfix;
        }
        /// <summary></summary>
        public List<Expr> Callee { get; private set; }
        /// <summary></summary>
        public List<Expr> Args { get; private set; }
        /// <summary></summary>
        public TokenType? Postfix { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override string Accept(Visitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all expressions
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a CallExpr
        /// </summary>
        /// <returns></returns>
        string Visit(CallExpr expr);
    }
}

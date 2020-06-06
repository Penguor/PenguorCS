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
    /// A BinaryExpr Expr
    /// </summary>
    public sealed class BinaryExpr : Expr
    {
        /// <summary>
        /// creates a new instance of BinaryExpr
        /// </summary>
        public BinaryExpr(Expr lhs, TokenType op, Expr rhs)
        {
            Lhs = lhs;
            Op = op;
            Rhs = rhs;
        }
        /// <summary></summary>
        public Expr Lhs { get; private set; }
        /// <summary></summary>
        public TokenType Op { get; private set; }
        /// <summary></summary>
        public Expr Rhs { get; private set; }

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
        /// <returns></returns>
        T Visit(BinaryExpr expr);
    }
}

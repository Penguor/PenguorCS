/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
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
        /// <summary></summary>
        public TokenType? Op { get; private set; }
        /// <summary></summary>
        public Expr Rhs { get; private set; }

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
        /// visit a UnaryExpr
        /// </summary>
        /// <returns></returns>
        T Visit(UnaryExpr expr);
    }
}

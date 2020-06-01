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
    /// A AssignExpr Expr
    /// </summary>
    public sealed class AssignExpr : Expr
    {
        /// <summary>
        /// creates a new instance of AssignExpr
        /// </summary>
        public AssignExpr(Expr lhs, TokenType op, Expr value)
        {
            Lhs = lhs;
            Op = op;
            Value = value;
        }
        /// <summary></summary>
        public Expr Lhs { get; private set; }
        /// <summary></summary>
        public TokenType Op { get; private set; }
        /// <summary></summary>
        public Expr Value { get; private set; }

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
        /// visit a AssignExpr
        /// </summary>
        /// <returns></returns>
        T Visit(AssignExpr expr);
    }
}

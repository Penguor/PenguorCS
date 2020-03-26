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
using System;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A UnaryExpr expression
    /// </summary>
    public sealed class UnaryExpr : Expr
    {
        /// <summary>
        /// creates a new instance of UnaryExpr
        /// </summary>
        public UnaryExpr(TokenType op, Expr rhs, LinkedList<Guid> id)
        {
            Op = op;
            Rhs = rhs;
            Id = id;
        }
        /// <summary></summary>
        public TokenType Op { get; private set; }
        /// <summary></summary>
        public Expr Rhs { get; private set; }
        /// <summary></summary>
        public LinkedList<Guid> Id { get; private set; }

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
        /// visit a UnaryExpr
        /// </summary>
        /// <returns></returns>
        string Visit(UnaryExpr expr);
    }
}

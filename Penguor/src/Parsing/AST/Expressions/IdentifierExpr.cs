/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# all rights reserved
# 
*/

using System.Collections.Generic;
using System;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A IdentifierExpr expression
    /// </summary>
    public sealed class IdentifierExpr : Expr
    {
        /// <summary>
        /// creates a new instance of IdentifierExpr
        /// </summary>
        public IdentifierExpr(object value, LinkedList<Guid> id)
        {
            Value = value;
            Id = id;
        }
        /// <summary></summary>
        public object Value { get; private set; }
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
        /// visit a IdentifierExpr
        /// </summary>
        /// <returns></returns>
        string Visit(IdentifierExpr expr);
    }
}

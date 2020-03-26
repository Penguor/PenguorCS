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
    /// A AssignExpr expression
    /// </summary>
    public sealed class AssignExpr : Expr
    {
        /// <summary>
        /// creates a new instance of AssignExpr
        /// </summary>
        public AssignExpr(Expr name, Expr value, LinkedList<Guid> id)
        {
            Name = name;
            Value = value;
            Id = id;
        }
        /// <summary></summary>
        public Expr Name { get; private set; }
        /// <summary></summary>
        public Expr Value { get; private set; }
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
        /// visit a AssignExpr
        /// </summary>
        /// <returns></returns>
        string Visit(AssignExpr expr);
    }
}

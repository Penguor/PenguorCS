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
    /// A GroupingExpr expression
    /// </summary>
    public sealed class GroupingExpr : Expr
    {
        /// <summary>
        /// creates a new instance of GroupingExpr
        /// </summary>
        public GroupingExpr(LinkedList<Guid> id)
        {
            Id = id;
        }
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
        /// visit a GroupingExpr
        /// </summary>
        /// <returns></returns>
        string Visit(GroupingExpr expr);
    }
}

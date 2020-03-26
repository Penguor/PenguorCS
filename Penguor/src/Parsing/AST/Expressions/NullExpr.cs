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
    /// A NullExpr expression
    /// </summary>
    public sealed class NullExpr : Expr
    {
        /// <summary>
        /// creates a new instance of NullExpr
        /// </summary>
        public NullExpr(LinkedList<Guid> id)
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
        /// visit a NullExpr
        /// </summary>
        /// <returns></returns>
        string Visit(NullExpr expr);
    }
}

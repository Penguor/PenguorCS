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
    /// A StringExpr expression
    /// </summary>
    public sealed class StringExpr : Expr
    {
        /// <summary>
        /// creates a new instance of StringExpr
        /// </summary>
        public StringExpr(string value, LinkedList<Guid> id)
        {
            Value = value;
            Id = id;
        }
        /// <summary></summary>
        public string Value { get; private set; }
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
        /// visit a StringExpr
        /// </summary>
        /// <returns></returns>
        string Visit(StringExpr expr);
    }
}

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
    /// A CallExpr expression
    /// </summary>
    public sealed class CallExpr : Expr
    {
        /// <summary>
        /// creates a new instance of CallExpr
        /// </summary>
        public CallExpr(Expr callee, List<Expr> args, LinkedList<Guid> id)
        {
            Callee = callee;
            Args = args;
            Id = id;
        }
        /// <summary></summary>
        public Expr Callee { get; private set; }
        /// <summary></summary>
        public List<Expr> Args { get; private set; }
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
        /// visit a CallExpr
        /// </summary>
        /// <returns></returns>
        string Visit(CallExpr expr);
    }
}

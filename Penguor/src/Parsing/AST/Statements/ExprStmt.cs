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
    /// A ExprStmt expression
    /// </summary>
    public sealed class ExprStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ExprStmt
        /// </summary>
        public ExprStmt(Expr expr, LinkedList<Guid> id)
        {
            Expr = expr;
            Id = id;
        }
        /// <summary></summary>
        public Expr Expr { get; private set; }
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
    /// Contains methods to visit all statements
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a ExprStmt
        /// </summary>
        /// <returns></returns>
        string Visit(ExprStmt stmt);
    }
}

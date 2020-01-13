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
    /// A ForStmt expression
    /// </summary>
    public sealed class ForStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ForStmt
        /// </summary>
        public ForStmt(Stmt currentvar, Expr vars, List<Stmt> statements, LinkedList<Guid> id)
        {
            CurrentVar = currentvar;
            Vars = vars;
            Statements = statements;
            Id = id;
        }
        /// <summary></summary>
        public Stmt CurrentVar { get; private set; }
        /// <summary></summary>
        public Expr Vars { get; private set; }
        /// <summary></summary>
        public List<Stmt> Statements { get; private set; }
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
        /// visit a ForStmt
        /// </summary>
        /// <returns></returns>
        string Visit(ForStmt stmt);
    }
}

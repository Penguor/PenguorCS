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
    /// A WhileStmt expression
    /// </summary>
    public sealed class WhileStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of WhileStmt
        /// </summary>
        public WhileStmt(Expr condition, List<Stmt> statements, LinkedList<Guid> id)
        {
            Condition = condition;
            Statements = statements;
            Id = id;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
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
        /// visit a WhileStmt
        /// </summary>
        /// <returns></returns>
        string Visit(WhileStmt stmt);
    }
}

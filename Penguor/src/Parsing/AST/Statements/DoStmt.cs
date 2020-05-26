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

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A DoStmt Stmt
    /// </summary>
    public sealed class DoStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of DoStmt
        /// </summary>
        public DoStmt(List<Stmt> statements, Expr condition)
        {
            Statements = statements;
            Condition = condition;
        }
        /// <summary></summary>
        public List<Stmt> Statements { get; private set; }
        /// <summary></summary>
        public Expr Condition { get; private set; }

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
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a DoStmt
        /// </summary>
        /// <returns></returns>
        string Visit(DoStmt stmt);
    }
}

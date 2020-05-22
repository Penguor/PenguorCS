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
    /// A CaseStmt expression
    /// </summary>
    public sealed class CaseStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of CaseStmt
        /// </summary>
        public CaseStmt(Expr condition, List<Stmt> statements)
        {
            Condition = condition;
            Statements = statements;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public List<Stmt> Statements { get; private set; }

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
        /// visit a CaseStmt
        /// </summary>
        /// <returns></returns>
        string Visit(CaseStmt stmt);
    }
}

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
    /// A SwitchStmt expression
    /// </summary>
    public sealed class SwitchStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of SwitchStmt
        /// </summary>
        public SwitchStmt(Expr condition, List<Stmt> cases, List<Stmt> defaultcase)
        {
            Condition = condition;
            Cases = cases;
            DefaultCase = defaultcase;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public List<Stmt> Cases { get; private set; }
        /// <summary></summary>
        public List<Stmt> DefaultCase { get; private set; }

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
        /// visit a SwitchStmt
        /// </summary>
        /// <returns></returns>
        string Visit(SwitchStmt stmt);
    }
}

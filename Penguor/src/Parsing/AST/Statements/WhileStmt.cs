/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A WhileStmt Stmt
    /// </summary>
    public sealed class WhileStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of WhileStmt
        /// </summary>
        public WhileStmt(Expr condition, List<Stmt> statements)
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
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a WhileStmt
        /// </summary>
        /// <returns></returns>
        T Visit(WhileStmt stmt);
    }
}

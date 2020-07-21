/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A SwitchStmt Stmt
    /// </summary>
    public sealed class SwitchStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of SwitchStmt
        /// </summary>
        public SwitchStmt(Expr condition, List<Stmt> cases, Stmt? defaultcase)
        {
            Condition = condition;
            Cases = cases;
            DefaultCase = defaultcase;
        }
        /// <summary></summary>
        public Expr Condition { get; }
        /// <summary></summary>
        public List<Stmt> Cases { get; }
        /// <summary></summary>
        public Stmt? DefaultCase { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface IStmtVisitor<T>
    {
        /// <summary>
        /// visit a SwitchStmt
        /// </summary>
        /// <returns></returns>
        T Visit(SwitchStmt stmt);
    }
}

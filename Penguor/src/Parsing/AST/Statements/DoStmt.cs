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
    /// A DoStmt Stmt
    /// </summary>
    public sealed class DoStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of DoStmt
        /// </summary>
        public DoStmt(Stmt content, Expr condition)
        {
            Content = content;
            Condition = condition;
        }
        /// <summary></summary>
        public Stmt Content { get; private set; }
        /// <summary></summary>
        public Expr Condition { get; private set; }

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
        /// visit a DoStmt
        /// </summary>
        /// <returns></returns>
        T Visit(DoStmt stmt);
    }
}

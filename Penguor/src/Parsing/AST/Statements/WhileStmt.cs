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
    /// A WhileStmt Stmt
    /// </summary>
    public sealed class WhileStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of WhileStmt
        /// </summary>
        public WhileStmt(Expr condition, Stmt content)
        {
            Condition = condition;
            Content = content;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public Stmt Content { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface IVisitor<T>
    {
        /// <summary>
        /// visit a WhileStmt
        /// </summary>
        /// <returns></returns>
        T Visit(WhileStmt stmt);
    }
}

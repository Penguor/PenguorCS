/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A DoStmt Stmt
    /// </summary>
    public sealed record DoStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of DoStmt
        /// </summary>
        public DoStmt(int offset, Stmt content, Expr condition)
        {
            Offset = offset;
            Content = content;
            Condition = condition;
        }
        public int Offset { get; }
        public Stmt Content { get; }
        public Expr Condition { get; }

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
        T Visit(DoStmt stmt);
    }
}

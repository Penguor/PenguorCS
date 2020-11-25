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
    /// A ElifStmt Stmt
    /// </summary>
    public sealed record ElifStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ElifStmt
        /// </summary>
        public ElifStmt(int offset, Expr condition, Stmt content)
        {
            Offset = offset;
            Condition = condition;
            Content = content;
        }
        public int Offset { get; }
        public Expr Condition { get; }
        public Stmt Content { get; }

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
        /// visit a ElifStmt
        /// </summary>
        T Visit(ElifStmt stmt);
    }
}

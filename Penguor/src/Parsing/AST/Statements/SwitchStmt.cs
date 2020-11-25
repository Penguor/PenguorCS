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
    /// A SwitchStmt Stmt
    /// </summary>
    public sealed record SwitchStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of SwitchStmt
        /// </summary>
        public SwitchStmt(int offset, Expr condition, List<Stmt> cases, Stmt? defaultcase)
        {
            Offset = offset;
            Condition = condition;
            Cases = cases;
            DefaultCase = defaultcase;
        }
        public int Offset { get; init; }
        public Expr Condition { get; init; }
        public List<Stmt> Cases { get; init; }
        public Stmt? DefaultCase { get; init; }

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
        T Visit(SwitchStmt stmt);
    }
}

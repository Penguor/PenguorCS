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
    /// A CaseStmt Stmt
    /// </summary>
    public sealed record CaseStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of CaseStmt
        /// </summary>
        public CaseStmt(int offset, Expr? condition, List<Stmt> content)
        {
            Offset = offset;
            Condition = condition;
            Content = content;
        }
        public int Offset { get; }
        public Expr? Condition { get; }
        public List<Stmt> Content { get; }

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
        /// visit a CaseStmt
        /// </summary>
        T Visit(CaseStmt stmt);
    }
}

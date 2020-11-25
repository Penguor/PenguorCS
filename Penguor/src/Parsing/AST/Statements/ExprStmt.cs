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
    /// A ExprStmt Stmt
    /// </summary>
    public sealed record ExprStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ExprStmt
        /// </summary>
        public ExprStmt(int offset, Expr expr)
        {
            Offset = offset;
            Expr = expr;
        }
        public int Offset { get; init; }
        public Expr Expr { get; init; }

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
        /// visit a ExprStmt
        /// </summary>
        T Visit(ExprStmt stmt);
    }
}

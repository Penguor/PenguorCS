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
    public sealed class ExprStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ExprStmt
        /// </summary>
        public ExprStmt(Expr expr)
        {
            Expr = expr;
        }
        public Expr Expr { get; }

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
        /// <returns></returns>
        T Visit(ExprStmt stmt);
    }
}

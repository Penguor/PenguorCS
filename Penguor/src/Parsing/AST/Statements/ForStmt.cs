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
    /// A ForStmt Stmt
    /// </summary>
    public sealed class ForStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ForStmt
        /// </summary>
        public ForStmt(int offset, Expr currentvar, Expr vars, Stmt content)
        {
            Offset = offset;
            CurrentVar = currentvar;
            Vars = vars;
            Content = content;
        }
        public int Offset { get; }
        public Expr CurrentVar { get; }
        public Expr Vars { get; }
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
        /// visit a ForStmt
        /// </summary>
        /// <returns></returns>
        T Visit(ForStmt stmt);
    }
}

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
    /// A VarStmt Stmt
    /// </summary>
    public sealed record VarStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of VarStmt
        /// </summary>
        public VarStmt(int offset, CallExpr type, AddressFrame name, Expr? init)
        {
            Offset = offset;
            Type = type;
            Name = name;
            Init = init;
        }
        public int Offset { get; }
        public CallExpr Type { get; }
        public AddressFrame Name { get; }
        public Expr? Init { get; }

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
        /// visit a VarStmt
        /// </summary>
        T Visit(VarStmt stmt);
    }
}

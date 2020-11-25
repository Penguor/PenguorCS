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
    /// A IfStmt Stmt
    /// </summary>
    public sealed record IfStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of IfStmt
        /// </summary>
        public IfStmt(int offset, Expr condition, Stmt ifc, List<Stmt> elif, Stmt? elsec)
        {
            Offset = offset;
            Condition = condition;
            IfC = ifc;
            Elif = elif;
            ElseC = elsec;
        }
        public int Offset { get; init; }
        public Expr Condition { get; init; }
        public Stmt IfC { get; init; }
        public List<Stmt> Elif { get; init; }
        public Stmt? ElseC { get; init; }

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
        /// visit a IfStmt
        /// </summary>
        T Visit(IfStmt stmt);
    }
}

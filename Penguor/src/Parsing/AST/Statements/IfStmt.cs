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
    public sealed class IfStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of IfStmt
        /// </summary>
        public IfStmt(Expr condition, Stmt ifc, List<Stmt> elif, Stmt? elsec)
        {
            Condition = condition;
            IfC = ifc;
            Elif = elif;
            ElseC = elsec;
        }
        public Expr Condition { get; }
        public Stmt IfC { get; }
        public List<Stmt> Elif { get; }
        public Stmt? ElseC { get; }

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
        /// <returns></returns>
        T Visit(IfStmt stmt);
    }
}

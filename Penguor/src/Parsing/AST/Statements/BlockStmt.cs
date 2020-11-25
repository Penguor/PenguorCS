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
    /// A BlockStmt Stmt
    /// </summary>
    public sealed record BlockStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of BlockStmt
        /// </summary>
        public BlockStmt(int offset, List<Stmt> content)
        {
            Offset = offset;
            Content = content;
        }
        public int Offset { get; }
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
        /// visit a BlockStmt
        /// </summary>
        T Visit(BlockStmt stmt);
    }
}

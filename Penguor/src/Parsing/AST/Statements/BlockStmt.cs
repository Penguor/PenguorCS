/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{

    /// <summary>
    /// A BlockStmt Stmt
    /// </summary>
    public sealed class BlockStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of BlockStmt
        /// </summary>
        public BlockStmt(List<Stmt> content)
        {
            Content = content;
        }
        /// <summary></summary>
        public List<Stmt> Content { get; private set; }

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
        /// <returns></returns>
        T Visit(BlockStmt stmt);
    }
}

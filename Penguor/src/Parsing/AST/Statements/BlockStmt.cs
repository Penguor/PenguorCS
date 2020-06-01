/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A BlockStmt Stmt
    /// </summary>
    public sealed class BlockStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of BlockStmt
        /// </summary>
        public BlockStmt(List<Decl> content)
        {
            Content = content;
        }
        /// <summary></summary>
        public List<Decl> Content { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a BlockStmt
        /// </summary>
        /// <returns></returns>
        T Visit(BlockStmt stmt);
    }
}

/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A BlockStmt expression
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
        public override string Accept(Visitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all statements
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a BlockStmt
        /// </summary>
        /// <returns></returns>
        string Visit(BlockStmt stmt);
    }
}

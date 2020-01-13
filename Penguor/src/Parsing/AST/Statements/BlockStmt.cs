/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# all rights reserved
# 
*/

using System.Collections.Generic;
using System;

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
        public BlockStmt(List<Stmt> contents, LinkedList<Guid> id)
        {
            Contents = contents;
            Id = id;
        }
        /// <summary></summary>
        public List<Stmt> Contents { get; private set; }
        /// <summary></summary>
        public LinkedList<Guid> Id { get; private set; }

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

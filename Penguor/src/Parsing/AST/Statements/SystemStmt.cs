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
using System;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A SystemStmt expression
    /// </summary>
    public sealed class SystemStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of SystemStmt
        /// </summary>
        public SystemStmt(Token name, Token parent, Stmt content, LinkedList<Guid> id)
        {
            Name = name;
            Parent = parent;
            Content = content;
            Id = id;
        }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public Token Parent { get; private set; }
        /// <summary></summary>
        public Stmt Content { get; private set; }
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
        /// visit a SystemStmt
        /// </summary>
        /// <returns></returns>
        string Visit(SystemStmt stmt);
    }
}

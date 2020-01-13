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
    /// A VarStmt expression
    /// </summary>
    public sealed class VarStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of VarStmt
        /// </summary>
        public VarStmt(Token type, Token name, Expr rhs, LinkedList<Guid> id)
        {
            Type = type;
            Name = name;
            Rhs = rhs;
            Id = id;
        }
        /// <summary></summary>
        public Token Type { get; private set; }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public Expr Rhs { get; private set; }
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
        /// visit a VarStmt
        /// </summary>
        /// <returns></returns>
        string Visit(VarStmt stmt);
    }
}

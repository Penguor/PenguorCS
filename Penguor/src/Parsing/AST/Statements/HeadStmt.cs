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
    /// A HeadStmt expression
    /// </summary>
    public sealed class HeadStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of HeadStmt
        /// </summary>
        public HeadStmt(Dictionary<string, string> definition, List<Expr> includelhs, LinkedList<Guid> id)
        {
            Definition = definition;
            IncludeLhs = includelhs;
            Id = id;
        }
        /// <summary></summary>
        public Dictionary<string, string> Definition { get; private set; }
        /// <summary></summary>
        public List<Expr> IncludeLhs { get; private set; }
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
        /// visit a HeadStmt
        /// </summary>
        /// <returns></returns>
        string Visit(HeadStmt stmt);
    }
}

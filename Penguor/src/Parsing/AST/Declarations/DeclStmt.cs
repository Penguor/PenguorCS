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
    /// A DeclStmt Decl
    /// </summary>
    public sealed class DeclStmt : Decl
    {
        /// <summary>
        /// creates a new instance of DeclStmt
        /// </summary>
        public DeclStmt(Stmt stmt)
        {
            Stmt = stmt;
        }
        /// <summary></summary>
        public Stmt Stmt { get; private set; }

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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a DeclStmt
        /// </summary>
        /// <returns></returns>
        string Visit(DeclStmt decl);
    }
}

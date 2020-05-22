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
    /// A LibraryStmt expression
    /// </summary>
    public sealed class LibraryStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of LibraryStmt
        /// </summary>
        public LibraryStmt(Token name, Stmt content)
        {
            Name = name;
            Content = content;
        }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public Stmt Content { get; private set; }

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
        /// visit a LibraryStmt
        /// </summary>
        /// <returns></returns>
        string Visit(LibraryStmt stmt);
    }
}

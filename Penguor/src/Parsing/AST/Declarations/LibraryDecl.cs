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
    /// A LibraryDecl Decl
    /// </summary>
    public sealed class LibraryDecl : Decl
    {
        /// <summary>
        /// creates a new instance of LibraryDecl
        /// </summary>
        public LibraryDecl(Token name, Stmt content)
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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a LibraryDecl
        /// </summary>
        /// <returns></returns>
        string Visit(LibraryDecl decl);
    }
}

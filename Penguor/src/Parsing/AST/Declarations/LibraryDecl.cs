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
    /// A LibraryDecl Decl
    /// </summary>
    public sealed class LibraryDecl : Decl
    {
        /// <summary>
        /// creates a new instance of LibraryDecl
        /// </summary>
        public LibraryDecl(TokenType? accessmod, TokenType[] nonaccessmod, List<Token> name, Decl content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Content = content;
        }
        /// <summary></summary>
        public TokenType? AccessMod { get; }
        /// <summary></summary>
        public TokenType[] NonAccessMod { get; }
        /// <summary></summary>
        public List<Token> Name { get; }
        /// <summary></summary>
        public Decl Content { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IDeclVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a LibraryDecl
        /// </summary>
        /// <returns></returns>
        T Visit(LibraryDecl decl);
    }
}

/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A LibraryDecl Decl
    /// </summary>
    public sealed record LibraryDecl : Decl
    {
        /// <summary>
        /// creates a new instance of LibraryDecl
        /// </summary>
        public LibraryDecl(int offset, TokenType? accessmod, TokenType[] nonaccessmod, State name, BlockDecl content)
        {
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Content = content;
        }
        public int Offset { get; }
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }
        public State Name { get; }
        public BlockDecl Content { get; }

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
        T Visit(LibraryDecl decl);
    }
}

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
    /// A TypeDecl Decl
    /// </summary>
    public sealed class TypeDecl : Decl
    {
        /// <summary>
        /// creates a new instance of TypeDecl
        /// </summary>
        public TypeDecl(TokenType? accessmod, TokenType[] nonaccessmod, Token name, Token? parent, Decl content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }
        public Token Name { get; }
        public Token? Parent { get; }
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
        /// visit a TypeDecl
        /// </summary>
        /// <returns></returns>
        T Visit(TypeDecl decl);
    }
}

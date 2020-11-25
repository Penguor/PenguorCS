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
    public sealed record TypeDecl : Decl
    {
        /// <summary>
        /// creates a new instance of TypeDecl
        /// </summary>
        public TypeDecl(int offset, TokenType? accessmod, TokenType[] nonaccessmod, AddressFrame name, CallExpr? parent, BlockDecl content)
        {
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        public int Offset { get; init; }
        public TokenType? AccessMod { get; init; }
        public TokenType[] NonAccessMod { get; init; }
        public AddressFrame Name { get; init; }
        public CallExpr? Parent { get; init; }
        public BlockDecl Content { get; init; }

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
        T Visit(TypeDecl decl);
    }
}

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
    /// A SystemDecl Decl
    /// </summary>
    public sealed class SystemDecl : Decl
    {
        /// <summary>
        /// creates a new instance of SystemDecl
        /// </summary>
        public SystemDecl(TokenType? accessmod, TokenType[] nonaccessmod, Token name, CallExpr? parent, BlockDecl content)
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
        public CallExpr? Parent { get; }
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
        /// visit a SystemDecl
        /// </summary>
        /// <returns></returns>
        T Visit(SystemDecl decl);
    }
}

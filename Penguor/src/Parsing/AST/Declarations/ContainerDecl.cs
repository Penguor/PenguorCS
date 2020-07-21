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
    /// A ContainerDecl Decl
    /// </summary>
    public sealed class ContainerDecl : Decl
    {
        /// <summary>
        /// creates a new instance of ContainerDecl
        /// </summary>
        public ContainerDecl(TokenType? accessmod, TokenType[] nonaccessmod, Token name, Token? parent, Decl content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        /// <summary></summary>
        public TokenType? AccessMod { get; }
        /// <summary></summary>
        public TokenType[] NonAccessMod { get; }
        /// <summary></summary>
        public Token Name { get; }
        /// <summary></summary>
        public Token? Parent { get; }
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
        /// visit a ContainerDecl
        /// </summary>
        /// <returns></returns>
        T Visit(ContainerDecl decl);
    }
}

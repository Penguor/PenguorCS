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
    /// A DatatypeDecl Decl
    /// </summary>
    public sealed class DatatypeDecl : Decl
    {
        /// <summary>
        /// creates a new instance of DatatypeDecl
        /// </summary>
        public DatatypeDecl(TokenType? accessmod, TokenType[]? nonaccessmod, Token name, Token? parent, Stmt content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        /// <summary></summary>
        public TokenType? AccessMod { get; private set; }
        /// <summary></summary>
        public TokenType[]? NonAccessMod { get; private set; }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public Token? Parent { get; private set; }
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
        /// visit a DatatypeDecl
        /// </summary>
        /// <returns></returns>
        string Visit(DatatypeDecl decl);
    }
}

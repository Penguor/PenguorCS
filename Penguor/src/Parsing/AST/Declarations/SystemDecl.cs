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
    /// A SystemDecl Decl
    /// </summary>
    public sealed class SystemDecl : Decl
    {
        /// <summary>
        /// creates a new instance of SystemDecl
        /// </summary>
        public SystemDecl(Token? accessmod, Token[]? nonaccessmod, Token name, Token? parent, Stmt content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        /// <summary></summary>
        public Token? AccessMod { get; private set; }
        /// <summary></summary>
        public Token[]? NonAccessMod { get; private set; }
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
        /// visit a SystemDecl
        /// </summary>
        /// <returns></returns>
        string Visit(SystemDecl decl);
    }
}

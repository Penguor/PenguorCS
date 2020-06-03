/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A FunctionDecl Decl
    /// </summary>
    public sealed class FunctionDecl : Decl
    {
        /// <summary>
        /// creates a new instance of FunctionDecl
        /// </summary>
        public FunctionDecl(TokenType? accessmod, TokenType[]? nonaccessmod, Expr variable, List<Expr>? parameters, Decl content)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Variable = variable;
            Parameters = parameters;
            Content = content;
        }
        /// <summary></summary>
        public TokenType? AccessMod { get; private set; }
        /// <summary></summary>
        public TokenType[]? NonAccessMod { get; private set; }
        /// <summary></summary>
        public Expr Variable { get; private set; }
        /// <summary></summary>
        public List<Expr>? Parameters { get; private set; }
        /// <summary></summary>
        public Decl Content { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a FunctionDecl
        /// </summary>
        /// <returns></returns>
        T Visit(FunctionDecl decl);
    }
}

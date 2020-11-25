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
    /// A FunctionDecl Decl
    /// </summary>
    public sealed record FunctionDecl : Decl
    {
        /// <summary>
        /// creates a new instance of FunctionDecl
        /// </summary>
        public FunctionDecl(int offset, TokenType? accessmod, TokenType[] nonaccessmod, CallExpr returns, AddressFrame name, List<VarExpr> parameters, Decl content)
        {
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Returns = returns;
            Name = name;
            Parameters = parameters;
            Content = content;
        }
        public int Offset { get; }
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }
        public CallExpr Returns { get; }
        public AddressFrame Name { get; }
        public List<VarExpr> Parameters { get; }
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
        /// visit a FunctionDecl
        /// </summary>
        T Visit(FunctionDecl decl);
    }
}

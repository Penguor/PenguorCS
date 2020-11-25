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
    /// A VarDecl Decl
    /// </summary>
    public sealed record VarDecl : Decl
    {
        /// <summary>
        /// creates a new instance of VarDecl
        /// </summary>
        public VarDecl(int offset, TokenType? accessmod, TokenType[] nonaccessmod, CallExpr type, AddressFrame name, Expr? init)
        {
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Type = type;
            Name = name;
            Init = init;
        }
        public int Offset { get; }
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }
        public CallExpr Type { get; }
        public AddressFrame Name { get; }
        public Expr? Init { get; }

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
        /// visit a VarDecl
        /// </summary>
        T Visit(VarDecl decl);
    }
}

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
    /// A Var Decl
    /// </summary>
    public sealed record VarDecl : Decl
    {
        /// <summary>
        /// creates a new instance of VarDecl
        /// </summary>
        public VarDecl(int id, int offset, TokenType? accessmod, TokenType[] nonaccessmod, CallExpr type, AddressFrame name, Expr? init)
        {
            Id = id;
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Type = type;
            Name = name;
            Init = init;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public TokenType? AccessMod { get; init; }
        public TokenType[] NonAccessMod { get; init; }
        public CallExpr Type { get; init; }
        public AddressFrame Name { get; init; }
        public Expr? Init { get; init; }


        public override string ToString() => "var declaration";

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
    /// Contains methods to visit all Declarations
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a VarDecl
        /// </summary>
        T Visit(VarDecl decl);
    }
}

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
    /// A UsingDecl Decl
    /// </summary>
    public sealed record UsingDecl : Decl
    {
        /// <summary>
        /// creates a new instance of UsingDecl
        /// </summary>
        public UsingDecl(int offset, CallExpr lib)
        {
            Offset = offset;
            Lib = lib;
        }
        public int Offset { get; init; }
        public CallExpr Lib { get; init; }

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
        /// visit a UsingDecl
        /// </summary>
        T Visit(UsingDecl decl);
    }
}

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
    /// A VarExpr Expr
    /// </summary>
    public sealed record VarExpr : Expr
    {
        /// <summary>
        /// creates a new instance of VarExpr
        /// </summary>
        public VarExpr(int offset, CallExpr type, AddressFrame name)
        {
            Offset = offset;
            Type = type;
            Name = name;
        }
        public int Offset { get; init; }
        public CallExpr Type { get; init; }
        public AddressFrame Name { get; init; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface IExprVisitor<T>
    {
        /// <summary>
        /// visit a VarExpr
        /// </summary>
        T Visit(VarExpr expr);
    }
}

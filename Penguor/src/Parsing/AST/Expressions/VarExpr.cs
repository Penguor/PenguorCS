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
    public sealed class VarExpr : Expr
    {
        /// <summary>
        /// creates a new instance of VarExpr
        /// </summary>
        public VarExpr(CallExpr type, AddressFrame name)
        {
            Type = type;
            Name = name;
        }
        public CallExpr Type { get; }
        public AddressFrame Name { get; }

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
        /// <returns></returns>
        T Visit(VarExpr expr);
    }
}

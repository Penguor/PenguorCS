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
    /// A EOFExpr Expr
    /// </summary>
    public sealed class EOFExpr : Expr
    {
        /// <summary>
        /// creates a new instance of EOFExpr
        /// </summary>
        public EOFExpr()
        {
        }

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
        /// visit a EOFExpr
        /// </summary>
        /// <returns></returns>
        T Visit(EOFExpr expr);
    }
}

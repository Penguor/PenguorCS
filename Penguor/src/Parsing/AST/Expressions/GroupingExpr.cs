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
    /// A GroupingExpr Expr
    /// </summary>
    public sealed class GroupingExpr : Expr
    {
        /// <summary>
        /// creates a new instance of GroupingExpr
        /// </summary>
        public GroupingExpr(Expr content)
        {
            Content = content;
        }
        /// <summary></summary>
        public Expr Content { get; }

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
        /// visit a GroupingExpr
        /// </summary>
        /// <returns></returns>
        T Visit(GroupingExpr expr);
    }
}

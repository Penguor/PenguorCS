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
    /// A GroupingExpr Expr
    /// </summary>
    public sealed class GroupingExpr : Expr
    {
        /// <summary>
        /// creates a new instance of GroupingExpr
        /// </summary>
        public GroupingExpr()
        {
        }

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
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a GroupingExpr
        /// </summary>
        /// <returns></returns>
        T Visit(GroupingExpr expr);
    }
}

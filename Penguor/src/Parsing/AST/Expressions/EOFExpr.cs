/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A EOFExpr expression
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
        public override string Accept(Visitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all expressions
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a EOFExpr
        /// </summary>
        /// <returns></returns>
        string Visit(EOFExpr expr);
    }
}

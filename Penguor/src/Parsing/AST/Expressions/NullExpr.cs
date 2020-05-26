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
    /// A NullExpr Expr
    /// </summary>
    public sealed class NullExpr : Expr
    {
        /// <summary>
        /// creates a new instance of NullExpr
        /// </summary>
        public NullExpr()
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
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a NullExpr
        /// </summary>
        /// <returns></returns>
        string Visit(NullExpr expr);
    }
}

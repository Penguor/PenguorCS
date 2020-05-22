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
    /// A NumExpr expression
    /// </summary>
    public sealed class NumExpr : Expr
    {
        /// <summary>
        /// creates a new instance of NumExpr
        /// </summary>
        public NumExpr(double value)
        {
            Value = value;
        }
        /// <summary></summary>
        public double Value { get; private set; }

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
        /// visit a NumExpr
        /// </summary>
        /// <returns></returns>
        string Visit(NumExpr expr);
    }
}

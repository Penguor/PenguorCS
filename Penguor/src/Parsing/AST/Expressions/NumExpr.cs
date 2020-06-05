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
    /// A NumExpr Expr
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
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface IVisitor<T>
    {
        /// <summary>
        /// visit a NumExpr
        /// </summary>
        /// <returns></returns>
        T Visit(NumExpr expr);
    }
}

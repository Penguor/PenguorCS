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
    /// A BooleanExpr Expr
    /// </summary>
    public sealed class BooleanExpr : Expr
    {
        /// <summary>
        /// creates a new instance of BooleanExpr
        /// </summary>
        public BooleanExpr(bool value)
        {
            Value = value;
        }
        /// <summary></summary>
        public bool Value { get; private set; }

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
        /// visit a BooleanExpr
        /// </summary>
        /// <returns></returns>
        T Visit(BooleanExpr expr);
    }
}

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
    /// A StringExpr Expr
    /// </summary>
    public sealed class StringExpr : Expr
    {
        /// <summary>
        /// creates a new instance of StringExpr
        /// </summary>
        public StringExpr(string value)
        {
            Value = value;
        }
        /// <summary></summary>
        public string Value { get; private set; }

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
        /// visit a StringExpr
        /// </summary>
        /// <returns></returns>
        string Visit(StringExpr expr);
    }
}

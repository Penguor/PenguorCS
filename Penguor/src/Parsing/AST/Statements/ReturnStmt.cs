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
    /// A ReturnStmt Stmt
    /// </summary>
    public sealed class ReturnStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ReturnStmt
        /// </summary>
        public ReturnStmt(Expr? value)
        {
            Value = value;
        }
        /// <summary></summary>
        public Expr? Value { get; private set; }

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
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a ReturnStmt
        /// </summary>
        /// <returns></returns>
        T Visit(ReturnStmt stmt);
    }
}

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
    /// A UsingStmt expression
    /// </summary>
    public sealed class UsingStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of UsingStmt
        /// </summary>
        public UsingStmt(Expr lib)
        {
            Lib = lib;
        }
        /// <summary></summary>
        public Expr Lib { get; private set; }

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
    /// Contains methods to visit all statements
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a UsingStmt
        /// </summary>
        /// <returns></returns>
        string Visit(UsingStmt stmt);
    }
}

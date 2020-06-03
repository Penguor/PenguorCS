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
    /// A ForStmt Stmt
    /// </summary>
    public sealed class ForStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ForStmt
        /// </summary>
        public ForStmt(Expr currentvar, Expr vars, Stmt content)
        {
            CurrentVar = currentvar;
            Vars = vars;
            Content = content;
        }
        /// <summary></summary>
        public Expr CurrentVar { get; private set; }
        /// <summary></summary>
        public Expr Vars { get; private set; }
        /// <summary></summary>
        public Stmt Content { get; private set; }

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
        /// visit a ForStmt
        /// </summary>
        /// <returns></returns>
        T Visit(ForStmt stmt);
    }
}

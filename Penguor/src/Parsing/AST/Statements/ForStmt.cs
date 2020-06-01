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
    /// A ForStmt Stmt
    /// </summary>
    public sealed class ForStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ForStmt
        /// </summary>
        public ForStmt(Expr currentvar, Expr vars, List<Stmt> statements)
        {
            CurrentVar = currentvar;
            Vars = vars;
            Statements = statements;
        }
        /// <summary></summary>
        public Expr CurrentVar { get; private set; }
        /// <summary></summary>
        public Expr Vars { get; private set; }
        /// <summary></summary>
        public List<Stmt> Statements { get; private set; }

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
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a ForStmt
        /// </summary>
        /// <returns></returns>
        string Visit(ForStmt stmt);
    }
}

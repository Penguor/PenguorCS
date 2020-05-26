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
    /// A IfStmt Stmt
    /// </summary>
    public sealed class IfStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of IfStmt
        /// </summary>
        public IfStmt(Expr condition, List<Stmt> elif, List<Stmt> elsec)
        {
            Condition = condition;
            Elif = elif;
            ElseC = elsec;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public List<Stmt> Elif { get; private set; }
        /// <summary></summary>
        public List<Stmt> ElseC { get; private set; }

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
        /// visit a IfStmt
        /// </summary>
        /// <returns></returns>
        string Visit(IfStmt stmt);
    }
}

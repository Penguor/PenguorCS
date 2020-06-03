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
    /// A IfStmt Stmt
    /// </summary>
    public sealed class IfStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of IfStmt
        /// </summary>
        public IfStmt(Expr condition, Stmt ifc, List<Stmt> elif, Stmt? elsec)
        {
            Condition = condition;
            IfC = ifc;
            Elif = elif;
            ElseC = elsec;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public Stmt IfC { get; private set; }
        /// <summary></summary>
        public List<Stmt> Elif { get; private set; }
        /// <summary></summary>
        public Stmt? ElseC { get; private set; }

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
        /// visit a IfStmt
        /// </summary>
        /// <returns></returns>
        T Visit(IfStmt stmt);
    }
}

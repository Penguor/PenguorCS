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
    /// A ElifStmt expression
    /// </summary>
    public sealed class ElifStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ElifStmt
        /// </summary>
        public ElifStmt(Expr condition, List<Stmt> content)
        {
            Condition = condition;
            Content = content;
        }
        /// <summary></summary>
        public Expr Condition { get; private set; }
        /// <summary></summary>
        public List<Stmt> Content { get; private set; }

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
        /// visit a ElifStmt
        /// </summary>
        /// <returns></returns>
        string Visit(ElifStmt stmt);
    }
}

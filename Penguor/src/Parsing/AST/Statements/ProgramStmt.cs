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
    /// A ProgramStmt expression
    /// </summary>
    public sealed class ProgramStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ProgramStmt
        /// </summary>
        public ProgramStmt(List<Stmt> declarations)
        {
            Declarations = declarations;
        }
        /// <summary></summary>
        public List<Stmt> Declarations { get; private set; }

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
        /// visit a ProgramStmt
        /// </summary>
        /// <returns></returns>
        string Visit(ProgramStmt stmt);
    }
}

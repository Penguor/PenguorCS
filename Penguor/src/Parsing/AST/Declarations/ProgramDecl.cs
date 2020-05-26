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
    /// A ProgramDecl Decl
    /// </summary>
    public sealed class ProgramDecl : Decl
    {
        /// <summary>
        /// creates a new instance of ProgramDecl
        /// </summary>
        public ProgramDecl(List<Stmt> declarations)
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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a ProgramDecl
        /// </summary>
        /// <returns></returns>
        string Visit(ProgramDecl decl);
    }
}

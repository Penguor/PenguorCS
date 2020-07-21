/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A ProgramDecl Decl
    /// </summary>
    public sealed class ProgramDecl : Decl
    {
        /// <summary>
        /// creates a new instance of ProgramDecl
        /// </summary>
        public ProgramDecl(List<Decl> declarations)
        {
            Declarations = declarations;
        }
        /// <summary></summary>
        public List<Decl> Declarations { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IDeclVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a ProgramDecl
        /// </summary>
        /// <returns></returns>
        T Visit(ProgramDecl decl);
    }
}

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
        public List<Decl> Declarations { get; private set; }

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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a ProgramDecl
        /// </summary>
        /// <returns></returns>
        T Visit(ProgramDecl decl);
    }
}

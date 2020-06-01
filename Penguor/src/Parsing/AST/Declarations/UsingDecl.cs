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
    /// A UsingDecl Decl
    /// </summary>
    public sealed class UsingDecl : Decl
    {
        /// <summary>
        /// creates a new instance of UsingDecl
        /// </summary>
        public UsingDecl(Expr lib)
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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a UsingDecl
        /// </summary>
        /// <returns></returns>
        string Visit(UsingDecl decl);
    }
}
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
    /// A VarDecl Decl
    /// </summary>
    public sealed class VarDecl : Decl
    {
        /// <summary>
        /// creates a new instance of VarDecl
        /// </summary>
        public VarDecl(TokenType? accessmod, TokenType[]? nonaccessmod, Expr variable, Expr? init)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Variable = variable;
            Init = init;
        }
        /// <summary></summary>
        public TokenType? AccessMod { get; private set; }
        /// <summary></summary>
        public TokenType[]? NonAccessMod { get; private set; }
        /// <summary></summary>
        public Expr Variable { get; private set; }
        /// <summary></summary>
        public Expr? Init { get; private set; }

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
        /// visit a VarDecl
        /// </summary>
        /// <returns></returns>
        string Visit(VarDecl decl);
    }
}
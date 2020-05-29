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
    /// A FunctionDecl Decl
    /// </summary>
    public sealed class FunctionDecl : Decl
    {
        /// <summary>
        /// creates a new instance of FunctionDecl
        /// </summary>
        public FunctionDecl(Token? accessmod, Token[]? nonaccessmod, Expr variable, List<Expr>? parameters)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Variable = variable;
            Parameters = parameters;
        }
        /// <summary></summary>
        public Token? AccessMod { get; private set; }
        /// <summary></summary>
        public Token[]? NonAccessMod { get; private set; }
        /// <summary></summary>
        public Expr Variable { get; private set; }
        /// <summary></summary>
        public List<Expr>? Parameters { get; private set; }

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
        /// visit a FunctionDecl
        /// </summary>
        /// <returns></returns>
        string Visit(FunctionDecl decl);
    }
}

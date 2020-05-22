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
    /// A VarExpr expression
    /// </summary>
    public sealed class VarExpr : Expr
    {
        /// <summary>
        /// creates a new instance of VarExpr
        /// </summary>
        public VarExpr(Token accessmod, Token[] nonaccessmod, Expr type, Token name, Expr init)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Type = type;
            Name = name;
            Init = init;
        }
        /// <summary></summary>
        public Token AccessMod { get; private set; }
        /// <summary></summary>
        public Token[] NonAccessMod { get; private set; }
        /// <summary></summary>
        public Expr Type { get; private set; }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public Expr Init { get; private set; }

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
    /// Contains methods to visit all expressions
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a VarExpr
        /// </summary>
        /// <returns></returns>
        string Visit(VarExpr expr);
    }
}

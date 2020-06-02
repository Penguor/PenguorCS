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
    /// A VarExpr Expr
    /// </summary>
    public sealed class VarExpr : Expr
    {
        /// <summary>
        /// creates a new instance of VarExpr
        /// </summary>
        public VarExpr(Expr type, Token name)
        {
            Type = type;
            Name = name;
        }
        /// <summary></summary>
        public Expr Type { get; private set; }
        /// <summary></summary>
        public Token Name { get; private set; }

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
    /// Contains methods to visit all Expr
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a VarExpr
        /// </summary>
        /// <returns></returns>
        T Visit(VarExpr expr);
    }
}

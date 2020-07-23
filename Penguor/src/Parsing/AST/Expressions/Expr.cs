
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Expr
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Expr
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Expr</param>
        /// <returns></returns>
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }
}

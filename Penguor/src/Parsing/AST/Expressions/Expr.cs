/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

namespace Penguor.Parsing.AST
{
    /// <summary>
    /// Base class for penguor expressions
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// <c>Accept</c> returns this visit method for the expression type
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Expr</param>
        /// <returns></returns>
        public abstract string Accept(Visitor visitor);
    }
}
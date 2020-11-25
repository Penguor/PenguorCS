
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
    /// Base class for penguor Stmt
    /// </summary>
    public abstract record Stmt
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Stmt
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Stmt</param>
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }
}

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
    /// Base class for penguor statements
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the statement
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Stmt</param>
        /// <returns></returns>
        public abstract string Accept(Visitor visitor);
    }
}
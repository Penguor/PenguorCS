
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

namespace Penguor.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Stmt
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Stmt
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Stmt</param>
        /// <returns></returns>
        public abstract T Accept<T>(Visitor<T> visitor);
    }
}

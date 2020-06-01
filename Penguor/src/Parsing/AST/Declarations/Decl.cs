
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
*/

namespace Penguor.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Decl
    /// </summary>
    public abstract class Decl
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Decl
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Decl</param>
        /// <returns></returns>
        public abstract T Accept<T>(Visitor<T> visitor);
    }
}

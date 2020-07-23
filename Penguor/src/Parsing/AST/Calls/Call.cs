
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
    /// Base class for penguor Call
    /// </summary>
    public abstract class Call
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Call
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Call</param>
        /// <returns></returns>
        public abstract T Accept<T>(ICallVisitor<T> visitor);
    }
}

/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{

    /// <summary>
    /// A IdfCall Call
    /// </summary>
    public sealed class IdfCall : Call
    {
        /// <summary>
        /// creates a new instance of IdfCall
        /// </summary>
        public IdfCall(Token name)
        {
            Name = name;
        }
        /// <summary></summary>
        public Token Name { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Call
    /// </summary>
    public partial interface IVisitor<T>
    {
        /// <summary>
        /// visit a IdfCall
        /// </summary>
        /// <returns></returns>
        T Visit(IdfCall call);
    }
}

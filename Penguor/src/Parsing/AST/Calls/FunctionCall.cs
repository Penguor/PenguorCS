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
    /// A FunctionCall Call
    /// </summary>
    public sealed class FunctionCall : Call
    {
        /// <summary>
        /// creates a new instance of FunctionCall
        /// </summary>
        public FunctionCall(Token name, List<Expr>? args)
        {
            Name = name;
            Args = args;
        }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public List<Expr>? Args { get; private set; }

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
    /// Contains methods to visit all Call
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a FunctionCall
        /// </summary>
        /// <returns></returns>
        T Visit(FunctionCall call);
    }
}

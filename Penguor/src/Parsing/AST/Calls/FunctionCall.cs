/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A FunctionCall Call
    /// </summary>
    public sealed class FunctionCall : Call
    {
        /// <summary>
        /// creates a new instance of FunctionCall
        /// </summary>
        public FunctionCall(AddressFrame name, List<Expr> args)
        {
            Name = name;
            Args = args;
        }
        public AddressFrame Name { get; }
        public List<Expr> Args { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(ICallVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Call
    /// </summary>
    public partial interface ICallVisitor<T>
    {
        /// <summary>
        /// visit a FunctionCall
        /// </summary>
        /// <returns></returns>
        T Visit(FunctionCall call);
    }
}

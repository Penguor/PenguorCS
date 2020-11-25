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
    public sealed record FunctionCall : Call
    {
        /// <summary>
        /// creates a new instance of FunctionCall
        /// </summary>
        public FunctionCall(int offset, AddressFrame name, List<Expr> args)
        {
            Offset = offset;
            Name = name;
            Args = args;
        }
        public int Offset { get; init; }
        public AddressFrame Name { get; init; }
        public List<Expr> Args { get; init; }

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
        T Visit(FunctionCall call);
    }
}

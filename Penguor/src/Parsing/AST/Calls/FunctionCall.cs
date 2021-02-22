
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Function Call
    /// </summary>
    public sealed record FunctionCall : Call
    {
        /// <summary>
        /// creates a new instance of FunctionCall
        /// </summary>
        public FunctionCall(int id, int offset, AddressFrame name, List<Expr> args)
        {
            Id = id;
            Offset = offset;
            Name = name;
            Args = args;
        }
        public AddressFrame Name { get; init; }
        public List<Expr> Args { get; init; }

        public override string ToString() => "function call";

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
    /// Contains methods to visit all Calls
    /// </summary>
    public partial interface ICallVisitor<T>
    {
        /// <summary>
        /// visit a FunctionCall
        /// </summary>
        T Visit(FunctionCall call);
    }
}

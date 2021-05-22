
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Array Call
    /// </summary>
    public sealed record ArrayCall : Call
    {
        /// <summary>
        /// creates a new instance of ArrayCall
        /// </summary>
        public ArrayCall(int id, int offset, AddressFrame name, Expr index, TokenType? postfix)
        {
            Id = id;
            Offset = offset;
            Name = name;
            Index = index;
            Postfix = postfix;
        }
        public AddressFrame Name { get; init; }
        public Expr Index { get; init; }
        public TokenType? Postfix { get; init; }

        public override string ToString() => "array call";

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
        /// visit a ArrayCall
        /// </summary>
        T Visit(ArrayCall call);
    }
}

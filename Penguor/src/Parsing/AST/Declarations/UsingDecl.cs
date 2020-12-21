
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Using Decl
    /// </summary>
    public sealed record UsingDecl : Decl
    {
        /// <summary>
        /// creates a new instance of UsingDecl
        /// </summary>
        public UsingDecl(int id, int offset, CallExpr lib)
        {
            Id = id;
            Offset = offset;
            Lib = lib;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public CallExpr Lib { get; init; }


        public override string ToString() => "using declaration";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IDeclVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Declarations
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a UsingDecl
        /// </summary>
        T Visit(UsingDecl decl);
    }
}

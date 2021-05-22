
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Type Decl
    /// </summary>
    public sealed record TypeDecl : Decl
    {
        /// <summary>
        /// creates a new instance of TypeDecl
        /// </summary>
        public TypeDecl(int id, int offset, AddressFrame name, CallExpr? parent, BlockDecl content)
        {
            Id = id;
            Offset = offset;
            Name = name;
            Parent = parent;
            Content = content;
        }
        public AddressFrame Name { get; init; }
        public CallExpr? Parent { get; init; }
        public BlockDecl Content { get; init; }

        public override string ToString() => "type declaration";

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
        /// visit a TypeDecl
        /// </summary>
        T Visit(TypeDecl decl);
    }
}

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Library Decl
    /// </summary>
    public sealed record LibraryDecl : Decl
    {
        /// <summary>
        /// creates a new instance of LibraryDecl
        /// </summary>
        public LibraryDecl(int id, int offset, State name, BlockDecl content)
        {
            Id = id;
            Offset = offset;
            Name = name;
            Content = content;
        }
        public State Name { get; init; }
        public BlockDecl Content { get; init; }

        public override string ToString() => "library declaration";

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
        /// visit a LibraryDecl
        /// </summary>
        T Visit(LibraryDecl decl);
    }
}

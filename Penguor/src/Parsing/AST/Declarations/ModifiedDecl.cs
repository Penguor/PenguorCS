#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Modified Decl
    /// </summary>
    public sealed record ModifiedDecl : Decl
    {
        /// <summary>
        /// creates a new instance of ModifiedDecl
        /// </summary>
        public ModifiedDecl(int id, int offset, TokenType? accessmod, TokenType? nonaccessmod, Decl declaration)
        {
            Id = id;
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Declaration = declaration;
        }
        public TokenType? AccessMod { get; init; }
        public TokenType? NonAccessMod { get; init; }
        public Decl Declaration { get; init; }

        public override string ToString() => "modified declaration";

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
        /// visit a ModifiedDecl
        /// </summary>
        T Visit(ModifiedDecl decl);
    }
}

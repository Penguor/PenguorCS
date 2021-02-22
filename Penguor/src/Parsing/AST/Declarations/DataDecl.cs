
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Data Decl
    /// </summary>
    public sealed record DataDecl : Decl
    {
        /// <summary>
        /// creates a new instance of DataDecl
        /// </summary>
        public DataDecl(int id, int offset, TokenType? accessmod, TokenType[] nonaccessmod, AddressFrame name, CallExpr? parent, BlockDecl content)
        {
            Id = id;
            Offset = offset;
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Name = name;
            Parent = parent;
            Content = content;
        }
        public TokenType? AccessMod { get; init; }
        public TokenType[] NonAccessMod { get; init; }
        public AddressFrame Name { get; init; }
        public CallExpr? Parent { get; init; }
        public BlockDecl Content { get; init; }

        public override string ToString() => "data declaration";

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
        /// visit a DataDecl
        /// </summary>
        T Visit(DataDecl decl);
    }
}

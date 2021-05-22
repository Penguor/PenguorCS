
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A StmtBlock Decl
    /// </summary>
    public sealed record StmtBlockDecl : Decl
    {
        /// <summary>
        /// creates a new instance of StmtBlockDecl
        /// </summary>
        public StmtBlockDecl(int id, int offset, List<Decl> content)
        {
            Id = id;
            Offset = offset;
            Content = content;
        }
        public List<Decl> Content { get; init; }

        public override string ToString() => "stmtblock declaration";

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
        /// visit a StmtBlockDecl
        /// </summary>
        T Visit(StmtBlockDecl decl);
    }
}

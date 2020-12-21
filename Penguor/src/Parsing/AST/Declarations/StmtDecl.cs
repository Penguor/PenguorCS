
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Stmt Decl
    /// </summary>
    public sealed record StmtDecl : Decl
    {
        /// <summary>
        /// creates a new instance of StmtDecl
        /// </summary>
        public StmtDecl(int id, int offset, Stmt stmt)
        {
            Id = id;
            Offset = offset;
            Stmt = stmt;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public Stmt Stmt { get; init; }


        public override string ToString() => "stmt declaration";

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
        /// visit a StmtDecl
        /// </summary>
        T Visit(StmtDecl decl);
    }
}

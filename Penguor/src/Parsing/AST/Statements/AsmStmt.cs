
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Asm Stmt
    /// </summary>
    public sealed record AsmStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of AsmStmt
        /// </summary>
        public AsmStmt(int id, int offset, string[] contents)
        {
            Id = id;
            Offset = offset;
            Contents = contents;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public string[] Contents { get; init; }

        public override string ToString() => "asm statement";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Statements
    /// </summary>
    public partial interface IStmtVisitor<T>
    {
        /// <summary>
        /// visit a AsmStmt
        /// </summary>
        T Visit(AsmStmt stmt);
    }
}

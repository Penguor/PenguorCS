
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Compiler Stmt
    /// </summary>
    public sealed record CompilerStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of CompilerStmt
        /// </summary>
        public CompilerStmt(int id, int offset, TokenType dir, Token[] val)
        {
            Id = id;
            Offset = offset;
            Dir = dir;
            Val = val;
        }
        public TokenType Dir { get; init; }
        public Token[] Val { get; init; }

        public override string ToString() => "compiler statement";

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
        /// visit a CompilerStmt
        /// </summary>
        T Visit(CompilerStmt stmt);
    }
}

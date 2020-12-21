
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Elif Stmt
    /// </summary>
    public sealed record ElifStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ElifStmt
        /// </summary>
        public ElifStmt(int id, int offset, Expr condition, Stmt content)
        {
            Id = id;
            Offset = offset;
            Condition = condition;
            Content = content;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public Expr Condition { get; init; }
        public Stmt Content { get; init; }


        public override string ToString() => "elif statement";

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
        /// visit a ElifStmt
        /// </summary>
        T Visit(ElifStmt stmt);
    }
}


#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Do Stmt
    /// </summary>
    public sealed record DoStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of DoStmt
        /// </summary>
        public DoStmt(int id, int offset, Stmt content, Expr condition)
        {
            Id = id;
            Offset = offset;
            Content = content;
            Condition = condition;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public Stmt Content { get; init; }
        public Expr Condition { get; init; }

        public override string ToString() => "do statement";

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
        /// visit a DoStmt
        /// </summary>
        T Visit(DoStmt stmt);
    }
}

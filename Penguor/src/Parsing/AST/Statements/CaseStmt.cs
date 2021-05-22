#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Case Stmt
    /// </summary>
    public sealed record CaseStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of CaseStmt
        /// </summary>
        public CaseStmt(int id, int offset, Expr? condition, List<Stmt> content)
        {
            Id = id;
            Offset = offset;
            Condition = condition;
            Content = content;
        }
        public Expr? Condition { get; init; }
        public List<Stmt> Content { get; init; }

        public override string ToString() => "case statement";

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
        /// visit a CaseStmt
        /// </summary>
        T Visit(CaseStmt stmt);
    }
}

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Expr Stmt
    /// </summary>
    public sealed record ExprStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ExprStmt
        /// </summary>
        public ExprStmt(int id, int offset, Expr expr)
        {
            Id = id;
            Offset = offset;
            Expr = expr;
        }
        public Expr Expr { get; init; }

        public override string ToString() => "expr statement";

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
        /// visit a ExprStmt
        /// </summary>
        T Visit(ExprStmt stmt);
    }
}

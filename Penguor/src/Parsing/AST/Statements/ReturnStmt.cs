#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Return Stmt
    /// </summary>
    public sealed record ReturnStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ReturnStmt
        /// </summary>
        public ReturnStmt(int id, int offset, Expr? value)
        {
            Id = id;
            Offset = offset;
            Value = value;
        }
        public Expr? Value { get; init; }

        public override string ToString() => "return statement";

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
        /// visit a ReturnStmt
        /// </summary>
        T Visit(ReturnStmt stmt);
    }
}

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Foreach Stmt
    /// </summary>
    public sealed record ForeachStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of ForeachStmt
        /// </summary>
        public ForeachStmt(int id, int offset, VarExpr currentvar, CallExpr vars, Stmt content)
        {
            Id = id;
            Offset = offset;
            CurrentVar = currentvar;
            Vars = vars;
            Content = content;
        }
        public VarExpr CurrentVar { get; init; }
        public CallExpr Vars { get; init; }
        public Stmt Content { get; init; }

        public override string ToString() => "foreach statement";

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
        /// visit a ForeachStmt
        /// </summary>
        T Visit(ForeachStmt stmt);
    }
}

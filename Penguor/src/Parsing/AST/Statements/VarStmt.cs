#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Var Stmt
    /// </summary>
    public sealed record VarStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of VarStmt
        /// </summary>
        public VarStmt(int id, int offset, CallExpr type, AddressFrame name, Expr? init)
        {
            Id = id;
            Offset = offset;
            Type = type;
            Name = name;
            Init = init;
        }
        public CallExpr Type { get; init; }
        public AddressFrame Name { get; init; }
        public Expr? Init { get; init; }

        public override string ToString() => "var statement";

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
        /// visit a VarStmt
        /// </summary>
        T Visit(VarStmt stmt);
    }
}

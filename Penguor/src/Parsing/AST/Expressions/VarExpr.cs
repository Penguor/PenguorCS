
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Var Expr
    /// </summary>
    public sealed record VarExpr : Expr
    {
        /// <summary>
        /// creates a new instance of VarExpr
        /// </summary>
        public VarExpr(int id, int offset, CallExpr type, AddressFrame name)
        {
            Id = id;
            Offset = offset;
            Type = type;
            Name = name;
        }
        public int Id { get; init; }
        public int Offset { get; init; }
        public CallExpr Type { get; init; }
        public AddressFrame Name { get; init; }


        public override string ToString() => "var expression";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Expressions
    /// </summary>
    public partial interface IExprVisitor<T>
    {
        /// <summary>
        /// visit a VarExpr
        /// </summary>
        T Visit(VarExpr expr);
    }
}

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Extern Decl
    /// </summary>
    public sealed record ExternDecl : Decl
    {
        /// <summary>
        /// creates a new instance of ExternDecl
        /// </summary>
        public ExternDecl(int id, int offset, TypeCallExpr returns, AddressFrame name, List<VarExpr> parameters)
        {
            Id = id;
            Offset = offset;
            Returns = returns;
            Name = name;
            Parameters = parameters;
        }
        public TypeCallExpr Returns { get; init; }
        public AddressFrame Name { get; init; }
        public List<VarExpr> Parameters { get; init; }

        public override string ToString() => "extern declaration";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IDeclVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Declarations
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a ExternDecl
        /// </summary>
        T Visit(ExternDecl decl);
    }
}

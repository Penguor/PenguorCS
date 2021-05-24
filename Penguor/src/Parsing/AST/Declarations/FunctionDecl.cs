#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A Function Decl
    /// </summary>
    public sealed record FunctionDecl : Decl
    {
        /// <summary>
        /// creates a new instance of FunctionDecl
        /// </summary>
        public FunctionDecl(int id, int offset, TypeCallExpr returns, AddressFrame name, List<VarExpr> parameters, Decl content)
        {
            Id = id;
            Offset = offset;
            Returns = returns;
            Name = name;
            Parameters = parameters;
            Content = content;
        }
        public TypeCallExpr Returns { get; init; }
        public AddressFrame Name { get; init; }
        public List<VarExpr> Parameters { get; init; }
        public Decl Content { get; init; }

        public override string ToString() => "function declaration";

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
        /// visit a FunctionDecl
        /// </summary>
        T Visit(FunctionDecl decl);
    }
}

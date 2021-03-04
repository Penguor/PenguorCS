
#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Expr
    /// </summary>
    public abstract record Expr
    {
        public int Id { get; init; }
        public int Offset { get; init; }
        public ASTAttribute? Attribute { get; set; }

        /// <summary>
        /// <c>Accept</c> returns the visit method for the Expr
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Expr</param>
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }
}

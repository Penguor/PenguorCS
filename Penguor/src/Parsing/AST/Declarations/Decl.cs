
#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Decl
    /// </summary>
    public abstract record Decl
    {
        public int Id { get; init; }
        public int Offset { get; init; }

        /// <summary>
        /// <c>Accept</c> returns the visit method for the Decl
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Decl</param>
        public abstract T Accept<T>(IDeclVisitor<T> visitor);
    }
}

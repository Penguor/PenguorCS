
#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Stmt
    /// </summary>
    public abstract record Stmt
    {
        public int Id { get; init; }
        public int Offset { get; init; }
        public ASTAttribute? Attribute { get; set; }

        /// <summary>
        /// <c>Accept</c> returns the visit method for the Stmt
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Stmt</param>
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }
}

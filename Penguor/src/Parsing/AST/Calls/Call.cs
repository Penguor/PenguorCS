#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor Call
    /// </summary>
    public abstract record Call : ASTNode
    {
        /// <summary>
        /// <c>Accept</c> returns the visit method for the Call
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Call</param>
        public abstract T Accept<T>(ICallVisitor<T> visitor);
    }
}

#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// Base class for penguor ast nodes
    /// </summary>
    public abstract record ASTNode
    {
        public int Id { get; init; }
        public int Offset { get; init; }
        public ASTAttribute? Attribute { get; set; }
    }
}

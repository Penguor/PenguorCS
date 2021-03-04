using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.Analysis
{
    /// <summary>
    /// attribute for expressions
    /// </summary>
    public class ExprAttribute : ASTAttribute
    {
        /// <summary>
        /// the data type of the expression
        /// </summary>
        public State Type { get; set; }

        /// <summary>
        /// Create a new ExprAttribute instance
        /// </summary>
        /// <param name="type">the data type of the expression</param>
        public ExprAttribute(State type)
        {
            Type = type;
        }
    }
}
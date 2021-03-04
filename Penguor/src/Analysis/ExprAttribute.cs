using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.Analysis
{
    public class ExprAttribute : ASTAttribute
    {
        State Type { get; set; }

        public ExprAttribute(State type)
        {
            Type = type;
        }
    }
}
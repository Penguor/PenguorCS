
namespace Penguor.Parsing.AST
{
    /// <summary>
    /// base class for Penguor Declarations
    /// </summary>
    public abstract class Call
    {
        /// <summary>
        /// <c>Accept</c> returns this visit method for the declaration type
        /// </summary>
        /// <param name="visitor">the visitor which visits this instance of Decl</param>
        /// <returns></returns>
        public abstract string Accept(Visitor visitor);
    }
}
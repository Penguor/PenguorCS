
using Penguor.Compiler.Build;
using Penguor.Compiler.IR;

namespace Penguor.Compiler.Assembly
{
    /// <summary>
    /// Abstract base class for all assembly generators
    /// </summary>
    public abstract class AssemblyGenerator
    {
        /// <summary>
        /// the ir program used as input for the assembly generator
        /// </summary>
        protected IRProgram Program { get; }

        /// <summary>
        /// create a new instance of the <c>AssemblyGenerator</c> class
        /// </summary>
        /// <param name="program"></param>
        protected AssemblyGenerator(IRProgram program)
        {
            Program = program;
        }

        /// <summary>
        /// Generate the Assembly
        /// </summary>
        public abstract AsmProgram Generate();
    }
}
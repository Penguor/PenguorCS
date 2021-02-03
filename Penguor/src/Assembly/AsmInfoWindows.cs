namespace Penguor.Compiler.Assembly
{
    /// <summary>
    /// Information for generating assembly targeting amd64 windows
    /// </summary>
    public class AsmInfoWindowsAmd64 : AssemblyInfo
    {
        /// <summary>
        /// the offset where the value can be accessed on the stack
        /// </summary>
        public int? StackOffset { get; set; }

        /// <summary>
        /// how to get the value in assembly
        /// </summary>
        public string? Get { get; set; }

        public int ParamNumber { get; set; }
    }
}
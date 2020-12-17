/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/
using Penguor.Compiler.Build;
using Penguor.Compiler.IR;

namespace Penguor.Compiler.Assembly
{
    public interface IAssemblyGenerator
    {
        public IRProgram Program { get; }

        public Builder Builder { get; }

        /// <summary>
        /// Generate the Assembly
        /// </summary>
        public void Generate();
    }
}
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System.Collections.Generic;

using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Parsing;
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
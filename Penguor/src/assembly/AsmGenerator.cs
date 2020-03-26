/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
*/

using System.Collections.Generic;
using Penguor.Debugging;
using Penguor.Parsing.AST;

namespace Penguor.ASM
{
    /// <summary>
    /// generates intermediate language code from an AST
    /// </summary>
    public class AsmGenerator
    {

        /// <summary>
        /// Generate PIL from an AST
        /// </summary>
        /// <param name="Ast">the AST to convert from</param>
        /// <returns></returns>
        public Library GenerateFromAST(Stmt Ast)
        {
            Library library = new Library();
            AsmVisitor visitor = new AsmVisitor(ref library);
            Ast.Accept(visitor);

            return library;
        }



        /// <summary>
        /// generates a file of the assembly code
        /// </summary>
        /// <param name="asm">the asm code to write into a file</param>
        /// <param name="fileName">the file to write to</param>
        public void ToFile(List<string> asm, string fileName)
        {

        }
    }
}
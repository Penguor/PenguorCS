/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{
    /// <summary>
    /// A CompilerStmt Stmt
    /// </summary>
    public sealed class CompilerStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of CompilerStmt
        /// </summary>
        public CompilerStmt(TokenType dir, Token[] val)
        {
            Dir = dir;
            Val = val;
        }
        public TokenType Dir { get; }
        public Token[] Val { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface IStmtVisitor<T>
    {
        /// <summary>
        /// visit a CompilerStmt
        /// </summary>
        /// <returns></returns>
        T Visit(CompilerStmt stmt);
    }
}

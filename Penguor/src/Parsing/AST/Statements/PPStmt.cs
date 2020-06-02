/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A PPStmt Stmt
    /// </summary>
    public sealed class PPStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of PPStmt
        /// </summary>
        public PPStmt(TokenType dir, Token[] val)
        {
            Dir = dir;
            Val = val;
        }
        /// <summary></summary>
        public TokenType Dir { get; private set; }
        /// <summary></summary>
        public Token[] Val { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a PPStmt
        /// </summary>
        /// <returns></returns>
        T Visit(PPStmt stmt);
    }
}

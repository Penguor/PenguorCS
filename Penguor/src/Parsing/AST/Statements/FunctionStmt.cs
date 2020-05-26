/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{

    /// <summary>
    /// A FunctionStmt Stmt
    /// </summary>
    public sealed class FunctionStmt : Stmt
    {
        /// <summary>
        /// creates a new instance of FunctionStmt
        /// </summary>
        public FunctionStmt(Token accessmod, Token[] nonaccessmod, Token returns, Token name, List<Stmt> parameters)
        {
            AccessMod = accessmod;
            NonAccessMod = nonaccessmod;
            Returns = returns;
            Name = name;
            Parameters = parameters;
        }
        /// <summary></summary>
        public Token AccessMod { get; private set; }
        /// <summary></summary>
        public Token[] NonAccessMod { get; private set; }
        /// <summary></summary>
        public Token Returns { get; private set; }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public List<Stmt> Parameters { get; private set; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override string Accept(Visitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Stmt
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a FunctionStmt
        /// </summary>
        /// <returns></returns>
        string Visit(FunctionStmt stmt);
    }
}

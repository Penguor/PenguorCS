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
    /// A FunctionCall Call
    /// </summary>
    public sealed class FunctionCall : Call
    {
        /// <summary>
        /// creates a new instance of FunctionCall
        /// </summary>
        public FunctionCall(Token name, List<Expr>? args)
        {
            Name = name;
            Args = args;
        }
        /// <summary></summary>
        public Token Name { get; private set; }
        /// <summary></summary>
        public List<Expr>? Args { get; private set; }

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
    /// Contains methods to visit all Call
    /// </summary>
    public partial interface Visitor
    {
        /// <summary>
        /// visit a FunctionCall
        /// </summary>
        /// <returns></returns>
        string Visit(FunctionCall call);
    }
}

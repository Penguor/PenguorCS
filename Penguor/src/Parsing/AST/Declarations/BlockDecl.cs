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
    /// A BlockDecl Decl
    /// </summary>
    public sealed class BlockDecl : Decl
    {
        /// <summary>
        /// creates a new instance of BlockDecl
        /// </summary>
        public BlockDecl(List<Decl> content)
        {
            Content = content;
        }
        /// <summary></summary>
        public List<Decl> Content { get; private set; }

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
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface Visitor<T>
    {
        /// <summary>
        /// visit a BlockDecl
        /// </summary>
        /// <returns></returns>
        T Visit(BlockDecl decl);
    }
}

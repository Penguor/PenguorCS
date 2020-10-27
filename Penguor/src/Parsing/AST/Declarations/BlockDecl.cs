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
    /// A BlockDecl Decl
    /// </summary>
    public sealed class BlockDecl : Decl
    {
        /// <summary>
        /// creates a new instance of BlockDecl
        /// </summary>
        public BlockDecl(int offset, List<Decl> content)
        {
            Offset = offset;
            Content = content;
        }
        public int Offset { get; }
        public List<Decl> Content { get; }

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name="visitor">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(IDeclVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    /// <summary>
    /// Contains methods to visit all Decl
    /// </summary>
    public partial interface IDeclVisitor<T>
    {
        /// <summary>
        /// visit a BlockDecl
        /// </summary>
        /// <returns></returns>
        T Visit(BlockDecl decl);
    }
}

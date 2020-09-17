/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
#
#
*/

using Penguor.Compiler.Parsing;

namespace Penguor.Compiler.IR
{
    public class IRLibrary : IRDecl
    {
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }
        public IRLibrary(State name, TokenType? accessMod, TokenType[] nonAccessMod) : base(name)
        {
            AccessMod = accessMod;
            NonAccessMod = nonAccessMod;
        }
    }
}
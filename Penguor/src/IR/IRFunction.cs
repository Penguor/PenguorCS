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
    public class IRFunction : IRDecl
    {
        public TokenType? AccessMod { get; }
        public TokenType[] NonAccessMod { get; }

        public State[] Signature { get; }

        public IRFunction(State name, TokenType? accessMod, TokenType[] nonAccessMod, State[] signature) : base(name)
        {
            AccessMod = accessMod;
            NonAccessMod = nonAccessMod;
            Signature = signature;
        }
    }
}
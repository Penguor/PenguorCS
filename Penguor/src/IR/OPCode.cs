/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
#
#
*/

#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an instruction in the Penguor intermediate representation
    /// </summary>
    public enum OPCode : byte
    {
        LABEL, LIB,
        USE,
        LOAD, LOADARG, LOADPARAM,
        DEF, DFE, ASSIGN, DEFINT, DEFSTR, // define, define empty, assign
        CALL,
        RET, RETN,
        ADD, SUB, MUL, DIV,
        LESS, GREATER, LESS_EQUALS, GREATER_EQUALS,
        EQUALS,
        JTR, // jump if true
        JFL, // jump if false
        GOTO,
        ERR // invalid opcode, returned on error
    }
}
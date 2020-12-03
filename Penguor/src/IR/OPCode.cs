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
        RETURN,
        ADD, SUB, MUL, DIV,
        LESS, GREATER,
        JTR // jump if true
    }
}
#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an instruction in the Penguor intermediate representation
    /// </summary>
    public enum OPCode : byte
    {
        LABEL, LIB, FUNC,
        USE,
        LOAD, LOADARG, LOADPARAM,
        DEF, DFE, ASSIGN, // define, define empty, assign
        CALL,
        RET, RETN,
        ADD, SUB, MUL, DIV,
        INCR, DECR,
        LESS, GREATER, LESS_EQUALS, GREATER_EQUALS,
        EQUALS,
        INVERT,
        CHS, // change sign
        JTR, // jump if true
        JFL, // jump if false
        GOTO,
        ERR // invalid opcode, returned on error
    }
}
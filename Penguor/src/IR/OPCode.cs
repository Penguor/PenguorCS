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
        DEF, DFE, ASSIGN, DFL, DFLE, // define, define empty, assign, define local
        BCALL, ECALL, CALL,
        RET, RETN,
        ADD, SUB, MUL, DIV,
        INCR, DECR,
        LESS, GREATER, LESS_EQUALS, GREATER_EQUALS,
        EQUALS,
        INVERT,
        ABS, CHS, // absolute, change sign
        JMP, JTR, JFL,// jump, jump if true, jump if false
        GOTO,
        ERR, // invalid opcode, returned on error
        ASM
    }
}
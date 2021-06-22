#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an instruction in the Penguor intermediate representation
    /// </summary>
    public enum IROPCode : byte
    {
        LABEL, LIB, FUNC,
        USE,
        LOAD, LOADARG, LOADPARAM,
        LDARGDIR, // load arg direct
        DEFEXT, // define extern
        BCALL, ECALL, CALL,
        RET, RETN,
        ADD, SUB, MUL, DIV, REMAINDER,
        INCR, DECR,
        LESS, GREATER, LESS_EQUALS, GREATER_EQUALS,
        AND, OR,
        EQUALS,
        INVERT,
        ABS, CHS, // absolute, change sign
        JMP, JTR, JFL,// jump, jump if true, jump if false
        JL, JNL, JLE, JNLE, // jump if less, jump if less or equal
        JG, JNG, JGE, JNGE, // jump if greater, jump if greater or equal
        JE, JNE, // jump if equal, jump if not equal
        ERR, // invalid opcode, returned on error
        UNDEF, // undefined
        ASM,
        PHI, REROUTE, MOV
    }
}
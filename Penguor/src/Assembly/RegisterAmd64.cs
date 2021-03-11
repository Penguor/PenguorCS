#pragma warning disable 1591

namespace Penguor.Compiler.Assembly
{
    public enum RegisterAmd64
    {
        RAX = 1, RBX, RCX, RDX,
        RBP, RSP,
        RSI, RSD,
        R8, R9, R10, R11, R12, R13, R14, R15,
        // specifies that variable is on stack instead of register
        STACK = -1
    }
}
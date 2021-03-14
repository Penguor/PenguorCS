using static Penguor.Compiler.Assembly.RegisterAmd64;

namespace Penguor.Compiler.Assembly
{
    public static class RegisterSetAmd64
    {
        public static RegisterAmd64[] GeneralPurpose = { RAX, RBX, RCX, RDX, RDI, RSI, RBP, RSP, R8, R9, R10, R11, R12, R13, R14, R15 };
        public static RegisterAmd64[] XMM = { XMM0, XMM1, XMM2, XMM3, XMM4, XMM5, XMM6, XMM7, XMM8, XMM9, XMM10, XMM11, XMM12, XMM13, XMM14, XMM15 };
        public static RegisterAmd64[] Return64 = { RAX };
    }
}
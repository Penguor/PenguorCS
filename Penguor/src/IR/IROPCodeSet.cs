using static Penguor.Compiler.IR.IROPCode;

namespace Penguor.Compiler.IR
{
    public static class IROPCodeSet
    {
        public static IROPCode[] Jump = { JMP, JTR, JFL, JL, JNL, JLE, JNLE, JG, JNG, JGE, JNGE, JE, JNE };
    }
}
namespace Penguor.Compiler.Assembly
{
    public interface IEmitter
    {
        public string Emit(AsmSyntax syntax);
    }
}
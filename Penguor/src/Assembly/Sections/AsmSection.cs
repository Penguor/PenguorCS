namespace Penguor.Compiler.Assembly
{
    public abstract class AsmSection : IEmitter
    {
        protected string name;
        public AsmSection(string name)
        {
            this.name = name;
        }

        public abstract string Emit(AsmSyntax syntax);
    }
}

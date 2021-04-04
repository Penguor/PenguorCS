namespace Penguor.Compiler.Assembly
{
    public class AsmVariableAmd64 : IEmitter
    {

        public string Name { get; }

        public string Size { get; }
        public string[] Values { get; }

        public AsmVariableAmd64(string name, string size, string[] values)
        {
            Name = name;
            Size = size;
            Values = values;
        }

        public string Emit(AsmSyntax syntax)
        {
            return $"{Name} {Size} {string.Join(',', Values)}";
        }
    }
}
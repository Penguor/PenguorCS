namespace Penguor.Compiler.Assembly
{
    public abstract class AsmOperand { }

    public class AsmString : AsmOperand
    {
        string Value { get; set; }
        public AsmString(string value) => Value = value;
    }
}
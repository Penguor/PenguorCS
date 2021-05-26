namespace Penguor.Compiler.Assembly
{
    public abstract class AsmOperand : IEmitter
    {
        public abstract string Emit(AsmSyntax syntax);
    }

    public class AsmString : AsmOperand
    {
        string Value { get; set; }
        public AsmString(string value) => Value = value;

        public override string Emit(AsmSyntax syntax)
        {
            return Value;
        }
    }
    public class AsmNumber : AsmOperand
    {
        double Value { get; set; }
        public AsmNumber(double value) => Value = value;

        public override string Emit(AsmSyntax syntax)
        {
            return Value.ToString();
        }
    }

    public class AsmRegister : AsmOperand
    {
        RegisterAmd64 Value { get; set; }
        public AsmRegister(RegisterAmd64 value) => Value = value;
        public AsmRegister(int register) => Value = (RegisterAmd64)register;

        public override string Emit(AsmSyntax syntax)
        {
            return Value.ToString();
        }
    }

    public class AsmAddress : AsmOperand
    {
        RegisterAmd64 Value { get; set; }
        public AsmAddress(RegisterAmd64 value) => Value = value;
        public AsmAddress(int register) => Value = (RegisterAmd64)register;

        public override string Emit(AsmSyntax syntax)
        {
            return "[" + Value.ToString() + "]";
        }
    }
}


namespace Penguor.Compiler.IR
{
    /// <summary>
    /// an argument of a statement
    /// </summary>
    public record IRState : IRArgument
    {
        public State State { get; }

        public IRState(State state)
        {
            State = (State)state.Clone();
        }

        /// <inheritdoc/>
        public override string ToString() => State.ToString();
    }
}
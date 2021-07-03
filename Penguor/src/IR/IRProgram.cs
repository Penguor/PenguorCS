using System;
using System.Collections.Generic;
using System.Text;

namespace Penguor.Compiler.IR
{
    /// <summary>
    /// the base of a program
    /// </summary>
    public class IRProgram
    {
        public List<IRStatement> GlobalStatements { get; init; } = new();

        private int _globalInstructionNumber;
        private int GlobalInstructionNumber { get => _globalInstructionNumber++; }

        public Dictionary<State, IRFunction> Functions { get; init; } = new();
        public State? CurrentFunctionId { get; set; }

        public void AddFunction(State name)
        {
            var id = (State)name.Clone();
            Functions.Add(id, new IRFunction(id));
        }

        /// <summary>
        /// Sets the current function, creates it if there is no corresponding key in the dictionary
        /// creates and begins the first IRBlock
        /// adds the FUNC statement
        /// </summary>
        /// <param name="name">the name of the function to begin</param>
        /// <returns>the name of the function, which also is the id of the IRBlock created</returns>
        public State BeginFunction(State name)
        {
            var id = (State)name.Clone();
            if (!Functions.ContainsKey(id))
            {
                AddFunction(id);
            }

            CurrentFunctionId = id;
            Functions[id].BeginBlock(id);
            Functions[id].AddStmt(IROPCode.FUNC, new IRState(id));

            return id;
        }

        public void SealCurrentFunction()
        {
            var function = TryGetCurrentFunction()!;
            if (function.GetLastStatement().Code is not IROPCode.RET or IROPCode.RETN)
            {
                function.AddStmt(IROPCode.RETN);
            }
            function.SealBlock(function.CurrentBlock!);

            CurrentFunctionId = null;
        }

        public IRFunction? TryGetCurrentFunction()
        {
            if (CurrentFunctionId is not null)
                return Functions.GetValueOrDefault(CurrentFunctionId);
            else
                return null;
        }

        public IRFunction GetCurrentFunction()
        {
            if (CurrentFunctionId is not null)
                return Functions.GetValueOrDefault(CurrentFunctionId) ?? throw new NullReferenceException();
            else
                throw new NullReferenceException();
        }

        public State? TryGetCurrentBlock() => TryGetCurrentFunction()?.CurrentBlock;

        public IRReference AddGlobalStmt(IROPCode code, params IRArgument[] operands)
        {
            var num = GlobalInstructionNumber;
            GlobalStatements.Add(new IRStatement(num, code, operands));
            return new IRReference(num);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine();
            foreach (var i in GlobalStatements)
                builder.AppendLine(i.ToString());
            builder.AppendLine();
            foreach ((_, var i) in Functions)
                builder.AppendLine(i.ToString());
            return builder.ToString();
        }
    }
}
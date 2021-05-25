using System;
using System.Collections.Generic;
using Penguor.Compiler.Analysis;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IExceptionHandler, IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<IRReference>
    {
        private readonly Builder builder;
        private readonly ProgramDecl program;
        private IRProgram irProgram = new();

        private readonly List<State> scopes;
        private int _instructionNumber;
        private int InstructionNumber { get => _instructionNumber++; }

        private readonly List<IRStatement> statements = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
            scopes.Add(new State());
            currentBlock = CreateBlock(new State(), false, false);
            blocks[currentBlock].First = new IRReference(0);
        }

        //ssa related stuff
        // paper: Simple and Efficient Construction of Static Single Assignment Form, Braun et al.
        // https://pp.info.uni-karlsruhe.de/uploads/publikationen/braun13cc.pdf

        private readonly Dictionary<State, IRBlock> blocks = new();
        private readonly Dictionary<State, Dictionary<State, IRReference>> currentDefinition = new();

        private readonly List<State> sealedBlocks = new();

        private readonly Dictionary<State, Dictionary<State, IRReference>> incompletePhis = new();

        private State currentBlock;

        private int _currentBlockID;
        private int CurrentBlockID { get => _currentBlockID++; }

        private void WriteVariable(State variable, State block, IRReference value)
        {
            currentDefinition.TryAdd(variable, new());
            currentDefinition[variable][block] = value;
        }

        private IRReference ReadVariable(State variable, State block)
        {
            if (currentDefinition[variable].ContainsKey(block))
                return currentDefinition[variable][block];
            else
                return ReadVariableRecursive(variable, block);
        }

        private IRReference ReadVariableRecursive(State variable, State block)
        {
            IRReference value;
            if (!sealedBlocks.Contains(block))
            {
                value = AddPhi(block);
                incompletePhis.TryAdd(block, new());
                incompletePhis[block][variable] = value;
            }
            else if (blocks[block].Predecessors.Count == 1)
            {
                value = ReadVariable(variable, blocks[block].Predecessors[0]);
            }
            else
            {
                value = AddPhi(block);
                WriteVariable(variable, block, value);
                AddPhiOperands(variable, value);
            }
            WriteVariable(variable, block, value);
            return value;
        }

        private IRReference AddPhi(State block) => AddReference(AddStmt(IROPCode.PHI, new IRPhi(block)));

        private IRReference AddPhiOperands(State variable, IRReference phi)
        {
            IRPhi irPhi = (IRPhi)statements[phi.Referenced].Operands[0];
            foreach (var predecessor in FindBlock(irPhi.Block)?.Predecessors ?? throw new NullReferenceException())
                irPhi.AppendOperand(ReadVariable(variable, predecessor));
            return TryRemoveTrivialPhi(phi);
        }

        private IRReference TryRemoveTrivialPhi(IRReference phi)
        {
            IRPhi irPhi = (IRPhi)statements[phi.Referenced].Operands[0];
            IRReference? same = null;
            foreach (var op in irPhi.Operands)
            {
                if (op == same || op == phi)
                    continue;
                if (same != null)
                    return phi;
                same = op;
            }
            if (same == null)
                //todo: create undefined
                same = new IRReference(0);
            irPhi.Users.Remove(phi);

            for (int i = 0; i < statements.Count; i++)
            {
                for (int i2 = 0; i2 < statements[i].Operands.Length; i2++)
                {
                    if (statements[i].Operands[i2] is IRReference reference && reference.Equals(phi))
                    {
                        statements[i].Operands[i2] = new IRReference(same.Referenced);
                    }
                    else if (statements[i].Operands[i2] is IRPhi newPhi && newPhi.Operands.Contains(phi))
                    {
                        newPhi.Operands[newPhi.Operands.FindIndex(op => op.Equals(same))] = new IRReference(same.Referenced);
                    }
                }
            }
            statements[phi.Referenced] = statements[phi.Referenced] with { Code = IROPCode.REROUTE, Operands = new IRArgument[] { same } };

            foreach (var use in irPhi.Users)
            {
                if (statements[use.Referenced].Code == IROPCode.PHI)
                    TryRemoveTrivialPhi(use);
            }

            return same;
        }

        private State BeginBlock(State newBlock, bool labelled)
        {
            currentBlock = CreateBlock(newBlock, true, labelled);
            blocks[currentBlock].First = new IRReference(_instructionNumber);
            return currentBlock;
        }

        private State CreateBlock(State block, bool addPred, bool labelled)
        {
            if (labelled)
            {
                var blockID = (State)block.Clone();
                if (!blocks.ContainsKey(blockID))
                {
                    blocks.Add(blockID, new IRBlock(blockID));
                    incompletePhis[blockID] = new();
                }
                if (addPred && blockID != currentBlock)
                    blocks[blockID].AddPredecessor(currentBlock);
                return blockID;
            }
            else
            {
                var blockID = (State)block.Clone();
                blocks.Add(blockID, new IRBlock(blockID));
                incompletePhis[blockID] = new();
                if (addPred)
                    blocks[blockID].AddPredecessor(currentBlock);
                return blockID;
            }
        }

        private IRBlock? FindBlock(State state) => blocks.GetValueOrDefault(state);

        private void SealBlock(State block)
        {
            if (sealedBlocks.Contains(block)) return;
            foreach (var i in incompletePhis[block])
                AddPhiOperands(i.Key, incompletePhis[block][i.Key]);
            sealedBlocks.Add(block);
        }

        private State AddJumpStmt(IROPCode code, IRState jumpTo, params IRArgument[] args)
        {
            IRArgument[] allArguments = new IRArgument[args.Length + 1];
            allArguments[0] = new IRState((State)jumpTo.State.Clone());
            Array.Copy(args, 0, allArguments, 1, args.Length);
            AddStmt(code, allArguments);
            return CreateBlock(jumpTo.State, true, true);
        }

        private IRFunction? currentFunction;

        private void BeginFunction()
        {
            currentFunction = new IRFunction((State)scopes[0].Clone(), GetLastNumber());
        }

        private void EndFunction()
        {
            currentFunction?.Statements.AddRange(statements.GetRange(currentFunction.First.Referenced, GetLastNumber().Referenced - currentFunction.First.Referenced + 1));
            irProgram.Functions.Add(currentFunction!);
            currentFunction = null;
        }

        private int AddStmt(IROPCode code, params IRArgument[] operands)
        {
            var num = InstructionNumber;
            statements.Add(new IRStatement(num, code, operands));
            return num;
        }

        private IRReference AddReference(int referenced)
        {
            if (statements[referenced].Code == IROPCode.PHI) ((IRPhi)statements[referenced].Operands[0]).AddUser(GetLastNumber());
            return new IRReference(referenced);
        }

        private State AddLabel()
        {
            var block = BeginBlock(scopes[0], true);
            builder.TableManager.AddSymbol(scopes[0]);
            AddStmt(IROPCode.LABEL, new IRState(scopes[0]));
            return block;
        }

        private IRReference GetLastNumber() => new IRReference(statements[statements.Count - 1].Number);

        /// <summary>
        /// Generates ir from an ast (program node)
        /// </summary>
        /// <returns>an IRProgram</returns>
        public IRProgram Generate()
        {
            try
            {
                program.Accept(this);
                foreach (var function in irProgram.Functions)
                {
                    Dictionary<IRStatement, State> phis = new();
                    foreach (var statement in function.Statements)
                    {
                        if (statement.Code is IROPCode.PHI)
                        {
                            phis.Add(statement, ((IRPhi)statement.Operands[0]).Block);
                        }
                    }
                    Dictionary<State, IRStatement> firstStatements = new(blocks.Count);
                    foreach (var block in blocks)
                    {
                        if (block.Value.First!.Referenced >= function.First.Referenced && block.Value.First.Referenced <= function.Statements[^1].Number)
                        {
                            firstStatements.Add(block.Key, function.Statements.Find(s =>
                            s.Number.Equals(block.Value.First!.Referenced)) ?? throw new NullReferenceException("outer"));
                        }
                    }
                    foreach (var phi in phis)
                    {
                        function.Statements.Remove(phi.Key);
                        function.Statements.Insert(function.Statements.FindIndex(s => s.Equals(firstStatements[phi.Value])) + 1, phi.Key);
                    }

                    function.ResolveReroutes();
                    function.ResolveReroutes();
                }
            }
            finally
            {
                if (statements.Count != 0) Logger.Log(irProgram.ToString(), LogLevel.Debug);
            }
            return irProgram;
        }

        public int Visit(BlockDecl decl)
        {
            foreach (var i in decl.Content) i.Accept(this);
            return 0;
        }

        public int Visit(DataDecl decl)
        {
            scopes[0].Push(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(StmtDecl decl) => decl.Stmt.Accept(this);

        public int Visit(FunctionDecl decl)
        {
            scopes[0].Push(decl.Name);
            BeginBlock(scopes[0], true);
            AddStmt(IROPCode.FUNC, new IRState(scopes[0]));
            BeginFunction();
            foreach (var parameter in decl.Parameters)
            {
                AddStmt(IROPCode.LOADPARAM, new IRState(((ExprAttribute)parameter.Attribute!).Type));
                WriteVariable(builder.TableManager.GetStateBySymbol(parameter.Name, scopes)!, currentBlock, GetLastNumber());
            }
            decl.Content.Accept(this);
            if (statements[^1].Code != IROPCode.RET && statements[^1].Code != IROPCode.RETN) AddStmt(IROPCode.RETN);
            scopes[0].Pop();
            SealBlock(currentBlock);
            EndFunction();
            return 0;
        }

        public int Visit(LibraryDecl decl)
        {
            scopes[0].AddRange(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Remove(decl.Name);
            return 0;
        }

        public int Visit(ProgramDecl decl)
        {
            var programBlock = currentBlock;
            foreach (var i in decl.Declarations)
                i.Accept(this);
            SealBlock(programBlock);
            return 0;
        }

        public int Visit(SystemDecl decl)
        {
            scopes[0].Push(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(TypeDecl decl)
        {
            scopes[0].Push(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(UsingDecl decl)
        {
            scopes.Add(State.FromCall(decl.Lib));
            AddStmt(IROPCode.USE, new IRState(State.FromCall(decl.Lib)));
            return 0;
        }

        public int Visit(VarDecl decl)
        {
            State name = builder.TableManager.GetStateBySymbol(decl.Name, scopes) ?? throw new Exception();
            if (decl.Init != null)
            {
                WriteVariable(name, currentBlock, decl.Init.Accept(this));
            }
            return 0;
        }

        public int Visit(BlockStmt stmt)
        {
            SealBlock(currentBlock);
            scopes[0].Push(new AddressFrame(".block", AddressType.BlockStmt));
            var block = BeginBlock(scopes[0], false);
            foreach (var i in stmt.Content)
                i.Accept(this);
            SealBlock(block);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(CaseStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(CompilerStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(DoStmt stmt)
        {
            scopes[0].Push(new AddressFrame($".do{stmt.Id}", AddressType.Control));
            var label = new AddressFrame[scopes[0].Count];
            scopes[0].CopyTo(label, 0);
            stmt.Content.Accept(this);
            scopes[0].Pop();
            stmt.Condition.Accept(this);
            AddStmt(IROPCode.JTR, GetLastNumber(), new IRState(new State(label)));
            return 0;
        }

        public int Visit(ElifStmt stmt)
        {
            SealBlock(currentBlock);

            //condition
            scopes[0].Push(new AddressFrame($".elif{stmt.Id}", AddressType.Control));
            var conditionBlock = BeginBlock(scopes[0] + new AddressFrame(".c", AddressType.Control), true);
            stmt.Condition.Accept(this);
            AddJumpStmt(IROPCode.JFL, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)));
            SealBlock(conditionBlock);

            //content
            var contentBlock = BeginBlock(scopes[0], true);
            stmt.Content.Accept(this);

            AddJumpStmt(IROPCode.JMP, new IRState(scopes[0]));
            SealBlock(contentBlock);

            //rest
            scopes[0].Push(new AddressFrame(".e", AddressType.Control));
            AddLabel();
            scopes[0].Pop(2);

            return 0;
        }

        public int Visit(ExprStmt stmt)
        {
            stmt.Expr.Accept(this);
            return 0;
        }

        public int Visit(ForStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(IfStmt stmt)
        {
            SealBlock(currentBlock);

            bool hasElse = stmt.Elif.Count > 0 || stmt.ElseC != null;

            //condition
            scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
            var conditionBlock = BeginBlock(scopes[0] + new AddressFrame(".c", AddressType.Control), true);
            stmt.Condition.Accept(this);
            IROPCode jumpCode = statements[^1].Code switch
            {
                IROPCode.LOAD => IROPCode.JTR,
                IROPCode.LESS => IROPCode.JL,
                IROPCode.LESS_EQUALS => IROPCode.JLE,
                IROPCode.GREATER => IROPCode.JG,
                IROPCode.GREATER_EQUALS => IROPCode.JGE,
                IROPCode.EQUALS => IROPCode.JE
            };
            if (hasElse)
                AddJumpStmt(jumpCode, new IRState(scopes[0] + new AddressFrame(".else", AddressType.Control)));
            else
                AddJumpStmt(jumpCode, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)));
            SealBlock(conditionBlock);

            //content
            var contentBlock = BeginBlock(scopes[0], true);
            stmt.IfC.Accept(this);
            if (hasElse)
                AddJumpStmt(IROPCode.JMP, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)));

            SealBlock(contentBlock);

            //rest
            if (hasElse)
            {
                scopes[0].Push(new AddressFrame(".else", AddressType.Control));
                AddLabel();
                scopes[0].Pop();
                foreach (var i in stmt.Elif)
                {
                    i.Accept(this);
                }
                if (stmt.ElseC != null)
                {
                    BeginBlock(scopes[0], true);
                    stmt.ElseC.Accept(this);
                }
            }
            scopes[0].Push(new AddressFrame(".e", AddressType.Control));
            AddLabel();
            scopes[0].Pop(2);
            return 0;
        }

        public int Visit(AsmStmt stmt)
        {
            foreach (var i in stmt.Contents)
                AddStmt(IROPCode.ASM, new IRString(i));
            return 0;
        }

        public int Visit(ReturnStmt stmt)
        {
            stmt.Value?.Accept(this);
            if (stmt.Value != null) AddStmt(IROPCode.RET, GetLastNumber());
            else AddStmt(IROPCode.RETN);
            return 0;
        }

        public int Visit(SwitchStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(VarStmt stmt)
        {
            State name = builder.TableManager.GetStateBySymbol(stmt.Name, scopes) ?? throw new Exception();
            if (stmt.Init != null)
            {
                WriteVariable(name, currentBlock, stmt.Init.Accept(this));
            }
            return 0;
        }

        public int Visit(WhileStmt stmt)
        {
            SealBlock(currentBlock);

            // condition
            scopes[0].Push(new AddressFrame($".while{stmt.Id}", AddressType.Control));
            scopes[0].Push(new AddressFrame(".c", AddressType.Control));
            var conditionBlock = AddLabel();
            scopes[0].Pop();
            stmt.Condition.Accept(this);
            IROPCode jumpCode = statements[^1].Code switch
            {
                IROPCode.LOAD => IROPCode.JFL,
                IROPCode.LESS => IROPCode.JNL,
                IROPCode.LESS_EQUALS => IROPCode.JNLE,
                IROPCode.GREATER => IROPCode.JNG,
                IROPCode.GREATER_EQUALS => IROPCode.JNGE,
                IROPCode.EQUALS => IROPCode.JNE
            };
            AddJumpStmt(jumpCode, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)), GetLastNumber());

            // content
            var contentBlock = AddLabel();
            stmt.Content.Accept(this);
            SealBlock(currentBlock);
            AddJumpStmt(IROPCode.JMP, new IRState(scopes[0] + new AddressFrame(".c", AddressType.Control)));
            SealBlock(contentBlock);

            SealBlock(conditionBlock);

            //rest
            scopes[0].Push(new AddressFrame(".e", AddressType.Control));
            AddLabel();
            scopes[0].Pop(2);
            return 0;
        }

        public IRReference Visit(AssignExpr expr)
        {
            State sym = builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception();

            var value = expr.Value.Accept(this);
            switch (expr.Op)
            {
                case TokenType.ASSIGN:
                    break;
                case TokenType.ADD_ASSIGN:
                    AddStmt(IROPCode.ADD, ReadVariable(sym, currentBlock), value);
                    break;
                case TokenType.SUB_ASSIGN:
                    AddStmt(IROPCode.SUB, ReadVariable(sym, currentBlock), value);
                    break;
                case TokenType.MUL_ASSIGN:
                    AddStmt(IROPCode.MUL, ReadVariable(sym, currentBlock), value);
                    break;
                default:
                    throw new Exception();
            }
            WriteVariable(sym, currentBlock, GetLastNumber());

            return GetLastNumber();
        }

        public IRReference Visit(BinaryExpr expr)
        {
            IRReference? addr1 = expr.Lhs.Accept(this), addr2 = expr.Rhs.Accept(this);

            return AddReference(AddStmt(expr.Op switch
            {
                TokenType.PLUS => IROPCode.ADD,
                TokenType.MINUS => IROPCode.SUB,
                TokenType.MUL => IROPCode.MUL,
                TokenType.DIV => IROPCode.DIV,
                TokenType.GREATER => IROPCode.GREATER,
                TokenType.LESS => IROPCode.LESS,
                TokenType.GREATER_EQUALS => IROPCode.GREATER_EQUALS,
                TokenType.LESS_EQUALS => IROPCode.LESS_EQUALS,
                TokenType.EQUALS => IROPCode.EQUALS,
                TokenType a => builder.Except(IROPCode.ERR, 9, expr.Offset, Token.ToString(a))
            }
            , addr1
            , addr2));
        }

        public IRReference Visit(BooleanExpr expr) => AddReference(AddStmt(IROPCode.LOAD, new IRBool(expr.Value)));

        public IRReference Visit(CallExpr expr)
        {
            if (expr.Callee[^1] is IdfCall)
            {
                return ReadVariable(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception(), currentBlock);
            }
            else if (expr.Callee[^1] is FunctionCall fCall)
            {
                AddStmt(IROPCode.BCALL);

                IRArgument[] arguments = new IRArgument[fCall.Args.Count + 1];
                for (int i = 0; i < fCall.Args.Count; i++)
                {
                    var reference = fCall.Args[i].Accept(this);
                    AddStmt(IROPCode.LOADARG, new IRState(((ExprAttribute)fCall.Args[i].Attribute!).Type), reference);
                    arguments[i + 1] = GetLastNumber();
                }
                arguments[0] = new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception());
                return AddReference(AddStmt(IROPCode.CALL, arguments));
            }
            else
            {
                throw new Exception();
            }
        }

        public IRReference Visit(GroupingExpr expr) => expr.Content.Accept(this);
        public IRReference Visit(NullExpr expr) => AddReference(AddStmt(IROPCode.LOAD, new IRNull()));

        public IRReference Visit(NumExpr expr)
        {
            if (expr.Value.Contains('.')) return AddReference(AddStmt(IROPCode.LOAD, new IRDouble(expr.NumValue ?? throw new Exception())));
            else return AddReference(AddStmt(IROPCode.LOAD, new IRInt((int?)expr.NumValue ?? throw new Exception())));
        }

        public IRReference Visit(StringExpr expr) => AddReference(AddStmt(IROPCode.LOAD, new IRString(expr.Value)));

        public IRReference Visit(UnaryExpr expr)
        {
            //todo: verify this
            expr.Rhs.Accept(this);
            if (expr.Op != null)
            {
                AddStmt(expr.Op switch
                {
                    TokenType.EXCL_MARK => IROPCode.INVERT,
                    TokenType.PLUS => IROPCode.ABS,
                    TokenType.MINUS => IROPCode.CHS,
                    _ => throw new Exception(),
                }, GetLastNumber());
            }

            return GetLastNumber();
        }

        public IRReference Visit(VarExpr expr)
        {
            AddStmt(
                IROPCode.DFE,
                new IRState(builder.TableManager.GetStateBySymbol(expr.Name, scopes) ?? throw new NullReferenceException()));
            WriteVariable(builder.TableManager.GetStateBySymbol(expr.Name, scopes) ?? throw new NullReferenceException(), currentBlock, GetLastNumber());
            return GetLastNumber();
        }

        public int Visit(StmtBlockDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(ModifiedDecl decl)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(TypeCallExpr expr)
        {
            throw new NotImplementedException();
        }
    }
}
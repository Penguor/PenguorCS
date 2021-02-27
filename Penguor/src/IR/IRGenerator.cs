using System;
using System.Collections.Generic;
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

        private readonly List<State> scopes;
        private uint _instructionNumber;
        private uint InstructionNumber { get => _instructionNumber++; }

        private readonly SortedDictionary<uint, IRStatement> statements = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
            scopes.Add(new State());
            currentBlock = CreateBlock(new State(), false, false);
        }

        //ssa related stuff
        // paper: Simple and Efficient Construction of Static Single Assignment Form, Braun et al.
        // https://pp.info.uni-karlsruhe.de/uploads/publikationen/braun13cc.pdf

        private readonly Dictionary<BlockID, IRBlock> blocks = new();
        private readonly Dictionary<State, Dictionary<BlockID, IRReference>> currentDefinition = new();

        private readonly List<BlockID> sealedBlocks = new();

        private readonly Dictionary<BlockID, Dictionary<State, IRReference>> incompletePhis = new();

        private BlockID currentBlock;

        private int _currentBlockID;
        private int CurrentBlockID { get => _currentBlockID++; }

        private void WriteVariable(State variable, BlockID block, IRReference value)
        {
            currentDefinition.TryAdd(variable, new());
            currentDefinition[variable][block] = value;
        }

        private IRReference ReadVariable(State variable, BlockID block)
        {
            if (currentDefinition[variable].ContainsKey(block))
                return currentDefinition[variable][block];
            else
                return ReadVariableRecursive(variable, block);
        }

        private IRReference ReadVariableRecursive(State variable, BlockID block)
        {
            IRReference value;
            if (!sealedBlocks.Contains(block))
            {
                value = AddPhi();
                incompletePhis.TryAdd(block, new());
                incompletePhis[block][variable] = value;
            }
            else if (blocks[block].Predecessors.Count == 1)
            {
                value = ReadVariable(variable, blocks[block].Predecessors[0]);
            }
            else
            {
                value = AddPhi();
                WriteVariable(variable, block, value);
                AddPhiOperands(variable, value);
            }
            WriteVariable(variable, block, value);
            return value;
        }

        private IRReference AddPhi() => new(AddStmt(IROPCode.PHI, new IRPhi(currentBlock)));

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

            statements[phi.Referenced] = statements[phi.Referenced] with { Code = IROPCode.REROUTE, Operands = new IRArgument[] { same } };

            foreach (var use in irPhi.Users)
            {
                if (statements[use.Referenced].Code == IROPCode.PHI)
                    TryRemoveTrivialPhi(use);
            }

            return same;
        }

        private BlockID BeginBlock(State newBlock, bool labelled)
        {
            currentBlock = CreateBlock(newBlock, true, labelled);
            return currentBlock;
        }

        private BlockID CreateBlock(State block, bool addPred, bool labelled)
        {
            if (labelled)
            {
                var blockID = new BlockID(-1, (State)block.Clone());
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
                var blockID = new BlockID(CurrentBlockID, (State)block.Clone());
                blocks.Add(blockID, new IRBlock(blockID));
                incompletePhis[blockID] = new();
                if (addPred)
                    blocks[blockID].AddPredecessor(currentBlock);
                return blockID;
            }
        }

        private IRBlock? FindBlock(BlockID state) => blocks.GetValueOrDefault(state);

        private void SealBlock(BlockID block)
        {
            if (sealedBlocks.Contains(block)) return;
            foreach (var i in incompletePhis[block])
                AddPhiOperands(i.Key, incompletePhis[block][i.Key]);
            sealedBlocks.Add(block);
        }

        private BlockID AddJumpStmt(IROPCode code, IRState jumpTo)
        {
            AddStmt(code, jumpTo);
            return CreateBlock(jumpTo.State, true, true);
        }

        private uint AddStmt(IROPCode code, params IRArgument[] operands)
        {
            var num = InstructionNumber;
            statements.Add(num, new IRStatement(num, code, operands));
            return num;
        }

        private IRReference AddReference(uint referenced)
        {
            // if (statements[referenced].Code == IROPCode.PHI) ((IRPhi)statements[referenced].Operands[0]).AddUser(GetLastNumber());
            return new IRReference(referenced);
        }

        private BlockID AddLabel()
        {
            var block = BeginBlock(scopes[0], true);
            builder.TableManager.AddSymbol(scopes[0]);
            AddStmt(IROPCode.LABEL, new IRState(scopes[0]));
            return block;
        }

        private IRReference GetLastNumber() => new IRReference(statements[(uint)statements.Count - 1].Number);

        /// <summary>
        /// Generates ir from an ast (program node)
        /// </summary>
        /// <returns>an IRProgram</returns>
        public IRProgram Generate()
        {
            try
            {
                program.Accept(this);
            }
            finally
            {
                if (statements.Count != 0) Logger.Log(new IRProgram(statements.Values).ToString(), LogLevel.Debug);
            }
            return new IRProgram(statements.Values);
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
            foreach (var parameter in decl.Parameters)
            {
                parameter.Accept(this);
            }
            decl.Content.Accept(this);
            if (statements[(uint)statements.Count - 1].Code != IROPCode.RET) AddStmt(IROPCode.RETN);
            scopes[0].Pop();
            SealBlock(currentBlock);
            return 0;
        }

        public int Visit(LibraryDecl decl)
        {
            scopes[0].Append(decl.Name);
            AddStmt(IROPCode.LIB, new IRState(scopes[0]));
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
            scopes[0].Push(decl.Name);

            if (decl.Init != null)
            {
                IRArgument arg = decl.Init switch
                {
                    NumExpr e => new IRDouble(e.NumValue ?? throw new Exception()),
                    StringExpr s => new IRString(s.Value),
                    _ => AcceptInit()
                };
                AddStmt(IROPCode.DEF, new IRState(scopes[0]), arg);
            }
            else
            {
                AddStmt(IROPCode.DFE, new IRState(scopes[0]), new IRState(builder.TableManager.GetSymbol(scopes[0], scopes.ToArray())?.DataType ?? throw new Exception()));
            }
            scopes[0].Pop();

            return 0;

            IRArgument AcceptInit()
            {
                decl.Init!.Accept(this);
                return GetLastNumber();
            }
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

            //condition
            scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
            var conditionBlock = BeginBlock(scopes[0] + new AddressFrame(".c", AddressType.Control), true);
            stmt.Condition.Accept(this);
            AddJumpStmt(IROPCode.JFL, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)));
            SealBlock(conditionBlock);

            //content
            var contentBlock = BeginBlock(scopes[0], true);
            stmt.IfC.Accept(this);

            SealBlock(contentBlock);

            //todo: elif and else blocks

            //rest
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
            AddJumpStmt(IROPCode.JFL, new IRState(scopes[0] + new AddressFrame(".e", AddressType.Control)));

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
            IRReference? addr1 = null, addr2 = null;
            double? num1 = expr.Lhs is NumExpr e ? e.NumValue ?? throw new Exception() : null;
            if (num1 == null)
            {
                addr1 = expr.Lhs.Accept(this);
            }
            double? num2 = expr.Lhs is NumExpr e1 ? e1.NumValue ?? throw new Exception() : null;
            if (num2 == null)
            {
                addr2 = expr.Rhs.Accept(this);
            }

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
            , num1 == null ? addr1 ?? throw new Exception() : new IRDouble(num1 ?? throw new Exception())
            , num2 == null ? addr2 ?? throw new Exception() : new IRDouble(num2 ?? throw new Exception())));
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
                for (int a = 0; a < fCall.Args.Count; a++)
                {
                    if (fCall.Args[a] is StringExpr strExpr)
                    {
                        AddStmt(IROPCode.LOADARG, new IRString(strExpr.Value), new IRInt(a + 1));
                    }
                    else if (fCall.Args[a] is NumExpr numExpr)
                    {
                        if (numExpr.Value.Contains('.')) AddStmt(IROPCode.LOADARG, new IRDouble(numExpr.NumValue ?? throw new Exception()), new IRInt(a + 1));
                        else AddStmt(IROPCode.LOADARG, new IRInt((int?)numExpr.NumValue ?? throw new Exception()), new IRInt(a + 1));
                    }
                    fCall.Args[a].Accept(this);
                    AddStmt(IROPCode.LOADARG, GetLastNumber(), new IRInt(a + 1));
                }
                return AddReference(AddStmt(IROPCode.CALL, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception())));
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
            if (expr.Value.Contains('.')) AddStmt(IROPCode.LOAD, new IRDouble(expr.NumValue ?? throw new Exception()));
            else AddStmt(IROPCode.LOAD, new IRInt((int?)expr.NumValue ?? throw new Exception()));
            return GetLastNumber();
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
    }
}
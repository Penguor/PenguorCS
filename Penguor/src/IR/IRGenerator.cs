using System;
using System.Collections.Generic;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IExceptionHandler, IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<Reference>
    {
        private readonly Builder builder;
        private readonly ProgramDecl program;

        private readonly List<State> scopes;
        private uint _instructionNumber;
        private uint InstructionNumber { get => _instructionNumber++; }

        private readonly List<IRStatement> statements = new();

        //ssa related stuff

        private readonly Dictionary<State, Dictionary<int, Reference>> currentDefinition = new();
        private int currentBlock;

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
            scopes.Add(new State());
        }

        private void WriteVariable(State variable, int block, Reference value)
        {
            currentDefinition.TryAdd(variable, new Dictionary<int, Reference>());
            currentDefinition[variable][block] = value;
        }

        private Reference ReadVariable(State variable, int block)
        {
            if (currentDefinition[variable].ContainsKey(block))
                return currentDefinition[variable][block];
            else
                return ReadVariableRecursive();
        }

        private Reference ReadVariableRecursive() => new Reference(1);

        private void CloseBlock() => currentBlock++;


        private uint AddStmt(IROPCode code, params IRArgument[] operands)
        {
            var num = InstructionNumber;
            statements.Add(new IRStatement(num, code, operands));
            return num;
        }

        private uint AddLabel()
        {
            builder.TableManager.AddSymbol(scopes[0]);
            return AddStmt(IROPCode.LABEL, new IRState(scopes[0]));
        }

        private Reference GetLastNumber() => new Reference(statements[^1].Number);

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
                if (statements.Count != 0) Logger.Log(new IRProgram(statements).ToString(), LogLevel.Debug);
            }
            return new IRProgram(statements);
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
            AddStmt(IROPCode.FUNC, new IRState(scopes[0]));
            // for (int i = 0; i < decl.Parameters.Count; i++)
            //     AddStmt(IROPCode.LOADPARAM, new IRState(scopes[0] + decl.Parameters[i].Name), new Int(i + 1));
            var length = statements.Count;
            decl.Content.Accept(this);
            if (statements[^1].Code != IROPCode.RET) AddStmt(IROPCode.RETN);
            scopes[0].Pop();
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
            foreach (var i in decl.Declarations)
                i.Accept(this);
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
                    NumExpr e => new Double(e.NumValue ?? throw new Exception()),
                    StringExpr s => new String(s.Value),
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
            scopes[0].Push(new AddressFrame(".block", AddressType.BlockStmt));
            foreach (var i in stmt.Content)
                i.Accept(this);
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
            stmt.Condition.Accept(this);
            uint num = AddStmt(IROPCode.JFL, GetLastNumber());
            scopes[0].Push(new AddressFrame($".elif{stmt.Id}", AddressType.Control));
            stmt.Content.Accept(this);
            scopes[0].Pop();
            statements[(int)num] = statements[(int)num] with
            {
                Operands = new IRArgument[] { statements[(int)num].Operands[0], GetLastNumber() }
            };
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
            stmt.Condition.Accept(this);
            uint num = AddStmt(IROPCode.JFL, GetLastNumber());
            scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
            stmt.IfC.Accept(this);
            scopes[0].Pop();
            statements[(int)num] = statements[(int)num] with
            {
                Operands = new IRArgument[] { statements[(int)num].Operands[0], GetLastNumber() }
            };
            foreach (var i in stmt.Elif)
                i.Accept(this);
            stmt.ElseC?.Accept(this);

            CloseBlock();
            return 0;
        }

        public int Visit(AsmStmt stmt)
        {
            foreach (var i in stmt.Contents)
                AddStmt(IROPCode.ASM, new String(i));
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
            var jmpNum = AddStmt(IROPCode.ERR);
            scopes[0].Push(new AddressFrame($".while{stmt.Id}", AddressType.Control));
            AddLabel();
            stmt.Content.Accept(this);
            scopes[0].Push(new AddressFrame(".c", AddressType.Control));
            AddLabel();
            stmt.Condition.Accept(this);
            statements[(int)jmpNum] = new IRStatement(jmpNum, IROPCode.JMP, new IRArgument[] { new IRState(scopes[0]) });
            scopes[0].Pop();
            AddStmt(IROPCode.JTR, GetLastNumber(), new IRState(scopes[0]));
            scopes[0].Pop();

            CloseBlock();
            return 0;
        }

        public Reference Visit(AssignExpr expr)
        {
            if (expr.Op != TokenType.ASSIGN) throw new Exception();

            State sym = builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception();

            expr.Value.Accept(this);
            WriteVariable(sym, currentBlock, GetLastNumber());

            return GetLastNumber();
        }

        public Reference Visit(BinaryExpr expr)
        {
            Reference? addr1 = null, addr2 = null;
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

            return new Reference(AddStmt(expr.Op switch
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
            , num1 == null ? addr1 ?? throw new Exception() : new Double(num1 ?? throw new Exception())
            , num2 == null ? addr2 ?? throw new Exception() : new Double(num2 ?? throw new Exception())));
        }

        public Reference Visit(BooleanExpr expr) => new(AddStmt(IROPCode.LOAD, new Bool(expr.Value)));

        public Reference Visit(CallExpr expr)
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
                        AddStmt(IROPCode.LOADARG, new String(strExpr.Value), new Int(a + 1));
                    }
                    else if (fCall.Args[a] is NumExpr numExpr)
                    {
                        if (numExpr.Value.Contains('.')) AddStmt(IROPCode.LOADARG, new Double(numExpr.NumValue ?? throw new Exception()), new Int(a + 1));
                        else AddStmt(IROPCode.LOADARG, new Int((int?)numExpr.NumValue ?? throw new Exception()), new Int(a + 1));
                    }
                    fCall.Args[a].Accept(this);
                    AddStmt(IROPCode.LOADARG, GetLastNumber(), new Int(a + 1));
                }
                return new Reference(AddStmt(IROPCode.CALL, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception())));
            }
            else
            {
                throw new Exception();
            }
        }

        public Reference Visit(GroupingExpr expr) => expr.Content.Accept(this);
        public Reference Visit(NullExpr expr) => new(AddStmt(IROPCode.LOAD, new Null()));

        public Reference Visit(NumExpr expr)
        {
            if (expr.Value.Contains('.')) AddStmt(IROPCode.LOAD, new Double(expr.NumValue ?? throw new Exception()));
            else AddStmt(IROPCode.LOAD, new Int((int?)expr.NumValue ?? throw new Exception()));
            return GetLastNumber();
        }

        public Reference Visit(StringExpr expr) => new(AddStmt(IROPCode.LOAD, new String(expr.Value)));

        public Reference Visit(UnaryExpr expr)
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

        public Reference Visit(VarExpr expr) => new(AddStmt(
            IROPCode.DFE,
            new IRState(builder.TableManager.GetStateBySymbol(expr.Name, scopes) ?? throw new ArgumentNullException(nameof(expr)))));
    }
}
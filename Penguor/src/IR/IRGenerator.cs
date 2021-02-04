using System;
using System.Collections.Generic;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IExceptionHandler, IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<int>
    {
        private readonly Builder builder;
        private readonly ProgramDecl program;

        private readonly List<State> scopes;
        private uint _instructionNumber;
        private uint InstructionNumber { get => _instructionNumber++; }

        private readonly List<IRStatement> statements = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
            scopes.Add(new State());
        }

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

        private uint GetLastNumber() => statements[^1].Number;

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
            for (int i = 0; i < decl.Parameters.Count; i++)
                AddStmt(IROPCode.LOADPARAM, new IRState(scopes[0] + decl.Parameters[i].Name), new Int(i + 1));
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
                return new Reference(GetLastNumber());
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
            AddStmt(IROPCode.JTR, new Reference(GetLastNumber()), new IRState(new State(label)));
            return 0;
        }

        public int Visit(ElifStmt stmt)
        {
            stmt.Condition.Accept(this);
            uint num = AddStmt(IROPCode.JFL, new Reference(GetLastNumber()));
            scopes[0].Push(new AddressFrame($".elif{stmt.Id}", AddressType.Control));
            stmt.Content.Accept(this);
            scopes[0].Pop();
            statements[(int)num] = statements[(int)num] with
            {
                Operands = new IRArgument[] { statements[(int)num].Operands[0], new Reference(GetLastNumber()) }
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
            uint num = AddStmt(IROPCode.JFL, new Reference(GetLastNumber()));
            scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
            stmt.IfC.Accept(this);
            scopes[0].Pop();
            statements[(int)num] = statements[(int)num] with
            {
                Operands = new IRArgument[] { statements[(int)num].Operands[0], new Reference(GetLastNumber()) }
            };
            foreach (var i in stmt.Elif)
                i.Accept(this);
            stmt.ElseC?.Accept(this);
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
            if (stmt.Value != null) AddStmt(IROPCode.RET, new Reference(GetLastNumber()));
            else AddStmt(IROPCode.RETN);
            return 0;
        }

        public int Visit(SwitchStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(VarStmt stmt)
        {
            if (stmt.Init != null)
            {
                AddStmt(IROPCode.DFL, new IRState(builder.TableManager.GetStateBySymbol(stmt.Name, scopes) ?? throw new Exception()), stmt.Init switch
                {
                    //todo: verify that this code works
                    NumExpr e => new Double(e.NumValue ?? double.Parse(e.Value)),
                    StringExpr s => new String(s.Value),
                    _ => AcceptInit()
                });
            }
            else
            {
                AddStmt(IROPCode.DFLE, new IRState(builder.TableManager.GetStateBySymbol(stmt.Name, scopes) ?? throw new Exception()));
            }

            return 0;

            IRArgument AcceptInit()
            {
                stmt.Init!.Accept(this);
                return new Reference(GetLastNumber());
            }
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
            AddStmt(IROPCode.JTR, new Reference(GetLastNumber()), new IRState(scopes[0]));
            scopes[0].Pop();
            return 0;
        }

        public int Visit(AssignExpr expr)
        {
            if (expr.Op != TokenType.ASSIGN)
            {
                AddStmt(
                expr.Op switch
                {
                    TokenType.ADD_ASSIGN => IROPCode.ADD,
                    TokenType.SUB_ASSIGN => IROPCode.SUB,
                    TokenType.MUL_ASSIGN => IROPCode.MUL,
                    TokenType.DIV_ASSIGN => IROPCode.DIV,
                    _ => throw new Exception(),
                },
                new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception()),
                expr.Value switch
                {
                    NumExpr e => new Double(e.NumValue ?? throw new Exception()),
                    StringExpr e => new String(e.Value),
                    _ => AcceptInit(),
                });
            }

            AddStmt(
                IROPCode.ASSIGN,
                new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception()),
                new Reference(GetLastNumber()));
            return 0;

            IRArgument AcceptInit()
            {
                expr.Value.Accept(this);
                return new Reference(GetLastNumber());
            }
        }

        public int Visit(BinaryExpr expr)
        {
            uint addr1 = 0, addr2 = 0;
            double? num1 = expr.Lhs is NumExpr e ? e.NumValue ?? throw new Exception() : null;
            if (num1 == null)
            {
                expr.Lhs.Accept(this);
                addr1 = statements[^1].Number;
            }
            double? num2 = expr.Lhs is NumExpr e1 ? e1.NumValue ?? throw new Exception() : null;
            if (num2 == null)
            {
                expr.Rhs.Accept(this);
                addr2 = statements[^1].Number;
            }

            AddStmt(expr.Op switch
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
            , num1 == null ? new Reference(addr1) : new Double(num1 ?? throw new Exception())
            , num2 == null ? new Reference(addr2) : new Double(num2 ?? throw new Exception()));
            return 0;
        }

        public int Visit(BooleanExpr expr)
        {
            AddStmt(IROPCode.LOAD, new Bool(expr.Value));
            return 0;
        }

        public int Visit(CallExpr expr)
        {
            // AddStmt(OPCode.BCALL);
            for (int i = 0; i < expr.Callee.Count; i++)
            {
                switch (expr.Callee[i])
                {
                    case FunctionCall call:
                        for (int a = 0; a < call.Args.Count; a++)
                        {
                            if (call.Args[a] is StringExpr strExpr)
                            {
                                AddStmt(IROPCode.LOADARG, new String(strExpr.Value), new Int(a + 1));
                            }
                            else if (call.Args[a] is NumExpr numExpr)
                            {
                                if (numExpr.Value.Contains('.')) AddStmt(IROPCode.LOADARG, new Double(numExpr.NumValue ?? throw new Exception()), new Int(a + 1));
                                else AddStmt(IROPCode.LOADARG, new Int((int?)numExpr.NumValue ?? throw new Exception()), new Int(a + 1));
                            }
                            call.Args[a].Accept(this);
                            AddStmt(IROPCode.LOADARG, new Reference(GetLastNumber()), new Int(a + 1));
                        }
                        AddStmt(IROPCode.CALL, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception()));
                        break;
                    case IdfCall:
                        //todo: not all idfcalls need to be loaded
                        AddStmt(IROPCode.LOAD, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception()));
                        break;
                    default: throw new Exception();
                }
            }

            if (expr.Postfix == TokenType.DPLUS)
                AddStmt(IROPCode.INCR, new Reference(GetLastNumber()));
            else if (expr.Postfix == TokenType.DMINUS)
                AddStmt(IROPCode.DECR, new Reference(GetLastNumber()));

            // AddStmt(OPCode.ECALL);
            return 0;
        }

        public int Visit(GroupingExpr expr)
        {
            expr.Content.Accept(this);
            return 0;
        }

        public int Visit(NullExpr expr)
        {
            AddStmt(IROPCode.LOAD, new Null());
            return 0;
        }

        public int Visit(NumExpr expr)
        {
            if (expr.Value.Contains('.')) AddStmt(IROPCode.LOAD, new Double(expr.NumValue ?? throw new Exception()));
            else AddStmt(IROPCode.LOAD, new Int((int?)expr.NumValue ?? throw new Exception()));
            return 0;
        }

        public int Visit(StringExpr expr)
        {
            AddStmt(IROPCode.LOAD, new String(expr.Value));
            return 0;
        }

        public int Visit(UnaryExpr expr)
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
                }, new Reference(GetLastNumber()));
            }

            return 0;
        }

        public int Visit(VarExpr expr)
        {
            AddStmt(IROPCode.DFE, new IRState(builder.TableManager.GetStateBySymbol(expr.Name, scopes) ?? throw new ArgumentNullException(nameof(expr))));
            return 0;
        }
    }
}
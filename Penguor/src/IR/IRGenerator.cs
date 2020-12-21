/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System.Collections.Generic;

using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using System;

#pragma warning disable 1591

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IExceptionHandler, IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<int>
    {

        private readonly Builder builder;

        private readonly ProgramDecl program;

        private readonly List<State> scopes;
        private uint _instrNum;
        private uint InstNum { get => _instrNum++; }

        private readonly List<IRStatement> statements = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
            scopes.Add(new State());
        }

        private uint AddStmt(OPCode code, params IRArgument[] operands)
        {
            var num = InstNum;
            statements.Add(new IRStatement(num, code, operands));
            return num;
        }

        private uint AddLabel() => AddStmt(OPCode.LABEL, new IRState(scopes[0]));

        private uint GetLastNumber() => statements[^1].Number;

        void Except(uint msg, int offset, params string[] args) => Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));

        T Except<T>(T recover, uint msg, int offset, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));
            return recover;
        }
        T Except<T>(T recover, Notification notification)
        {
            Logger.Log(notification);
            return recover;
        }
        T Except<T>(Func<T> recover, uint msg, int offset, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));
            return recover();
        }

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
            AddStmt(OPCode.FUNC, new IRState(scopes[0]));
            foreach (var i in decl.Parameters)
                AddStmt(OPCode.LOADPARAM, new IRState(scopes[0] + i.Name));
            var length = statements.Count;
            decl.Content.Accept(this);
            if (statements[^1].Code != OPCode.RET) AddStmt(OPCode.RET);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(LibraryDecl decl)
        {
            scopes[0].Append(decl.Name);
            AddStmt(OPCode.LIB, new IRState(scopes[0]));
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
            AddStmt(OPCode.USE, new IRState(State.FromCall(decl.Lib)));
            return 0;
        }


        public int Visit(VarDecl decl)
        {
            scopes[0].Push(decl.Name);

            if (decl.Init != null)
            {
                IRArgument arg = decl.Init switch
                {
                    NumExpr e => new Double(e.Value),
                    StringExpr s => new String(s.Value),
                    _ => AcceptInit()
                };
                AddStmt(OPCode.DEF, new IRState(scopes[0]), arg);
            }
            else
            {
                AddStmt(OPCode.DFE, new IRState(scopes[0]), new IRState(builder.TableManager.GetSymbol(scopes[0], scopes.ToArray())?.DataType ?? throw new Exception()));
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
            AddStmt(OPCode.JTR, new Reference(GetLastNumber()), new IRState(new State(label)));
            return 0;
        }

        public int Visit(ElifStmt stmt)
        {
            stmt.Condition.Accept(this);
            uint num = AddStmt(OPCode.JFL, new Reference(GetLastNumber()));
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
            uint num = AddStmt(OPCode.JFL, new Reference(GetLastNumber()));
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

        public int Visit(ReturnStmt stmt)
        {
            stmt.Value?.Accept(this);
            if (stmt.Value != null) AddStmt(OPCode.RET, new Reference(GetLastNumber()));
            else AddStmt(OPCode.RETN);
            return 0;
        }

        public int Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(VarStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(WhileStmt stmt)
        {
            stmt.Condition.Accept(this);
            scopes[0].Push(new AddressFrame($".while{stmt.Id}", AddressType.Control));
            AddLabel();
            stmt.Content.Accept(this);
            AddStmt(OPCode.JTR, new Reference(GetLastNumber()), new IRState(scopes[0]));
            scopes[0].Pop();
            return 0;
        }

        public int Visit(AssignExpr expr)
        {
            expr.Value.Accept(this);
            if (expr.Op != TokenType.ASSIGN)
            {
                AddStmt(
                expr.Op switch
                {
                    TokenType.ADD_ASSIGN => OPCode.ADD,
                    TokenType.SUB_ASSIGN => OPCode.SUB,
                    TokenType.MUL_ASSIGN => OPCode.MUL,
                    TokenType.DIV_ASSIGN => OPCode.DIV,
                    _ => throw new Exception(),
                },
                new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception()),
                new Reference(GetLastNumber()));
            }

            AddStmt(
                OPCode.ASSIGN,
                new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray()) ?? throw new Exception()),
                new Reference(GetLastNumber()));
            return 0;
        }

        public int Visit(BinaryExpr expr)
        {
            uint addr1 = 0, addr2 = 0;
            double? num1 = expr.Lhs is NumExpr e ? e.Value : null;
            if (num1 == null)
            {
                expr.Lhs.Accept(this);
                addr1 = statements[^1].Number;
            }
            double? num2 = expr.Lhs is NumExpr e1 ? e1.Value : null;
            if (num2 == null)
            {
                expr.Rhs.Accept(this);
                addr2 = statements[^1].Number;
            }

            AddStmt(expr.Op switch
            {
                TokenType.PLUS => OPCode.ADD,
                TokenType.MINUS => OPCode.SUB,
                TokenType.MUL => OPCode.MUL,
                TokenType.DIV => OPCode.DIV,
                TokenType.GREATER => OPCode.GREATER,
                TokenType.LESS => OPCode.LESS,
                TokenType.GREATER_EQUALS => OPCode.GREATER_EQUALS,
                TokenType.LESS_EQUALS => OPCode.LESS_EQUALS,
                TokenType.EQUALS => OPCode.EQUALS,
                TokenType a => Except(OPCode.ERR, new Notification(builder.SourceFile, expr.Offset, 9, MsgType.PGRCS, Token.ToString(a))),
            }, num1 == null ? new Reference(addr1) : new Double(num1 ?? throw new Exception())
            , num2 == null ? new Reference(addr2) : new Double(num2 ?? throw new Exception()));
            return 0;
        }

        public int Visit(BooleanExpr expr)
        {
            AddStmt(OPCode.LOAD, new Bool(expr.Value));
            return 0;
        }

        public int Visit(CallExpr expr)
        {
            for (int i = 0; i < expr.Callee.Count; i++)
            {
                switch (expr.Callee[i])
                {
                    case FunctionCall call:
                        foreach (var arg in call.Args)
                        {
                            arg.Accept(this);
                            AddStmt(OPCode.LOADARG, new Reference(GetLastNumber()));
                        }
                        AddStmt(OPCode.CALL, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception()));
                        break;
                    case IdfCall call:
                        AddStmt(OPCode.LOAD, new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception()));
                        break;
                    default: throw new Exception();
                }
            }

            if (expr.Postfix == TokenType.DPLUS)
                AddStmt(OPCode.INCR, new Reference(GetLastNumber()));
            else if (expr.Postfix == TokenType.DMINUS)
                AddStmt(OPCode.DECR, new Reference(GetLastNumber()));
            return 0;
        }

        public int Visit(GroupingExpr expr)
        {
            expr.Content.Accept(this);
            return 0;
        }

        public int Visit(NullExpr expr)
        {
            AddStmt(OPCode.LOAD, new Null());
            return 0;
        }

        public int Visit(NumExpr expr)
        {
            AddStmt(OPCode.LOAD, new Double(expr.Value));
            return 0;
        }

        public int Visit(StringExpr expr)
        {
            AddStmt(OPCode.LOAD, new String(expr.Value));
            return 0;
        }

        public int Visit(UnaryExpr expr)
        {
            expr.Rhs.Accept(this);
            if (expr.Op != null)
            {
                return 0;
                //TODO: add unary expr code
                AddStmt(expr.Op switch
                {
                    TokenType.EXCL_MARK => OPCode.INVERT,
                    TokenType.PLUS => throw new Exception(),
                    TokenType.MINUS => OPCode.CHS,
                    _ => throw new Exception(),
                });
            }

            return 0;
        }

        public int Visit(VarExpr expr)
        {
            throw new System.NotImplementedException();
        }
    }
}
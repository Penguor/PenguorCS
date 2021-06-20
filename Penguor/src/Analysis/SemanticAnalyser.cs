using System;
using System.Collections.Generic;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class SemanticAnalyser : IDeclVisitor<Decl>, IStmtVisitor<Stmt>, IExprVisitor<Expr>, ICallVisitor<Call>
    {
        private readonly Builder builder;

        private readonly ProgramDecl program;

        private List<State> scopes;

        private byte pass;

        public SemanticAnalyser(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;

            scopes = new();
        }

        public Decl Analyse(byte pass)
        {
            this.pass = pass;
            Logger.Blocked = pass == 1;
            scopes = new();
            scopes.Add(new State());
            var newProgram = program.Accept(this);
            Logger.Blocked = false;
            return newProgram;
        }

        private void AddVarExpr(VarExpr expr)
        {
            builder.TableManager.AddSymbol(scopes[0], expr.Name);
        }

        private void AddVarExpr(VarExpr[] expr)
        {
            foreach (var e in expr)
                builder.TableManager.AddSymbol(scopes[0], e.Name);
        }

        private void IsAccessible(State callee, State called, int offset)
        {
            bool accessable = false;
            if (pass <= 1) return;

            Symbol? calleeSymbol;
            if (callee.Count != 0)
                calleeSymbol = builder.TableManager.GetSymbol(callee, scopes.ToArray());
            else
                calleeSymbol = null;

            Symbol? calledSymbol = builder.TableManager.GetSymbol(called, scopes.ToArray());

            if (calleeSymbol != null && calledSymbol != null)
            {
                switch (calledSymbol.AccessMod)
                {
                    case TokenType.PUBLIC:
                        accessable = true;
                        break;
                    case TokenType.PRIVATE:
                        var calleeState = builder.TableManager.GetStateBySymbol(callee, scopes.ToArray());
                        var calledState = builder.TableManager.GetStateBySymbol(called, scopes.ToArray());
                        if (calledState == null || calleeState == null)
                        {
                            accessable = false;
                            break;
                        }
                        calledState.Pop();
                        accessable = calleeState.IsChildOf(calledState);
                        break;
                    case TokenType.PROTECTED:
                        accessable = calleeSymbol.Parent == callee;
                        break;
                    case TokenType.RESTRICTED:
                        if (called.Count == 1) accessable = true;
                        else if (callee.Count > 1 && called.Count > 1) accessable = callee[0].Symbol == called[0].Symbol;
                        else accessable = false;
                        break;
                    case null:
                        accessable = true;
                        break;
                }
            }
            else if (calleeSymbol == null && calledSymbol != null)
            {
                switch (calledSymbol.AccessMod)
                {
                    case TokenType.PUBLIC:
                        accessable = true;
                        break;
                    case TokenType.PRIVATE:
                    case TokenType.PROTECTED:
                    case TokenType.RESTRICTED:
                        var tempCalled = (State)called.Clone();
                        accessable = called.Count > 0 && (builder.TableManager.GetStateBySymbol(tempCalled.Pop(), tempCalled) ?? throw new Exception())[0].Type != AddressType.LibraryDecl;
                        break;
                }
            }

            if (!accessable) builder.Except(19, offset, called.GetRaw().ToString(), callee.GetRaw().ToString());
        }

        private void SetDataType(AddressFrame name, TypeCallExpr type)
        {
            Symbol? e = builder.TableManager.GetSymbol(name, scopes);
            if (e != null) e.DataType = State.FromTypeCall((TypeCallExpr)type.Accept(this));
        }

        public Decl Visit(BlockDecl decl)
        {
            List<Decl> content = new(decl.Content.Count);
            foreach (var i in decl.Content) content.Add(i.Accept(this));
            return decl with { Content = content };
        }

        public Decl Visit(DataDecl decl)
        {
            var parent = decl.Parent?.Accept(this);
            if (parent != null)
            {
                var c = State.FromCall((CallExpr)parent);
                var s = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                s.Parent = c;
                IsAccessible(scopes[0] + new State(decl.Name), c, decl.Offset);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Parent = (CallExpr?)parent, Content = (BlockDecl)content };
        }

        public Decl Visit(TypeDecl decl)
        {
            var parent = decl.Parent?.Accept(this);
            if (parent != null)
            {
                var c = State.FromCall((CallExpr)parent);
                var parentSymbol = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                parentSymbol.Parent = c;
                IsAccessible(scopes[0] + new State(decl.Name), c, decl.Offset);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            builder.TableManager.GetSymbol(decl.Name, scopes.ToArray())!.DataType = builder.TableManager.GetStateBySymbol(decl.Name, scopes.ToArray());

            return decl with { Parent = (CallExpr?)parent, Content = (BlockDecl)content };
        }

        public Decl Visit(StmtDecl decl) => decl with { Stmt = decl.Stmt.Accept(this) };

        public Decl Visit(FunctionDecl decl)
        {
            SetDataType(decl.Name, decl.Returns);
            List<VarExpr> parameters = new(decl.Parameters.Count);
            foreach (var i in decl.Parameters) parameters.Add((VarExpr)i.Accept(this));
            scopes[0].Push(decl.Name);
            foreach (var i in parameters)
            {
                var symbol = builder.TableManager.GetSymbol(i.Name, scopes);
                if (symbol != null) symbol.DataType = builder.TableManager.GetStateBySymbol(State.FromTypeCall(i.Type), scopes.ToArray());
            }
            var content = decl.Content.Accept(this);

            scopes[0].Pop();

            //TODO: functions should always return something, void is also a data type in Penguor
            return decl with { Parameters = parameters, Content = content };
        }

        public Decl Visit(LibraryDecl decl)
        {
            scopes[0].AddRange(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Remove(decl.Name);
            return decl with { Content = (BlockDecl)content };
        }

        public Decl Visit(ProgramDecl decl)
        {
            List<Decl> decls = new();
            foreach (var i in decl.Declarations) decls.Add(i.Accept(this));
            return decl with { Declarations = decls };
        }

        public Decl Visit(SystemDecl decl)
        {
            var parent = decl.Parent?.Accept(this);
            if (parent != null)
            {
                var c = State.FromCall((CallExpr)parent);
                var s = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                s.Parent = c;
                IsAccessible(scopes[0] + new State(decl.Name), c, decl.Offset);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Parent = (CallExpr?)parent, Content = (BlockDecl)content };
        }

        public Decl Visit(UsingDecl decl)
        {
            var call = State.FromTypeCall(decl.Lib);
            if (builder.TableManager.FindTable(call))
                scopes.Add(call);
            else builder.Except(21, decl.Offset, call.ToString());
            return decl;
        }

        public Decl Visit(VarDecl decl)
        {
            SetDataType(decl.Name, decl.Type);
            if (decl.Init is not null)
                return decl with { Init = decl.Init.Accept(this) };
            else return decl;
        }

        public Stmt Visit(BlockStmt stmt)
        {
            scopes[0].Push(new AddressFrame(".block", AddressType.BlockStmt));
            builder.TableManager.AddTable(scopes[0]);
            List<Stmt> content = new List<Stmt>();
            foreach (var i in stmt.Content)
                content.Add(i.Accept(this));
            scopes[0].Pop();
            return stmt with { Content = content };
        }

        public Stmt Visit(CaseStmt stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt Visit(DoStmt stmt)
        {
            scopes[0].Push(new AddressFrame($".do{stmt.Id}", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);
            var content = stmt.Content.Accept(this);
            scopes[0].Pop();
            return stmt with { Content = content, Condition = stmt.Condition.Accept(this) };
        }

        public Stmt Visit(ElifStmt stmt)
        {
            Expr condition = stmt.Condition.Accept(this);
            if (((ExprAttribute?)condition.Attribute)!.Type.Equals(new State("bool")))
            {
                scopes[0].Push(new AddressFrame($".elif{stmt.Id}", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                Stmt content = stmt.Content.Accept(this);
                scopes[0].Pop();
                return stmt with { Content = content, Condition = condition };
            }
            else
            {
                throw new Exception();
            }
        }

        public Stmt Visit(ExprStmt stmt)
        {
            if (stmt.Expr is AssignExpr or CallExpr)
                return stmt with { Expr = stmt.Expr.Accept(this) };
            else
                return builder.Except(stmt with { Expr = stmt.Expr.Accept(this) }, 15, stmt.Offset);
        }

        public Stmt Visit(ForStmt stmt)
        {
            scopes[0].Push(new AddressFrame($".for{stmt.Id}", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);

            var init = stmt.Init?.Accept(this);
            if (init is VarExpr varExpr)
                AddVarExpr(varExpr);

            var condition = stmt.Condition?.Accept(this);
            var change = stmt.Change?.Accept(this);

            var content = stmt.Content.Accept(this);
            scopes[0].Pop();
            if (pass == 1 || ((ExprAttribute?)condition?.Attribute)!.Type.Equals(new State("bool")))
            {
                return stmt with { Init = init, Condition = condition, Change = change, Content = content };
            }
            else
            {
                throw new Exception();
            }
        }

        public Stmt Visit(ForeachStmt stmt)
        {
            scopes[0].Push(new AddressFrame($".foreach{stmt.Id}", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);

            var currentVar = stmt.CurrentVar.Accept(this);
            var vars = stmt.Vars.Accept(this);
            AddVarExpr(stmt.CurrentVar);
            var content = stmt.Content.Accept(this);
            scopes[0].Pop();

            return stmt with { CurrentVar = (VarExpr)currentVar, Vars = (CallExpr)vars, Content = content };
        }

        public Stmt Visit(IfStmt stmt)
        {
            Expr condition = stmt.Condition.Accept(this);
            if (condition is BooleanExpr booleanExpr)
            {
                if (booleanExpr.Value)
                {
                    return stmt.IfC.Accept(this);
                }
                else if (stmt.Elif.Count > 0)
                {
                    for (int i = 0; i < stmt.Elif.Count; i++)
                    {
                        if (stmt.Elif[i].Condition.Accept(this) is BooleanExpr expr && expr.Value)
                        {
                            return stmt.Elif[i].Accept(this);
                        }
                    }
                }
                else if (stmt.ElseC != null)
                {
                    return stmt.ElseC.Accept(this);
                }
            }

            if (pass == 1 || ((ExprAttribute?)condition.Attribute)!.Type.Equals(new State("bool")))
            {
                scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                Stmt ifC = stmt.IfC.Accept(this);

                scopes[0].Push(new AddressFrame(".else", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                List<ElifStmt> elif = new(stmt.Elif.Count);
                foreach (var i in stmt.Elif)
                    elif.Add((ElifStmt)i.Accept(this));

                scopes[0].Push(new AddressFrame(".else", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                Stmt? elseC = stmt.ElseC?.Accept(this);
                scopes[0].Pop(3);
                return stmt with { Condition = condition, IfC = ifC, Elif = elif, ElseC = elseC };
            }
            else
            {
                throw new Exception();
            }
        }

        public Stmt Visit(CompilerStmt stmt) => stmt;

        public Stmt Visit(AsmStmt stmt) => stmt;

        public Stmt Visit(ReturnStmt stmt) => stmt with { Value = stmt.Value?.Accept(this) };

        public Stmt Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(VarStmt stmt)
        {
            SetDataType(stmt.Name, stmt.Type);
            var init = stmt.Init?.Accept(this);
            return stmt with { Init = init };
        }

        public Stmt Visit(WhileStmt stmt)
        {
            scopes[0].Push(new AddressFrame($".while{stmt.Id}", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);
            Expr condition = stmt.Condition.Accept(this);
            if (((ExprAttribute?)condition.Attribute)!.Type.Equals(new State(new AddressFrame("bool", AddressType.IdfCall))))
            {
                var content = stmt.Content.Accept(this);
                scopes[0].Pop();
                return stmt with { Condition = condition, Content = content };
            }
            else
            {
                scopes[0].Pop();
                return builder.Except(stmt, 1, stmt.Condition.Offset);
            }
        }

        public Expr Visit(AssignExpr expr)
        {
            //todo: verify operators
            var lhs = expr.Lhs.Accept(this);
            var rhs = expr.Value.Accept(this);

            ASTAttribute? attribute;
            if (lhs is CallExpr call)
            {
                attribute = call.Attribute;
            }
            else if (lhs is VarExpr varExpr)
            {
                attribute = new ExprAttribute(State.FromTypeCall(varExpr.Type));
            }
            else
            {
                throw new Exception();
            }

            return expr with { Lhs = lhs, Value = rhs, Attribute = attribute };
        }

        public Expr Visit(BinaryExpr expr)
        {
            var lhs = expr.Lhs.Accept(this);
            var rhs = expr.Rhs.Accept(this);

            State type;

            if (expr.Op == TokenType.LESS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.GREATER)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.LESS_EQUALS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.GREATER_EQUALS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.EQUALS)
            {
                type = new State("bool");
            }
            else if (expr.Op == TokenType.AND)
            {
                if (lhs is NumExpr && rhs is NumExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.OR)
            {
                if (lhs is NumExpr && rhs is NumExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State("bool");
            }
            else if (expr.Op == TokenType.PLUS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State(new AddressFrame("i32", AddressType.IdfCall));
            }
            else if (expr.Op == TokenType.MINUS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State(new AddressFrame("i32", AddressType.IdfCall));
            }
            else if (expr.Op == TokenType.MUL)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State(new AddressFrame("i32", AddressType.IdfCall));
            }
            else if (expr.Op == TokenType.DIV)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State(new AddressFrame("i32", AddressType.IdfCall));
            }
            else if (expr.Op == TokenType.PERCENT)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();

                type = new State(new AddressFrame("i32", AddressType.IdfCall));
            }
            else
            {
                Console.WriteLine(expr.Op);
                throw new Exception();
            }

            return expr with { Lhs = lhs, Rhs = rhs, Attribute = new ExprAttribute(type) };
        }

        public Expr Visit(BooleanExpr expr) => expr with { Attribute = new ExprAttribute(new State(new AddressFrame("bool", AddressType.IdfCall))) };

        public Expr Visit(CallExpr expr)
        {
            var e = State.FromCall(expr);
            if (!builder.TableManager.FindSymbol(e, scopes.ToArray()) && pass > 1)
            {
                builder.Except(20, expr.Offset, e.ToString());
                return expr;
            }

            IsAccessible(scopes[0], e, expr.Offset);

            for (int i = 1; i <= e.Count; i++)
            {
                State tmp = new(e[0..i]);

                if (builder.TableManager.FindSymbol(tmp, scopes.ToArray()) && pass > 1)
                {
                    builder.TableManager.GetSymbol(tmp, scopes.ToArray());
                }
            }

            var callee = new List<Call>(expr.Callee.Count);
            foreach (var i in expr.Callee)
                callee.Add(i.Accept(this));

            if (pass == 1) return expr;
            return expr with { Callee = callee, Attribute = new ExprAttribute(builder.TableManager.GetSymbol(e, scopes.ToArray())?.DataType ?? throw new NullReferenceException()) };
        }

        public Expr Visit(GroupingExpr expr)
        {
            var content = expr.Content.Accept(this);
            if (content is UnaryExpr or CallExpr) return content;
            return expr with { Content = content };
        }

        public Expr Visit(NullExpr expr) => expr;

        public Expr Visit(NumExpr expr)
        {
            double number = 0;
            int iteration = 0;
            int dotPosition = 0;
            if (expr.Value.Contains('.'))
            {
                dotPosition = expr.Value.Length - expr.Value.IndexOf('.') - 1;
            }

            for (int i = expr.Value.Length - 1; i >= 0; i--)
            {
                if (expr.Value.Contains('.') && i == expr.Value.IndexOf('.'))
                    continue;
                number += Math.Pow(expr.NumBase, iteration - dotPosition) * GetNumberFromChar(expr.Value[i]);
                iteration++;
            }
            //todo: assign correct type based on if it's a floating point number
            return expr with { NumBase = 10, NumValue = number, Attribute = new ExprAttribute(new State("i32")) };

            // return the numeric (base-10) representation of the digit
            double GetNumberFromChar(char chr)
            {
                int value;
                if (chr is >= '0' and <= '9')
                    value = chr - '0';
                else if (chr is >= 'A' and <= 'Z')
                    value = chr - 'A' + 10;
                else
                    throw new Exception();

                if (value > (expr.NumBase - 1))
                    throw new Exception();
                else return value;
            }
        }

        public Expr Visit(StringExpr expr) => expr with { Attribute = new ExprAttribute(new State(new AddressFrame("str", AddressType.IdfCall))) };

        public Expr Visit(CharExpr expr) => expr with { Attribute = new ExprAttribute(new State(new AddressFrame("char", AddressType.IdfCall))) };

        public Expr Visit(TypeCallExpr expr)
        {
            var e = State.FromTypeCall(expr);
            if (!builder.TableManager.FindSymbol(e, scopes.ToArray()) && pass > 1)
            {
                builder.Except(20, expr.Offset, e.ToString());
                return expr;
            }

            IsAccessible(scopes[0], e, expr.Offset);

            if (pass == 1) return expr;
            return expr with { Attribute = new ExprAttribute(builder.TableManager.GetSymbol(e, scopes.ToArray())?.DataType ?? throw new NullReferenceException()) };
        }

        public Expr Visit(UnaryExpr expr)
        {
            Expr e = expr.Rhs.Accept(this);
            if (pass == 1)
            {
                return expr with { Rhs = e };
            }
            if (expr.Op == null) return e;
            else if (((ExprAttribute?)e.Attribute)!.Type.ToString() is "i8" or "i16" or "i32" or "i64" or "u8" or "u16" or "u32" or "u64" or "str" or "f32" or "f64" && expr.Op is TokenType.MINUS or TokenType.PLUS or TokenType.BW_NOT or TokenType.DPLUS or TokenType.DMINUS) return expr with { Rhs = e, Attribute = new ExprAttribute(((ExprAttribute?)e.Attribute)!.Type) };
            else if (((ExprAttribute?)e.Attribute)!.Type.ToString() is "bool" && expr.Op is TokenType.EXCL_MARK) return expr with { Rhs = e, Attribute = new ExprAttribute(new State("bool")) };
            else return builder.Except(expr, new Notification(builder.SourceFile, expr.Offset, 1, MsgType.PGRCS));
        }

        public Expr Visit(VarExpr expr)
        {
            var type = (TypeCallExpr)expr.Type.Accept(this);
            SetDataType(expr.Name, type);

            return expr with
            {
                Type = type,
                Attribute = pass == 1 ? null : new ExprAttribute(builder.TableManager.GetStateBySymbol(State.FromTypeCall(type), scopes.ToArray()) ?? throw new NullReferenceException())
            };
        }

        public Call Visit(FunctionCall call)
        {
            var args = new List<Expr>(call.Args.Count);
            foreach (var i in call.Args)
                args.Add(i.Accept(this));
            return call with { Args = args };
        }

        public Call Visit(IdfCall call) => call;

        public Call Visit(ArrayCall call)
        {
            throw new NotImplementedException();
        }

        public Decl Visit(StmtBlockDecl decl)
        {
            throw new NotImplementedException();
        }

        public Decl Visit(ModifiedDecl decl)
        {
            return decl with { Declaration = decl.Declaration.Accept(this) };
        }

        public Expr Visit(IncrementExpr expr)
        {
            return expr with { Child = (CallExpr)expr.Child.Accept(this) };
        }
    }
}
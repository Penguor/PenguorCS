using System.Collections.Generic;

using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using System;

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class SemanticAnalyser : IDeclVisitor<Decl>, IStmtVisitor<Stmt>, IExprVisitor<Expr>
    {
        private readonly Builder builder;

        private readonly ProgramDecl program;

        private readonly List<State> scopes;

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
            scopes.Add(new State());
            return program.Accept(this);
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

        private void IsAccessable(State callee, State called)
        {
            bool accessable = false;
            if (pass <= 1) accessable = true;
            Symbol? calleeSymbol = builder.TableManager.GetSymbol(callee, scopes.ToArray());
            Symbol? calledSymbol = builder.TableManager.GetSymbol(called, scopes.ToArray());

            if (calleeSymbol != null && calledSymbol != null)
            {
                switch (calledSymbol.AccessMod)
                {
                    case TokenType.PUBLIC:
                        accessable = true;
                        break;
                    case TokenType.PRIVATE:
                        accessable = callee.IsChildOf(called);
                        break;
                    case TokenType.PROTECTED:
                        accessable = calleeSymbol.Parent == callee;
                        break;
                    case TokenType.RESTRICTED:
                        if (callee.Count == 0 && called.Count != 0) accessable = false;
                        else if (callee.Count == 0 && called.Count == 0) accessable = false;
                        else if (callee.Count != 0 && called.Count == 0) accessable = false;
                        else accessable = callee[0].Symbol == called[0].Symbol;
                        break;
                }
            }
            if (!accessable) throw new Exception();
        }

        private void SetDataType(AddressFrame name, CallExpr type)
        {
            Symbol? e = builder.TableManager.GetSymbol(name, scopes);
            if (e != null) e.DataType = State.FromCall((CallExpr)type.Accept(this));
        }

        private void Except(uint msg, int offset, params string[] args) => Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));

        private T Except<T>(T recover, uint msg, int offset, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));
            return recover;
        }

        private T Except<T>(Func<T> recover, uint msg, int offset, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, offset, msg, MsgType.PGR, args));
            return recover();
        }

        public Decl Visit(BlockDecl decl)
        {
            List<Decl> content = new(decl.Content.Count);
            foreach (var i in decl.Content) content.Add(i.Accept(this));
            return decl with { Content = content };
        }

        public Decl Visit(DataDecl decl)
        {
            if (decl.Parent != null)
            {
                var c = State.FromCall((CallExpr)decl.Parent.Accept(this));
                var s = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                s.Parent = c;
                IsAccessable(scopes[0] + new State(decl.Name), c);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Content = (BlockDecl)content };
        }

        public Decl Visit(TypeDecl decl)
        {
            if (decl.Parent != null)
            {
                var c = State.FromCall((CallExpr)decl.Parent.Accept(this));
                var s = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                s.Parent = c;
                IsAccessable(scopes[0] + new State(decl.Name), c);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Content = (BlockDecl)content };
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
                if (symbol != null) symbol.DataType = builder.TableManager.GetStateBySymbol(State.FromCall(i.Type), scopes.ToArray());
            }
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Parameters = parameters, Content = content };
        }

        public Decl Visit(LibraryDecl decl)
        {
            scopes[0].Append(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Remove(decl.Name);
            return decl with { Content = (BlockDecl)content };
        }

        public Decl Visit(ProgramDecl decl)
        {
            List<Decl> decls = new List<Decl>();
            foreach (var i in decl.Declarations) decls.Add(i.Accept(this));
            return decl with { Declarations = decls };
        }

        public Decl Visit(SystemDecl decl)
        {
            if (decl.Parent != null)
            {
                var c = State.FromCall((CallExpr)decl.Parent.Accept(this));
                var s = builder.TableManager.GetSymbol(c, scopes.ToArray())!;
                s.Parent = c;
                IsAccessable(scopes[0] + new State(decl.Name), c);
            }

            scopes[0].Push(decl.Name);
            var content = decl.Content.Accept(this);
            scopes[0].Pop();

            return decl with { Content = (BlockDecl)content };
        }

        public Decl Visit(UsingDecl decl)
        {
            var call = State.FromCall(decl.Lib);
            if (builder.TableManager.FindTable(call))
                scopes.Add(call);
            else throw new Exception();
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
            throw new System.NotImplementedException();
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
            if (condition is BooleanExpr
                || (condition is CallExpr cExpr
                    && builder.TableManager.GetSymbol(State.FromCall(cExpr), scopes.ToArray())?.DataType?.ToString() == "bool")
                || (condition is BinaryExpr bExpr && (bExpr.Op is TokenType.LESS
                                                           or TokenType.GREATER
                                                           or TokenType.LESS_EQUALS
                                                           or TokenType.GREATER_EQUALS
                                                           or TokenType.EQUALS
                                                           or TokenType.NEQUALS
                                                           or TokenType.AND
                                                           or TokenType.OR)))
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
                return Except(stmt with { Expr = stmt.Expr.Accept(this) }, 15, stmt.Offset);
        }

        public Stmt Visit(ForStmt stmt)
        {
            var currentVar = stmt.CurrentVar.Accept(this);
            var vars = stmt.Vars.Accept(this);
            scopes[0].Push(new AddressFrame($".for{stmt.Id}", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);
            AddVarExpr(stmt.CurrentVar);
            var content = stmt.Content.Accept(this);
            scopes[0].Pop();

            return stmt with { CurrentVar = (VarExpr)currentVar, Vars = (CallExpr)vars, Content = content };
        }

        public Stmt Visit(IfStmt stmt)
        {
            Expr condition = stmt.Condition.Accept(this);
            if (condition is BooleanExpr
                || (condition is CallExpr cExpr
                    && builder.TableManager.GetSymbol(State.FromCall(cExpr), scopes.ToArray())?.DataType?.ToString() == "bool")
                || (condition is BinaryExpr bExpr && (bExpr.Op is TokenType.LESS
                                                           or TokenType.GREATER
                                                           or TokenType.LESS_EQUALS
                                                           or TokenType.GREATER_EQUALS
                                                           or TokenType.EQUALS
                                                           or TokenType.NEQUALS
                                                           or TokenType.AND
                                                           or TokenType.OR)))
            {
                scopes[0].Push(new AddressFrame($".if{stmt.Id}", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                Stmt ifC = stmt.IfC.Accept(this);
                scopes[0].Pop();
                List<Stmt> elif = new List<Stmt>(stmt.Elif.Count);
                foreach (var i in stmt.Elif) elif.Add(i.Accept(this));
                scopes[0].Push(new AddressFrame($".else{stmt.Id}", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                Stmt? elseC = stmt.ElseC?.Accept(this);
                scopes[0].Pop();
                return stmt with { Condition = condition, IfC = ifC, Elif = elif, ElseC = elseC };
            }
            else
            {
                throw new Exception();
            }
        }

        public Stmt Visit(CompilerStmt stmt) => stmt;

        public Stmt Visit(ReturnStmt stmt) => stmt with { Value = stmt.Value?.Accept(this) };

        public Stmt Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(VarStmt stmt)
        {
            SetDataType(stmt.Name, stmt.Type);
            builder.TableManager.AddSymbol(scopes[0], stmt.Name);
            stmt.Init?.Accept(this);
            return stmt;
        }

        public Stmt Visit(WhileStmt stmt)
        {
            Expr condition = stmt.Condition.Accept(this);
            if (condition is BooleanExpr
                || (condition is CallExpr cExpr
                    && builder.TableManager.GetSymbol(State.FromCall(cExpr), scopes.ToArray())?.DataType?.ToString() == "bool")
                || (condition is BinaryExpr bExpr && (bExpr.Op is TokenType.LESS
                                                           or TokenType.GREATER
                                                           or TokenType.LESS_EQUALS
                                                           or TokenType.GREATER_EQUALS
                                                           or TokenType.EQUALS
                                                           or TokenType.NEQUALS
                                                           or TokenType.AND
                                                           or TokenType.OR)))
            {
                scopes[0].Push(new AddressFrame($".while{stmt.Id}", AddressType.Control));
                builder.TableManager.AddTable(scopes[0]);
                var content = stmt.Content.Accept(this);
                scopes[0].Pop();
                return stmt with { Condition = condition, Content = content };
            }
            else
            {
                throw new Exception();
            }
        }

        public Expr Visit(AssignExpr expr)
        {
            //todo: verify operators
            expr.Lhs.Accept(this);
            expr.Value.Accept(this);
            return expr;
        }

        public Expr Visit(BinaryExpr expr)
        {
            var lhs = expr.Lhs.Accept(this);
            var rhs = expr.Rhs.Accept(this);

            if (expr.Op == TokenType.EQUALS)
            {
                { if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is StringExpr expr1 && rhs is StringExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is NullExpr expr1 && rhs is NullExpr) return new BooleanExpr(expr1.Id, expr.Offset, true); }
            }
            else if (expr.Op == TokenType.NEQUALS)
            {
                { if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is StringExpr expr1 && rhs is StringExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is NullExpr expr1 && rhs is NullExpr) return new BooleanExpr(expr1.Id, expr.Offset, false); }
            }
            else if (expr.Op == TokenType.LESS)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value < expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.GREATER)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value > expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.LESS_EQUALS)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value <= expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.GREATER_EQUALS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value >= expr2.Value);
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.AND)
            {
                if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value && expr2.Value);
                if (lhs is NumExpr && rhs is NumExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.OR)
            {
                if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr1.Id, expr.Offset, expr1.Value || expr2.Value);
                if (lhs is NumExpr && rhs is NumExpr) throw new Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.PLUS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                { if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new NumExpr(expr1.Id, expr1.Offset, expr1.Value + expr2.Value); }
                { if (lhs is StringExpr expr1 && rhs is StringExpr expr2) return new StringExpr(expr1.Id, expr1.Offset, expr1.Value + expr2.Value); }
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.MINUS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new NumExpr(expr1.Id, expr1.Offset, expr1.Value - expr2.Value);
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.MUL)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new NumExpr(expr1.Id, expr1.Offset, expr1.Value * expr2.Value);
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            else if (expr.Op == TokenType.DIV)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new Exception();
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new NumExpr(expr1.Id, expr1.Offset, expr1.Value / expr2.Value);
                if (lhs is StringExpr && rhs is StringExpr) throw new Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new Exception();
            }
            return expr;
        }

        public Expr Visit(BooleanExpr expr) => expr;

        public Expr Visit(CallExpr expr)
        {
            var e = State.FromCall(expr);
            if (!builder.TableManager.FindSymbol(e, scopes.ToArray()) && pass > 1) throw new Exception();
            else return expr;
        }

        public Expr Visit(GroupingExpr expr)
        {
            expr.Content.Accept(this);
            if (expr.Content is UnaryExpr or CallExpr) return expr.Content;
            return expr;
        }

        public Expr Visit(NullExpr expr) => expr;

        public Expr Visit(NumExpr expr) => expr;

        public Expr Visit(StringExpr expr) => expr;

        public Expr Visit(UnaryExpr expr)
        {
            Expr e = expr.Rhs.Accept(this);
            if (expr.Op == null) return expr;
            else if (e is NumExpr && expr.Op is TokenType.MINUS or TokenType.PLUS or TokenType.BW_NOT or TokenType.DPLUS or TokenType.DMINUS) return expr;
            else if (e is BooleanExpr booleanExpr && expr.Op is TokenType.EXCL_MARK) return booleanExpr with { Value = !booleanExpr.Value };
            else throw new Exception();
        }

        public Expr Visit(VarExpr expr)
        {
            expr.Type.Accept(this);
            return expr;
        }
    }
}
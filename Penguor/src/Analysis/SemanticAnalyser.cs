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

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class SemanticAnalyser : IDeclVisitor<Decl>, IStmtVisitor<Stmt>, IExprVisitor<Expr>, ICallVisitor<Call>
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
            if (!accessable) throw new System.Exception();
        }

        private void SetDataType(AddressFrame name, CallExpr type)
        {
            Symbol? e = builder.TableManager.GetSymbol(name, scopes);
            if (e != null) e.DataType = State.FromCall((CallExpr)type.Accept(this));
        }

        public Decl Visit(BlockDecl decl)
        {
            foreach (var i in decl.Content) i.Accept(this);
            return decl;
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
            decl.Content.Accept(this);
            scopes[0].Pop();

            return decl;
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
            decl.Content.Accept(this);
            scopes[0].Pop();

            return decl;
        }

        public Decl Visit(DeclStmt decl)
        {
            decl.Stmt.Accept(this);
            return decl;
        }

        public Decl Visit(FunctionDecl decl)
        {
            SetDataType(decl.Name, decl.Returns);
            foreach (var i in decl.Parameters) i.Accept(this);
            scopes[0].Push(decl.Name);
            AddVarExpr(decl.Parameters.ToArray());
            decl.Content.Accept(this);
            scopes[0].Pop();

            return decl;
        }

        public Decl Visit(LibraryDecl decl)
        {
            scopes[0].Append(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Remove(decl.Name);
            return decl;
        }

        public Decl Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations) i.Accept(this);
            return decl;
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
            decl.Content.Accept(this);
            scopes[0].Pop();

            return decl;
        }

        public Decl Visit(UsingDecl decl)
        {
            var call = State.FromCall(decl.Lib);
            if (builder.TableManager.FindTable(call))
                scopes.Add(call);
            else throw new System.Exception();
            return decl;
        }

        public Decl Visit(VarDecl decl)
        {
            SetDataType(decl.Name, decl.Type);
            if (decl.Init is not null)
            {
                decl.Init.Accept(this);
            }
            return decl;
        }

        public Stmt Visit(BlockStmt stmt)
        {
            scopes[0].Push(new AddressFrame(".block", AddressType.BlockStmt));
            builder.TableManager.AddTable(scopes[0]);
            foreach (var i in stmt.Content)
                i.Accept(this);
            scopes[0].Pop();
            return stmt;
        }

        public Stmt Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(DoStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(ElifStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(ExprStmt stmt)
        {
            if (stmt.Expr is AssignExpr || stmt.Expr is CallExpr)
            {
                stmt.Expr.Accept(this);
                return stmt;
            }
            else
            {
                throw new System.Exception();
            }
        }

        public Stmt Visit(ForStmt stmt)
        {
            stmt.CurrentVar.Accept(this);
            stmt.Vars.Accept(this);
            scopes[0].Push(new AddressFrame(".for", AddressType.Control));
            builder.TableManager.AddTable(scopes[0]);
            AddVarExpr(stmt.CurrentVar);
            stmt.Content.Accept(this);
            scopes[0].Pop();
            return stmt;
        }

        public Stmt Visit(IfStmt stmt)
        {
            stmt.Condition.Accept(this);
            if (stmt.Condition is BooleanExpr
                || (stmt.Condition is CallExpr cExpr
                    && builder.TableManager.GetSymbol(State.FromCall(cExpr), scopes.ToArray())?.DataType == new State(new AddressFrame[] { new AddressFrame("bool", AddressType.TypeDecl) }))
                || (stmt.Condition is BinaryExpr bExpr && (bExpr.Op == TokenType.LESS
                                                           || bExpr.Op == TokenType.GREATER
                                                           || bExpr.Op == TokenType.LESS_EQUALS
                                                           || bExpr.Op == TokenType.GREATER_EQUALS
                                                           || bExpr.Op == TokenType.EQUALS
                                                           || bExpr.Op == TokenType.NEQUALS
                                                           || bExpr.Op == TokenType.AND
                                                           || bExpr.Op == TokenType.OR)))
            {
                stmt.IfC.Accept(this);
                foreach (var i in stmt.Elif) i.Accept(this);
                stmt.ElseC?.Accept(this);
                return stmt;
            }
            else
            {
                throw new System.Exception();
            }
        }

        public Stmt Visit(CompilerStmt stmt) => stmt;

        public Stmt Visit(ReturnStmt stmt)
        {
            stmt.Value?.Accept(this);
            return stmt;
        }

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
            throw new System.NotImplementedException();
        }

        public Expr Visit(AssignExpr expr)
        {
            expr.Lhs.Accept(this);
            throw new System.NotImplementedException();
        }

        public Expr Visit(BinaryExpr expr)
        {
            var lhs = expr.Lhs.Accept(this);
            var rhs = expr.Rhs.Accept(this);

            if (expr.Op == TokenType.EQUALS)
            {
                { if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is StringExpr expr1 && rhs is StringExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value == expr2.Value); }
                { if (lhs is NullExpr && rhs is NullExpr) return new BooleanExpr(expr.Offset, true); }
            }
            else if (expr.Op == TokenType.NEQUALS)
            {
                { if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is StringExpr expr1 && rhs is StringExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value != expr2.Value); }
                { if (lhs is NullExpr && rhs is NullExpr) return new BooleanExpr(expr.Offset, false); }
            }
            else if (expr.Op == TokenType.LESS)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value < expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new System.Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            else if (expr.Op == TokenType.GREATER)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value > expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new System.Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            else if (expr.Op == TokenType.LESS_EQUALS)
            {
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value <= expr2.Value);
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new System.Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            else if (expr.Op == TokenType.GREATER_EQUALS)
            {
                if (lhs is BooleanExpr && rhs is BooleanExpr) throw new System.Exception();
                if (lhs is NumExpr expr1 && rhs is NumExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value >= expr2.Value);
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            else if (expr.Op == TokenType.AND)
            {
                if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value && expr2.Value);
                if (lhs is NumExpr && rhs is NumExpr) throw new System.Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            else if (expr.Op == TokenType.OR)
            {
                if (lhs is BooleanExpr expr1 && rhs is BooleanExpr expr2) return new BooleanExpr(expr.Offset, expr1.Value || expr2.Value);
                if (lhs is NumExpr && rhs is NumExpr) throw new System.Exception();
                if (lhs is StringExpr && rhs is StringExpr) throw new System.Exception();
                if (lhs is NullExpr && rhs is NullExpr) throw new System.Exception();
            }
            return expr;
        }

        public Expr Visit(BooleanExpr expr) => expr;

        public Expr Visit(CallExpr expr)
        {
            var e = State.FromCall(expr);
            if (!builder.TableManager.FindSymbol(e, scopes.ToArray()) && pass > 1) throw new System.Exception();
            return expr;
        }

        public Expr Visit(GroupingExpr expr)
        {
            expr.Content.Accept(this);
            return expr;
        }

        public Expr Visit(NullExpr expr) => expr;

        public Expr Visit(NumExpr expr) => expr;

        public Expr Visit(StringExpr expr) => expr;

        public Expr Visit(UnaryExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public Expr Visit(VarExpr expr)
        {
            expr.Type.Accept(this);
            return expr;
        }

        public Call Visit(FunctionCall call) => throw new System.NotImplementedException();

        public Call Visit(IdfCall call) => throw new System.NotImplementedException();
    }
}
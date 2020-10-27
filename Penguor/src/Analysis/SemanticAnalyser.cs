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

        public SemanticAnalyser(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;

            scopes = new();
        }

        public Decl Analyse()
        {
            scopes.Add(new State());
            scopes.Add(new State());
            return program.Accept(this);
        }

        public Decl Visit(BlockDecl decl)
        {
            foreach (var i in decl.Content) i.Accept(this);
            return decl;
        }

        public Decl Visit(DataDecl decl)
        {
            decl.Parent?.Accept(this);

            scopes[0].Push(decl.Name);
            decl.Content.Accept(this);
            scopes[0].Pop();

            return decl;
        }

        public Decl Visit(TypeDecl decl)
        {
            decl.Parent?.Accept(this);

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
            decl.Variable.Accept(this);
            foreach (var i in decl.Parameters) i.Accept(this);
            scopes[0].Push(decl.Variable.Name);
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
            decl.Parent?.Accept(this);

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
            decl.Variable.Accept(this);
            if (decl.Init is not null) decl.Init.Accept(this);
            return decl;
        }

        public Stmt Visit(BlockStmt stmt)
        {
            foreach (var i in stmt.Content)
            {
                i.Accept(this);
            }
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
            throw new System.NotImplementedException();
        }

        public Stmt Visit(IfStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(CompilerStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(ReturnStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(VarStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(WhileStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public Expr Visit(AssignExpr expr)
        {
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
            return expr;
        }

        public Expr Visit(BooleanExpr expr) => expr;

        public Expr Visit(CallExpr expr)
        {
            var e = State.FromCall(expr);
            if (!builder.TableManager.FindSymbol(e, scopes.ToArray())) throw new System.Exception();
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
            //TODO: sometimes the identifier may need to be added to the current table
            return expr;
        }

        public Call Visit(FunctionCall call) => throw new System.NotImplementedException();

        public Call Visit(IdfCall call) => throw new System.NotImplementedException();
    }
}
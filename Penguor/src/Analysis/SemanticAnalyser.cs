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
using Penguor.Compiler.Build;

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class SemanticAnalyser : IDeclVisitor<Decl>, IStmtVisitor<Stmt>, IExprVisitor<Expr>, ICallVisitor<Call>
    {
        private readonly Builder builder;

        private readonly ProgramDecl program;

        private readonly List<State> scopes;
        private readonly State state;

        public SemanticAnalyser(ProgramDecl program, Builder builder)
        {
            this.program = program;
            state = new();
            this.builder = builder;

            scopes = new();
        }

        public Decl Analyse()
        {
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
            if (decl.Parent != null) decl.Parent.Accept(this);

            state.Push(decl.Name);
            decl.Content.Accept(this);
            state.Pop();

            return decl;
        }

        public Decl Visit(TypeDecl decl)
        {
            if (decl.Parent != null) decl.Parent.Accept(this);

            state.Push(decl.Name);
            decl.Content.Accept(this);
            state.Pop();

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
            state.Push(decl.Variable.Name);
            decl.Content.Accept(this);
            state.Pop();

            return decl;
        }

        public Decl Visit(LibraryDecl decl)
        {
            state.Append(decl.Name);
            decl.Content.Accept(this);
            state.Remove(decl.Name);
            return decl;
        }

        public Decl Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations) i.Accept(this);
            return decl;
        }

        public Decl Visit(SystemDecl decl)
        {
            if (decl.Parent != null) decl.Parent.Accept(this);

            state.Push(decl.Name);
            decl.Content.Accept(this);
            state.Pop();

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
            else throw new System.Exception();
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
            throw new System.NotImplementedException();
        }

        public Expr Visit(BooleanExpr expr) => expr;

        public Expr Visit(CallExpr expr)
        {
            var e = State.FromCall(expr);
            if (!builder.TableManager.FindSymbol(e, state, scopes.ToArray())) throw new System.Exception();
            return expr;
        }

        public Expr Visit(GroupingExpr expr) => new GroupingExpr(expr.Content.Accept(this));

        public Expr Visit(NullExpr expr) => expr;

        public Expr Visit(NumExpr expr) => expr;

        public Expr Visit(StringExpr expr) => expr;

        public Expr Visit(UnaryExpr expr)
        {
            Expr rhs = expr.Rhs.Accept(this);

            return expr.Op switch
            {
                null => expr.Rhs.Accept(this),
                _ => new UnaryExpr(expr.Op, rhs),
            };
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
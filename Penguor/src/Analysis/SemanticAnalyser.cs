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

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class SemanticAnalyser : IDeclVisitor<Decl>, IStmtVisitor<Stmt>, IExprVisitor<Expr>, ICallVisitor<Call>
    {
        private readonly ProgramDecl program;

        private readonly Stack<string> state;

        public SemanticAnalyser(ProgramDecl program)
        {
            this.program = program;
            state = new Stack<string>();
        }

        public Decl Analyse()
        {
            return program.Accept(this);
        }

        public Decl Visit(BlockDecl decl)
        {
            foreach (var i in decl.Content) i.Accept(this);
            return decl;
        }

        public Decl Visit(DataDecl decl)
        {
            state.Push(decl.Name.Name);
            throw new System.NotImplementedException();

        }

        public Decl Visit(TypeDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(DeclStmt decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(FunctionDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(LibraryDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations) i.Accept(this);

            return decl;
        }

        public Decl Visit(SystemDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(VarDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Stmt Visit(BlockStmt stmt)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
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

        public Expr Visit(BooleanExpr expr)
        {
            throw new System.NotImplementedException();

        }

        public Expr Visit(CallExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public Expr Visit(EOFExpr expr) => expr;

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
            throw new System.NotImplementedException();
        }

        public Call Visit(FunctionCall call)
        {
            throw new System.NotImplementedException();
        }

        public Call Visit(IdfCall call)
        {
            state.Push(call.Name.Name);
            // return call.Name;
            throw new System.NotImplementedException();

        }
    }
}
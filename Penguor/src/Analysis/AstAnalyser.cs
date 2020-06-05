/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.Analysis
{
    public class AstAnalyser : IVisitor<string>
    {
        public string Visit(FunctionCall call)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(IdfCall call)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(BlockDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ContainerDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DatatypeDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DeclStmt decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(FunctionDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(LibraryDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ProgramDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(SystemDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(VarDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(AssignExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(BinaryExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(BooleanExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(CallExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(EOFExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(GroupingExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(IdentifierExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(NullExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(NumExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(StringExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(UnaryExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(VarExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(BlockStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DoStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ElifStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ExprStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ForStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(IfStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(PPStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ReturnStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(WhileStmt stmt)
        {
            throw new System.NotImplementedException();
        }
    }
}
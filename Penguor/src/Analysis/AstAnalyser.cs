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

using static Penguor.Compiler.Parsing.TokenType;

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class AstAnalyser : IDeclVisitor<object>, IStmtVisitor<object>, IExprVisitor<object>, ICallVisitor<object>
    {

        public object Visit(FunctionCall call)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(IdfCall call)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(BlockDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ContainerDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(DatatypeDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(DeclStmt decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(FunctionDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(LibraryDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ProgramDecl decl)
        {
            foreach (var declaration in decl.Declarations) declaration.Accept(this);
            throw new System.NotImplementedException();
        }

        public object Visit(SystemDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(VarDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(AssignExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(BinaryExpr expr)
        {
            switch (expr.Op)
            {
                case PLUS:
                    if (expr.Lhs.GetType() == typeof(NumExpr) && expr.Rhs.GetType() == typeof(NumExpr))
                    {
                        NumExpr lhs = (NumExpr)expr.Lhs;
                        NumExpr rhs = (NumExpr)expr.Rhs;

                        return new NumExpr(lhs.Value + rhs.Value);
                    }
                    break;
                case MINUS:
                    if (expr.Lhs.GetType() == typeof(NumExpr) && expr.Rhs.GetType() == typeof(NumExpr))
                    {
                        NumExpr lhs = (NumExpr)expr.Lhs;
                        NumExpr rhs = (NumExpr)expr.Rhs;

                        return new NumExpr(lhs.Value - rhs.Value);
                    }
                    break;
                case MUL:
                    if (expr.Lhs.GetType() == typeof(NumExpr) && expr.Rhs.GetType() == typeof(NumExpr))
                    {
                        NumExpr lhs = (NumExpr)expr.Lhs;
                        NumExpr rhs = (NumExpr)expr.Rhs;

                        return new NumExpr(lhs.Value * rhs.Value);
                    }
                    break;
                case DIV:
                    if (expr.Lhs.GetType() == typeof(NumExpr) && expr.Rhs.GetType() == typeof(NumExpr))
                    {
                        NumExpr lhs = (NumExpr)expr.Lhs;
                        NumExpr rhs = (NumExpr)expr.Rhs;

                        return new NumExpr(lhs.Value / rhs.Value);
                    }
                    break;
                case AND:
                    if (expr.Lhs.GetType() == typeof(BooleanExpr))
                    {
                        bool lhs = ((BooleanExpr)expr.Lhs).Value;
                        bool rhs = ((BooleanExpr)expr.Rhs).Value;
                        return (lhs && rhs);
                    }
                    break;
                case OR:
                    if (expr.Lhs.GetType() == typeof(BooleanExpr))
                    {
                        bool lhs = ((BooleanExpr)expr.Lhs).Value;
                        bool rhs = ((BooleanExpr)expr.Rhs).Value;
                        return (lhs || rhs);
                    }
                    break;
                default:
                    return expr;
            }
            throw new System.ArgumentException();
        }

        public object Visit(BooleanExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(CallExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(EOFExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(GroupingExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(IdentifierExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(NullExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(NumExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(StringExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(UnaryExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(VarExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(BlockStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(DoStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ElifStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ExprStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ForStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(IfStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(PPStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(ReturnStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(WhileStmt stmt)
        {
            throw new System.NotImplementedException();
        }
    }
}
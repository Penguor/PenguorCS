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
    public class AstAnalyser : IDeclVisitor<Decl>, IStmtVisitor<object>, IExprVisitor<Expr>, ICallVisitor<object>
    {
        private ProgramDecl program;

        public AstAnalyser(ProgramDecl program)
        {
            this.program = program;
        }

        public Decl Analyse()
        {
            return program.Accept(this);
        }

        public Decl Visit(BlockDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(ContainerDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public Decl Visit(DatatypeDecl decl)
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
            foreach (var declaration in decl.Declarations) declaration.Accept(this);
            throw new System.NotImplementedException();
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

        public object Visit(CompilerStmt stmt)
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

        public Expr Visit(AssignExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public Expr Visit(BinaryExpr expr)
        {
            Expr lhs = expr.Lhs.Accept(this);
            Expr rhs = expr.Rhs.Accept(this);
            switch (expr.Op)
            {
                case AND:
                    if (typeof(BooleanExpr).Equals(lhs.GetType()) && typeof(BooleanExpr).Equals(rhs.GetType()))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        bool b = ((BooleanExpr)rhs).Value;
                        return new BooleanExpr(a && b);
                    }
                    else if (lhs.GetType() == typeof(BooleanExpr))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        if (a == false) return new BooleanExpr(false);
                    }
                    break;
                case OR:
                    if (typeof(BooleanExpr).Equals(lhs.GetType()) && typeof(BooleanExpr).Equals(rhs.GetType()))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        bool b = ((BooleanExpr)rhs).Value;
                        return new BooleanExpr(a || b);
                    }
                    else if (lhs.GetType() == typeof(BooleanExpr))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        if (a == true) return new BooleanExpr(true);
                    }
                    break;
                case XOR:
                    if (typeof(BooleanExpr).Equals(lhs.GetType()) && typeof(BooleanExpr).Equals(rhs.GetType()))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        bool b = ((BooleanExpr)rhs).Value;
                        return new BooleanExpr(a != b);
                    }
                    break;
                case NEQUALS:
                    if (typeof(BooleanExpr).Equals(lhs.GetType()) && typeof(BooleanExpr).Equals(rhs.GetType()))
                    {
                        bool a = ((BooleanExpr)lhs).Value;
                        bool b = ((BooleanExpr)rhs).Value;
                        return new BooleanExpr(a != b);
                    }
                    if (typeof(StringExpr).Equals(lhs.GetType()) && typeof(StringExpr).Equals(rhs.GetType()))
                    {
                        string a = ((StringExpr)lhs).Value;
                        string b = ((StringExpr)rhs).Value;
                        return new BooleanExpr(a != b);
                    }
                    if (typeof(NumExpr).Equals(lhs.GetType()) && typeof(NumExpr).Equals(rhs.GetType()))
                    {
                        double a = ((NumExpr)lhs).Value;
                        double b = ((NumExpr)rhs).Value;
                        return new BooleanExpr(a != b);
                    }
                    break;
                case PLUS:
                    if (typeof(NumExpr).Equals(lhs.GetType()) && typeof(NumExpr).Equals(rhs.GetType()))
                    {
                        double a = ((NumExpr)lhs).Value;
                        double b = ((NumExpr)rhs).Value;

                        return new NumExpr(a + b);
                    }
                    break;
                case MINUS:
                    if (typeof(NumExpr).Equals(lhs.GetType()) && typeof(NumExpr).Equals(rhs.GetType()))
                    {
                        double a = ((NumExpr)lhs).Value;
                        double b = ((NumExpr)rhs).Value;

                        return new NumExpr(a - b);
                    }
                    break;
                case MUL:
                    if (typeof(NumExpr).Equals(lhs.GetType()) && typeof(NumExpr).Equals(rhs.GetType()))
                    {
                        double a = ((NumExpr)lhs).Value;
                        double b = ((NumExpr)rhs).Value;

                        return new NumExpr(a * b);
                    }
                    break;
                case DIV:
                    if (typeof(NumExpr).Equals(lhs.GetType()) && typeof(NumExpr).Equals(rhs.GetType()))
                    {
                        double a = ((NumExpr)lhs).Value;
                        double b = ((NumExpr)rhs).Value;

                        return new NumExpr(a / b);
                    }
                    break;
            }
            return new BinaryExpr(lhs, expr.Op, rhs);
        }

        public Expr Visit(BooleanExpr expr) => expr;

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

            switch (expr.Op)
            {
                case null:
                    return (expr.Rhs.Accept(this));
                    // case EXCL_MARK:
                    //     if (typeof(rhs).IsIn)
            }
            return new UnaryExpr(expr.Op, rhs);
        }

        public Expr Visit(VarExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(FunctionCall call)
        {
            throw new System.NotImplementedException();
        }

        public object Visit(IdfCall call)
        {
            throw new System.NotImplementedException();
        }
    }
}
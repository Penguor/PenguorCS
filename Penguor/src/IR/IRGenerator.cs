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

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<int>
    {

        private readonly Builder builder;

        private readonly ProgramDecl program;

        private readonly List<State> scopes;

        private int _labelNum;
        private string LabelNum
        {
            get => $"L{_labelNum++}";
        }

        private uint _instrNum;
        private uint InstNum { get => _instrNum++; }

        private readonly List<IRStatement> statements = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;
            scopes = new();
            scopes.Add(new State());
        }

        private uint AddStmt(OPCode code, params string[] operands)
        {
            var num = InstNum;
            statements.Add(new IRStatement(num, code, operands));
            return num;
        }

        private uint AddLabel() => AddStmt(OPCode.LABEL, scopes[0].ToString());

        private uint AddNumLabel() => AddStmt(OPCode.LABEL, LabelNum);

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
                Debugging.Debug.Log(new IRProgram(statements).ToString(), Debugging.LogLevel.Debug);
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

        public int Visit(DeclStmt decl) => decl.Stmt.Accept(this);

        public int Visit(FunctionDecl decl)
        {
            scopes[0].Push(decl.Name);
            AddLabel();
            foreach (var i in decl.Parameters)
                AddStmt(OPCode.LOADPARAM, (scopes[0] + i.Name).ToString());
            var length = statements.Count;
            decl.Content.Accept(this);
            if (statements[^1].Code != OPCode.RETURN) AddStmt(OPCode.RETURN);
            scopes[0].Pop();
            return 0;
        }

        public int Visit(LibraryDecl decl)
        {
            scopes[0].Append(decl.Name);
            AddStmt(OPCode.LIB, scopes[0].ToString());
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
            AddStmt(OPCode.USE, State.FromCall(decl.Lib).ToString());
            return 0;
        }

        public int Visit(VarDecl decl)
        {
            scopes[0].Push(decl.Name);

            if (decl.Init != null)
            {
                decl.Init.Accept(this);
                AddStmt(OPCode.DEF, scopes[0].ToString(), $"({statements[^1].Number})");
            }
            else
            {
                AddStmt(OPCode.DFE, scopes[0].ToString());
            }
            scopes[0].Pop();

            return 0;
        }

        public int Visit(BlockStmt stmt)
        {
            foreach (var i in stmt.Content)
                i.Accept(this);
            return 0;
        }

        public int Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(CompilerStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(DoStmt stmt)
        {
            var num = AddNumLabel();
            stmt.Content.Accept(this);
            stmt.Condition.Accept(this);
            AddStmt(OPCode.JTR, $"({statements[^1].Number})", $"({num})");
            return 0;
        }

        public int Visit(ElifStmt stmt)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public int Visit(ReturnStmt stmt)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public int Visit(AssignExpr expr)
        {
            expr.Value.Accept(this);
            if (expr.Op != TokenType.ASSIGN) throw new System.Exception();
            AddStmt(
                OPCode.ASSIGN,
                builder.TableManager.GetStateBySymbol(State.FromCall(expr.Lhs), scopes.ToArray())?.ToString() ?? throw new System.Exception(),
                $"({statements[^1].Number})");
            return 0;
        }

        public int Visit(BinaryExpr expr)
        {
            expr.Lhs.Accept(this);
            var num1 = statements[^1].Number;
            expr.Rhs.Accept(this);
            var num2 = statements[^1].Number;
            AddStmt(expr.Op switch
            {
                TokenType.PLUS => OPCode.ADD,
                TokenType.MINUS => OPCode.SUB,
                TokenType.MUL => OPCode.MUL,
                TokenType.DIV => OPCode.DIV,
                TokenType.GREATER => OPCode.GREATER,
                TokenType.LESS => OPCode.LESS,
                _ => throw new System.Exception()
            }, $"({num1})", $"({num2})");
            return 0;
        }

        public int Visit(BooleanExpr expr)
        {
            AddStmt(OPCode.LOAD, expr.Value.ToString());
            return 0;
        }

        public int Visit(CallExpr expr)
        {
            AddStmt(OPCode.CALL, builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray())?.ToString() ?? throw new System.Exception());
            if (expr.Postfix == TokenType.DPLUS)
                AddStmt(OPCode.ADD, $"({statements[^1].Number})", "1");
            else if (expr.Postfix == TokenType.DMINUS)
                AddStmt(OPCode.SUB, $"({statements[^1].Number})", "1");
            return 0;
        }

        public int Visit(GroupingExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(NullExpr expr)
        {
            AddStmt(OPCode.LOAD, "null");
            return 0;
        }

        public int Visit(NumExpr expr)
        {
            AddStmt(OPCode.LOAD, expr.Value.ToString());
            return 0;
        }

        public int Visit(StringExpr expr)
        {
            AddStmt(OPCode.LOAD, $"\"{expr.Value}\"");
            return 0;
        }

        public int Visit(UnaryExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public int Visit(VarExpr expr)
        {
            throw new System.NotImplementedException();
        }
    }
}
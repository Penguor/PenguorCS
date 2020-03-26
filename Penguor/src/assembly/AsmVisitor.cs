/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
*/

using System.Collections.Generic;
using Penguor.Parsing;
using Penguor.Parsing.AST;

#pragma warning disable 1591

namespace Penguor.ASM
{
    public class AsmVisitor : Visitor
    {
        Library library;
        public AsmVisitor(ref Library file)
        {
            this.library = file;
        }

        public string Visit(AssignExpr expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(BinaryExpr expr)
        {
            Instruction ins = new Instruction();
            switch (expr.Op)
            {
                case TokenType.PLUS:
                    ins.Mnemonic = "add";
                    break;
            }
            string lhs = expr.Lhs.Accept(this);
            return expr.Op.ToString();
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

        public string Visit(BlockStmt stmt)
        {
            foreach (Stmt i in stmt.Contents) stmt.Accept(this);
            return null;
        }

        public string Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ComponentStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DataTypeStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DoStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ExprStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ForStmt expr)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(FunctionStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(HeadStmt stmt)
        {
            if (stmt.Definition.ContainsKey("library"))
            {
                string library;
                stmt.Definition.TryGetValue("library", out library);
                this.library.Name = library;
            }

            for (int i = 0; i < stmt.IncludeLhs.Count; i++)
            {
                Instruction ins = new Instruction("include");
                ins.Operands[0] = stmt.IncludeLhs[i].Accept(this);
            }
            return null;
        }

        public string Visit(IfStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(ProgramStmt stmt)
        {
            stmt.Head.Accept(this);
            foreach (Stmt dec in stmt.Declarations)
            {
                dec.Accept(this);
            }
            return "";
        }

        public string Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(SystemStmt stmt)
        {
            stmt.Content.Accept(this);
            return stmt.Name.token;
        }

        public string Visit(VarStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(WhileStmt stmt)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using Penguor.Compiler.Analysis;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.IR
{
    public class IRGenerator : IExceptionHandler, IDeclVisitor<int>, IStmtVisitor<int>, IExprVisitor<IRReference>
    {
        private readonly Builder builder;
        private readonly ProgramDecl program;

        private readonly IRProgram irProgram = new();

        private readonly List<State> scopes = new();

        public IRGenerator(ProgramDecl program, Builder builder)
        {
            this.program = program;
            this.builder = builder;

            scopes.Add(new State());
        }

        public IRProgram Generate()
        {
            try
            {
                program.Accept(this);
            }
            finally
            {
                Console.WriteLine(irProgram.ToString());
            }

            return irProgram;
        }

        // helper methods
        private IRReference AddStmt(IROPCode code, params IRArgument[] operands) => irProgram.GetCurrentFunction().AddStmt(code, operands);
        private IRReference GetLastNumber() => new(irProgram.GetCurrentFunction().GetInstructionNumber());

        //ssa related stuff
        // paper: Simple and Efficient Construction of Static Single Assignment Form, Braun et al.
        // https://pp.info.uni-karlsruhe.de/uploads/publikationen/braun13cc.pdf

        private void WriteVariable(State variable, State block, IRReference value) => irProgram.GetCurrentFunction().WriteVariable(variable, block, value);
        private IRReference ReadVariable(State variable, State block) => irProgram.GetCurrentFunction().ReadVariable(variable, block);

        // stuff to make ssa easier to handle
        private State BeginBlock(State block) => irProgram.TryGetCurrentFunction()?.BeginBlock(block) ?? throw new NullReferenceException();
        private State GetCurrentBlock() => irProgram.GetCurrentFunction().CurrentBlock ?? throw new NullReferenceException("no active block");

        // visitor
        public int Visit(BlockDecl decl)
        {
            foreach (var item in decl.Content)
            {
                item.Accept(this);
            }
            return 0;
        }

        public int Visit(DataDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(ExternDecl decl)
        {
            irProgram.AddGlobalStmt(IROPCode.DEFEXT, new IRState(new State(decl.Name.ToString())));
            return 0;
        }

        public int Visit(FunctionDecl decl)
        {
            // push name
            scopes[0].Push(decl.Name);

            // set function in ir program
            irProgram.BeginFunction(scopes[0]);

            // set up parameters
            foreach (var parameter in decl.Parameters)
            {
                AddStmt(IROPCode.LOADPARAM, new IRState(((ExprAttribute)parameter.Attribute!).Type));
                WriteVariable(builder.TableManager.GetStateBySymbol(parameter.Name, scopes)!, irProgram.TryGetCurrentBlock()!, GetLastNumber());
            }

            //generate content ir
            decl.Content.Accept(this);

            //seal function
            irProgram.SealCurrentFunction();
            scopes[0].Pop();

            return 0;
        }

        public int Visit(LibraryDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(ModifiedDecl decl)
        {
            decl.Declaration.Accept(this);
            return 0;
        }

        public int Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations)
                i.Accept(this);

            return 0;
        }

        public int Visit(StmtBlockDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(StmtDecl decl)
        {
            decl.Stmt.Accept(this);
            return 0;
        }

        public int Visit(SystemDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(TypeDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(UsingDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(VarDecl decl)
        {
            throw new NotImplementedException();
        }

        public int Visit(AsmStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(BlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(CaseStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(CompilerStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(DoStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(ElifStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(ExprStmt stmt)
        {
            stmt.Expr.Accept(this);
            return 0;
        }

        public int Visit(ForeachStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(ForStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(IfStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(ReturnStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(SwitchStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(VarStmt stmt)
        {
            throw new NotImplementedException();
        }

        public int Visit(WhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(AssignExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(BinaryExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(BooleanExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(CallExpr expr)
        {
            if (expr.Callee[^1] is IdfCall)
            {
                return ReadVariable(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception(), GetCurrentBlock());
            }
            else if (expr.Callee[^1] is FunctionCall fCall)
            {
                AddStmt(IROPCode.BCALL);

                IRArgument[] arguments = new IRArgument[fCall.Args.Count + 1];
                for (int i = 0; i < fCall.Args.Count; i++)
                {
                    arguments[i + 1] = fCall.Args[i].Accept(this);
                }
                for (int i = 1; i < arguments.Length; i++)
                {
                    arguments[i] = AddStmt(IROPCode.LOADARG, new IRState(((ExprAttribute)fCall.Args[i - 1].Attribute!).Type), arguments[i]);
                }
                arguments[0] = new IRState(builder.TableManager.GetStateBySymbol(State.FromCall(expr), scopes.ToArray()) ?? throw new Exception());
                return AddStmt(IROPCode.CALL, arguments);
            }
            else
            {
                throw new Exception();
            }
        }

        public IRReference Visit(CharExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(GroupingExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(IncrementExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(NullExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(NumExpr expr)
        {
            if (expr.Value.Contains('.')) return AddStmt(IROPCode.LOAD, new IRDouble(expr.NumValue ?? throw new Exception()));
            else return AddStmt(IROPCode.LOAD, new IRInt((int?)expr.NumValue ?? throw new Exception()));
        }

        public IRReference Visit(StringExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(TypeCallExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(UnaryExpr expr)
        {
            throw new NotImplementedException();
        }

        public IRReference Visit(VarExpr expr)
        {
            throw new NotImplementedException();
        }
    }
}
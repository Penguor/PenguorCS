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

using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.IR;

using static Penguor.Compiler.Parsing.TokenType;

#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    public class StructDiscover : IDeclVisitor<IRStruct>
    {
        private ProgramDecl program;

        private Stack<string> state;

        public StructDiscover(ProgramDecl program)
        {
            this.program = program;
            state = new Stack<string>();
        }

        public IRStruct Discover() => program.Accept(this);


        public IRStruct Visit(BlockDecl decl)
        {
            IRBlock block = new IRBlock(new State(new string[0]));
            foreach (var d in decl.Content) block.Structures.Add(d.Accept(this));
            return block;
        }

        public IRStruct Visit(ContainerDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public IRStruct Visit(DatatypeDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public IRStruct Visit(DeclStmt decl)
        {
            throw new System.NotImplementedException();
        }

        public IRStruct Visit(FunctionDecl decl)
        {
            Token name = ((VarExpr)decl.Variable).Name;

            List<State> signature = new List<State>(decl.Parameters.Count);
            foreach (VarExpr expr in decl.Parameters)
            {
                var type = (CallExpr)expr.Type;

                signature.Add(State.FromCall(type));
            }

            IRFunction fun = new IRFunction(State.FromToken(name), decl.AccessMod, decl.NonAccessMod, signature.ToArray());
            fun.Structures.Add(decl.Content.Accept(this));

            return fun;
        }

        public IRStruct Visit(LibraryDecl decl)
        {
            string[] name = new string[decl.Name.Count];
            for (int i = 0; i < name.Length; i++) name[i] = decl.Name[i].token;

            IRLibrary lib = new IRLibrary(new State(name), decl.AccessMod, decl.NonAccessMod);
            lib.Structures.Add(decl.Content.Accept(this));

            return lib;
        }

        public IRStruct Visit(ProgramDecl decl)
        {
            IRProgram ir = new IRProgram(program);
            foreach (var i in decl.Declarations) ir.Structures.Add(i.Accept(this));
            return ir;
        }

        public IRStruct Visit(SystemDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public IRStruct Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public IRStruct Visit(VarDecl decl)
        {
            throw new System.NotImplementedException();
        }
    }
}
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
    /*
     The following procedure should be followed in the visitor methods:
        1. push the structure name to the "state" Stack
        2. add a State instance representing the structure to the "symbols" List
        3. call Accept(this) for all children declarations
        4. pop the name from the "state" Stack to restore previous state
        5. return null
     
     this may be done using the provided Register() methods
    */

    public class DeclDiscover : IDeclVisitor<object?>
    {
        private readonly ProgramDecl program;

        private readonly List<State> symbols;
        private readonly Stack<string> state;

        public DeclDiscover(ProgramDecl program)
        {
            this.program = program;
            state = new Stack<string>();
            symbols = new List<State>();
        }

        public List<State> Discover()
        {
            program.Accept(this);
            return symbols;
        }

        private object? Register(string name, Decl child)
        {
            state.Push(name);

            symbols.Add(new State(state.ToArray()));

            child.Accept(this);

            state.Pop();

            return null;
        }

        private object? Register(string name, Decl[] child)
        {
            state.Push(name);

            symbols.Add(new State(state.ToArray()));

            foreach (var decl in child) decl.Accept(this);

            state.Pop();

            return null;
        }

        public object? Visit(BlockDecl decl)
        {
            foreach (var d in decl.Content) d.Accept(this);
            return null;
        }

        public object? Visit(DataDecl decl) => Register(decl.Name.token, decl.Content);
        public object? Visit(TypeDecl decl) => Register(decl.Name.token, decl.Content);
        public object? Visit(DeclStmt decl) => null;

        public object? Visit(FunctionDecl decl) => Register(((VarExpr)decl.Variable).Name.token, decl.Content);

        public object? Visit(LibraryDecl decl)
        {
            for (int i = 0; i < decl.Name.Count; i++)
                state.Push(decl.Name[i].token);

            symbols.Add(new State(state.ToArray()));

            decl.Content.Accept(this);

            for (int i = 0; i < decl.Name.Count; i++) state.Pop();

            return null;
        }

        public object? Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations) i.Accept(this);
            return null;
        }

        public object? Visit(SystemDecl decl) => Register(decl.Name.token, decl.Content);

        public object? Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object? Visit(VarDecl decl)
        {
            state.Push(((VarExpr)decl.Variable).Name.token);
            symbols.Add(new State(state.ToArray()));
            state.Pop();

            return null;
        }
    }
}
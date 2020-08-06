/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System;
using System.Collections.Generic;

using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;


#pragma warning disable 1591

namespace Penguor.Compiler.Analysis
{
    /*
     The following procedure should be followed in the visitor methods:
        1. push the structure Frame to the "state" Stack
        2. add a State instance representing the structure to the SymbolTableManager
        3. call Accept(this) for all child declarations
        4. pop the name from the "state" Stack to restore previous state
        5. return null
     
     this may be done using the provided Register() methods
    */

    public class DeclDiscover : IDeclVisitor<object?>
    {
        private SymbolTableManager manager;
        private readonly ProgramDecl program;

        private readonly Stack<AddressFrame> state;

        public DeclDiscover(ProgramDecl program)
        {
            this.program = program;
            state = new Stack<AddressFrame>();
        }

        public void Discover(ref SymbolTableManager tableManager)
        {
            manager = tableManager;
            program.Accept(this);
        }

        private State GetState()
        {
            AddressFrame[] frames = state.ToArray();
            Array.Reverse(frames);
            return new State(frames);
        }

        private object? Register(Token name, AddressType type)
        {
            state.Push(new AddressFrame(name, type));

            manager.AddDeclaration(GetState());

            state.Pop();

            return null;
        }

        private object? Register(Token name, AddressType type, Decl child)
        {
            state.Push(new AddressFrame(name, type));

            manager.AddDeclaration(GetState());

            child.Accept(this);

            state.Pop();

            return null;
        }

        private object? Register(Token name, AddressType type, IEnumerable<Decl> children)
        {
            state.Push(new AddressFrame(name, type));

            manager.AddDeclaration(GetState());

            foreach (var decl in children) decl.Accept(this);

            state.Pop();

            return null;
        }

        public object? Visit(BlockDecl decl)
        {
            foreach (var d in decl.Content) d.Accept(this);
            return null;
        }

        public object? Visit(DataDecl decl) => Register(decl.Name, AddressType.DataDecl, decl.Content);
        public object? Visit(TypeDecl decl) => Register(decl.Name, AddressType.TypeDecl, decl.Content);
        public object? Visit(DeclStmt decl) => null;

        public object? Visit(FunctionDecl decl) => Register(decl.Variable.Name, AddressType.FunctionDecl, decl.Content);

        public object? Visit(LibraryDecl decl)
        {
            for (int i = 0; i < decl.Name.Count; i++)
                state.Push(new AddressFrame(decl.Name[i], i == (decl.Name.Count - 1) ? AddressType.LibraryDecl : AddressType.LibraryDeclPart));

            manager.AddDeclaration(GetState());

            decl.Content.Accept(this);

            for (int i = 0; i < decl.Name.Count; i++) state.Pop();

            return null;
        }

        public object? Visit(ProgramDecl decl)
        {
            foreach (var i in decl.Declarations) i.Accept(this);
            return null;
        }

        public object? Visit(SystemDecl decl) => Register(decl.Name, AddressType.SystemDecl, decl.Content);

        public object? Visit(UsingDecl decl)
        {
            throw new System.NotImplementedException();
        }

        public object? Visit(VarDecl decl)
        {
            var variable = decl.Variable;
            state.Push(new AddressFrame(decl.Variable.Name, AddressType.VarDecl, State.FromCall((CallExpr)variable.Type))); //! this is not finished
            manager.AddDeclaration(GetState());
            state.Pop();

            return null;
        }
    }
}
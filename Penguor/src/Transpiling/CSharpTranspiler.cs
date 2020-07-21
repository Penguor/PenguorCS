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
using System;
using System.IO;
using System.Text;
using Penguor.Compiler.Parsing;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing.AST;

using static Penguor.Compiler.Parsing.TokenType;

#pragma warning disable 1591

namespace Penguor.Compiler.Transpiling
{
    public class CSharpTranspiler : IDeclVisitor<string>, IStmtVisitor<string>, IExprVisitor<string>, ICallVisitor<string>
    {
        private readonly ProgramDecl program;

        public CSharpTranspiler(ProgramDecl program)
        {
            this.program = program;
        }

        public void Transpile(string output)
        {
            string code = Visit(program);
            using StreamWriter writer = new StreamWriter(output);
            writer.Write(code);
        }
        public string Visit(FunctionCall call)
        {
            StringBuilder builder = new StringBuilder($"{call.Name.token}(");

            foreach (var arg in call.Args) builder.Append(arg.Accept(this)).Append(',');
            if (call.Args.Count > 0) builder.Length--;

            builder.Append(")");

            return builder.ToString();
        }

        public string Visit(IdfCall call) => call.Name.token;

        public string Visit(BlockDecl decl)
        {
            StringBuilder builder = new StringBuilder("\n{\n");
            foreach (var c in decl.Content) builder.Append(c.Accept(this));
            builder.Append("\n}");

            return builder.ToString();
        }

        public string Visit(ContainerDecl decl)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(ConvertAccessMods(decl.AccessMod)).Append(' ');
            foreach (var naMod in decl.NonAccessMod) builder.Append(ConvertAccessMods(naMod)).Append(' ');

            builder.Append("struct ").Append(decl.Name.token).Append(' ');
            if (decl.Parent != null) builder.Append(": ").Append(decl.Parent.Value.token).Append(' ');

            builder.Append(decl.Content.Accept(this));

            return builder.ToString();
        }

        public string Visit(DatatypeDecl decl)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(ConvertAccessMods(decl.AccessMod)).Append(' ');
            foreach (var naMod in decl.NonAccessMod) builder.Append(ConvertAccessMods(naMod)).Append(' ');

            builder.Append("struct ").Append(decl.Name.token).Append(' ');
            if (decl.Parent != null) builder.Append(": ").Append(decl.Parent.Value.token).Append(' ');

            builder.Append(decl.Content.Accept(this));

            return builder.ToString();
        }

        public string Visit(DeclStmt decl) => decl.Stmt.Accept(this);

        public string Visit(FunctionDecl decl)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(ConvertAccessMods(decl.AccessMod)).Append(' ');

            if (decl.Variable.Accept(this).EndsWith("execute"))
            {
                builder.Append("static void Main(");
            }
            else
            {
                foreach (var naMod in decl.NonAccessMod) builder.Append(ConvertAccessMods(naMod)).Append(' ');
                builder.Append(' ').Append(decl.Variable.Accept(this)).Append('(');
            }

            foreach (var p in decl.Parameters!) builder.Append(p.Accept(this)).Append(',');
            if (decl.Parameters.Count > 0) builder.Length--;
            builder.Append(")");

            builder.Append(decl.Content.Accept(this));

            return builder.ToString();
        }

        public string Visit(LibraryDecl decl)
        {
            StringBuilder builder = new StringBuilder("namespace ");
            builder.Append(String.Join('.', decl.Name));
            builder.Append(decl.Content.Accept(this));
            return builder.ToString();
        }

        public string Visit(ProgramDecl decl)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Decl declaration in decl.Declarations) builder.Append(declaration.Accept(this));
            return builder.ToString();
        }

        public string Visit(SystemDecl decl)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ConvertAccessMods(decl.AccessMod)).Append(' ');

            foreach (var naMod in decl.NonAccessMod) builder.Append(ConvertAccessMods(naMod)).Append(' ');

            builder.Append("class ").Append(decl.Name.token).Append(' ');
            if (decl.Parent != null) builder.Append(": ").Append(decl.Parent.Value.token).Append(' ');

            builder.Append(decl.Content.Accept(this));

            return builder.ToString();
        }

        public string Visit(UsingDecl decl) => $"using {decl.Lib.Accept(this)};";

        public string Visit(VarDecl decl)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ConvertAccessMods(decl.AccessMod)).Append(' ');
            foreach (var type in decl.NonAccessMod) builder.Append(ConvertAccessMods(type)).Append(' ');
            builder.Append(decl.Variable.Accept(this)).Append(' ');

            if (decl.Init != null) builder.Append("= ").Append(decl.Init.Accept(this)).Append(";\n");
            else builder.Append(";\n");

            return builder.ToString();
        }

        public string Visit(AssignExpr expr)
        {
            StringBuilder builder = new StringBuilder($"{expr.Lhs.Accept(this)} ");

            builder.Append(expr.Op switch
            {
                ASSIGN => "=",
                ADD_ASSIGN => "+=",
                SUB_ASSIGN => "-=",
                MUL_ASSIGN => "*=",
                DIV_ASSIGN => "/=",
                PERCENT_ASSIGN => "%=",
                BS_LEFT_ASSIGN => "<<=",
                BS_RIGHT_ASSIGN => ">>=",
                BW_AND_ASSIGN => "&=",
                BW_OR_ASSIGN => "|=",
                BW_XOR_ASSIGN => "^=",
                _ => throw new PenguorCSException(1),
            });

            builder.Append(' ').Append(expr.Value.Accept(this)).Append(' ');

            return builder.ToString();
        }

        public string Visit(BinaryExpr expr)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(expr.Lhs.Accept(this));

            builder.Append(expr.Op switch
            {
                OR => "||",
                XOR => "!=",
                AND => "&&",
                BW_OR => "|",
                BW_XOR => "^",
                BW_AND => "&",
                EQUALS => "==",
                NEQUALS => "!=",
                LESS => "<",
                GREATER => ">",
                LESS_EQUALS => "<=",
                GREATER_EQUALS => ">=",
                BS_LEFT => "<<",
                BS_RIGHT => ">>",
                PLUS => "+",
                MINUS => "-",
                MUL => "*",
                DIV => "/",
                PERCENT => "%",
                _ => throw new PenguorCSException(1),
            });

            builder.Append(' ').Append(expr.Rhs.Accept(this)).Append(' ');

            return builder.ToString();
        }

        public string Visit(BooleanExpr expr) => expr.Value switch
        {
            true => " true ",
            false => " false ",
        };

        public string Visit(CallExpr expr)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var callee in expr.Callee) builder.Append(callee.Accept(this)).Append('.');
            builder.Length--;

            if (expr.Postfix != null) builder.Append(expr.Postfix);

            return builder.ToString();
        }

        public string Visit(EOFExpr expr) => "";

        public string Visit(GroupingExpr expr) => $" ({expr.Content.Accept(this)}) ";

        public string Visit(NullExpr expr) => " null ";

        public string Visit(NumExpr expr) => expr.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        public string Visit(StringExpr expr) => $"\"{expr.Value}\"";

        public string Visit(UnaryExpr expr) => expr.Op switch
        {
            EXCL_MARK => "!",
            PLUS => "+",
            MINUS => "-",
            BW_NOT => "~",
            DPLUS => "++",
            DMINUS => "--",
            _ => throw new PenguorCSException(1)
        } + expr.Rhs.Accept(this);

        public string Visit(VarExpr expr) => $"{expr.Type.Accept(this)} {expr.Name.token}";

        public string Visit(BlockStmt stmt)
        {
            StringBuilder builder = new StringBuilder("\n{\n");

            foreach (var c in stmt.Content) builder.Append(c.Accept(this));
            builder.Append("\n}\n");

            return builder.ToString();
        }

        public string Visit(CaseStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(DoStmt stmt) => $"do {stmt.Content.Accept(this)} while({stmt.Condition.Accept(this)});";

        public string Visit(ElifStmt stmt) => $"else if({stmt.Condition.Accept(this)}) {stmt.Content.Accept(this)}";
        public string Visit(ExprStmt stmt) => $"{stmt.Expr.Accept(this)};\n";
        public string Visit(ForStmt stmt) => $"for({stmt.CurrentVar.Accept(this)} : {stmt.Vars.Accept(this)}) {stmt.Content.Accept(this)}";

        public string Visit(IfStmt stmt)
        {
            StringBuilder builder = new StringBuilder($"if({stmt.Condition.Accept(this)}) {stmt.IfC.Accept(this)}");

            foreach (Stmt c in stmt.Elif) builder.Append(' ').Append(c.Accept(this)).Append(' ');
            if (stmt.ElseC != null) builder.Append(" else ").Append(stmt.ElseC.Accept(this));

            return builder.ToString();
        }

        public string Visit(CompilerStmt stmt) => "\n";

        public string Visit(ReturnStmt stmt)
        {
            if (stmt.Value != null) return $"return {stmt.Value.Accept(this)};\n";
            return "return;\n";
        }

        public string Visit(SwitchStmt stmt)
        {
            throw new System.NotImplementedException();
        }

        public string Visit(WhileStmt stmt) => $"while ({stmt.Condition.Accept(this)}) {stmt.Condition.Accept(this)}";

        private string ConvertAccessMods(TokenType? type)
        {
            if (type == null) return "";
            return type switch
            {
                PUBLIC => "public",
                PRIVATE => "private",
                PROTECTED => "protected",
                RESTRICTED => "internal",
                STATIC => "static",
                DYNAMIC => "",
                ABSTRACT => "abstract",
                CONST => "const",
                // default value in arrays etc.
                HASHTAG => "",
                _ => throw new System.ArgumentException("invalid type", nameof(type)),
            };
        }
    }
}
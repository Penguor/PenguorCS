/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
*/
// TODO: remove name tokens form ast nodes, now replaced by id

using System;
using System.Collections.Generic;
using Penguor.Debugging;
using Penguor.Parsing.AST;

namespace Penguor.Parsing
{
    /// <summary>
    /// The Penguor parser
    /// </summary>
    public class Parser
    {
        private List<Token> tokens;
        private int current = 0;
        /// <summary>
        /// create a new parser with the tokens that should be parsed
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        /// <summary>
        /// start the Parsing process
        /// </summary> 
        /// <returns>a statement containing the AST of the parsed file</returns>
        public Decl Parse()
        {
            List<Decl> declarations = new List<Decl>();
            while (!Match(TokenType.EOF)) declarations.Add(Declaration());
            return new ProgramDecl(declarations);
        }

        private Decl Declaration()
        {
            Token? accessMod = null;
            bool hasAccessMod = false;
            Token[] nonAccessMods = new Token[3];

            TokenType[] accessMods = { TokenType.PUBLIC, TokenType.PRIVATE, TokenType.PROTECTED, TokenType.RESTRICTED };
            if (Array.Exists(accessMods, tok => tok == GetCurrent().type))
            {
                accessMod = Advance();
                hasAccessMod = true;
            }
            if (Check(TokenType.STATIC) != Check(TokenType.DYNAMIC))
            {
                nonAccessMods[0] = Advance();
                hasAccessMod = true;
            }
            if (Check(TokenType.ABSTRACT))
            {
                nonAccessMods[1] = Advance();
                hasAccessMod = true;
            }
            if (Check(TokenType.CONST))
            {
                nonAccessMods[2] = Advance();
                hasAccessMod = true;
            }

            if (!hasAccessMod)
            {
                if (Match(TokenType.USING)) return UsingDecl();
                if (Match(TokenType.SYSTEM)) return SystemDecl(null, null);
                if (Match(TokenType.CONTAINER)) return ContainerDecl(null, null);
                if (Match(TokenType.DATATYPE)) return DatatypeDecl(null, null);
                if (Match(TokenType.LIBRARY)) return LibraryDecl(null, null);
                if (Check(TokenType.IDF) && lookAhead(1).type == TokenType.IDF && lookAhead(2).type == TokenType.LPAREN)
                    return FunctionDecl(null, null);
                else if (Check(TokenType.IDF) && lookAhead(1).type == TokenType.IDF) return VarDecl(null, null);
                return DeclStmt();
            }
            else
            {
                if (Match(TokenType.SYSTEM)) return SystemDecl(accessMod, nonAccessMods);
                if (Match(TokenType.CONTAINER)) return ContainerDecl(accessMod, nonAccessMods);
                if (Match(TokenType.DATATYPE)) return DatatypeDecl(accessMod, nonAccessMods);
                if (Match(TokenType.LIBRARY)) return LibraryDecl(accessMod, nonAccessMods);
                if (Check(TokenType.IDF) && lookAhead(1).type == TokenType.IDF && lookAhead(2).type == TokenType.LPAREN)
                    return FunctionDecl(accessMod, nonAccessMods);
                else if (Check(TokenType.IDF) && lookAhead(1).type == TokenType.IDF) return VarDecl(accessMod, nonAccessMods);
                throw new PenguorException(1, GetCurrent().offset);
            }
        }

        private Decl UsingDecl()
        {
            Expr call = CallExpr();
            Consume(TokenType.SEMICOLON);
            return new UsingDecl(call);
        }

        private Decl SystemDecl(Token? accessMod, Token[]? nonAccessMods) => new SystemDecl(accessMod, nonAccessMods, Consume(TokenType.IDF), GetParent(), BlockStmt());
        private Decl ContainerDecl(Token? accessMod, Token[]? nonAccessMods) => new ContainerDecl(accessMod, nonAccessMods, Consume(TokenType.IDF), GetParent(), BlockStmt());
        private Decl DatatypeDecl(Token? accessMod, Token[]? nonAccessMods) => new DatatypeDecl(accessMod, nonAccessMods, Consume(TokenType.IDF), GetParent(), BlockStmt());
        private Token? GetParent() => Match(TokenType.LESS) ? Consume(TokenType.IDF) : (Token?)null;

        private Decl FunctionDecl(Token? accessMod, Token[]? nonAccessMods)
        {
            Expr variable = VarExpr();

            Consume(TokenType.LPAREN);
            if (Match(TokenType.RPAREN)) return new FunctionDecl(accessMod, nonAccessMods, variable, null);
            List<Expr> parameters = new List<Expr>();
            while (true)
            {
                parameters.Add(VarExpr());
                if (Match(TokenType.COMMA)) continue;
                Consume(TokenType.RPAREN);
                break;
            }
            return new FunctionDecl(accessMod, nonAccessMods, variable, parameters);
        }

        private Decl VarDecl(Token? accessMod, Token[]? nonAccessMods)
        {
            Decl dec = new VarDecl(accessMod, nonAccessMods, VarExpr(), Match(TokenType.ASSIGN) ? Expression() : null);
            Consume(TokenType.SEMICOLON);
            return dec;
        }

        private Decl LibraryDecl(Token? accessMod, Token[]? nonAccessMods) => new LibraryDecl(accessMod, nonAccessMods, CallExpr(), BlockStmt());
        private Decl DeclStmt() => new DeclStmt(Statement());

        private Stmt Statement()
        {
            if (Match(TokenType.LBRACE)) return BlockStmt();
            if (Match(TokenType.IF)) return IfStmt();
            if (Match(TokenType.WHILE)) return WhileStmt();
            if (Match(TokenType.FOR)) return ForStmt();
            if (Match(TokenType.DO)) return DoStmt();
            if (Match(TokenType.SWITCH)) return SwitchStmt();

            return ExprStmt();
        }

        private Stmt BlockStmt()
        {
            List<Decl> declarations = new List<Decl>();
            while (!Match(TokenType.RBRACE)) declarations.Add(Declaration());
            return new BlockStmt(declarations);
        }

        private Stmt IfStmt()
        {
            Consume(TokenType.LPAREN);
            Expr condition = Expression();
            Consume(TokenType.RPAREN);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement());
            }

            return new IfStmt(condition, statements, null);
        }

        private Stmt WhileStmt()
        {
            Consume(TokenType.LPAREN);
            Expr condition = Expression();
            Consume(TokenType.RPAREN);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement());
            }

            return new WhileStmt(condition, statements);
        }

        private Stmt ForStmt()
        {
            Consume(TokenType.LPAREN);
            Stmt currentVar = null;
            Consume(TokenType.COLON);
            Expr vars = VarExpr();
            Consume(TokenType.RPAREN);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE);
            while (!Match(TokenType.RBRACE)) statements.Add(Statement());

            return new ForStmt(currentVar, vars, statements);
        }

        private Stmt DoStmt()
        {
            Consume(TokenType.LBRACE);

            List<Stmt> statements = new List<Stmt>();
            while (!Match(TokenType.RBRACE)) statements.Add(Statement());

            Consume(TokenType.WHILE);
            Consume(TokenType.LPAREN);
            Expr condition = Expression();
            Consume(TokenType.RPAREN);

            return new DoStmt(statements, condition);
        }

        private Stmt SwitchStmt()
        {
            Consume(TokenType.LPAREN);
            Expr condition = VarExpr();
            Consume(TokenType.RPAREN);

            List<Stmt> cases = new List<Stmt>();
            Consume(TokenType.RBRACE);
            while (!Match(TokenType.RBRACE))
            {
                if (Match(TokenType.CASE, TokenType.DEFAULT)) cases.Add(CaseStmt());
            }

            return new SwitchStmt(condition, cases, null);
        }

        private Stmt CaseStmt()
        {
            Expr condition;
            if (GetPrevious().type == TokenType.CASE)
            {
                Consume(TokenType.LPAREN);
                condition = VarExpr();
                Consume(TokenType.RPAREN);
            }
            else condition = null;

            Consume(TokenType.COLON);

            List<Stmt> statements = new List<Stmt>();
            while (!Check(TokenType.CASE) && !Check(TokenType.DEFAULT))
            {
                statements.Add(Statement());
            }

            return new CaseStmt(condition, statements);
        }

        private Stmt ExprStmt()
        {

            Expr expression = Expression();
            Consume(TokenType.SEMICOLON);

            return new ExprStmt(expression);
        }

        private Expr Expression() => AssignExpr();

        private Expr AssignExpr()
        {
            Expr lhs = OrExpr();

            if (Match(TokenType.ASSIGN))
            {
                Expr rhs = AssignExpr();
                return new AssignExpr(lhs, rhs);
            }

            return lhs;
        }

        private Expr OrExpr()
        {
            Expr lhs = AndExpr();

            if (Match(TokenType.OR))
            {
                Expr rhs = AndExpr();
                return new BinaryExpr(lhs, TokenType.OR, rhs);
            }

            return lhs;
        }

        private Expr AndExpr()
        {
            Expr lhs = EqualityExpr();

            if (Match(TokenType.AND))
            {
                Expr rhs = EqualityExpr();
                return new BinaryExpr(lhs, TokenType.AND, rhs);
            }

            return lhs;
        }

        private Expr EqualityExpr()
        {
            Expr lhs = RelationExpr();

            if (Match(TokenType.EQUALS))
            {
                Expr rhs = RelationExpr();
                return new BinaryExpr(lhs, TokenType.EQUALS, rhs);
            }
            else if (Match(TokenType.NEQUALS))
            {
                Expr rhs = RelationExpr();
                return new BinaryExpr(lhs, TokenType.NEQUALS, rhs);
            }

            return lhs;
        }

        private Expr RelationExpr()
        {
            Expr lhs = AdditionExpr();

            if (Match(TokenType.LESS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.LESS, rhs);
            }
            else if (Match(TokenType.LESS_EQUALS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.LESS_EQUALS, rhs);
            }
            else if (Match(TokenType.GREATER_EQUALS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.GREATER_EQUALS, rhs);
            }
            else if (Match(TokenType.GREATER))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.GREATER, rhs);
            }

            return lhs;
        }

        private Expr AdditionExpr()
        {
            Expr lhs = MultiplicationExpr();

            if (Match(TokenType.PLUS))
            {
                Expr rhs = MultiplicationExpr();
                return new BinaryExpr(lhs, TokenType.PLUS, rhs);
            }
            else if (Match(TokenType.MINUS))
            {
                Expr rhs = MultiplicationExpr();
                return new BinaryExpr(lhs, TokenType.MINUS, rhs);
            }

            return lhs;
        }

        private Expr MultiplicationExpr()
        {
            Expr lhs = UnaryExpr();

            if (Match(TokenType.MUL))
            {
                Expr rhs = UnaryExpr();
                return new BinaryExpr(lhs, TokenType.MUL, rhs);
            }
            else if (Match(TokenType.DIV))
            {
                Expr rhs = UnaryExpr();
                return new BinaryExpr(lhs, TokenType.DIV, rhs);
            }

            return lhs;
        }

        private Expr UnaryExpr()
        {
            Expr rhs;
            if (Match(TokenType.EXCL_MARK, TokenType.MINUS))
            {
                TokenType op = GetPrevious().type;

                if (Check(TokenType.LPAREN))
                {
                    rhs = GroupingExpr();
                    return new UnaryExpr(op, rhs);
                }
                rhs = UnaryExpr();
                return new UnaryExpr(op, rhs);
            }
            if (Check(TokenType.LPAREN))
            {
                rhs = GroupingExpr();
                //TODO: fix this mess
                return new UnaryExpr(TokenType.EOF, rhs); //! this will error later on
            }
            return CallExpr();
        }

        private Expr CallExpr()
        {
            Expr expr = BaseExpr();
            List<Expr> arguments = new List<Expr>();

            while (true)
            {
                if (Match(TokenType.LPAREN))
                {
                    if (Match(TokenType.RPAREN)) return new CallExpr(null, arguments, null);
                    while (true)
                    {
                        arguments.Add(Expression());
                        if (!Match(TokenType.RPAREN)) Consume(TokenType.COMMA);
                        else break;
                    }
                }
                else if (Match(TokenType.DOT)) { } else break;
            }

            return new CallExpr(null, arguments, null);
        }

        private Expr BaseExpr()
        {
            if (Match(TokenType.NUM)) return new NumExpr(Convert.ToDouble(GetPrevious().token));
            else if (Match(TokenType.STRING)) return new StringExpr(GetPrevious().token);
            else if (Match(TokenType.TRUE, TokenType.FALSE)) return new BooleanExpr(Convert.ToBoolean(GetPrevious().token));
            else if (Match(TokenType.NULL)) return new NullExpr();
            else if (Match(TokenType.IDF)) return new IdentifierExpr(GetPrevious().type);

            throw new PenguorException(1, GetCurrent().offset);
        }

        private Expr GroupingExpr()
        {
            Consume(TokenType.LPAREN);
            Expr expr = Expression();
            Consume(TokenType.RPAREN);

            return expr;
        }

        private Expr VarExpr()
        {
            return CallExpr();
        }

        /// <summary>
        /// <c>Match() </c> consumes a token if matching
        /// </summary>
        /// <param name="types">TokenTypes to compare to</param>
        /// <returns>true if the current token matches, otherwise false</returns>
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <c>Consume() </c> consumes a token if matching and returns it, otherwise an error is thrown
        /// </summary>
        /// <param name="type">the type to compare the current token to</param>
        /// <returns>the Token of the desired type</returns>
        private Token Consume(TokenType type)
        {
            if (Check(type)) return Advance();
            Debug.CastPGR(6, GetCurrent().offset, TTypePrettyString(type));
            var tmp = Advance();
            return new Token(type, "", tmp.offset, tmp.length);
        }

        /// <summary>
        /// <c>Check() </c> checks the type of the current token.
        /// </summary>
        /// <param name="type">the type to compare the current token with</param>
        /// <param name="n">look up using lookAhead</param>
        /// <returns></returns>
        private bool Check(TokenType type, int n = 0) => lookAhead(n).type == type;

        // TODO: improve xml documentation 
        /// <summary>
        /// <c>Advance() </c> advances to the next token if the eof isn't reached yet.
        /// </summary>
        /// <returns>the token before advancing</returns>
        private Token Advance()
        {
            if (!AtEnd()) current++;
            else return GetCurrent();
            return (GetPrevious());
        }

        /// <summary>
        /// <c>GetNext() </c> returns the previous token.
        /// </summary>
        /// <returns>the previous item in <c>tokens</c></returns>
        private Token GetPrevious() => lookAhead(-1);
        /// <summary>
        /// <c>GetNext() </c> returns the current token without advancing.
        /// </summary>
        /// <returns>the current item in <c>tokens</c></returns>
        private Token GetCurrent() => lookAhead(0);
        /// <summary>
        /// <c>GetNext() </c> returns the next token without advancing.
        /// </summary>
        /// <returns>the next item in <c>tokens</c></returns>
        private Token GetNext() => lookAhead(1);
        /// <summary>
        /// <c>GetNext() </c> returns the next token without advancing.
        /// </summary>
        /// <returns>the nth-next item in <c>tokens</c></returns>
        private Token lookAhead(int n) => tokens[current + n];

        /// <summary>
        /// <c>AtEnd() </c> checks if the parser has reached the end of the file.
        /// </summary>
        /// <returns>true if the parser has reached the end of the file, otherwise false</returns>
        private bool AtEnd()
        {
            if (GetCurrent().type == TokenType.EOF) return true;
            else return false;
        }

        private string TTypePrettyString(TokenType type)
        {
            switch (type)
            {
                case TokenType.HASHTAG:
                    return "#";
                case TokenType.FROM:
                    return "from";
                case TokenType.INCLUDE:
                    return "include";
                case TokenType.SAFETY:
                    return "safety";
                case TokenType.PUBLIC:
                    return "public";
                case TokenType.PRIVATE:
                    return "private";
                case TokenType.PROTECTED:
                    return "protected";
                case TokenType.RESTRICTED:
                    return "restricted";
                case TokenType.STATIC:
                    return "static";
                case TokenType.DYNAMIC:
                    return "dynamic";
                case TokenType.ABSTRACT:
                    return "abstract";
                case TokenType.CONST:
                    return "const";
                case TokenType.LPAREN:
                    return "(";
                case TokenType.RPAREN:
                    return ")";
                case TokenType.LBRACE:
                    return "{";
                case TokenType.RBRACE:
                    return "}";
                case TokenType.LBRACK:
                    return "[";
                case TokenType.RBRACK:
                    return "]";
                case TokenType.PLUS:
                    return "+";
                case TokenType.MINUS:
                    return "-";
                case TokenType.MUL:
                    return "*";
                case TokenType.DIV:
                    return "/";
                case TokenType.PERCENT:
                    return "%";
                case TokenType.DPLUS:
                    return "++";
                case TokenType.DMINUS:
                    return "--";
                case TokenType.GREATER:
                    return ">";
                case TokenType.LESS:
                    return "<";
                case TokenType.GREATER_EQUALS:
                    return ">=";
                case TokenType.LESS_EQUALS:
                    return "<=";
                case TokenType.EQUALS:
                    return "==";
                case TokenType.NEQUALS:
                    return "!=";
                case TokenType.AND:
                    return "&&";
                case TokenType.OR:
                    return "||";
                case TokenType.XOR:
                    return "^^";
                case TokenType.BW_AND:
                    return "&";
                case TokenType.BW_OR:
                    return "|";
                case TokenType.BW_XOR:
                    return "^";
                case TokenType.BW_NOT:
                    return "~";
                case TokenType.BS_LEFT:
                    return "<<";
                case TokenType.BS_RIGHT:
                    return ">>";
                case TokenType.ASSIGN:
                    return "=";
                case TokenType.ADD_ASSIGN:
                    return "+=";
                case TokenType.SUB_ASSIGN:
                    return "-=";
                case TokenType.MUL_ASSIGN:
                    return "*=";
                case TokenType.DIV_ASSIGN:
                    return "/=";
                case TokenType.PERCENT_ASSIGN:
                    return "%=";
                case TokenType.BW_AND_ASSIGN:
                    return "&=";
                case TokenType.BW_OR_ASSIGN:
                    return "|=";
                case TokenType.BW_XOR_ASSIGN:
                    return "^=";
                case TokenType.BS_LEFT_ASSIGN:
                    return "<<=";
                case TokenType.BS_RIGHT_ASSIGN:
                    return ">>=";
                case TokenType.NULL:
                    return "null";
                case TokenType.COLON:
                    return ":";
                case TokenType.SEMICOLON:
                    return ";";
                case TokenType.DOT:
                    return ".";
                case TokenType.COMMA:
                    return ",";
                case TokenType.EXCL_MARK:
                    return "!";
                case TokenType.NUM:
                    return "number";
                case TokenType.STRING:
                    return "string";
                case TokenType.IDF:
                    return "identifier";
                case TokenType.TRUE:
                    return "true";
                case TokenType.FALSE:
                    return "false";
                case TokenType.CONTAINER:
                    return "container";
                case TokenType.SYSTEM:
                    return "system";
                case TokenType.DATATYPE:
                    return "datatype";
                case TokenType.LIBRARY:
                    return "library";
                case TokenType.IF:
                    return "if";
                case TokenType.ELIF:
                    return "elif";
                case TokenType.ELSE:
                    return "else";
                case TokenType.FOR:
                    return "for";
                case TokenType.WHILE:
                    return "while";
                case TokenType.DO:
                    return "do";
                case TokenType.SWITCH:
                    return "switch";
                case TokenType.CASE:
                    return "case";
                case TokenType.DEFAULT:
                    return "default";
                case TokenType.EOF:
                    return "end of file";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
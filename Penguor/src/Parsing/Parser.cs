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

using Penguor.Debugging;
using System.Collections.Generic;
using Penguor.Parsing.AST;
using System;
using id = System.Collections.Generic.KeyValuePair<System.Guid, string>;

namespace Penguor.Parsing
{
    /// <summary>
    /// The Penguor parser
    /// </summary>
    public class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        private List<id> idList;

        /// <summary>
        /// create a new parser with the tokens that should be parsed
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;

            idList = new List<id>();
        }

        /// <summary>
        /// start the Parsing process
        /// </summary>
        /// <returns>a statement containing the AST of the parsed file</returns>
        public Stmt Parse()
        {
            Consume(TokenType.HASHTAG, 1);
            string library;
            Stmt head = HeadStmt(out library);

            List<Stmt> declarations = new List<Stmt>();

            while (!Match(TokenType.EOF)) declarations.Add(Declaration());

            return new ProgramStmt(head, declarations, null);
        }

        // todo: implement form ... include ...
        /// <summary>
        /// The <c>HeadStmt() </c> method parses the file header
        /// </summary>
        /// <returns>An AST of the program head</returns>
        private Stmt HeadStmt(out string library)
        {
            Dictionary<string, string> definition = new Dictionary<string, string>();

            while (!Check(TokenType.INCLUDE))
            {
                Token lhs = Consume(TokenType.IDF, 1);
                Consume(TokenType.COLON, 1);
                Token rhs = Consume(TokenType.IDF, 1);
                Consume(TokenType.SEMICOLON, 1);
                definition.Add(lhs.token, rhs.token);
            }

            List<Expr> includeLhs = new List<Expr>();
            // List<Expr> includeRhs = new List<Expr>();

            definition.TryGetValue("library", out library);

            while (!Match(TokenType.HASHTAG))
            {
                Consume(TokenType.INCLUDE, 1);
                includeLhs.Add(CallExpr());
                Consume(TokenType.SEMICOLON, 1);
            }

            return new HeadStmt(definition, includeLhs, null);
        }

        private Stmt Declaration()
        {
            if (Match(TokenType.SYSTEM)) return SystemStmt();
            if (Match(TokenType.CONTAINER)) return ComponentStmt();
            if (Match(TokenType.DATATYPE)) return DataTypeStmt();
            if (Match(TokenType.IDF)) return VarStmt();
            if (Match(TokenType.IDF)) return FunctionStmt();
            return Statement();
        }

        private Stmt SystemStmt()
        {
            Token name = Consume(TokenType.IDF, 6);

            Token parentSystem = new Token();
            if (Match(TokenType.LESS)) parentSystem = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt();
            else block = null;

            return new SystemStmt(name, parentSystem, block, null);
        }

        private Stmt ComponentStmt()
        {
            Token name = Consume(TokenType.IDF, 6);


            Token parentComponent = new Token();
            if (Match(TokenType.LESS)) parentComponent = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt();
            else block = null;

            return new ComponentStmt(name, parentComponent, block, null);
        }

        private Stmt DataTypeStmt()
        {
            Token name = Consume(TokenType.IDF, 6);

            Token parentType = new Token();
            if (Match(TokenType.LESS)) parentType = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt();
            else block = null;

            return new DataTypeStmt(name, parentType, block, null);
        }

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
            List<Stmt> declarations = new List<Stmt>();

            while (!Match(TokenType.RBRACE))
            {
                declarations.Add(Declaration());
            }

            return new BlockStmt(declarations, null);
        }

        private Stmt IfStmt()
        {
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement());
            }

            return new IfStmt(condition, statements, null);
        }

        private Stmt WhileStmt()
        {
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement());
            }

            return new WhileStmt(condition, statements, null);
        }

        private Stmt ForStmt()
        {
            Consume(TokenType.LPAREN, 1);
            Stmt currentVar = VarStmt();
            Consume(TokenType.COLON, 1);
            Expr vars = VariableExpr();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE)) statements.Add(Statement());

            return new ForStmt(currentVar, vars, statements, null);
        }

        private Stmt DoStmt()
        {
            Consume(TokenType.LBRACE, 1);

            List<Stmt> statements = new List<Stmt>();
            while (!Match(TokenType.RBRACE)) statements.Add(Statement());

            Consume(TokenType.WHILE, 1);
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            return new DoStmt(statements, condition, null);
        }

        private Stmt SwitchStmt()
        {
            Consume(TokenType.LPAREN, 1);
            Expr condition = VariableExpr();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> cases = new List<Stmt>();
            Consume(TokenType.RBRACE, 1);
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
                Consume(TokenType.LPAREN, 1);
                condition = VariableExpr();
                Consume(TokenType.RPAREN, 1);
            }
            else condition = null;

            Consume(TokenType.COLON, 1);

            List<Stmt> statements = new List<Stmt>();
            while (!Check(TokenType.CASE) && !Check(TokenType.DEFAULT))
            {
                statements.Add(Statement());
            }

            return new CaseStmt(condition, statements, null);
        }

        private Stmt ExprStmt()
        {

            Expr expression = Expression();
            Consume(TokenType.SEMICOLON, 7);

            return new ExprStmt(expression, null);
        }

        private Expr Expression() => AssignExpr();

        private Expr AssignExpr()
        {
            Expr lhs = OrExpr();

            if (Match(TokenType.ASSIGN))
            {
                Expr rhs = AssignExpr();
                return new AssignExpr(lhs, rhs, null);
            }

            return lhs;
        }

        private Expr OrExpr()
        {
            Expr lhs = AndExpr();

            if (Match(TokenType.OR))
            {
                Expr rhs = AndExpr();
                return new BinaryExpr(lhs, TokenType.OR, rhs, null);
            }

            return lhs;
        }

        private Expr AndExpr()
        {
            Expr lhs = EqualityExpr();

            if (Match(TokenType.AND))
            {
                Expr rhs = EqualityExpr();
                return new BinaryExpr(lhs, TokenType.AND, rhs, null);
            }

            return lhs;
        }

        private Expr EqualityExpr()
        {
            Expr lhs = RelationExpr();

            if (Match(TokenType.EQUALS))
            {
                Expr rhs = RelationExpr();
                return new BinaryExpr(lhs, TokenType.EQUALS, rhs, null);
            }
            else if (Match(TokenType.NEQUALS))
            {
                Expr rhs = RelationExpr();
                return new BinaryExpr(lhs, TokenType.NEQUALS, rhs, null);
            }

            return lhs;
        }

        private Expr RelationExpr()
        {
            Expr lhs = AdditionExpr();

            if (Match(TokenType.LESS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.LESS, rhs, null);
            }
            else if (Match(TokenType.LESS_EQUALS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.LESS_EQUALS, rhs, null);
            }
            else if (Match(TokenType.GREATER_EQUALS))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.GREATER_EQUALS, rhs, null);
            }
            else if (Match(TokenType.GREATER))
            {
                Expr rhs = AdditionExpr();
                return new BinaryExpr(lhs, TokenType.GREATER, rhs, null);
            }

            return lhs;
        }

        private Expr AdditionExpr()
        {
            Expr lhs = MultiplicationExpr();

            if (Match(TokenType.PLUS))
            {
                Expr rhs = MultiplicationExpr();
                return new BinaryExpr(lhs, TokenType.PLUS, rhs, null);
            }
            else if (Match(TokenType.MINUS))
            {
                Expr rhs = MultiplicationExpr();
                return new BinaryExpr(lhs, TokenType.MINUS, rhs, null);
            }

            return lhs;
        }

        private Expr MultiplicationExpr()
        {
            Expr lhs = UnaryExpr();

            if (Match(TokenType.MUL))
            {
                Expr rhs = UnaryExpr();
                return new BinaryExpr(lhs, TokenType.MUL, rhs, null);
            }
            else if (Match(TokenType.DIV))
            {
                Expr rhs = UnaryExpr();
                return new BinaryExpr(lhs, TokenType.DIV, rhs, null);
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
                    return new UnaryExpr(op, rhs, null);
                }
                rhs = UnaryExpr();
                return new UnaryExpr(op, rhs, null);
            }
            if (Check(TokenType.LPAREN))
            {
                rhs = GroupingExpr();
                //TODO: fix this mess
                return new UnaryExpr(TokenType.EOF, rhs, null); //! this will error later on
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
                    if (Match(TokenType.RPAREN)) return new CallExpr(expr, arguments, null);
                    while (true)
                    {
                        arguments.Add(Expression());
                        if (!Match(TokenType.RPAREN)) Consume(TokenType.COMMA, 1);
                        else break;
                    }
                }
                else if (Match(TokenType.DOT))
                {
                }
                else break;
            }

            return new CallExpr(expr, arguments, null);
        }

        private Expr BaseExpr()
        {
            if (Match(TokenType.NUM)) return new NumExpr(Convert.ToDouble(GetPrevious().token), null);
            else if (Match(TokenType.STRING)) return new StringExpr(GetPrevious().token, null);
            else if (Match(TokenType.TRUE, TokenType.FALSE)) return new BooleanExpr(Convert.ToBoolean(GetPrevious().token), null);
            else if (Match(TokenType.NULL)) return new NullExpr(null);
            else if (Match(TokenType.IDF)) return new IdentifierExpr(GetPrevious().type, null);

            throw new PenguorException(1, GetCurrent().offset);
        }

        private Expr GroupingExpr()
        {
            Consume(TokenType.LPAREN, 1);
            Expr expr = Expression();
            Consume(TokenType.RPAREN, 1);

            return expr;
        }

        private Expr VariableExpr()
        {
            return CallExpr();
        }

        private Stmt VarStmt()
        {
            Token type = Consume(TokenType.IDF, 7);
            Token name = Consume(TokenType.IDF, 7);

            Expr rhs = null;
            if (Match(TokenType.ASSIGN)) rhs = Expression();
            Consume(TokenType.SEMICOLON, 1);

            return new VarStmt(type, name, rhs, null);
        }

        private Stmt FunctionStmt()
        {
            List<Expr> arguments = new List<Expr>();
            Token returns = Consume(TokenType.IDF, 1);
            Token name = Consume(TokenType.IDF, 1);

            Consume(TokenType.LPAREN, 1);
            List<Stmt> parameters = new List<Stmt>();
            if (Match(TokenType.RPAREN)) return new FunctionStmt(returns, name, parameters, null);
            while (true)
            {
                parameters.Add(VarStmt());
                if (!Match(TokenType.RPAREN)) Consume(TokenType.COMMA, 1);
                else break;
            }
            return new FunctionStmt(returns, name, parameters, null);
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
        /// <param name="msgNumber">the debug message to cast if token doesn't match</param>
        /// <returns></returns>
        private Token Consume(TokenType type, ushort msgNumber)
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
        /// <returns></returns>
        private bool Check(TokenType type) => GetCurrent().type == type;

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
        private Token GetPrevious() => tokens[current - 1];
        /// <summary>
        /// <c>GetNext() </c> returns the current token without advancing.
        /// </summary>
        /// <returns>the current item in <c>tokens</c></returns>
        private Token GetCurrent() => tokens[current];
        /// <summary>
        /// <c>GetNext() </c> returns the next token without advancing.
        /// </summary>
        /// <returns>the next item in <c>tokens</c></returns>
        private Token GetNext() => tokens[current + 1];

        /// <summary>
        /// <c>AtEnd() </c> checks if the parser has reached the end of the file.
        /// </summary>
        /// <returns>true if the parser has reached the end of the file, otherwise false</returns>
        private bool AtEnd()
        {
            if (tokens[current].type == TokenType.EOF) return true;
            else return false;
        }

        private string TTypePrettyString(TokenType type)
        {
            switch (type)
            {
                case TokenType.HASHTAG: return "#";
                case TokenType.FROM: return "from";
                case TokenType.INCLUDE: return "include";
                case TokenType.SAFETY: return "safety";
                case TokenType.PUBLIC: return "public";
                case TokenType.PRIVATE: return "private";
                case TokenType.PROTECTED: return "protected";
                case TokenType.RESTRICTED: return "restricted";
                case TokenType.STATIC: return "static";
                case TokenType.DYNAMIC: return "dynamic";
                case TokenType.ABSTRACT: return "abstract";
                case TokenType.CONST: return "const";
                case TokenType.LPAREN: return "(";
                case TokenType.RPAREN: return ")";
                case TokenType.LBRACE: return "{";
                case TokenType.RBRACE: return "}";
                case TokenType.LBRACK: return "[";
                case TokenType.RBRACK: return "]";
                case TokenType.PLUS: return "+";
                case TokenType.MINUS: return "-";
                case TokenType.MUL: return "*";
                case TokenType.DIV: return "/";
                case TokenType.PERCENT: return "%";
                case TokenType.DPLUS: return "++";
                case TokenType.DMINUS: return "--";
                case TokenType.GREATER: return ">";
                case TokenType.LESS: return "<";
                case TokenType.GREATER_EQUALS: return ">=";
                case TokenType.LESS_EQUALS: return "<=";
                case TokenType.EQUALS: return "==";
                case TokenType.NEQUALS: return "!=";
                case TokenType.AND: return "&&";
                case TokenType.OR: return "||";
                case TokenType.XOR: return "^^";
                case TokenType.BW_AND: return "&";
                case TokenType.BW_OR: return "|";
                case TokenType.BW_XOR: return "^";
                case TokenType.BW_NOT: return "~";
                case TokenType.BS_LEFT: return "<<";
                case TokenType.BS_RIGHT: return ">>";
                case TokenType.ASSIGN: return "=";
                case TokenType.ADD_ASSIGN: return "+=";
                case TokenType.SUB_ASSIGN: return "-=";
                case TokenType.MUL_ASSIGN: return "*=";
                case TokenType.DIV_ASSIGN: return "/=";
                case TokenType.PERCENT_ASSIGN: return "%=";
                case TokenType.BW_AND_ASSIGN: return "&=";
                case TokenType.BW_OR_ASSIGN: return "|=";
                case TokenType.BW_XOR_ASSIGN: return "^=";
                case TokenType.BS_LEFT_ASSIGN: return "<<=";
                case TokenType.BS_RIGHT_ASSIGN: return ">>=";
                case TokenType.NULL: return "null";
                case TokenType.COLON: return ":";
                case TokenType.SEMICOLON: return ";";
                case TokenType.DOT: return ".";
                case TokenType.COMMA: return ",";
                case TokenType.EXCL_MARK: return "!";
                case TokenType.NUM: return "number";
                case TokenType.STRING: return "string";
                case TokenType.IDF: return "identifier";
                case TokenType.TRUE: return "true";
                case TokenType.FALSE: return "false";
                case TokenType.CONTAINER: return "container";
                case TokenType.SYSTEM: return "system";
                case TokenType.DATATYPE: return "datatype";
                case TokenType.LIBRARY: return "library";
                case TokenType.IF: return "if";
                case TokenType.ELIF: return "elif";
                case TokenType.ELSE: return "else";
                case TokenType.FOR: return "for";
                case TokenType.WHILE: return "while";
                case TokenType.DO: return "do";
                case TokenType.SWITCH: return "switch";
                case TokenType.CASE: return "case";
                case TokenType.DEFAULT: return "default";
                case TokenType.EOF: return "end of file";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
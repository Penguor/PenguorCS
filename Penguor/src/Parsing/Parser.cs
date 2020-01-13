/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
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
            Consume(TokenType.HEADSTART, 1);
            string library;
            Stmt head = HeadStmt(out library);

            LinkedList<Guid> parent = new LinkedList<Guid>();
            parent.AddLast(new LinkedListNode<Guid>(AddId(library)));

            List<Stmt> declarations = new List<Stmt>();

            while (!Match(TokenType.EOF)) declarations.Add(Declaration(parent));

            return new ProgramStmt(head, declarations, parent);
        }

        // todo: implement form ... include ...
        /// <summary>
        /// The <c>HeadStmt() </c> method parses the file header
        /// </summary>
        /// <returns>An AST of the program head</returns>
        private Stmt HeadStmt(out string library)
        {
            LinkedList<Guid> parent = new LinkedList<Guid>();
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

            while (!Match(TokenType.HEADEND))
            {
                Consume(TokenType.INCLUDE, 1);
                includeLhs.Add(CallExpr());
                Consume(TokenType.SEMICOLON, 1);
            }

            return new HeadStmt(definition, includeLhs, parent);
        }

        private Stmt Declaration(LinkedList<Guid> parent)
        {
            if (Match(TokenType.SYSTEM)) return SystemStmt(parent);
            if (Match(TokenType.COMPONENT)) return ComponentStmt(parent);
            if (Match(TokenType.DATATYPE)) return DataTypeStmt(parent);
            if (Match(TokenType.VAR)) return VarStmt(parent);
            if (Match(TokenType.FN)) return FunctionStmt(parent);
            return Statement(parent);
        }

        private Stmt SystemStmt(LinkedList<Guid> parent)
        {
            Token name = Consume(TokenType.IDF, 6);
            parent.AddLast(new LinkedListNode<Guid>(AddId(name.token)));

            Token parentSystem = new Token();
            if (Match(TokenType.LESS)) parentSystem = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt(parent);
            else block = null;

            return new SystemStmt(name, parentSystem, block, parent);
        }

        private Stmt ComponentStmt(LinkedList<Guid> parent)
        {
            Token name = Consume(TokenType.IDF, 6);
            parent.AddLast(new LinkedListNode<Guid>(AddId(name.token)));

            Token parentComponent = new Token();
            if (Match(TokenType.LESS)) parentComponent = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt(parent);
            else block = null;

            return new ComponentStmt(name, parentComponent, block, parent);
        }

        private Stmt DataTypeStmt(LinkedList<Guid> parent)
        {
            Token name = Consume(TokenType.IDF, 6);
            parent.AddLast(new LinkedListNode<Guid>(AddId(name.token)));

            Token parentType = new Token();
            if (Match(TokenType.LESS)) parentType = Consume(TokenType.IDF, 1);

            Stmt block;
            if (Match(TokenType.LBRACE)) block = BlockStmt(parent);
            else block = null;

            return new DataTypeStmt(name, parentType, block, parent);
        }

        private Stmt Statement(LinkedList<Guid> parent)
        {
            if (Match(TokenType.LBRACE)) return BlockStmt(parent);
            if (Match(TokenType.IF)) return IfStmt(parent);
            if (Match(TokenType.WHILE)) return WhileStmt(parent);
            if (Match(TokenType.FOR)) return ForStmt(parent);
            if (Match(TokenType.DO)) return DoStmt(parent);
            if (Match(TokenType.SWITCH)) return SwitchStmt(parent);

            return ExprStmt();
        }

        private Stmt BlockStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            List<Stmt> declarations = new List<Stmt>();

            while (!Match(TokenType.RBRACE))
            {
                declarations.Add(Declaration(parent));
            }

            return new BlockStmt(declarations, parent);
        }

        private Stmt IfStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement(parent));
            }

            return new IfStmt(condition, statements, parent);
        }

        private Stmt WhileStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE))
            {
                statements.Add(Statement(parent));
            }

            return new WhileStmt(condition, statements, parent);
        }

        private Stmt ForStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Consume(TokenType.LPAREN, 1);
            Stmt currentVar = VarStmt();
            Consume(TokenType.COLON, 1);
            Expr vars = VariableExpr();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> statements = new List<Stmt>();
            Consume(TokenType.LBRACE, 1);
            while (!Match(TokenType.RBRACE)) statements.Add(Statement(parent));

            return new ForStmt(currentVar, vars, statements, parent);
        }

        private Stmt DoStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Consume(TokenType.LBRACE, 1);

            List<Stmt> statements = new List<Stmt>();
            while (!Match(TokenType.RBRACE)) statements.Add(Statement(parent));

            Consume(TokenType.WHILE, 1);
            Consume(TokenType.LPAREN, 1);
            Expr condition = Expression();
            Consume(TokenType.RPAREN, 1);

            return new DoStmt(statements, condition, parent);
        }

        private Stmt SwitchStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Consume(TokenType.LPAREN, 1);
            Expr condition = VariableExpr();
            Consume(TokenType.RPAREN, 1);

            List<Stmt> cases = new List<Stmt>();
            Consume(TokenType.RBRACE, 1);
            while (!Match(TokenType.RBRACE))
            {
                if (Match(TokenType.CASE, TokenType.DEFAULT)) cases.Add(CaseStmt(parent));
            }

            return new SwitchStmt(condition, cases, parent);
        }

        private Stmt CaseStmt(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
            Expr condition;
            if (GetPrevious().type == TokenType.CASE)
            {
                Consume(TokenType.LPAREN, 1);
                condition = VariableExpr(parent);
                Consume(TokenType.RPAREN, 1);
            }
            else condition = null;

            Consume(TokenType.COLON, 1);

            List<Stmt> statements = new List<Stmt>();
            while (!Check(TokenType.CASE) && !Check(TokenType.DEFAULT))
            {
                statements.Add(Statement(parent));
            }

            return new CaseStmt(condition, statements, parent);
        }

        private Stmt ExprStmt(LinkedList<Guid> parent)
        {
            Expr expression = Expression(parent);
            Consume(TokenType.SEMICOLON, 7, ';');

            return new ExprStmt(expression, parent);
        }

        private Expr Expression(LinkedList<Guid> parent) => AssignExpr(parent);

        private Expr AssignExpr(LinkedList<Guid> parent)
        {
            parent.AddLast(new LinkedListNode<Guid>(AddId()));
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
            if (Match(TokenType.EXCL_MARK, TokenType.MINUS))
            {
                TokenType op = GetPrevious().type;

                Expr rhs;
                if (Check(TokenType.LPAREN))
                {
                    rhs = GroupingExpr();
                    return new UnaryExpr(op, rhs);
                }
                rhs = UnaryExpr();
                return new UnaryExpr(op, rhs);
            }

            return CallExpr();
        }

        // TODO: Implement variable ids for type storing
        private Expr CallExpr()
        {
            Expr expr = BaseExpr();

            while (true)
            {
                if (Match(TokenType.LPAREN))
                {
                    List<Expr> arguments = new List<Expr>();
                    if (Match(TokenType.RPAREN)) return new CallExpr(expr, arguments);
                    while (true)
                    {
                        arguments.Add(Expression());
                        if (!Match(TokenType.RPAREN)) Consume(TokenType.COMMA, 1);
                        else break;
                    }
                }
                else if (Match(TokenType.DOT))
                {
                    Consume(TokenType.IDF, 1);
                }
                else break;
            }

            return expr;
        }

        private Expr BaseExpr()
        {
            if (Match(TokenType.NUM)) return new NumExpr(Convert.ToDouble(GetPrevious().token));
            else if (Match(TokenType.STRING)) return new StringExpr(GetPrevious().token);
            else if (Match(TokenType.TRUE, TokenType.FALSE)) return new BooleanExpr(Convert.ToBoolean(GetPrevious().token));
            else if (Match(TokenType.NULL)) return new NullExpr();
            else if (Match(TokenType.IDF)) return new IdentifierExpr(GetPrevious().type);

            throw new PenguorException(1, GetCurrent().lineNumber);
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

            return new VarStmt(type, name, rhs);
        }

        private Stmt FunctionStmt()
        {
            Token returns = Consume(TokenType.IDF, 1);
            Token name = Consume(TokenType.IDF, 1);

            Consume(TokenType.LPAREN, 1);
            List<Stmt> parameters = new List<Stmt>();
            if (Match(TokenType.RPAREN)) return new FunctionStmt(returns, name, parameters);
            while (true)
            {
                parameters.Add(VarStmt());
                if (!Match(TokenType.RPAREN)) Consume(TokenType.COMMA, 1);
                else break;
            }
            return new FunctionStmt(returns, name, parameters);
        }


        private Guid AddId(string name = "")
        {
        Start:
            Guid newId = new Guid();
            foreach (id item in idList)
            {
                if (item.Value == name && name != "") return item.Key;
                if (item.Key == newId) goto Start;
            }
            idList.Add(new id(newId, name));

            return newId;
        }

        private Guid GetId(string name)
        {
            foreach (id item in idList)
            {
                if (item.Value == name) return item.Key;
            }
            throw new PenguorException(1, GetCurrent().lineNumber); // TODO: add own exception for "idf not found"
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
        /// <param name="expected">the expected char for error message</param>
        /// <returns></returns>
        private Token Consume(TokenType type, ushort msgNumber, char expected = ' ')
        {
            if (Check(type)) return Advance();
            throw new PenguorException(msgNumber, GetCurrent().lineNumber, expected);
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
    }
}
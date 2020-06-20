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
using Penguor.Debugging;
using Penguor.Compiler.Parsing.AST;
using Penguor.Compiler.Build;
using static Penguor.Compiler.Parsing.TokenType;

namespace Penguor.Compiler.Parsing
{
    /// <summary>
    /// The Penguor parser
    /// </summary>
    public class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        private Builder builder;


        /// <summary>
        /// create a new parser with the tokens that should be parsed
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        /// <param name="builder">the builder which executed the parser</param>
        public Parser(List<Token> tokens, Builder builder)
        {
            this.builder = builder;
            this.tokens = tokens;
        }

        /// <summary>
        /// start the Parsing process
        /// </summary> 
        /// <returns>a statement containing the AST of the parsed file</returns>
        public Decl Parse()
        {
            List<Decl> statements = new List<Decl>();
            while (!Match(EOF)) statements.Add(Declaration());
            return new ProgramDecl(statements);
        }

        private Decl Declaration()
        {
            TokenType? accessMod = null;
            bool hasAccessMod = false;
            TokenType[] nonAccessMods = new TokenType[3];

            if (Match(PUBLIC, PRIVATE, PROTECTED, RESTRICTED))
            {
                accessMod = GetPrevious().type;
                hasAccessMod = true;
            }
            if (Check(STATIC) != Check(DYNAMIC))
            {
                nonAccessMods[0] = Advance().type;
                hasAccessMod = true;
            }
            if (Check(ABSTRACT))
            {
                nonAccessMods[1] = Advance().type;
                hasAccessMod = true;
            }
            if (Check(CONST))
            {
                nonAccessMods[2] = Advance().type;
                hasAccessMod = true;
            }

            if (!hasAccessMod)
            {
                if (Match(USING)) return UsingDecl();
                if (Match(SYSTEM)) return SystemDecl(null, new TokenType[0]);
                if (Match(CONTAINER)) return ContainerDecl(null, new TokenType[0]);
                if (Match(DATATYPE)) return DatatypeDecl(null, new TokenType[0]);
                if (Match(LIBRARY)) return LibraryDecl(null, new TokenType[0]);
                if (Check(IDF) && lookAhead(1).type == IDF && lookAhead(2).type == LPAREN)
                    return FunctionDecl(null, new TokenType[0]);
                else if (Check(IDF) && lookAhead(1).type == IDF) return VarDecl(null, new TokenType[0]);
                return DeclStmt();
            }
            else
            {
                if (Match(SYSTEM)) return SystemDecl(accessMod, nonAccessMods);
                if (Match(CONTAINER)) return ContainerDecl(accessMod, nonAccessMods);
                if (Match(DATATYPE)) return DatatypeDecl(accessMod, nonAccessMods);
                if (Match(LIBRARY)) return LibraryDecl(accessMod, nonAccessMods);
                if (Check(IDF) && lookAhead(1).type == IDF && lookAhead(2).type == LPAREN)
                    return FunctionDecl(accessMod, nonAccessMods);
                else if (Check(IDF) && lookAhead(1).type == IDF) return VarDecl(accessMod, nonAccessMods);
                throw new PenguorException(1, GetCurrent().offset);
            }
        }

        private Decl UsingDecl()
        {
            Expr call = CallExpr();
            Consume(SEMICOLON);
            return new UsingDecl(call);
        }

        private Decl SystemDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new SystemDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Decl ContainerDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new ContainerDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Decl DatatypeDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new DatatypeDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Token? GetParent() => Match(LESS) ? Consume(IDF) : (Token?)null;

        private Decl FunctionDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            Expr variable = VarExpr();

            Consume(LPAREN);
            if (Match(RPAREN)) return new FunctionDecl(accessMod, nonAccessMods, variable, new List<Expr>(), BlockDecl());
            List<Expr> parameters = new List<Expr>();
            while (true)
            {
                parameters.Add(VarExpr());
                if (Match(COMMA)) continue;
                Consume(RPAREN);
                break;
            }
            return new FunctionDecl(accessMod, nonAccessMods, variable, parameters, BlockDecl());
        }

        private Decl VarDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            Decl dec = new VarDecl(accessMod, nonAccessMods, VarExpr(), Match(ASSIGN) ? Expression() : null);
            Consume(SEMICOLON);
            return dec;
        }

        private Decl LibraryDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new LibraryDecl(accessMod, nonAccessMods, CallExpr(), BlockDecl());

        private Decl BlockDecl()
        {
            Consume(LBRACE);
            List<Decl> declarations = new List<Decl>();
            while (!Match(RBRACE)) declarations.Add(Declaration());

            return new BlockDecl(declarations);
        }

        private Decl DeclStmt() => new DeclStmt(Statement());

        private Stmt Statement()
        {
            if (Match(HASHTAG)) return CompilerStmt();
            if (Check(LBRACE)) return BlockStmt();
            if (Match(IF)) return IfStmt();
            if (Match(WHILE)) return WhileStmt();
            if (Match(FOR)) return ForStmt();
            if (Match(DO)) return DoStmt();
            if (Match(SWITCH)) return SwitchStmt();
            if (Match(RETURN)) return ReturnStmt();
            return ExprStmt();
        }

        private Stmt CompilerStmt()
        {
            Token[] val;
            Token dir = Advance();
            switch (dir.type)
            {
                case SAFETY:
                    val = new Token[1];
                    val[0] = Consume(NUM);
                    if (Convert.ToInt32(val[0].token) < 0 || Convert.ToInt32(val[0].token) > 2) builder.Exception(1, val[0].offset);
                    break;
                default:
                    val = new Token[0];
                    builder.Exception(8, dir.offset, (dir.type == IDF || dir.type == NUM) ? dir.token : TTypePrettyString(dir.type));
                    break;
            }
            Consume(SEMICOLON);
            return new PPStmt(dir.type, val);
        }

        private Stmt BlockStmt()
        {
            Match(LBRACE);
            List<Stmt> statements = new List<Stmt>();
            while (!Match(RBRACE)) statements.Add(Statement());

            return new BlockStmt(statements);
        }

        private Stmt IfStmt()
        {
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            Stmt ifC = Statement();

            List<Stmt> elif = new List<Stmt>();
            while (Match(ELIF)) elif.Add(ElifStmt());

            Stmt? elseC = null;
            if (Match(ELSE)) elseC = Statement();

            return new IfStmt(condition, ifC, elif, elseC);
        }

        private Stmt ElifStmt()
        {
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new ElifStmt(condition, Statement());
        }

        private Stmt WhileStmt()
        {
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new WhileStmt(condition, Statement());
        }

        private Stmt ForStmt()
        {
            Consume(LPAREN);
            Expr current = VarExpr();
            Consume(COLON);
            Expr vars = CallExpr();
            Consume(RPAREN);

            return new ForStmt(current, vars, Statement());
        }

        private Stmt DoStmt()
        {
            Stmt content = Statement();

            Consume(WHILE);
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new DoStmt(content, condition);
        }

        private Stmt SwitchStmt()
        {
            Consume(LPAREN);
            Expr condition = CallExpr();
            Consume(RPAREN);

            Consume(LBRACE);
            List<Stmt> cases = new List<Stmt>();
            do
            {
                Match(CASE);
                cases.Add(CaseStmt());
            } while (!Match(RBRACE) && !Check(DEFAULT));

            Stmt? defaultCase = null;
            if (Match(DEFAULT)) defaultCase = CaseStmt();

            return new SwitchStmt(condition, cases, defaultCase);
        }

        private Stmt CaseStmt()
        {
            Expr? condition;
            if (GetPrevious().type == CASE)
            {
                Consume(LPAREN);
                condition = VarExpr();
                Consume(RPAREN);
            }
            else condition = null;

            Consume(COLON);

            List<Stmt> statements = new List<Stmt>();
            while (!Check(CASE) && !Check(DEFAULT) && Check(RBRACE))
            {
                statements.Add(Statement());
            }

            return new CaseStmt(condition, statements);
        }

        private Stmt ReturnStmt()
        {
            if (Match(SEMICOLON)) return new ReturnStmt(null);
            else
            {
                Expr expr = Expression();
                Consume(SEMICOLON);
                return new ReturnStmt(expr);
            }
        }

        private Stmt ExprStmt()
        {

            Expr expression = Expression();
            Consume(SEMICOLON);

            return new ExprStmt(expression);
        }

        private Expr Expression() => AssignExpr();

        private Expr AssignExpr()
        {
            Expr lhs = CondOrExpr();
            if (Match(ASSIGN,
                      ADD_ASSIGN,
                      MUL_ASSIGN,
                      DIV_ASSIGN,
                      PERCENT_ASSIGN,
                      BS_LEFT_ASSIGN,
                      BS_RIGHT_ASSIGN,
                      BW_AND_ASSIGN,
                      BW_OR_ASSIGN,
                      BW_XOR_ASSIGN)) return new AssignExpr(lhs, GetPrevious().type, CondOrExpr());

            return lhs;
        }

        private Expr CondOrExpr() => Check(OR, 1) ? new BinaryExpr(CondXorExpr(), Consume(OR).type, CondOrExpr()) : CondXorExpr();
        private Expr CondXorExpr() => Check(XOR, 1) ? new BinaryExpr(CondAndExpr(), Consume(XOR).type, CondXorExpr()) : CondAndExpr();
        private Expr CondAndExpr() => Check(AND, 1) ? new BinaryExpr(BWOrExpr(), Consume(AND).type, CondAndExpr()) : BWOrExpr();

        private Expr BWOrExpr() => Check(BW_OR, 1) ? new BinaryExpr(BWXorExpr(), Consume(BW_OR).type, BWOrExpr()) : BWXorExpr();
        private Expr BWXorExpr() => Check(BW_XOR, 1) ? new BinaryExpr(BWAndExpr(), Consume(BW_XOR).type, BWXorExpr()) : BWAndExpr();
        private Expr BWAndExpr() => Check(BW_AND, 1) ? new BinaryExpr(EqualityExpr(), Consume(BW_AND).type, BWAndExpr()) : EqualityExpr();

        private Expr EqualityExpr()
        {
            Expr lhs = RelationExpr();
            if (Match(EQUALS, NEQUALS)) return new BinaryExpr(lhs, GetPrevious().type, EqualityExpr());
            return lhs;
        }

        private Expr RelationExpr()
        {
            Expr lhs = AdditionExpr();
            if (Match(LESS, GREATER, LESS_EQUALS, GREATER_EQUALS)) return new BinaryExpr(lhs, GetPrevious().type, RelationExpr());
            return lhs;
        }

        private Expr AdditionExpr()
        {
            Expr lhs = MultiplicationExpr();
            if (Match(PLUS, MINUS)) return new BinaryExpr(lhs, GetPrevious().type, AdditionExpr());
            return lhs;
        }

        private Expr MultiplicationExpr()
        {
            Expr lhs = UnaryExpr();
            if (Match(MUL, DIV)) return new BinaryExpr(lhs, GetPrevious().type, MultiplicationExpr());
            return lhs;
        }

        private Expr UnaryExpr()
        {
            TokenType? op = null;
            if (Match(EXCL_MARK, PLUS, MINUS, BW_NOT, DPLUS, DMINUS)) op = GetPrevious().type;
            if (Check(LPAREN)) return new UnaryExpr(op, GroupingExpr());
            return BaseExpr();
        }

        private Expr BaseExpr()
        {
            if (Match(TRUE)) return new BooleanExpr(true);
            if (Match(FALSE)) return new BooleanExpr(false);
            if (Match(NULL)) return new NullExpr();
            if (Match(NUM)) return new NumExpr(Double.Parse(GetPrevious().token, System.Globalization.CultureInfo.InvariantCulture));
            if (Match(STRING)) return new StringExpr(GetPrevious().token);
            return CallExpr();
        }

        private Expr CallExpr()
        {
            List<Call> callee = new List<Call>();
            TokenType? postfix = null;

            while (true)
            {
                Token idf = Consume(IDF);
                if (Match(DOT))
                {
                    callee.Add(new IdfCall(idf));
                    continue;
                }
                else if (Match(LPAREN))
                {
                    callee.Add(FunctionCall(idf));
                    if (Match(DOT)) continue;
                }
                else if (Match(DPLUS, DMINUS, ARRAY))
                {
                    postfix = GetPrevious().type;
                }
                else
                {
                    callee.Add(new IdfCall(idf));
                }
                break;

            }

            return new CallExpr(callee, postfix);

            Call FunctionCall(Token name)
            {
                List<Expr> args = new List<Expr>();
                if (!Match(RPAREN)) args.Add(Expression());
                else return new FunctionCall(name, new List<Expr>());
                while (true)
                {
                    if (Match(COMMA)) args.Add(Expression());
                    else break;
                }
                Consume(RPAREN);
                return new FunctionCall(name, args);
            }
        }

        private Expr GroupingExpr()
        {
            Consume(LPAREN);
            Expr expr = Expression();
            Consume(RPAREN);

            return new GroupingExpr(expr);
        }

        private Expr VarExpr() => new VarExpr(CallExpr(), Consume(IDF));

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
            builder.Exception(6, GetCurrent().offset, TTypePrettyString(type));
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
            if (GetCurrent().type == EOF) return true;
            else return false;
        }

        private string TTypePrettyString(TokenType type)
        {
            switch (type)
            {
                case HASHTAG: return "#";
                case USING: return "using";
                case SAFETY: return "safety";
                case PUBLIC: return "public";
                case PRIVATE: return "private";
                case PROTECTED: return "protected";
                case RESTRICTED: return "restricted";
                case STATIC: return "static";
                case DYNAMIC: return "dynamic";
                case ABSTRACT: return "abstract";
                case CONST: return "const";
                case LPAREN: return "(";
                case RPAREN: return ")";
                case LBRACE: return "{";
                case RBRACE: return "}";
                case LBRACK: return "[";
                case RBRACK: return "]";
                case PLUS: return "+";
                case MINUS: return "-";
                case MUL: return "*";
                case DIV: return "/";
                case PERCENT: return "%";
                case DPLUS: return "++";
                case DMINUS: return "--";
                case GREATER: return ">";
                case LESS: return "<";
                case GREATER_EQUALS: return ">=";
                case LESS_EQUALS: return "<=";
                case EQUALS: return "==";
                case NEQUALS: return "!=";
                case AND: return "&&";
                case OR: return "||";
                case XOR: return "^^";
                case BW_AND: return "&";
                case BW_OR: return "|";
                case BW_XOR: return "^";
                case BW_NOT: return "~";
                case BS_LEFT: return "<<";
                case BS_RIGHT: return ">>";
                case ASSIGN: return "=";
                case ADD_ASSIGN: return "+=";
                case SUB_ASSIGN: return "-=";
                case MUL_ASSIGN: return "*=";
                case DIV_ASSIGN: return "/=";
                case PERCENT_ASSIGN: return "%=";
                case BW_AND_ASSIGN: return "&=";
                case BW_OR_ASSIGN: return "|=";
                case BW_XOR_ASSIGN: return "^=";
                case BS_LEFT_ASSIGN: return "<<=";
                case BS_RIGHT_ASSIGN: return ">>=";
                case NULL: return "null";
                case COLON: return ":";
                case SEMICOLON: return ";";
                case DOT: return ".";
                case COMMA: return ",";
                case EXCL_MARK: return "!";
                case NUM: return "number";
                case STRING: return "string";
                case IDF: return "identifier";
                case TRUE: return "true";
                case FALSE: return "false";
                case CONTAINER: return "container";
                case SYSTEM: return "system";
                case DATATYPE: return "datatype";
                case LIBRARY: return "library";
                case IF: return "if";
                case ELIF: return "elif";
                case ELSE: return "else";
                case FOR: return "for";
                case WHILE: return "while";
                case DO: return "do";
                case SWITCH: return "switch";
                case CASE: return "case";
                case DEFAULT: return "default";
                case EOF: return "end of file";
                default: throw new ArgumentException();
            }
        }
    }
}
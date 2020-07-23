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

using Penguor.Compiler.Debugging;
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
        private readonly List<Token> tokens;
        private int current;
        private readonly Builder builder;

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
            List<Decl> declarations = new List<Decl>();
            while (!Match(EOF)) declarations.Add(Declaration());
            return new ProgramDecl(declarations);
        }

        private Decl Declaration(bool allowDeclStmt = false)
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
                if (Match(DATA)) return DataDecl(null, new TokenType[0]);
                if (Match(TYPE)) return TypeDecl(null, new TokenType[0]);
                if (Match(LIBRARY)) return LibraryDecl(null, new TokenType[0]);
                if (Match(HASHTAG)) return new DeclStmt(CompilerStmt());
                if (Check(IDF) && LookAhead(1).type == IDF && LookAhead(2).type == LPAREN)
                    return FunctionDecl(null, new TokenType[0]);
                else if (Check(IDF) && LookAhead(1).type == IDF) return VarDecl(null, new TokenType[0]);
                if (!allowDeclStmt) throw new ParsingException(1, GetCurrent(), new TokenType[0]);
                return DeclStmt();
            }
            else
            {
                if (Match(SYSTEM)) return SystemDecl(accessMod, nonAccessMods);
                if (Match(DATA)) return DataDecl(accessMod, nonAccessMods);
                if (Match(TYPE)) return TypeDecl(accessMod, nonAccessMods);
                if (Match(LIBRARY)) return LibraryDecl(accessMod, nonAccessMods);
                if (Check(IDF) && LookAhead(1).type == IDF && LookAhead(2).type == LPAREN)
                    return FunctionDecl(accessMod, nonAccessMods);
                else if (Check(IDF) && LookAhead(1).type == IDF) return VarDecl(accessMod, nonAccessMods);
                else return Error(DeclStmt, 1, GetCurrent(), SYSTEM, DATA, TYPE, LIBRARY, IDF);
            }
        }

        private Decl UsingDecl()
        {
            Expr call = CallExpr();
            GetEnding();
            return new UsingDecl(call);
        }

        private SystemDecl SystemDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new SystemDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Decl DataDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new DataDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Decl TypeDecl(TokenType? accessMod, TokenType[] nonAccessMods) => new TypeDecl(accessMod, nonAccessMods, Consume(IDF), GetParent(), BlockDecl());
        private Token? GetParent() => Match(LESS) ? Consume(IDF) : (Token?)null;

        private FunctionDecl FunctionDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            Expr variable = VarExpr();

            Consume(LPAREN);
            if (Match(RPAREN)) return new FunctionDecl(accessMod, nonAccessMods, variable, new List<Expr>(), DeclContent());
            List<Expr> parameters = new List<Expr>();
            while (true)
            {
                parameters.Add(VarExpr());
                if (Match(COMMA)) continue;
                Consume(RPAREN);
                break;
            }
            return new FunctionDecl(accessMod, nonAccessMods, variable, parameters, DeclContent());
        }

        private VarDecl VarDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            VarDecl dec = new VarDecl(accessMod, nonAccessMods, VarExpr(), Match(ASSIGN) ? Expression() : null);
            GetEnding();
            return dec;
        }

        private LibraryDecl LibraryDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            List<Token> name = new List<Token>
            {
                Consume(IDF)
            };
            while (Match(DOT)) name.Add(Consume(IDF));

            return new LibraryDecl(accessMod, nonAccessMods, name, BlockDecl());
        }

        // returns either a DeclStmt or a BlockDecl
        private Decl DeclContent()
        {
            if (Check(LBRACE)) return BlockDecl(true);
            if (Check(COLON)) return DeclStmt();
            return Error(new DeclStmt(null!), 1, GetCurrent(), LBRACE, COLON);
        }

        private Decl BlockDecl(bool allowStmt = false)
        {
            Consume(LBRACE);
            List<Decl> declarations = new List<Decl>();
            while (!Match(RBRACE)) declarations.Add(Declaration(allowStmt));

            return new BlockDecl(declarations);
        }

        private Decl DeclStmt() => new DeclStmt(Statement());

        private Stmt Statement()
        {
            if (Match(HASHTAG)) return CompilerStmt();
            if (Check(LBRACE)) return BlockStmt();
            if (Check(IDF) && LookAhead(1).type == IDF) return VarStmt();
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
                    if (Convert.ToInt32(val[0].token) < 0 || Convert.ToInt32(val[0].token) > 2) Error(1, val[0], NUM);
                    break;
                default:
                    val = new Token[0];
                    Error(8, dir, SAFETY);
                    break;
            }
            GetEnding();
            return new CompilerStmt(dir.type, val);
        }

        private Stmt BlockStmt()
        {
            Match(LBRACE);
            List<Stmt> statements = new List<Stmt>();
            while (!Match(RBRACE)) statements.Add(Statement());

            return new BlockStmt(statements);
        }

        private Stmt VarStmt()
        {
            VarDecl variable = VarDecl(null, new TokenType[0]);
            return new VarStmt(variable.Variable, variable.Init);
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
            else
            {
                condition = null;
            }

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
            if (TryGetEnding())
            {
                return new ReturnStmt(null);
            }
            else
            {
                Expr expr = Expression();
                GetEnding();
                return new ReturnStmt(expr);
            }
        }

        private Stmt ExprStmt()
        {
            Expr expression = Expression();
            GetEnding();

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
                      BW_XOR_ASSIGN))
            {
                return new AssignExpr(lhs, GetPrevious().type, CondOrExpr());
            }

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

        private CallExpr CallExpr()
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

            FunctionCall FunctionCall(Token name)
            {
                List<Expr> args = new List<Expr>();
                if (!Match(RPAREN)) args.Add(Expression());
                else return new FunctionCall(name, new List<Expr>());
                while (Match(COMMA))
                {
                    args.Add(Expression());
                }
                Consume(RPAREN);
                return new FunctionCall(name, args);
            }
        }

        private GroupingExpr GroupingExpr()
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
            Error(6, GetCurrent(), type);
            var tmp = Advance();
            return new Token(type, "", tmp.offset, tmp.length);
        }

        /// <summary>
        /// <c>Check() </c> checks the type of the current token.
        /// </summary>
        /// <param name="type">the type to compare the current token with</param>
        /// <param name="n">look up using lookAhead</param>
        /// <param name="matchEnding">should Endings be consumed</param>
        /// <returns></returns>
        private bool Check(TokenType type, int n = 0, bool matchEnding = true)
        {
            int endings = 0;
            if (matchEnding)
                while (LookAhead(endings).type == ENDING) endings++;
            if (LookAhead(n + endings).type == type)
            {
                for (int i = 0; i < endings; i++)
                    Advance();
                return true;
            }
            return false;
        }

        // TODO: improve xml documentation 
        /// <summary>
        /// <c>Advance() </c> advances to the next token if the eof isn't reached yet.
        /// </summary>
        /// <returns>the token before advancing</returns>
        private Token Advance()
        {
            if (!AtEnd()) current++;
            else return GetCurrent();
            return GetPrevious();
        }

        /// <summary>
        /// <c>GetNext() </c> returns the previous token.
        /// </summary>
        /// <returns>the previous item in <c>tokens</c></returns>
        private Token GetPrevious() => LookAhead(-1);
        /// <summary>
        /// <c>GetNext() </c> returns the current token without advancing.
        /// </summary>
        /// <returns>the current item in <c>tokens</c></returns>
        private Token GetCurrent() => LookAhead(0);
        /// <summary>
        /// <c>GetNext() </c> returns the next token without advancing.
        /// </summary>
        /// <returns>the next item in <c>tokens</c></returns>
        private Token GetNext() => LookAhead(1);
        /// <summary>
        /// <c>GetNext() </c> returns the next token without advancing.
        /// </summary>
        /// <returns>the nth-next item in <c>tokens</c></returns>
        private Token LookAhead(int n) => tokens[current + n];

        /// <summary>
        /// <c>AtEnd() </c> checks if the parser has reached the end of the file.
        /// </summary>
        /// <returns>true if the parser has reached the end of the file, otherwise false</returns>
        private bool AtEnd()
        {
            return GetCurrent().type == EOF;
        }

        private bool GetEnding()
        {
            if (Check(SEMICOLON, 0, false) || Check(ENDING, 0, false))
            {
                Advance();
                return true;
            }
            Error(6, GetCurrent(), SEMICOLON, ENDING);
            return false;
        }

        private bool TryGetEnding()
        {
            if (Check(SEMICOLON, 0, false) || Check(ENDING, 0, false))
            {
                Advance();
                return true;
            }
            return false;
        }

        // Report compile errors to the builder

        private void Error(uint msg, Token actual, params TokenType[] expected) => builder.Exceptions.Add(new ParsingException(msg, actual, expected));

        private T Error<T>(T recover, uint msg, Token actual, params TokenType[] expected)
        {
            builder.Exceptions.Add(new ParsingException(msg, actual, expected));
            return recover;
        }

        private T Error<T>(Func<T> recover, uint msg, Token actual, params TokenType[] expected)
        {
            builder.Exceptions.Add(new ParsingException(msg, actual, expected));
            return recover();
        }
    }
}
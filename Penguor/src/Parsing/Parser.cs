using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Parsing.AST;
using static Penguor.Compiler.Parsing.TokenType;

namespace Penguor.Compiler.Parsing
{
    /// <summary>
    /// The Penguor parser
    /// </summary>
    public class Parser : IExceptionHandler
    {
        private readonly Builder builder;  // the builder which is holding the Lexer
        private readonly List<Token> tokens;
        private int current; // the position of the current token in the tokens list

        private readonly State state;

        private int id;
        private int ID { get => id++; }

        /// <summary>
        /// create a new parser with the tokens that should be parsed
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        /// <param name="builder">the builder which executed the parser</param>
        public Parser(List<Token> tokens, Builder builder)
        {
            this.builder = builder;
            this.tokens = tokens;
            state = new State();
        }

        /// <summary>
        /// start the Parsing process
        /// </summary>
        /// <returns>a statement containing the AST of the parsed file</returns>
        public ProgramDecl Parse()
        {
            List<Decl> declarations = new List<Decl>();
            AddTable();
            while (!Match(EOF)) declarations.Add(Declaration());
            return new ProgramDecl(ID, GetCurrent().Offset, declarations);
        }

        private Decl Declaration(bool allowStmtDecl = false)
        {
            TokenType? accessMod = null;
            bool hasAccessMod = false;
            TokenType[] nonAccessMods = new TokenType[3];

            if (Match(PUBLIC, PRIVATE, PROTECTED, RESTRICTED))
            {
                accessMod = GetPrevious().Type;
                hasAccessMod = true;
            }
            if (Check(STATIC) != Check(DYNAMIC))
            {
                nonAccessMods[0] = Advance().Type;
                hasAccessMod = true;
            }
            if (Check(ABSTRACT))
            {
                nonAccessMods[1] = Advance().Type;
                hasAccessMod = true;
            }
            if (Check(CONST))
            {
                nonAccessMods[2] = Advance().Type;
                hasAccessMod = true;
            }

            if (!hasAccessMod)
            {
                if (Match(USING)) return UsingDecl();
                if (Match(SYSTEM)) return SystemDecl(null, Array.Empty<TokenType>());
                if (Match(DATA)) return DataDecl(null, Array.Empty<TokenType>());
                if (Match(TYPE)) return TypeDecl(null, Array.Empty<TokenType>());
                if (Match(LIBRARY)) return LibraryDecl(null, Array.Empty<TokenType>());
                if (Match(HASHTAG)) return new StmtDecl(ID, GetPrevious().Offset, CompilerStmt());
                if (Check(IDF) && LookAhead(1).Type == IDF && LookAhead(2).Type == LPAREN)
                    return FunctionDecl(null, Array.Empty<TokenType>());
                else if (!state.ContainsAdType(AddressType.FunctionDecl) && Check(IDF) && LookAhead(1).Type == IDF) return VarDecl(null, Array.Empty<TokenType>());
                if (!allowStmtDecl)
                {
                    StmtDecl decl = StmtDecl();
                    return Except(decl, 6, decl.Stmt.ToString(), state[^1].Type.ToString());
                }
                return StmtDecl();
            }
            else
            {
                if (Match(SYSTEM)) return SystemDecl(accessMod, nonAccessMods);
                if (Match(DATA)) return DataDecl(accessMod, nonAccessMods);
                if (Match(TYPE)) return TypeDecl(accessMod, nonAccessMods);
                if (Match(LIBRARY)) return LibraryDecl(accessMod, nonAccessMods);
                if (Check(IDF) && LookAhead(1).Type == IDF && LookAhead(2).Type == LPAREN)
                    return FunctionDecl(accessMod, nonAccessMods);
                if (!state.ContainsAdType(AddressType.FunctionDecl) && Check(IDF) && LookAhead(1).Type == IDF) return VarDecl(accessMod, nonAccessMods);
                if (Match(HASHTAG))
                {
                    CompilerStmt stmt = CompilerStmt();
                    return Except(new StmtDecl(ID, stmt.Offset, stmt), 13, stmt.ToString());
                }
                else
                {
                    StmtDecl decl = StmtDecl();
                    return Except(decl, 6, decl.Stmt.ToString(), state[^1].Type.ToString());
                }
            }
        }

        private Decl UsingDecl()
        {
            int offset = GetCurrent().Offset;
            var call = CallExpr();
            GetEnding();
            return new UsingDecl(ID, offset, call);
        }

        private SystemDecl SystemDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new AddressFrame(Consume(IDF).Name, AddressType.SystemDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name, accessMod, nonAccessMods);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new SystemDecl(ID, offset, accessMod, nonAccessMods, name, parent, content);
        }

        private Decl DataDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new AddressFrame(Consume(IDF).Name, AddressType.DataDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name, accessMod, nonAccessMods);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new DataDecl(ID, offset, accessMod, nonAccessMods, name, parent, content);
        }

        private Decl TypeDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new AddressFrame(Consume(IDF).Name, AddressType.TypeDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name, accessMod, nonAccessMods);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new TypeDecl(ID, offset, accessMod, nonAccessMods, name, parent, content);
        }

        private CallExpr? GetParent() => Match(LESS) ? CallExpr() : null;

        private FunctionDecl FunctionDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            var variable = VarExpr(AddressType.FunctionDecl);
            AddressFrame name = variable.Name;
            AddSymbol(name, accessMod, nonAccessMods);
            state.Push(name);
            AddTable();

            Consume(LPAREN);
            List<VarExpr> parameters = new();
            if (!Match(RPAREN))
            {
                while (true)
                {
                    VarExpr var = VarExpr(AddressType.VarExpr);
                    parameters.Add(var);
                    AddSymbol(var.Name);
                    if (Match(COMMA)) continue;
                    Consume(RPAREN);
                    break;
                }
            }

            Decl content = DeclContent();

            state.Pop();
            return new FunctionDecl(ID, offset, accessMod, nonAccessMods, variable.Type, name, parameters, content);
        }

        private VarDecl VarDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            var variable = VarExpr(AddressType.VarDecl);
            AddSymbol(variable.Name);
            VarDecl dec = new VarDecl(ID, offset, accessMod, nonAccessMods, variable.Type, variable.Name, Match(ASSIGN) ? CondOrExpr() : null);
            GetEnding();
            return dec;
        }

        private LibraryDecl LibraryDecl(TokenType? accessMod, TokenType[] nonAccessMods)
        {
            int offset = GetCurrent().Offset;
            State name = new State { new AddressFrame(Consume(IDF).Name, AddressType.LibraryDecl) };
            while (Match(DOT)) name.Add(new AddressFrame(Consume(IDF).Name, AddressType.LibraryDecl));
            state.Append(name);

            AddTable();

            var content = BlockDecl();

            state.Remove(name);

            return new LibraryDecl(ID, offset, accessMod, nonAccessMods, name, content);
        }

        // returns either a StmtDecl or a BlockDecl
        private Decl DeclContent()
        {
            if (Check(LBRACE)) return BlockDecl(true);
            if (Check(COLON)) return StmtDecl();
            return Except(new StmtDecl(ID, GetCurrent().Offset, null!), 12);
        }

        private BlockDecl BlockDecl(bool allowStmt = false)
        {
            int offset = Consume(LBRACE).Offset;
            List<Decl> declarations = new List<Decl>();

            while (!Match(RBRACE)) declarations.Add(Declaration(allowStmt));

            return new BlockDecl(ID, offset, declarations);
        }

        private StmtDecl StmtDecl() => new StmtDecl(ID, GetCurrent().Offset, Statement());

        private Stmt Statement()
        {
            if (Match(HASHTAG)) return CompilerStmt();
            if (Check(LBRACE)) return BlockStmt();
            if (Check(IDF) && LookAhead(1).Type == IDF) return VarStmt();
            if (Match(IF)) return IfStmt();
            if (Match(WHILE)) return WhileStmt();
            if (Match(FOR)) return ForStmt();
            if (Match(DO)) return DoStmt();
            if (Match(SWITCH)) return SwitchStmt();
            if (Match(RETURN)) return ReturnStmt();
            return ExprStmt();
        }

        private CompilerStmt CompilerStmt()
        {
            Token[] val;
            Token dir = Advance();
            switch (dir.Type)
            {
                case SAFETY:
                    val = new Token[1];
                    val[0] = Consume(NUM);
                    if (Convert.ToInt32(val[0].Name) < 0 || Convert.ToInt32(val[0].Name) > 2) Except(1);
                    break;
                default:
                    val = Array.Empty<Token>();
                    Except(1);
                    break;
            }
            GetEnding();
            return new CompilerStmt(ID, dir.Offset, dir.Type, val);
        }

        private BlockStmt BlockStmt()
        {
            int offset = Consume(LBRACE).Offset;
            List<Stmt> statements = new List<Stmt>();
            while (!Match(RBRACE)) statements.Add(Statement());

            return new BlockStmt(ID, offset, statements);
        }

        private VarStmt VarStmt()
        {
            var variable = VarExpr(AddressType.VarStmt);
            VarStmt stmt = new VarStmt(ID, GetCurrent().Offset, variable.Type, variable.Name, Match(ASSIGN) ? CondOrExpr() : null);
            AddSymbol(stmt.Name);
            GetEnding();
            return stmt;
        }

        private IfStmt IfStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            Stmt ifC = Statement();

            List<Stmt> elif = new List<Stmt>();
            while (Match(ELIF)) elif.Add(ElifStmt());

            Stmt? elseC = null;
            if (Match(ELSE)) elseC = Statement();

            return new IfStmt(ID, offset, condition, ifC, elif, elseC);
        }

        private ElifStmt ElifStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new ElifStmt(ID, offset, condition, Statement());
        }

        private WhileStmt WhileStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new WhileStmt(ID, offset, condition, Statement());
        }

        private ForStmt ForStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            VarExpr current = VarExpr(AddressType.VarExpr);
            Consume(COLON);
            CallExpr vars = CallExpr();
            Consume(RPAREN);

            return new ForStmt(ID, offset, current, vars, Statement());
        }

        private DoStmt DoStmt()
        {
            int offset = GetPrevious().Offset;
            Stmt content = Statement();

            Consume(WHILE);
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            return new DoStmt(ID, offset, content, condition);
        }

        private SwitchStmt SwitchStmt()
        {
            int offset = GetPrevious().Offset;
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

            return new SwitchStmt(ID, offset, condition, cases, defaultCase);
        }

        private CaseStmt CaseStmt()
        {
            int offset = GetPrevious().Offset;
            Expr? condition;
            if (GetPrevious().Type == CASE)
                condition = VarExpr(AddressType.VarExpr);
            else
                condition = null;

            Consume(COLON);

            List<Stmt> statements = new List<Stmt>();
            while (!Check(CASE) && !Check(DEFAULT) && Check(RBRACE))
            {
                statements.Add(Statement());
            }

            return new CaseStmt(ID, offset, condition, statements);
        }

        private ReturnStmt ReturnStmt()
        {
            int offset = GetCurrent().Offset;
            if (TryGetEnding())
            {
                return new ReturnStmt(ID, offset, null);
            }
            else
            {
                Expr expr = Expression();
                GetEnding();
                return new ReturnStmt(ID, offset, expr);
            }
        }

        private ExprStmt ExprStmt()
        {
            int offset = GetCurrent().Offset;
            Expr expression = Expression();
            GetEnding();

            return new ExprStmt(ID, offset, expression);
        }

        private Expr Expression() => AssignExpr();

        private Expr AssignExpr()
        {
            int offset = GetCurrent().Offset;
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
                if (lhs is CallExpr e)
                    return new AssignExpr(ID, offset, e, GetPrevious().Type, CondOrExpr());
                else
                    return Except(new BinaryExpr(ID, offset, lhs, GetPrevious().Type, CondOrExpr()), 14, lhs.ToString());
            }

            return lhs;
        }

        private Expr CondOrExpr() => Check(OR, 1) ? new BinaryExpr(ID, GetCurrent().Offset, CondAndExpr(), Consume(OR).Type, CondOrExpr()) : CondAndExpr();
        private Expr CondAndExpr() => Check(AND, 1) ? new BinaryExpr(ID, GetCurrent().Offset, BWOrExpr(), Consume(AND).Type, CondAndExpr()) : BWOrExpr();
        private Expr BWOrExpr() => Check(BW_OR, 1) ? new BinaryExpr(ID, GetCurrent().Offset, BWXorExpr(), Consume(BW_OR).Type, BWOrExpr()) : BWXorExpr();
        private Expr BWXorExpr() => Check(BW_XOR, 1) ? new BinaryExpr(ID, GetCurrent().Offset, BWAndExpr(), Consume(BW_XOR).Type, BWXorExpr()) : BWAndExpr();
        private Expr BWAndExpr() => Check(BW_AND, 1) ? new BinaryExpr(ID, GetCurrent().Offset, EqualityExpr(), Consume(BW_AND).Type, BWAndExpr()) : EqualityExpr();

        private Expr EqualityExpr()
        {
            int offset = GetCurrent().Offset;
            Expr lhs = RelationExpr();
            if (Match(EQUALS, NEQUALS)) return new BinaryExpr(ID, offset, lhs, GetPrevious().Type, EqualityExpr());
            return lhs;
        }

        private Expr RelationExpr()
        {
            int offset = GetCurrent().Offset;
            Expr lhs = AdditionExpr();
            if (Match(LESS, GREATER, LESS_EQUALS, GREATER_EQUALS)) return new BinaryExpr(ID, offset, lhs, GetPrevious().Type, RelationExpr());
            return lhs;
        }

        private Expr AdditionExpr()
        {
            int offset = GetCurrent().Offset;
            Expr lhs = MultiplicationExpr();
            if (Match(PLUS, MINUS)) return new BinaryExpr(ID, offset, lhs, GetPrevious().Type, AdditionExpr());
            return lhs;
        }

        private Expr MultiplicationExpr()
        {
            int offset = GetCurrent().Offset;
            Expr lhs = UnaryExpr();
            if (Match(MUL, DIV)) return new BinaryExpr(ID, offset, lhs, GetPrevious().Type, MultiplicationExpr());
            return lhs;
        }

        private Expr UnaryExpr()
        {
            int offset = GetCurrent().Offset;
            TokenType? op = null;
            if (Match(EXCL_MARK, PLUS, MINUS, BW_NOT, DPLUS, DMINUS)) op = GetPrevious().Type;
            if (Check(LPAREN)) return new UnaryExpr(ID, offset, op, GroupingExpr());
            return BaseExpr();
        }

        private Expr BaseExpr()
        {
            int offset = GetCurrent().Offset;
            if (Match(TRUE)) return new BooleanExpr(ID, offset, true);
            if (Match(FALSE)) return new BooleanExpr(ID, offset, false);
            if (Match(NULL)) return new NullExpr(ID, offset);
            if (Match(NUM_BASE)) return new NumExpr(ID, offset, int.Parse(GetPrevious().Name), Advance().Name, null);
            if (Match(STRING)) return new StringExpr(ID, offset, GetPrevious().Name);
            return CallExpr();
        }

        private CallExpr CallExpr()
        {
            int offset = GetCurrent().Offset;
            List<Call> callee = new List<Call>();
            TokenType? postfix = null;

            while (true)
            {
                Token idf = Consume(IDF);
                if (Match(DOT))
                {
                    callee.Add(new IdfCall(ID, idf.Offset, new AddressFrame(idf.Name, AddressType.IdfCall)));
                    continue;
                }
                else if (Match(LPAREN))
                {
                    callee.Add(FunctionCall(new AddressFrame(idf.Name, AddressType.FunctionCall)));
                    if (Match(DOT)) continue;
                }
                else if (Match(DPLUS, DMINUS, ARRAY))
                {
                    postfix = GetPrevious().Type;
                    callee.Add(new IdfCall(ID, idf.Offset, new AddressFrame(idf.Name, AddressType.IdfCall)));
                }
                else
                {
                    callee.Add(new IdfCall(ID, idf.Offset, new AddressFrame(idf.Name, AddressType.IdfCall)));
                }
                break;
            }

            return new CallExpr(ID, offset, callee, postfix);

            FunctionCall FunctionCall(AddressFrame name)
            {
                int offset = GetCurrent().Offset;
                List<Expr> args = new List<Expr>();
                if (!Match(RPAREN)) args.Add(Expression());
                else return new FunctionCall(ID, offset, name, new List<Expr>());
                while (Match(COMMA))
                {
                    args.Add(Expression());
                }
                Consume(RPAREN);
                return new FunctionCall(ID, offset, name, args);
            }
        }

        private GroupingExpr GroupingExpr()
        {
            int offset = Consume(LPAREN).Offset;
            Expr expr = Expression();
            Consume(RPAREN);

            return new GroupingExpr(ID, offset, expr);
        }

        private VarExpr VarExpr(AddressType type) => new VarExpr(ID, GetCurrent().Offset, CallExpr(), new AddressFrame(Consume(IDF).Name, type));

        private void AddTable()
        {
            builder.TableManager.AddTable(state);
        }

        private void AddSymbol(AddressFrame frame)
        {
            bool succeeded = builder.TableManager.AddSymbol(state, frame);
            if (!succeeded) throw new PenguorCSException();
        }

        private void AddSymbol(AddressFrame frame, TokenType? accessMod, TokenType[]? nonAccessMods)
        {
            bool succeeded = builder.TableManager.AddSymbol(state, new Symbol(frame.Symbol, frame.Type)
            {
                AccessMod = accessMod,
                NonAccessMods = nonAccessMods,
            });
            if (!succeeded) throw new PenguorCSException();
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
            Except(11, GetCurrent().ToTTypeString(), LookAhead(1).ToTTypeString());
            var tmp = Advance();
            return new Token(type, "", tmp.Offset, tmp.Length);
        }

        /// <summary>
        /// <c>Check() </c> checks the type of the current token.
        /// </summary>
        /// <param name="type">the type to compare the current token with</param>
        /// <param name="n">look up using lookAhead</param>
        /// <param name="matchEnding">should Endings be consumed</param>
        private bool Check(TokenType type, int n = 0, bool matchEnding = true)
        {
            int endings = 0;
            if (matchEnding)
                while (LookAhead(endings).Type == ENDING) endings++;
            if (LookAhead(n + endings).Type == type)
            {
                for (int i = 0; i < endings; i++)
                    Advance();
                return true;
            }
            return false;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Token LookAhead(int n) => tokens[current + n];

        /// <summary>
        /// <c>AtEnd() </c> checks if the parser has reached the end of the file.
        /// </summary>
        /// <returns>true if the parser has reached the end of the file, otherwise false</returns>
        private bool AtEnd()
        {
            return GetCurrent().Type == EOF;
        }

        private bool GetEnding()
        {
            if (Check(SEMICOLON, 0, false) || Check(ENDING, 0, false))
            {
                Advance();
                return true;
            }
            Except(5);
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

        private void Except(uint msg, params string[] args) => Logger.Log(new Notification(builder.SourceFile, GetCurrent().Offset, msg, MsgType.PGR, args));

        private T Except<T>(T recover, uint msg, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, GetCurrent().Offset, msg, MsgType.PGR, args));
            return recover;
        }
        private T Except<T>(Func<T> recover, uint msg, params string[] args)
        {
            Logger.Log(new Notification(builder.SourceFile, GetCurrent().Offset, msg, MsgType.PGR, args));
            return recover();
        }
    }
}
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
        private bool failed;

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

            Logger.OnTryLogged += GetLoggedErrors;
        }

        /// <summary>
        /// deconstructor for the parser
        /// </summary>
        ~Parser()
        {
            Logger.OnTryLogged -= GetLoggedErrors;
        }

        private void CheckBraces(Stmt content, string name)
        {
            if (!(content is AST.ExprStmt or AST.ReturnStmt or AST.BlockStmt))
                builder.Except(17, content.Offset, name);
            else if (!(content is AST.BlockStmt or AST.ReturnStmt))
                builder.Except(18, content.Offset, name);
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

        private Decl Declaration()
        {
            return Either(ModifiedDecl, EmbeddedDeclaration);
        }

        private Decl ModifiableDeclaration()
        {
            if (Match(SYSTEM)) return SystemDecl();
            if (Match(DATA)) return DataDecl();
            if (Match(TYPE)) return TypeDecl();
            if (Match(LIBRARY)) return LibraryDecl();
            if (Check(IDF) && Check(IDF, 1) && Check(LPAREN, 2))
                return FunctionDecl();
            if (Check(IDF) && LookAhead(1).Type == IDF) return VarDecl();

            if (Match(HASHTAG))
            {
                CompilerStmt stmt = CompilerStmt();
                return Except(new StmtDecl(ID, stmt.Offset, stmt), 13, stmt.ToString());
            }
            else
            {
                StmtDecl decl = StmtDecl();
                return Except(decl, 6, decl.Stmt.ToString(), state.Count > 0 ? state[^1].Type.ToString() : "global scope");
            }
        }

        private Decl EmbeddedDeclaration()
        {
            if (Match(USING)) return UsingDecl();
            if (Check(IDF) && LookAhead(1).Type == IDF && LookAhead(2).Type == LPAREN)
                return FunctionDecl();
            return StmtDecl();
        }

        private Decl ModifiedDecl()
        {
            TokenType? accessMod = null;
            TokenType? nonAccessMod = null;

            if (Match(PUBLIC, PRIVATE, PROTECTED, RESTRICTED))
            {
                accessMod = GetPrevious().Type;
            }
            if (Match(STATIC, DYNAMIC, ABSTRACT, CONST))
            {
                nonAccessMod = GetPrevious().Type;
            }

            Decl decl = ModifiableDeclaration();
            (var declName, var defaultAccessMod) = decl switch
            {
                SystemDecl d => new Tuple<AddressFrame?, TokenType?>(d.Name, RESTRICTED),
                DataDecl d => new Tuple<AddressFrame?, TokenType?>(d.Name, RESTRICTED),
                TypeDecl d => new Tuple<AddressFrame?, TokenType?>(d.Name, RESTRICTED),
                FunctionDecl d => new Tuple<AddressFrame?, TokenType?>(d.Name, state.ContainsAdType(AddressType.SystemDecl, AddressType.DataDecl, AddressType.TypeDecl) ? PROTECTED : RESTRICTED),
                VarDecl d => new Tuple<AddressFrame?, TokenType?>(d.Name, RESTRICTED),
                _ => new Tuple<AddressFrame?, TokenType?>(null, null)
            };

            if (declName != null)
            {
                var symbol = builder.TableManager.GetSymbol(declName, state) ?? throw new Exception();
                symbol.AccessMod = accessMod ?? defaultAccessMod;
                symbol.NonAccessMod = nonAccessMod;
            }

            return new ModifiedDecl(id, GetCurrent().Offset, accessMod, nonAccessMod, decl);
        }

        private Decl UsingDecl()
        {
            int offset = GetCurrent().Offset;
            var call = CallExpr();
            GetEnding();
            return new UsingDecl(ID, offset, call);
        }

        private SystemDecl SystemDecl()
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new(Consume(IDF).Name, AddressType.SystemDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new SystemDecl(ID, offset, name, parent, content);
        }

        private Decl DataDecl()
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new(Consume(IDF).Name, AddressType.DataDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new DataDecl(ID, offset, name, parent, content);
        }

        private Decl TypeDecl()
        {
            int offset = GetCurrent().Offset;
            AddressFrame name = new(Consume(IDF).Name, AddressType.TypeDecl);
            CallExpr? parent = GetParent();
            AddSymbol(name);
            state.Push(name);
            AddTable();
            BlockDecl content = BlockDecl();
            state.Pop();
            return new TypeDecl(ID, offset, name, parent, content);
        }

        private CallExpr? GetParent() => Match(LESS) ? CallExpr() : null;

        private FunctionDecl FunctionDecl()
        {
            int offset = GetCurrent().Offset;
            var variable = VarExpr(AddressType.FunctionDecl);
            AddressFrame name = variable.Name;
            AddSymbol(name);
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
            return new FunctionDecl(ID, offset, variable.Type, name, parameters, content);
        }

        private VarDecl VarDecl()
        {
            int offset = GetCurrent().Offset;
            var variable = VarExpr(AddressType.VarDecl);
            AddSymbol(variable.Name);
            VarDecl dec = new(ID, offset, variable.Type, variable.Name, Match(ASSIGN) ? CondOrExpr() : null);
            GetEnding();
            return dec;
        }

        private LibraryDecl LibraryDecl()
        {
            int offset = GetCurrent().Offset;
            State name = new() { new AddressFrame(Consume(IDF).Name, AddressType.LibraryDecl) };
            while (Match(DOT)) name.Add(new AddressFrame(Consume(IDF).Name, AddressType.LibraryDecl));
            state.AddRange(name);

            AddTable();

            var content = BlockDecl();

            state.Remove(name);

            return new LibraryDecl(ID, offset, name, content);
        }

        // returns either a StmtDecl or a BlockDecl
        private Decl DeclContent()
        {
            if (Check(LBRACE)) return StmtBlockDecl();
            if (Match(COLON)) return StmtDecl();
            return Except(new StmtDecl(ID, GetCurrent().Offset, null!), 12);
        }

        private BlockDecl BlockDecl()
        {
            int offset = Consume(LBRACE).Offset;
            List<Decl> declarations = new();

            while (!Match(RBRACE))
            {
                var declaration = Declaration();
                if (declaration is StmtDecl)
                    Except(23);
                declarations.Add(declaration);
            }

            return new BlockDecl(ID, offset, declarations);
        }

        private BlockDecl StmtBlockDecl()
        {
            int offset = Consume(LBRACE).Offset;
            List<Decl> declarations = new();

            while (!Match(RBRACE)) declarations.Add(EmbeddedDeclaration());

            return new BlockDecl(ID, offset, declarations);
        }

        private StmtDecl StmtDecl() => new(ID, GetCurrent().Offset, Statement());

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
            if (Match(ASM)) return AsmStmt();
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
            List<Stmt> statements = new();
            while (!Match(RBRACE)) statements.Add(Statement());

            return new BlockStmt(ID, offset, statements);
        }

        private VarStmt VarStmt()
        {
            var variable = VarExpr(AddressType.VarStmt);
            VarStmt stmt = new(ID, GetCurrent().Offset, variable.Type, variable.Name, Match(ASSIGN) ? CondOrExpr() : null);
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
            CheckBraces(ifC, "if statement");

            List<ElifStmt> elif = new();
            while (Check(ELSE) && Check(IF, 1))
            {
                Advance(); Advance();
                elif.Add(ElifStmt());
            }

            Stmt? elseC = null;
            if (Match(ELSE)) elseC = Statement();
            if (elseC != null)
                CheckBraces(elseC, "else statement");

            return new IfStmt(ID, offset, condition, ifC, elif, elseC);

            ElifStmt ElifStmt()
            {
                int offset = GetPrevious().Offset;
                Consume(LPAREN);
                Expr condition = Expression();
                Consume(RPAREN);

                Stmt content = Statement();
                CheckBraces(content, "else if statement");

                return new ElifStmt(ID, offset, condition, content);
            }
        }

        private WhileStmt WhileStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            Expr condition = Expression();
            Consume(RPAREN);

            Stmt content = Statement();
            CheckBraces(content, "while statement");

            return new WhileStmt(ID, offset, condition, content);
        }

        private ForStmt ForStmt()
        {
            int offset = GetPrevious().Offset;
            Consume(LPAREN);
            VarExpr current = VarExpr(AddressType.VarExpr);
            Consume(COLON);
            CallExpr vars = CallExpr();
            Consume(RPAREN);

            Stmt content = Statement();
            CheckBraces(content, "for statement");
            return new ForStmt(ID, offset, current, vars, content);
        }

        private DoStmt DoStmt()
        {
            int offset = GetPrevious().Offset;
            Stmt content = Statement();
            CheckBraces(content, "do statement");

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
            List<Stmt> cases = new();
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

            List<Stmt> statements = new();
            while (!Check(CASE) && !Check(DEFAULT) && Check(RBRACE))
            {
                statements.Add(Statement());
            }

            return new CaseStmt(ID, offset, condition, statements);
        }

        private AsmStmt AsmStmt()
        {
            string assembly = Consume(STRING).Name;
            string[] strings = assembly.Split('\n');
            return new AsmStmt(ID, GetPrevious().Offset, strings);
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
            if (Match(MUL, DIV, PERCENT)) return new BinaryExpr(ID, offset, lhs, GetPrevious().Type, MultiplicationExpr());
            return lhs;
        }

        private Expr UnaryExpr()
        {
            int offset = GetCurrent().Offset;
            TokenType? op = null;
            if (Match(EXCL_MARK, PLUS, MINUS, BW_NOT, DPLUS, DMINUS)) op = GetPrevious().Type;
            if (op != null)
            {
                if (Check(LPAREN)) return new UnaryExpr(ID, offset, op, GroupingExpr());
                return new UnaryExpr(ID, offset, op, BaseExpr());
            }
            else
            {
                if (Check(LPAREN)) return GroupingExpr();
                return BaseExpr();
            }
        }

        private Expr BaseExpr()
        {
            int offset = GetCurrent().Offset;
            if (Match(TRUE)) return new BooleanExpr(ID, offset, true);
            if (Match(FALSE)) return new BooleanExpr(ID, offset, false);
            if (Match(NULL)) return new NullExpr(ID, offset);
            if (Match(NUM_BASE)) return new NumExpr(ID, offset, int.Parse(GetPrevious().Name), Advance().Name, null);
            if (Match(STRING)) return new StringExpr(ID, offset, GetPrevious().Name);
            if (Match(CHAR)) return new CharExpr(ID, offset, GetPrevious().Name[0]);
            return CallExpr();
        }

        private CallExpr CallExpr()
        {
            int offset = GetCurrent().Offset;
            List<Call> callees = new();

            do
            {
                if (Check(LPAREN, 1))
                    callees.Add(FunctionCall());
                else if (Check(LBRACK, 1))
                    callees.Add(ArrayCall());
                else
                    callees.Add(new IdfCall(id, GetCurrent().Offset, new AddressFrame(Consume(IDF).Name, AddressType.IdfCall)));
            } while (Match(DOT));

            return new CallExpr(id, offset, callees, null);

            FunctionCall FunctionCall()
            {
                int offset = GetCurrent().Offset;

                AddressFrame name = new(Consume(IDF).Name, AddressType.FunctionCall);
                List<Expr> args = new();

                Consume(LPAREN);
                if (!Match(RPAREN))
                {
                    do
                    {
                        args.Add(Expression());
                    } while (Match(COMMA));
                    Consume(RPAREN);
                }

                return new FunctionCall(ID, offset, name, args);
            }

            ArrayCall ArrayCall()
            {
                int offset = GetCurrent().Offset;

                AddressFrame name = new(Consume(IDF).Name, AddressType.ArrayCall);
                List<List<Expr>> indices = new();

                while (Match(LBRACK))
                {
                    List<Expr> expressions = new();
                    do
                    {
                        expressions.Add(Expression());
                    } while (Match(COMMA));
                    Consume(RBRACK);
                }

                return new ArrayCall(id, offset, name, indices);
            }
        }

        private TypeCallExpr TypeCallExpr()
        {
            State name = new();
            int offset = GetCurrent().Offset;

            do
            {
                name.Add(new AddressFrame(Consume(IDF).Name, AddressType.TypeCall));
            } while (Match(DOT));

            List<uint> dimensions = new();
            while (Match(LBRACK))
            {
                uint dim = 1;
                while (Match(COMMA))
                    dim++;
            }

            name.Add(name.Pop() with { ArrayDimensions = dimensions });

            return new TypeCallExpr(id, offset, name, dimensions);
        }

        private GroupingExpr GroupingExpr()
        {
            int offset = Consume(LPAREN).Offset;
            Expr expr = Expression();
            Consume(RPAREN);

            return new GroupingExpr(ID, offset, expr);
        }

        private VarExpr VarExpr(AddressType type) => new(ID, GetCurrent().Offset, TypeCallExpr(), new AddressFrame(Consume(IDF).Name, type));

        private void AddTable()
        {
            builder.TableManager.AddTable(state);
        }

        private void AddSymbol(AddressFrame frame)
        {
            bool succeeded = builder.TableManager.AddSymbol(state, frame);
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

        private T Either<T>(Func<T> first, Func<T> second) where T : ASTNode
        {
            return Try(first) ?? second();
        }

        private T? Try<T>(Func<T> parser) where T : ASTNode
        {
            int savedCurrent = current;
            bool saveFailed = failed;
            failed = false;

            Logger.Blocked = true;

            T result = parser();
            Logger.Blocked = false;
            if (failed)
            {
                current = savedCurrent;
                failed = saveFailed;
                return null;
            }
            failed = saveFailed;
            return result;
        }

        private void GetLoggedErrors(object? sender, OnLoggedEventArgs e)
        {
            if (e.LogLevel == LogLevel.Error && e.SourceFile == builder.SourceFile)
            {
                failed = true;
            }
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
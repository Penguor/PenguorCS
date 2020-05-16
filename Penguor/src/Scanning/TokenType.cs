/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
*/
#pragma warning disable 1591

namespace Penguor.Parsing
{
    public enum TokenType : byte
    {
        HASHTAG
    , FROM, INCLUDE, SAFETY // pp directives // ? maybe make them statements?
    , PUBLIC, PRIVATE, PROTECTED, RESTRICTED // access modifiers
    , STATIC, DYNAMIC // non-access modifiers
    , ABSTRACT, CONST // non-access modifiers
    , LPAREN, RPAREN // ()
    , LBRACE, RBRACE // {}
    , LBRACK, RBRACK // []
    , PLUS, MINUS // basic math operations
    , MUL, DIV, PERCENT // basic math operations
    , DPLUS, DMINUS // ++ and //
    , GREATER, LESS
    , GREATER_EQUALS, LESS_EQUALS
    , EQUALS, NEQUALS
    , AND, OR, XOR, NOT // logical operations
    , BW_AND, BW_OR, BW_XOR, BW_NOT // bitwise operations
    , BS_LEFT, BS_RIGHT // bitshift operations
    , ASSIGN // assignment
    , ADD_ASSIGN, SUB_ASSIGN
    , MUL_ASSIGN, DIV_ASSIGN, PERCENT_ASSIGN
    , BW_AND_ASSIGN, BW_OR_ASSIGN, BW_XOR_ASSIGN
    , BS_LEFT_ASSIGN, BS_RIGHT_ASSIGN
    , NULL
    , COLON, SEMICOLON, DOT, COMMA, EXCL_MARK
    , NUM
    , STRING
    , IDF
    , TRUE, FALSE
    , CONTAINER, SYSTEM, DATATYPE
    , LIBRARY
    , IF, ELIF, ELSE
    , FOR, WHILE, DO
    , SWITCH, CASE, DEFAULT
    , EOF
    , OTHER
    }
}
/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/
#pragma warning disable 1591

namespace Penguor.Parsing
{
    public enum TokenType : byte
    {
        // head content
        HEADSTART, HEADEND,
        FROM, INCLUDE, NF,

        // (,),{,},[,]
        LPAREN, RPAREN, // round
        LBRACE, RBRACE, // curly
        LBRACK, RBRACK,

        // operators
        PLUS, MINUS, MUL, DIV,
        GREATER, LESS, GREATER_EQUALS, LESS_EQUALS, EQUALS, NEQUALS,
        AND, OR, NOT,

        ASSIGN,

        // null
        NULL,

        // symbols
        COLON, SEMICOLON,
        DOT, COMMA,
        EXCL_MARK,

        VAR,

        // literal
        NUM, STRING, IDF,
        TRUE, FALSE,

        // keywords
        FN,
        COMPONENT, SYSTEM, DATATYPE,
        IF, ELIF, ELSE,
        FOR, WHILE, DO,
        SWITCH, CASE, DEFAULT,

        // end of file
        EOF,

        // TODO: implement error instead of other
        // other
        OTHER
    }
}
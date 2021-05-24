#pragma warning disable 1591

namespace Penguor.Compiler
{
    public enum AddressType
    {
        // LibraryDecl refers to the last AddressFrame of a LibraryDecl, while a LibraryDeclPart is part of a LibraryDecl, but not a last one
        LibraryDecl,
        SystemDecl, DataDecl, TypeDecl,
        FunctionDecl,
        VarDecl, VarExpr, VarStmt,
        BlockStmt,
        Control,
        IdfCall, FunctionCall, ArrayCall, TypeCall
    }
}